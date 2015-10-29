using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using SEV.Crm.Business.Agents;
using SEV.Crm.Business.Configurators;
using SEV.Crm.Services;

namespace SEV.Crm.Business
{
    public abstract class CrmBusiness<TEntity> : IPluginExecutor
        where TEntity : Entity
    {
        private BusinessConfigurator m_configurator;
        private readonly List<IBusinessAgent> m_businessAgents = new List<IBusinessAgent>();

        private ICrmPluginEventExtractor m_pluginEventExtractor;
        protected ICrmPluginEventExtractor PluginEventExtractor
        {
            get
            {
                return m_pluginEventExtractor ?? 
                        (m_pluginEventExtractor = CrmServiceProvider.Current.GetService<ICrmPluginEventExtractor>());
            }
        }

        private IBusinessConfiguratorFactory m_configuratorFactory;
        protected IBusinessConfiguratorFactory ConfiguratorFactory
        {
            get
            {
                return m_configuratorFactory ?? (m_configuratorFactory = GetBusinessConfiguratorFactory());
            }
        }

        private IBusinessConfiguratorFactory GetBusinessConfiguratorFactory()
        {
            var configuratorAbsractFactory = 
                                        CrmServiceProvider.Current.GetService<IBusinessConfiguratorAbsractFactory>();
            return configuratorAbsractFactory.GetFactory<TEntity>();
        }

        public void Configure(IPluginExecutorContext executorContext)
        {
            CrmPluginEvent pluginEvent = PluginEventExtractor.GetPluginEvent(executorContext);
            m_configurator = ConfiguratorFactory.GetConfigurator(pluginEvent);
            m_businessAgents.AddRange(m_configurator.Configure(executorContext));
        }

        public void Execute(IPluginExecutorContext executorContext)
        {
            foreach (var agent in m_businessAgents.OrderBy(x => x.ExecutionOrder))
            {
                agent.Execute(executorContext);
            }
        }

        public void Dispose()
        {
            m_configurator = null;
            m_businessAgents.ForEach(agent => agent.Dispose());
            m_businessAgents.Clear();
        }
    }
}
