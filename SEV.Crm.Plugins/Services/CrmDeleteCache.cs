using System.Collections.Generic;

namespace SEV.Crm.Services
{
    internal class CrmDeleteCache : ICrmCache
    {
        private readonly Dictionary<string, object> m_cache = new Dictionary<string, object>();

        public bool Contains(string key)
        {
            return m_cache.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            if (m_cache.ContainsKey(key))
            {
                m_cache.Remove(key);
            }
            m_cache.Add(key, value);
        }

        public object Get(string key)
        {
            return m_cache[key];
        }

        public void Remove(string key)
        {
            m_cache.Remove(key);
        }
    }
}