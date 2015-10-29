using System;
using System.Globalization;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using SEV.Crm.Business;
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

        private IPluginExecutor m_pluginExecutor;
        internal IPluginExecutor PluginExecutor
        {
            get
            {
                if (m_pluginExecutor == null)
                {
                    var executorFactory = GetServiceProvider().GetService<IPluginExecutorFactory>();
                    m_pluginExecutor = executorFactory.GetExecutor(m_entityType);
                }
                return m_pluginExecutor;
            }
        }

        private IPluginContextFactory m_contextFactory;
        internal IPluginContextFactory PluginContextFactory
        {
            get
            {
                return m_contextFactory ?? (m_contextFactory = GetServiceProvider().GetService<IPluginContextFactory>());
            }
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutorContext pluginContext = PluginContextFactory.CreateContext(serviceProvider, m_entityType);

            try
            {
                PluginExecutor.Configure(pluginContext);
                PluginExecutor.Execute(pluginContext);
            }
            catch (Exception e)
            {
                if (e is InvalidPluginExecutionException)
                {
                    throw;
                }
                HandleException(e, pluginContext.TracingService);
            }
            finally
            {
                PluginExecutor.Dispose();
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