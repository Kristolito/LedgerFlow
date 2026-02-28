using LedgerFlow.Application.Billing.Dtos;
using MediatR;

namespace LedgerFlow.Application.Billing.Queries.GetSubscriptionInvoices;

public sealed record GetSubscriptionInvoicesQuery(Guid SubscriptionId) : IRequest<IReadOnlyList<InvoiceDto>>;
