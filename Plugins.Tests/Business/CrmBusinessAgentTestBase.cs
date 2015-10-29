using Moq;
using NUnit.Framework;
using SEV.Crm.Business;
using SEV.Crm.Business.Agents;
using SEV.Crm.ServiceContext;
using SEV.Crm.Services;

namespace Sample.Crm.Business.Agents.Tests
{
    public class CrmBusinessAgentTestBase
    {
        protected Mock<IPluginExecutorContext> ExecutorContextMock;
        protected Mock<ICrmServiceContext> CrmServiceContextMock;
        protected Mock<ICrmServiceProvider> CrmServiceProviderMock;
        protected IBusinessAgent BusinessAgent;

        [SetUp]
        public void InitFixure()
        {
            InitializaMocks();
        }

        protected virtual void InitializaMocks()
        {
            ExecutorContextMock = new Mock<IPluginExecutorContext>();
            CrmServiceContextMock = new Mock<ICrmServiceContext>();
            CrmServiceProviderMock = new Mock<ICrmServiceProvider>();

            CrmServiceProvider.Load(CrmServiceProviderMock.Object);
        }
    }
}