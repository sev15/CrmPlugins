using System;
using Microsoft.Xrm.Sdk;
using SEV.Crm.Business;

namespace SEV.Crm.Plugins
{
    internal class PluginContext : IPluginExecutorContext
    {
        public const string TargetEntityAlias = "Target";
        public const string ImageEntityAlias = "Entity";

        private readonly IServiceProvider m_serviceProvider;
        private readonly Type m_entityType;

        public PluginContext(IServiceProvider serviceProvider, Type entityType)
        {
            m_serviceProvider = serviceProvider;
            m_entityType = entityType;
        }

        private ITracingService m_tracingService;
        public ITracingService TracingService
        {
            get
            {
                return m_tracingService ?? (m_tracingService =
                                            (ITracingService)m_serviceProvider.GetService(typeof(ITracingService)));
            }
        }

        private IPluginExecutionContext m_pluginExecutionContext;
        public IPluginExecutionContext PluginExecutionContext
        {
            get
            {
                return m_pluginExecutionContext ?? (m_pluginExecutionContext =
                           (IPluginExecutionContext)m_serviceProvider.GetService(typeof(IPluginExecutionContext)));
            }
        }

        private IOrganizationService m_organizationService;
        public IOrganizationService OrganizationService
        {
            get
            {
                return m_organizationService ?? (m_organizationService = GetOrganizationService());
            }
        }

        private IOrganizationService GetOrganizationService()
        {
            var factory = (IOrganizationServiceFactory)m_serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            ((IProxyTypesAssemblyProvider)factory).ProxyTypesAssembly = m_entityType.Assembly;

            return factory.CreateOrganizationService(PluginExecutionContext.UserId);
        }

        public TEntity GetTargetEntity<TEntity>() where TEntity : Entity
        {
            return ((Entity)PluginExecutionContext.InputParameters[TargetEntityAlias]).ToEntity<TEntity>();
        }

        public EntityReference GetTargetEntityReference()
        {
            return (EntityReference)PluginExecutionContext.InputParameters[TargetEntityAlias];
        }

        public TEntity GetPreEntityImage<TEntity>() where TEntity : Entity
        {
            return PluginExecutionContext.PreEntityImages[ImageEntityAlias].ToEntity<TEntity>();
        }

        public TEntity GetPostEntityImage<TEntity>() where TEntity : Entity
        {
            return PluginExecutionContext.PostEntityImages[ImageEntityAlias].ToEntity<TEntity>();
        }
    }
}
