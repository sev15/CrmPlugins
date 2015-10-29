using System;
using SEV.Crm.Business;

namespace SEV.Crm.Plugins
{
    internal class PluginContextFactory : IPluginContextFactory
    {
        public IPluginExecutorContext CreateContext(IServiceProvider serviceProvider, Type entityType)
        {
            return new PluginContext(serviceProvider, entityType);
        }
    }
}
