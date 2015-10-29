using System;
using SEV.Crm.Business;

namespace SEV.Crm.Plugins
{
    public interface IPluginExecutorFactory
    {
        void AddExecutor(Type executorType, IPluginExecutor executor);
        IPluginExecutor GetExecutor(Type entityType);
    }
}