using System;
using System.Collections.Generic;
using SEV.Crm.Plugins.Properties;

namespace SEV.Crm.Business
{
    internal class BusinessConfiguratorAbsractFactory : IBusinessConfiguratorAbsractFactory
    {
        private readonly Dictionary<Type, IBusinessConfiguratorFactory> m_factories =
                                                                new Dictionary<Type, IBusinessConfiguratorFactory>();

        public void AddFactory(Type factoryType, IBusinessConfiguratorFactory factory)
        {
            if (m_factories.ContainsKey(factoryType))
            {
                return;
            }
            m_factories.Add(factoryType, factory);
        }

        public IBusinessConfiguratorFactory GetFactory<TEntity>() where TEntity : Microsoft.Xrm.Sdk.Entity
        {
            IBusinessConfiguratorFactory factory;
            Type entityType = typeof(TEntity);
            if (m_factories.TryGetValue(entityType, out factory))
            {
                return factory;
            }
            throw new InvalidOperationException(
                                        String.Format(Resources.BusinessConfiguratorAbsractFactoryError, entityType));
        }
    }
}
