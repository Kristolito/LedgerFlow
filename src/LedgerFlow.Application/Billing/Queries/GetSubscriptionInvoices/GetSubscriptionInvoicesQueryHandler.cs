using LedgerFlow.Application.Abstractions.Persistence;
using LedgerFlow.Application.Billing.Dtos;
using MediatR;

namespace LedgerFlow.Application.Billing.Queries.GetSubscriptionInvoices;

public sealed class GetSubscriptionInvoicesQueryHandler(
    IInvoiceRepository invoiceRepository) : IRequestHandler<GetSubscriptionInvoicesQuery, IReadOnlyList<InvoiceDto>>
{
    public async Task<IReadOnlyList<InvoiceDto>> Handle(
        GetSubscriptionInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        var invoices = await invoiceRepository.ListBySubscriptionIdAsync(request.SubscriptionId, cancellationToken);
        return invoices
            .Select(x => new InvoiceDto(
                x.Id,
                x.SubscriptionId,
                x.AmountCents,
                x.Currency,
                x.Status.ToString(),
                x.CreatedAt))
            .ToList();
    }
}
