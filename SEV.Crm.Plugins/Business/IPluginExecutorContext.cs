using Microsoft.Xrm.Sdk;

namespace SEV.Crm.Business
{
    public interface IPluginExecutorContext
    {
        ITracingService TracingService { get; }
        IPluginExecutionContext PluginExecutionContext { get; }
        IOrganizationService OrganizationService { get; }

        TEntity GetTargetEntity<TEntity>() where TEntity : Entity;
        EntityReference GetTargetEntityReference();
        TEntity GetPreEntityImage<TEntity>() where TEntity : Entity;
        TEntity GetPostEntityImage<TEntity>() where TEntity : Entity;
    }
}
