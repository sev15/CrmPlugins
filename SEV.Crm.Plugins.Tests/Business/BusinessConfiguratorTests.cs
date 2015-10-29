using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Moq;
using NUnit.Framework;
using SEV.Crm.Business.Agents;
using SEV.Crm.ServiceContext;
using SEV.Crm.Services;

namespace SEV.Crm.Business.Configurators.Tests
{
    [TestFixture]
    public class BusinessConfiguratorTests
    {
        private Mock<ICrmServiceProvider> m_crmServiceProviderMock;
        private Mock<IPluginExecutorContext> m_executorContextMock;
        private BusinessConfigurator m_configurator;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitMocks();

            m_configurator = new TestBusinessConfigurator();
        }

        private void InitMocks()
        {
            m_executorContextMock = new Mock<IPluginExecutorContext>();
            m_crmServiceProviderMock = new Mock<ICrmServiceProvider>();
            m_crmServiceProviderMock.Setup(x =>
                x.GetService(typeof(ICrmServiceContextFactory))).Returns(new Mock<ICrmServiceContextFactory>().Object);
            CrmServiceProvider.Load(m_crmServiceProviderMock.Object);
        }

        #endregion

        [Test]
        public void CreateCrmServiceContext_ShouldCallGetOrganizationServiceOfPluginExecutorContext()
        {
            ((TestBusinessConfigurator)m_configurator).CreateCrmServiceContext(m_executorContextMock.Object);

            m_executorContextMock.VerifyGet(x => x.OrganizationService, Times.Once);
        }

        [Test]
        public void CreateCrmServiceContext_ShouldCallGetServiceOfCrmServiceProviderForCrmServiceContextFactory()
        {
            ((TestBusinessConfigurator)m_configurator).CreateCrmServiceContext(m_executorContextMock.Object);

            m_crmServiceProviderMock.Verify(x => x.GetService(typeof(ICrmServiceContextFactory)), Times.Once);
        }

        [Test]
        public void CreateCrmServiceContext_ShouldCallCreateServiceContextOfCrmServiceContextFactory()
        {
            var organizationServiceMock = new Mock<IOrganizationService>();
            m_executorContextMock.SetupGet(x => x.OrganizationService).Returns(organizationServiceMock.Object);
            var serviceContextFactoryMock = new Mock<ICrmServiceContextFactory>();
            m_crmServiceProviderMock.Setup(x =>
                        x.GetService(typeof(ICrmServiceContextFactory))).Returns(serviceContextFactoryMock.Object);

            ((TestBusinessConfigurator)m_configurator).CreateCrmServiceContext(m_executorContextMock.Object);

            serviceContextFactoryMock.Verify(x => x.CreateServiceContext(organizationServiceMock.Object), Times.Once);
        }

        [Test]
        public void Configure_ShouldBeImplemented()
        {
            Assert.That(() => m_configurator.Configure(m_executorContextMock.Object),
                                                                Throws.Exception.TypeOf<NotImplementedException>());
        }

        private class TestBusinessConfigurator : BusinessConfigurator
        {
            // ReSharper disable UnusedMethodReturnValue.Local
            public new ICrmServiceContext CreateCrmServiceContext(IPluginExecutorContext executorContext)
            // ReSharper restore UnusedMethodReturnValue.Local
            {
                return base.CreateCrmServiceContext(executorContext);
            }

            public override IEnumerable<IBusinessAgent> Configure(IPluginExecutorContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
