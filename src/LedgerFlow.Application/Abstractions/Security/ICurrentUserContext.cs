namespace LedgerFlow.Application.Abstractions.Security;

public interface ICurrentUserContext
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
}
