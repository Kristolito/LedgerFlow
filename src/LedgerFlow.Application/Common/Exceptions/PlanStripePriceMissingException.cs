namespace LedgerFlow.Application.Common.Exceptions;

public sealed class PlanStripePriceMissingException
    : Exception
{
    public PlanStripePriceMissingException()
        : base("Plan is not linked to a Stripe Price. Run POST /stripe/prices/sync.")
    {
    }
}
