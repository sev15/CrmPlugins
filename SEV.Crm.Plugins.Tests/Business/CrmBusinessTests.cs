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

        private Mock<IBusinessAgent> crmBusinessAgentMock;
        private Mock<ICrmServiceProvider> crmServiceProviderMock;
        private Mock<ICrmPluginEventExtractor> crmPluginEventExtractorMock;
        private Mock<IBusinessConfiguratorAbsractFactory> businessConfiguratorAbsractFactoryMock;
        private Mock<BusinessConfiguratorFactory> businessConfiguratorFactoryMock;
        private Mock<BusinessConfigurator> businessConfiguratorMock;
        private IPluginExecutor m_crmBusiness;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitializeMocks();

            var crmBusinessMock = new Mock<CrmBusiness<Entity>> { CallBase = true };
            m_crmBusiness = crmBusinessMock.Object;
        }

        private void InitializeMocks()
        {
            crmPluginEventExtractorMock = new Mock<ICrmPluginEventExtractor>();
            crmPluginEventExtractorMock.Setup(x => x.GetPluginEvent(TestContext))
                                       .Returns(TestCrmPluginEvent);
            crmBusinessAgentMock = new Mock<IBusinessAgent>();
            crmBusinessAgentMock.SetupProperty(x => x.ExecutionOrder, 1);
            businessConfiguratorMock = new Mock<BusinessConfigurator>();
            businessConfiguratorMock.Setup(x => x.Configure(TestContext))
                                    .Returns(new List<IBusinessAgent> { crmBusinessAgentMock.Object, crmBusinessAgentMock.Object });
            businessConfiguratorFactoryMock = new Mock<BusinessConfiguratorFactory>();
            businessConfiguratorFactoryMock.Protected().Setup<List<Tuple<CrmPluginEvent, BusinessConfigurator>>>("CreateConfiguratorEntries")
                                           .Returns(new List<Tuple<CrmPluginEvent, BusinessConfigurator>>());
            businessConfiguratorFactoryMock.As<IBusinessConfiguratorFactory>()
                                           .Setup(x => x.GetConfigurator(It.IsAny<CrmPluginEvent>()))
                                           .Returns(businessConfiguratorMock.Object);
            businessConfiguratorAbsractFactoryMock = new Mock<IBusinessConfiguratorAbsractFactory>();
            businessConfiguratorAbsractFactoryMock.Setup(x => x.GetFactory<Entity>())
                                                  .Returns(businessConfiguratorFactoryMock.Object);
            crmServiceProviderMock = new Mock<ICrmServiceProvider>();
            crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(ICrmPluginEventExtractor))))
                                      .Returns(crmPluginEventExtractorMock.Object);
            crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IBusinessConfiguratorAbsractFactory))))
                                      .Returns(businessConfiguratorAbsractFactoryMock.Object);

            CrmServiceProvider.Load(crmServiceProviderMock.Object);
        }

        #endregion

        [Test]
        public void Configure_ShouldCallGetServiceOfCrmServiceProviderForCrmPluginEventExtractor()
        {
            m_crmBusiness.Configure(TestContext);

            crmServiceProviderMock.Verify(x => x.GetService(typeof(ICrmPluginEventExtractor)), Times.Once);
        }

        [Test]
        public void Configure_ShouldCallGetPluginEventOfCrmPluginEventExtractor()
        {
            m_crmBusiness.Configure(TestContext);

            crmPluginEventExtractorMock.Verify(x => x.GetPluginEvent(TestContext), Times.Once);
        }

        [Test]
        public void Configure_ShouldCallGetServiceOfCrmServiceProviderForBusinessConfiguratorAbsractFactory()
        {
            m_crmBusiness.Configure(TestContext);

            crmServiceProviderMock.Verify(x => x.GetService(typeof(IBusinessConfiguratorAbsractFactory)), Times.AtLeastOnce);
        }

        [Test]
        public void Configure_ShouldCallGetFactoryOfBusinessConfiguratorAbsractFactory()
        {
            m_crmBusiness.Configure(TestContext);

            businessConfiguratorAbsractFactoryMock.Verify(x => x.GetFactory<Entity>(), Times.AtLeastOnce);
        }

        [Test]
        public void Configure_ShouldCallGetConfiguratorOfBusinessConfiguratorFactory()
        {
            m_crmBusiness.Configure(TestContext);

            businessConfiguratorFactoryMock.As<IBusinessConfiguratorFactory>()
                                           .Verify(x => x.GetConfigurator(TestCrmPluginEvent), Times.AtLeastOnce);
        }

        [Test]
        public void Configure_ShouldCallConfigureOfBusinessConfigurator()
        {
            m_crmBusiness.Configure(TestContext);

            businessConfiguratorMock.Verify(x => x.Configure(TestContext), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallExecuteOfCrmBusinessAgents()
        {
            m_crmBusiness.Configure(TestContext);

            m_crmBusiness.Execute(TestContext);

            crmBusinessAgentMock.Verify(x => x.Execute(TestContext), Times.Exactly(2));
        }

        [Test]
        public void Dispose_ShouldCallDisposeOfCrmBusinessAgents()
        {
            m_crmBusiness.Configure(TestContext);

            m_crmBusiness.Dispose();

            crmBusinessAgentMock.Verify(x => x.Dispose(), Times.Exactly(2));
        }
    }
}
