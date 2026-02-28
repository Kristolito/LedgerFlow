namespace LedgerFlow.Application.Common.Exceptions;

public sealed class DuplicateSlugException(string slug)
    : Exception($"A tenant with slug '{slug}' already exists.");
