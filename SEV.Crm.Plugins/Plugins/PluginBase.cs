using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using SEV.Crm.Business;
using SEV.Crm.Business.Agents;
using SEV.Crm.ServiceContext;
using SEV.Crm.Services;

namespace SEV.Crm.Plugins
{
    /// <summary>
    /// Base class for all Plugins.
    /// </summary>    
    public abstract class PluginBase : IPlugin
    {
        private static readonly Lazy<ICrmServiceProvider> m_crmServiceProvider =
                                                    new Lazy<ICrmServiceProvider>(ConfigureCrmServiceProvider, true);
        private readonly Type m_entityType;

        protected PluginBase(Type entityType)
        {
            m_entityType = entityType;
            LoadPluginExecutor();
            LoadBusinessConfiguratorFactory();
        }

        private void LoadPluginExecutor()
        {
            var factory = GetServiceProvider().GetService<IPluginExecutorFactory>();
            factory.AddExecutor(m_entityType, CreatePluginExecutor());
        }

        protected abstract IPluginExecutor CreatePluginExecutor();

        private void LoadBusinessConfiguratorFactory()
        {
            var absractFactory = GetServiceProvider().GetService<IBusinessConfiguratorAbsractFactory>();
            absractFactory.AddFactory(m_entityType, CreateBusinessConfiguratorFactory());
        }

        protected abstract IBusinessConfiguratorFactory CreateBusinessConfiguratorFactory();

        public void Execute(IServiceProvider serviceProvider)
        {
            var contextFactory = GetServiceProvider().GetService<IPluginContextFactory>();
            IPluginExecutorContext pluginContext = contextFactory.CreateContext(serviceProvider, m_entityType);
            var executorFactory = GetServiceProvider().GetService<IPluginExecutorFactory>();
            IPluginExecutor pluginExecutor = executorFactory.GetExecutor(m_entityType);

            try
            {
                IEnumerable<IBusinessAgent> businessAgents = pluginExecutor.Configure(pluginContext);
                pluginExecutor.Execute(pluginContext, businessAgents);
            }
            catch (Exception e)
            {
                if (e is InvalidPluginExecutionException)
                {
                    throw;
                }
                HandleException(e, pluginContext.TracingService);
            }
        }

        private static void HandleException(Exception e, ITracingService tracingService)
        {
            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", e));
            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "StackTrace: {0}", e.StackTrace));

            var organizationServiceFault = new OrganizationServiceFault { Message = e.Message };
            organizationServiceFault.ErrorDetails.Add("Original Exception", e);

            throw new FaultException<OrganizationServiceFault>(organizationServiceFault, e.Message);
        }

        // For testing.
        protected virtual ICrmServiceProvider GetServiceProvider()
        {
            return m_crmServiceProvider.Value;
        }

        private static ICrmServiceProvider ConfigureCrmServiceProvider()
        {
            var provider = new CrmServiceProvider();
            provider.LoadService<ICrmServiceContextFactory>(new CrmServiceContextFactory());
            provider.LoadService<IPluginContextFactory>(new PluginContextFactory());
            provider.LoadService<IPluginExecutorFactory>(new PluginExecutorFactory());
            provider.LoadService<IBusinessConfiguratorAbsractFactory>(new BusinessConfiguratorAbsractFactory());
            provider.LoadService<ICrmPluginEventExtractor>(new CrmPluginEventExtractor());
            provider.LoadService<ICrmCache>(new CrmDeleteCache());
            CrmServiceProvider.Load(provider);

            return provider;
        }
    }
}