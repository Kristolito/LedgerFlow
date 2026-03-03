using System.Net;
using System.Text;
using LedgerFlow.Application.Abstractions.Billing;
using LedgerFlow.Domain.Entities;
using LedgerFlow.Domain.Enums;
using LedgerFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stripe;
using Xunit;
using DomainCustomer = LedgerFlow.Domain.Entities.Customer;
using DomainPlan = LedgerFlow.Domain.Entities.Plan;
using DomainSubscription = LedgerFlow.Domain.Entities.Subscription;

namespace LedgerFlow.IntegrationTests;

public sealed class StripeWebhookIdempotencyTests : IClassFixture<StripeWebhookApiFactory>
{
    private readonly StripeWebhookApiFactory _factory;

    public StripeWebhookIdempotencyTests(StripeWebhookApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostingSameEventTwice_ProcessesOnlyOnce()
    {
        using var client = _factory.CreateClient();

        var payload = "{\"id\":\"evt_duplicate\"}";
        using var content1 = new StringContent(payload, Encoding.UTF8, "application/json");
        content1.Headers.Add("Stripe-Signature", "test-signature");

        var first = await client.PostAsync("/webhooks/stripe", content1);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        using var content2 = new StringContent(payload, Encoding.UTF8, "application/json");
        content2.Headers.Add("Stripe-Signature", "test-signature");

        var second = await client.PostAsync("/webhooks/stripe", content2);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LedgerFlowDbContext>();

        var webhookEvents = await dbContext.WebhookEvents.AsNoTracking().ToListAsync();
        Assert.Single(webhookEvents);
        Assert.Equal(WebhookEventStatus.Processed, webhookEvents[0].Status);

        var subscription = await dbContext.Subscriptions
            .IgnoreQueryFilters()
            .SingleAsync(x => x.StripeSubscriptionId == StripeWebhookApiFactory.SeededStripeSubscriptionId);

        Assert.Equal(SubscriptionStatus.Active, subscription.Status);

        var invoices = await dbContext.Invoices
            .IgnoreQueryFilters()
            .Where(x => x.StripeInvoiceId == "in_phase5")
            .ToListAsync();

        Assert.Single(invoices);
        Assert.Equal(InvoiceStatus.Paid, invoices[0].Status);
    }
}

public sealed class StripeWebhookApiFactory : WebApplicationFactory<Program>
{
    public const string SeededStripeSubscriptionId = "sub_phase5";

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var values = new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience",
                ["Jwt:SigningKey"] = "test-signing-key-test-signing-key",
                ["Redis:Host"] = "localhost",
                ["ConnectionStrings:Postgres"] = "Host=localhost;Port=5432;Database=ledgerflow;Username=ledgerflow;Password=ledgerflow",
                ["Stripe:SecretKey"] = "sk_test_x",
                ["Stripe:WebhookSecret"] = "whsec_test_x"
            };

            config.AddInMemoryCollection(values);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<LedgerFlowDbContext>>();
            services.AddDbContext<LedgerFlowDbContext>(options =>
                options.UseInMemoryDatabase("ledgerflow-webhook-tests"));

            services.RemoveAll<IStripeWebhookEventParser>();
            services.AddSingleton<IStripeWebhookEventParser, FakeStripeWebhookEventParser>();

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LedgerFlowDbContext>();
            Seed(dbContext);
        });
    }

    private static void Seed(LedgerFlowDbContext dbContext)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var now = DateTimeOffset.UtcNow;
        var tenantId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var planId = Guid.NewGuid();

        dbContext.Plans.Add(new DomainPlan
        {
            Id = planId,
            TenantId = tenantId,
            Name = "Growth",
            PriceCents = 1000,
            Currency = "GBP",
            Interval = PlanInterval.Month,
            IsActive = true,
            StripePriceId = "price_phase5",
            CreatedAt = now
        });

        dbContext.Customers.Add(new DomainCustomer
        {
            Id = customerId,
            TenantId = tenantId,
            UserId = Guid.NewGuid(),
            StripeCustomerId = "cus_phase5",
            CreatedAt = now
        });

        dbContext.Subscriptions.Add(new DomainSubscription
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CustomerId = customerId,
            PlanId = planId,
            Status = SubscriptionStatus.Trialing,
            CurrentPeriodStart = now,
            CurrentPeriodEnd = now.AddMonths(1),
            CancelAtPeriodEnd = false,
            StripeSubscriptionId = SeededStripeSubscriptionId,
            CreatedAt = now
        });

        dbContext.SaveChanges();
    }

    private sealed class FakeStripeWebhookEventParser : IStripeWebhookEventParser
    {
        public Event ParseAndVerify(string payloadJson, string signatureHeader, string webhookSecret)
        {
            return new Event
            {
                Id = "evt_duplicate",
                Type = "invoice.paid",
                Data = new EventData
                {
                    Object = new Stripe.Invoice
                    {
                        Id = "in_phase5",
                        SubscriptionId = SeededStripeSubscriptionId,
                        AmountPaid = 1000,
                        Currency = "gbp"
                    }
                }
            };
        }
    }
}
