using LedgerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LedgerFlow.Infrastructure.Persistence.Configurations;

public sealed class WebhookEventConfiguration : IEntityTypeConfiguration<WebhookEvent>
{
    public void Configure(EntityTypeBuilder<WebhookEvent> builder)
    {
        builder.ToTable("webhook_events");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StripeEventId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.ReceivedAt).IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Error)
            .HasMaxLength(2000);

        builder.Property(x => x.PayloadJson)
            .IsRequired();

        builder.HasIndex(x => x.StripeEventId)
            .IsUnique();
    }
}