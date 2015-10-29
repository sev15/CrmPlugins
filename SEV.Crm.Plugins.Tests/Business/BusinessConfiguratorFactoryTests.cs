using System;
using System.Collections.Generic;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SEV.Crm.Business.Configurators;
using SEV.Crm.Plugins.Properties;

namespace SEV.Crm.Business.Tests
{
    [TestFixture]
    public class BusinessConfiguratorFactoryTests
    {
        private const string TestFactoryName = "TestFactoryName";
        private const CrmPluginEvent TestPluginEvent = CrmPluginEvent.PostCreate;

        private Mock<BusinessConfigurator> m_configuratorMock;
        private IBusinessConfiguratorFactory m_factory;

        #region SetUp

        [SetUp]
        public void Init()
        {
            m_factory = CreateBusinessConfiguratorFactory();
        }

        private BusinessConfiguratorFactory CreateBusinessConfiguratorFactory()
        {
            m_configuratorMock = new Mock<BusinessConfigurator>();
            var configurators = new List<Tuple<CrmPluginEvent, BusinessConfigurator>>()
            {
                new Tuple<CrmPluginEvent, BusinessConfigurator>(TestPluginEvent, m_configuratorMock.Object)
            };
            var factoryMock = new Mock<BusinessConfiguratorFactory> { CallBase = true };
            factoryMock.Protected().Setup<List<Tuple<CrmPluginEvent, BusinessConfigurator>>>("CreateConfiguratorEntries")
                                   .Returns(configurators);
            factoryMock.Protected().Setup<string>("GetFactoryName").Returns(TestFactoryName);

            return factoryMock.Object;
        }

        #endregion

        [Test]
        public void GetConfigurator_ShouldReturnBusinessConfiguratorForSpecifiedPluginEvent()
        {
            var result = m_factory.GetConfigurator(TestPluginEvent);

            Assert.That(result, Is.SameAs(m_configuratorMock.Object));
        }

        [Test]
        public void GetConfigurator_ShouldThrowInvalidOperationException_WhenSpecifiedPluginEventIsUnsupported()
        {
            string errorMessage =
                        String.Format(Resources.BusinessConfiguratorFactoryError, TestFactoryName, TestPluginEvent);

            Assert.That(() => m_factory.GetConfigurator(CrmPluginEvent.None),
                                                Throws.Exception.TypeOf<InvalidOperationException>(), errorMessage);
        }
    }
}
