using System;

namespace SEV.Crm.Business.Agents
{
    public abstract class CrmBusinessAgent : IBusinessAgent
    {
        public int ExecutionOrder
        {
            get;
            set;
        }

        public object[] Context
        {
            get;
            set;
        }

        public virtual void Execute(IPluginExecutorContext pluginContext)
        {
            if (ExecutionOrder < 1)
            {
                throw new InvalidOperationException("Invalid Execution Order.");
            }
        }
    }
}