using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SEV.Crm.Business.Agents;
using SEV.Crm.Business.Configurators;
using SEV.Crm.Services;

namespace SEV.Crm.Business.Tests
{
    [TestFixture]
    public class CrmBusinessTest
    {
        private const CrmPluginEvent TestCrmPluginEvent = CrmPluginEvent.PostCreate;
        private static readonly IPluginExecutorContext TestContext = new Mock<IPluginExecutorContext>().Object;

        private Mock<ICrmServiceProvider> m_crmServiceProviderMock;
        private Mock<ICrmPluginEventExtractor> m_crmPluginEventExtractorMock;
        private Mock<IBusinessConfiguratorAbsractFactory> m_businessConfiguratorAbsractFactoryMock;
        private Mock<BusinessConfiguratorFactory> m_businessConfiguratorFactoryMock;
        private Mock<BusinessConfigurator> m_businessConfiguratorMock;
        private IPluginExecutor m_crmBusiness;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitializeMocks();

            m_crmBusiness = new Mock<CrmBusiness<Entity>> { CallBase = true }.Object;
        }

        private void InitializeMocks()
        {
            m_crmPluginEventExtractorMock = new Mock<ICrmPluginEventExtractor>();
            m_crmPluginEventExtractorMock.Setup(x => x.GetPluginEvent(TestContext)).Returns(TestCrmPluginEvent);
            m_businessConfiguratorMock = new Mock<BusinessConfigurator>();
            m_businessConfiguratorFactoryMock = new Mock<BusinessConfiguratorFactory>();
            m_businessConfiguratorFactoryMock.Protected().Setup<List<Tuple<CrmPluginEvent, BusinessConfigurator>>>("CreateConfiguratorEntries")
                                             .Returns(new List<Tuple<CrmPluginEvent, BusinessConfigurator>>());
            m_businessConfiguratorFactoryMock.As<IBusinessConfiguratorFactory>()
                                             .Setup(x => x.GetConfigurator(It.IsAny<CrmPluginEvent>()))
                                             .Returns(m_businessConfiguratorMock.Object);
            m_businessConfiguratorAbsractFactoryMock = new Mock<IBusinessConfiguratorAbsractFactory>();
            m_businessConfiguratorAbsractFactoryMock.Setup(x => x.GetFactory<Entity>())
                                                    .Returns(m_businessConfiguratorFactoryMock.Object);
            m_crmServiceProviderMock = new Mock<ICrmServiceProvider>();
            m_crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(ICrmPluginEventExtractor))))
                                    .Returns(m_crmPluginEventExtractorMock.Object);
            m_crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IBusinessConfiguratorAbsractFactory))))
                                    .Returns(m_businessConfiguratorAbsractFactoryMock.Object);

            CrmServiceProvider.Load(m_crmServiceProviderMock.Object);
        }

        #endregion

        [Test]
        public void Configure_ShouldCallGetServiceOfCrmServiceProviderForCrmPluginEventExtractor()
        {
            m_crmBusiness.Configure(TestContext);

            m_crmServiceProviderMock.Verify(x => x.GetService(typeof(ICrmPluginEventExtractor)), Times.Once);
        }

        [Test]
        public void Configure_ShouldCallGetPluginEventOfCrmPluginEventExtractor()
        {
            m_crmBusiness.Configure(TestContext);

            m_crmPluginEventExtractorMock.Verify(x => x.GetPluginEvent(TestContext), Times.Once);
        }

        [Test]
        public void Configure_ShouldCallGetServiceOfCrmServiceProviderForBusinessConfiguratorAbsractFactory()
        {
            m_crmBusiness.Configure(TestContext);

            m_crmServiceProviderMock.Verify(x => x.GetService(typeof(IBusinessConfiguratorAbsractFactory)), Times.AtLeastOnce);
        }

        [Test]
        public void Configure_ShouldCallGetFactoryOfBusinessConfiguratorAbsractFactory()
        {
            m_crmBusiness.Configure(TestContext);

            m_businessConfiguratorAbsractFactoryMock.Verify(x => x.GetFactory<Entity>(), Times.AtLeastOnce);
        }

        [Test]
        public void Configure_ShouldCallGetConfiguratorOfBusinessConfiguratorFactory()
        {
            m_crmBusiness.Configure(TestContext);

            m_businessConfiguratorFactoryMock.As<IBusinessConfiguratorFactory>()
                                             .Verify(x => x.GetConfigurator(TestCrmPluginEvent), Times.AtLeastOnce);
        }

        [Test]
        public void Configure_ShouldCallConfigureOfBusinessConfigurator()
        {
            m_crmBusiness.Configure(TestContext);

            m_businessConfiguratorMock.Verify(x => x.Configure(TestContext), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallExecuteOfCrmBusinessAgents()
        {
            var businessAgentMock1 = new Mock<IBusinessAgent>();
            businessAgentMock1.SetupProperty(x => x.ExecutionOrder, 1);
            var businessAgentMock2 = new Mock<IBusinessAgent>();
            businessAgentMock2.SetupProperty(x => x.ExecutionOrder, 2);

            m_crmBusiness.Execute(TestContext,
                                  new List<IBusinessAgent> { businessAgentMock1.Object, businessAgentMock2.Object });

            businessAgentMock1.Verify(x => x.Execute(TestContext), Times.Once);
            businessAgentMock2.Verify(x => x.Execute(TestContext), Times.Once);
        }
    }
}
