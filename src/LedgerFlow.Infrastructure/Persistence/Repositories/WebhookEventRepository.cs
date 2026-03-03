using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Domain.Entities;
using LedgerFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Persistence.Repositories;

public sealed class WebhookEventRepository(LedgerFlowDbContext dbContext) : IWebhookEventRepository
{
    public async Task<bool> TryCreateReceivedAsync(WebhookEvent webhookEvent, CancellationToken cancellationToken)
    {
        var exists = await dbContext.WebhookEvents
            .AsNoTracking()
            .AnyAsync(x => x.StripeEventId == webhookEvent.StripeEventId, cancellationToken);

        if (exists)
        {
            return false;
        }

        dbContext.WebhookEvents.Add(webhookEvent);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task MarkProcessedAsync(string stripeEventId, CancellationToken cancellationToken)
    {
        var webhookEvent = await dbContext.WebhookEvents
            .FirstOrDefaultAsync(x => x.StripeEventId == stripeEventId, cancellationToken);

        if (webhookEvent is null)
        {
            return;
        }

        webhookEvent.Status = WebhookEventStatus.Processed;
        webhookEvent.ProcessedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(string stripeEventId, string error, CancellationToken cancellationToken)
    {
        var webhookEvent = await dbContext.WebhookEvents
            .FirstOrDefaultAsync(x => x.StripeEventId == stripeEventId, cancellationToken);

        if (webhookEvent is null)
        {
            return;
        }

        webhookEvent.Status = WebhookEventStatus.Failed;
        webhookEvent.Error = error.Length > 2000 ? error[..2000] : error;
        webhookEvent.ProcessedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}