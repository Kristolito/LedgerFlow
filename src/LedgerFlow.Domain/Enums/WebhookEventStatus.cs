namespace LedgerFlow.Domain.Enums;

public enum WebhookEventStatus
{
    Received = 1,
    Processed = 2,
    SkippedDuplicate = 3,
    Failed = 4
}