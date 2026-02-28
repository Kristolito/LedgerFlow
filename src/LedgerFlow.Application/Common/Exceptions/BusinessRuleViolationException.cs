namespace LedgerFlow.Application.Common.Exceptions;

public sealed class BusinessRuleViolationException(string message) : Exception(message);
