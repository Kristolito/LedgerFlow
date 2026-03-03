namespace LedgerFlow.Domain.Entities;

public sealed class WebhookEvent
{
    public Guid Id { get; set; }
    public string StripeEventId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset ReceivedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public LedgerFlow.Domain.Enums.WebhookEventStatus Status { get; set; }
    public string? Error { get; set; }
    public string PayloadJson { get; set; } = string.Empty;
}