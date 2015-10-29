using System;
using NUnit.Framework;
using Sample.Crm.Business.Configurators;
using SEV.Crm.Business;
using SEV.Crm.Business.Configurators;

namespace Sample.Crm.Business.Tests
{
    [TestFixture]
    public class AccountBusinessConfiguratorFactoryTests
    {
        private BusinessConfigurator m_configurator;
        private IBusinessConfiguratorFactory m_factory;

        [SetUp]
        public void Init()
        {
            m_factory = new AccountBusinessConfiguratorFactory();
        }

        [Test]
        public void GetExecutor_ShouldReturnInstanceOfPostUpdateAccountBusinessConfigurator_WhenPluginEventIsPostUpdate()
        {
            m_configurator = m_factory.GetConfigurator(CrmPluginEvent.PostUpdate);

            Assert.That(m_configurator, Is.TypeOf<PostUpdateAccountBusinessConfigurator>());
        }

        [Test]
        public void GetConfigurator_ShouldThrowInvalidOperationException_WhenPluginEventIsUnsupported()
        {
            Assert.That(() => m_factory.GetConfigurator(CrmPluginEvent.None),
                Throws.Exception.TypeOf<InvalidOperationException>().With.Message.StartsWith("AccountBusinessConfiguratorFactory"));
        }
    }
}
