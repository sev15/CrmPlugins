using System;
using System.Collections.Generic;
using SEV.Crm.Business;
using SEV.Crm.Plugins.Properties;

namespace SEV.Crm.Plugins
{
    internal class PluginExecutorFactory : IPluginExecutorFactory
    {
        private readonly Dictionary<Type, IPluginExecutor> m_executors = new Dictionary<Type, IPluginExecutor>();

        public void AddExecutor(Type executorType, IPluginExecutor executor)
        {
            if (m_executors.ContainsKey(executorType))
            {
                return;
            }
            m_executors.Add(executorType, executor);
        }

        public IPluginExecutor GetExecutor(Type entityType)
        {
            IPluginExecutor executor;
            if (m_executors.TryGetValue(entityType, out executor))
            {
                return executor;
            }
            throw new InvalidOperationException(String.Format(Resources.PluginExecutorFactoryError, entityType));
        }
    }
}
