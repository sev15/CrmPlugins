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
    public class IncidentPluginTests
    {
        [Test]
        public void IncidentPluginCreation_ShouldConfigureIncidentPluginBusiness()
        {
            IPlugin plugin = new IncidentPlugin();

            var pluginExecutor =
                    CrmServiceProvider.Current.GetService<IPluginExecutorFactory>().GetExecutor(typeof(Incident));
            Assert.That(pluginExecutor, Is.InstanceOf<IncidentCrmBusiness>());
            var businessConfiguratorFactory =
                CrmServiceProvider.Current.GetService<IBusinessConfiguratorAbsractFactory>().GetFactory<Incident>();
            Assert.That(businessConfiguratorFactory, Is.InstanceOf<IncidentBusinessConfiguratorFactory>());
        }
    }
}
