namespace LedgerFlow.Api.Tenancy;

public interface IRequireTenantMetadata;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireTenantAttribute : Attribute, IRequireTenantMetadata;
