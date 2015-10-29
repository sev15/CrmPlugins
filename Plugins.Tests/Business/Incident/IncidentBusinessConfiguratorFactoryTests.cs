using System;
using NUnit.Framework;
using Sample.Crm.Business.Configurators;
using SEV.Crm.Business;
using SEV.Crm.Business.Configurators;

namespace Sample.Crm.Business.Tests
{
    [TestFixture]
    public class IncidentBusinessConfiguratorFactoryTests
    {
        private BusinessConfigurator m_configurator;
        private IBusinessConfiguratorFactory m_factory;

        [SetUp]
        public void Init()
        {
            m_factory = new IncidentBusinessConfiguratorFactory();
        }

        [Test]
        public void GetExecutor_ShouldReturnInstanceOfPreValidateCreateIncidentBusinessConfigurator_WhenPluginEventIsPreValidateCreate()
        {
            m_configurator = m_factory.GetConfigurator(CrmPluginEvent.PreValidateCreate);

            Assert.That(m_configurator, Is.TypeOf<PreValidateCreateIncidentBusinessConfigurator>());
        }

        [Test]
        public void GetExecutor_ShouldReturnInstanceOfPreCreateIncidentBusinessConfigurator_WhenPluginEventIsPreCreate()
        {
            m_configurator = m_factory.GetConfigurator(CrmPluginEvent.PreCreate);

            Assert.That(m_configurator, Is.TypeOf<PreCreateIncidentBusinessConfigurator>());
        }

        [Test]
        public void GetExecutor_ShouldReturnInstanceOfPreValidateUpdateIncidentBusinessConfigurator_WhenPluginEventIsPreValidateUpdate()
        {
            m_configurator = m_factory.GetConfigurator(CrmPluginEvent.PreValidateUpdate);

            Assert.That(m_configurator, Is.TypeOf<PreValidateUpdateIncidentBusinessConfigurator>());
        }

        [Test]
        public void GetConfigurator_ShouldThrowInvalidOperationException_WhenPluginEventIsUnsupported()
        {
            Assert.That(() => m_factory.GetConfigurator(CrmPluginEvent.None),
                Throws.Exception.TypeOf<InvalidOperationException>().With.Message.StartsWith("IncidentBusinessConfiguratorFactory"));
        }
    }
}
