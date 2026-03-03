using LedgerFlow.Domain.Entities;

namespace LedgerFlow.Application.Abstractions.Persistence;

public interface IWebhookEventRepository
{
    Task<bool> TryCreateReceivedAsync(WebhookEvent webhookEvent, CancellationToken cancellationToken);
    Task MarkProcessedAsync(string stripeEventId, CancellationToken cancellationToken);
    Task MarkFailedAsync(string stripeEventId, string error, CancellationToken cancellationToken);
}