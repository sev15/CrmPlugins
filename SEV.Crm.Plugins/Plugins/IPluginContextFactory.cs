using System;
using SEV.Crm.Business;

namespace SEV.Crm.Plugins
{
    public interface IPluginContextFactory
    {
        IPluginExecutorContext CreateContext(IServiceProvider serviceProvider, Type entityType);
    }
}