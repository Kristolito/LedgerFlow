namespace LedgerFlow.Application.Common.Exceptions;

public sealed class TenantContextMissingException(string message) : Exception(message);
