using Moq;
using NUnit.Framework;
using SEV.Crm.Business;
using SEV.Crm.Business.Configurators;
using SEV.Crm.ServiceContext;
using SEV.Crm.Services;

namespace Sample.Crm.Business.Configurators.Tests
{
    public abstract class BusinessConfiguratorTestBase
    {
        protected Mock<IPluginExecutorContext> ExecutorContextMock;
        protected Mock<ICrmServiceContext> CrmServiceContextMock;
        protected Mock<ICrmServiceProvider> CrmServiceProviderMock;
        protected BusinessConfigurator Configurator;

        [SetUp]
        public void InitFixure()
        {
            MockServiceProviders();
        }

        protected virtual void MockServiceProviders()
        {
            ExecutorContextMock = new Mock<IPluginExecutorContext>();
            var organizationServiceMock = new Mock<Microsoft.Xrm.Sdk.IOrganizationService>();
            ExecutorContextMock.SetupGet(x => x.OrganizationService).Returns(organizationServiceMock.Object);
            CrmServiceContextMock = new Mock<ICrmServiceContext>();
            var crmServiceContextfactoryMock = new Mock<ICrmServiceContextFactory>();
            crmServiceContextfactoryMock.Setup(x => x.CreateServiceContext(organizationServiceMock.Object))
                                        .Returns(CrmServiceContextMock.Object);
            CrmServiceProviderMock = new Mock<ICrmServiceProvider>();
            CrmServiceProviderMock.Setup(x => x.GetService(typeof(ICrmServiceContextFactory)))
                                  .Returns(crmServiceContextfactoryMock.Object);

            CrmServiceProvider.Load(CrmServiceProviderMock.Object);
        }
    }
}
