using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using Moq;
using NUnit.Framework;

namespace SEV.Crm.ServiceContext.Tests
{
    [TestFixture]
    public class CrmServiceContextFactoryTests
    {
        private ICrmServiceContextFactory m_factory;

        [SetUp]
        public void Init()
        {
            m_factory = new CrmServiceContextFactory();
        }

        [Test]
        public void CreateServiceContext_ShouldReturnInstanceOfCrmServiceContext()
        {
            // Must have connectionString with the name "Xrm" in the config file.
            var context = m_factory.CreateServiceContext();

            Assert.That(context, Is.Not.Null);
            Assert.That(context, Is.InstanceOf<ICrmServiceContext>());
        }

        [Test]
        public void CreateServiceContext_Service_ShouldReturnInstanceOfCrmServiceContext()
        {
            var organizationService = new Mock<IOrganizationService>();

            var context = m_factory.CreateServiceContext(organizationService.Object);

            Assert.That(context, Is.Not.Null);
            Assert.That(context, Is.InstanceOf<ICrmServiceContext>());
        }

        [Test]
        public void CreateServiceContext_CrmConnection_ShouldReturnInstanceOfCrmServiceContext()
        {
            // Must have connectionString with the name "Xrm" in the config file.
            var connection = new CrmConnection("Xrm");

            var context = m_factory.CreateServiceContext(connection);

            Assert.That(context, Is.Not.Null);
            Assert.That(context, Is.InstanceOf<ICrmServiceContext>());
        }
    }
}