using System;
using System.Collections.Generic;
using SEV.Crm.Business.Configurators;
using SEV.Crm.Plugins.Properties;

namespace SEV.Crm.Business
{
    public abstract class BusinessConfiguratorFactory : IBusinessConfiguratorFactory
    {        
        private readonly Dictionary<CrmPluginEvent, BusinessConfigurator> m_configurators = 
                                                            new Dictionary<CrmPluginEvent, BusinessConfigurator>();

        protected BusinessConfiguratorFactory()
        {
            InitializeFactoryDictionary();
        }

        private void InitializeFactoryDictionary()
        {
            CreateConfiguratorEntries().ForEach(entry => m_configurators.Add(entry.Item1, entry.Item2));
        }

        protected abstract List<Tuple<CrmPluginEvent, BusinessConfigurator>> CreateConfiguratorEntries();

        public BusinessConfigurator GetConfigurator(CrmPluginEvent pluginEvent)
        {
            BusinessConfigurator configurator;
            if (m_configurators.TryGetValue(pluginEvent, out configurator))
            {
                return configurator;
            }
            throw new InvalidOperationException(String.Format(Resources.BusinessConfiguratorFactoryError, 
                                                                                GetFactoryName(), pluginEvent));
        }

        protected abstract string GetFactoryName();
    }
}
