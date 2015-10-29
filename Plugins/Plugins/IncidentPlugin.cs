using Sample.Crm.Business;
using SEV.Crm.Business;
using SEV.Crm.Plugins;

namespace Sample.Crm.Plugins
{
    public class IncidentPlugin : PluginBase
    {
        public IncidentPlugin() : base(typeof(Sample.Crm.Entities.Incident))
        {
        }

        protected override IPluginExecutor CreatePluginExecutor()
        {
            return new IncidentCrmBusiness();
        }

        protected override IBusinessConfiguratorFactory CreateBusinessConfiguratorFactory()
        {
            return new IncidentBusinessConfiguratorFactory();
        }
    }
}
