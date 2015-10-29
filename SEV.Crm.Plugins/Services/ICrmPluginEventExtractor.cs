using SEV.Crm.Business;

namespace SEV.Crm.Services
{
    public interface ICrmPluginEventExtractor
    {
        CrmPluginEvent GetPluginEvent(IPluginExecutorContext executorContext);
    }
}