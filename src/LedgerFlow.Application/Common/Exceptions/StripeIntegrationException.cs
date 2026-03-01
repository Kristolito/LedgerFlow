namespace LedgerFlow.Application.Common.Exceptions;

public sealed class StripeIntegrationException(string message, Exception? innerException = null)
    : Exception(message, innerException);
