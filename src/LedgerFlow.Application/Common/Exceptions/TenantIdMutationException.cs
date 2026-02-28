namespace LedgerFlow.Application.Common.Exceptions;

public sealed class TenantIdMutationException(string message) : Exception(message);
