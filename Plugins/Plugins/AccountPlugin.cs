using Sample.Crm.Business;
using SEV.Crm.Business;
using SEV.Crm.Plugins;

namespace Sample.Crm.Plugins
{
    public class AccountPlugin : PluginBase
    {
        public AccountPlugin() : base(typeof(Sample.Crm.Entities.Account))
        {
        }

        protected override IPluginExecutor CreatePluginExecutor()
        {
            return new AccountCrmBusiness();
        }

        protected override IBusinessConfiguratorFactory CreateBusinessConfiguratorFactory()
        {
            return new AccountBusinessConfiguratorFactory();
        }
    }
}
