namespace SEV.Crm.Business.Agents
{
    public interface IBusinessAgent
    {
        int ExecutionOrder { get; set; }
        object[] Context { get; set; }

        void Execute(IPluginExecutorContext pluginContext);
    }
}