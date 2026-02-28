namespace LedgerFlow.Domain.Interfaces;

public interface ITenantScoped
{
    Guid TenantId { get; set; }
}
