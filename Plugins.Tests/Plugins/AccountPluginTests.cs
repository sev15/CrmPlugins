using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Sample.Crm.Business;
using Sample.Crm.Entities;
using SEV.Crm.Business;
using SEV.Crm.Plugins;
using SEV.Crm.Services;

namespace Sample.Crm.Plugins.Tests
{
    [TestFixture]
    public class AccountPluginTests
    {
        [Test]
        public void AccountPluginCreation_ShouldConfigureAccountPluginBusiness()
        {
            IPlugin plugin = new AccountPlugin();

            var pluginExecutor =
                    CrmServiceProvider.Current.GetService<IPluginExecutorFactory>().GetExecutor(typeof(Account));
            Assert.That(pluginExecutor, Is.InstanceOf<AccountCrmBusiness>());
            var businessConfiguratorFactory =
                    CrmServiceProvider.Current.GetService<IBusinessConfiguratorAbsractFactory>().GetFactory<Account>();
            Assert.That(businessConfiguratorFactory, Is.InstanceOf<AccountBusinessConfiguratorFactory>());
        }
    }
}
