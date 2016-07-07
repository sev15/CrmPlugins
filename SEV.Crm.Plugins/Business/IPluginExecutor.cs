
using System.Collections.Generic;
using SEV.Crm.Business.Agents;

namespace SEV.Crm.Business
{
    public interface IPluginExecutor
    {
        IEnumerable<IBusinessAgent> Configure(IPluginExecutorContext context);
        void Execute(IPluginExecutorContext context, IEnumerable<IBusinessAgent> businessAgents);
    }
}
