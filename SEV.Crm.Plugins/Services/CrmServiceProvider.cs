using System;
using System.Collections.Generic;
using SEV.Crm.Plugins.Properties;

namespace SEV.Crm.Services
{
    public class CrmServiceProvider : ICrmServiceProvider
    {
        private static readonly Dictionary<Type, object> m_services = new Dictionary<Type, object>();

        public object GetService(Type serviceType)
        {
            object service;

            if (m_services.TryGetValue(serviceType, out service))
            {
                return service;
            }
            throw new InvalidOperationException(String.Format(Resources.CrmServiceProviderError, serviceType));
        }

        public void LoadService(Type key, object service)
        {
            m_services.Add(key, service);
        }

        private static readonly object m_lockObj = new object();

        private static ICrmServiceProvider m_instance;
        public static ICrmServiceProvider Current
        {
            get
            {
                lock (m_lockObj)
                {
                    return m_instance ?? (m_instance = new CrmServiceProvider());
                }
            }
        }

        public static void Load(ICrmServiceProvider instance)
        {
            m_instance = instance;
        }
    }
}