
namespace SEV.Crm.Business
{
    public interface IPluginExecutor : System.IDisposable
    {
        void Configure(IPluginExecutorContext context);
        void Execute(IPluginExecutorContext context);
    }
}
