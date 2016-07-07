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
        private ICrmPluginEventExtractor m_pluginEventExtractor;
        protected ICrmPluginEventExtractor PluginEventExtractor
        {
            get
            {
                return m_pluginEventExtractor ?? 
                        (m_pluginEventExtractor = CrmServiceProvider.Current.GetService<ICrmPluginEventExtractor>());
            }
        }

        public IEnumerable<IBusinessAgent> Configure(IPluginExecutorContext executorContext)
        {
            CrmPluginEvent pluginEvent = PluginEventExtractor.GetPluginEvent(executorContext);
            IBusinessConfiguratorFactory configuratorFactory = GetBusinessConfiguratorFactory();
            BusinessConfigurator configurator = configuratorFactory.GetConfigurator(pluginEvent);

            return configurator.Configure(executorContext);
        }

        private IBusinessConfiguratorFactory GetBusinessConfiguratorFactory()
        {
            var configuratorAbsractFactory =
                                        CrmServiceProvider.Current.GetService<IBusinessConfiguratorAbsractFactory>();
            return configuratorAbsractFactory.GetFactory<TEntity>();
        }

        public void Execute(IPluginExecutorContext executorContext, IEnumerable<IBusinessAgent> businessAgents)
        {
            foreach (var agent in businessAgents.OrderBy(x => x.ExecutionOrder))
            {
                agent.Execute(executorContext);
            }
        }
    }
}
