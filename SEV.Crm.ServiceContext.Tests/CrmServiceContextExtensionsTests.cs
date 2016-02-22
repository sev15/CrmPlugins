using System;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Moq;
using NUnit.Framework;
using SEV.Crm.ServiceContext.Extensions;

namespace SEV.Crm.ServiceContext.Tests
{
    public class CrmServiceContextExtensionsTests
    {
        private const string TestName = "test_entity_name";
// ReSharper disable InconsistentNaming
        private static readonly Guid TestGuid = Guid.NewGuid();
// ReSharper restore InconsistentNaming

        private Mock<IOrganizationServiceCache> m_serviceCacheMock;

        private ICrmServiceContext m_context;

        [SetUp]
        public void Init()
        {
            m_serviceCacheMock = new Mock<IOrganizationServiceCache>();
        }

        [Test]
        public void RemoveCachedData_ShouldNotCallRemoveOfOrganizationServiceCache_WhenContextIsOfWrongType()
        {
            var contextMock = new Mock<ICrmServiceContext>();
            m_context = contextMock.Object;

            m_context.RemoveCachedData(TestName, TestGuid);

            m_serviceCacheMock.Verify(x => x.Remove(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
        }

        [Test]
        public void RemoveCachedData_ShouldNotCallRemoveOfOrganizationServiceCache_WhenInternalServiceIsNotOfCachedOrganizationServiceType()
        {
            var contextMock = new Mock<ICrmServiceContext>();
            contextMock.As<IOrganizationServiceContainer>().SetupGet(x => x.Service).Returns((IOrganizationService)null);
            m_context = contextMock.Object;

            m_context.RemoveCachedData(TestName, TestGuid);

            m_serviceCacheMock.Verify(x => x.Remove(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
        }

        [Test]
        public void RemoveCachedData_ShouldNotCallRemoveOfOrganizationServiceCache_WhenOrganizationServiceCacheIsNotConfigured()
        {
            var contextMock = new Mock<ICrmServiceContext>();
            contextMock.As<IOrganizationServiceContainer>().SetupGet(x => x.Service)
                                                           .Returns(new CachedOrganizationService("Xrm", null));
            m_context = contextMock.Object;

            m_context.RemoveCachedData(TestName, TestGuid);

            m_serviceCacheMock.Verify(x => x.Remove(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
        }

        [Test]
        public void RemoveCachedData_ShouldCallRemoveOfOrganizationServiceCache_WhenOrganizationServiceCacheIsConfigured()
        {
            var contextMock = new Mock<ICrmServiceContext>();
            contextMock.As<IOrganizationServiceContainer>().SetupGet(x => x.Service)
                       .Returns(new CachedOrganizationService("Xrm", m_serviceCacheMock.Object));
            m_context = contextMock.Object;

            m_context.RemoveCachedData(TestName, TestGuid);

            m_serviceCacheMock.Verify(x => x.Remove(TestName, TestGuid), Times.Once);
        }
    }
}