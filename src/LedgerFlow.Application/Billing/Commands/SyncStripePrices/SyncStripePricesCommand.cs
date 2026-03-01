using LedgerFlow.Application.Billing.Dtos;
using MediatR;

namespace LedgerFlow.Application.Billing.Commands.SyncStripePrices;

public sealed record SyncStripePricesCommand : IRequest<StripePriceSyncResultDto>;
