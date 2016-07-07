using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Moq;
using NUnit.Framework;
using SEV.Crm.Business;

namespace SEV.Crm.Plugins.Tests
{
    [TestFixture]
    public class PluginContextTests
    {
        private static readonly Guid TestId = Guid.NewGuid();
        private static readonly Entity TestEntity = new Entity();
        private readonly ParameterCollection TestParameterCollection =
            new ParameterCollection { new KeyValuePair<string, object>(PluginContext.TargetEntityAlias, TestEntity) };

        private Mock<ITracingService> tracingServiceMock;
        private Mock<IPluginExecutionContext> pluginExecutionContextMock;
        private Mock<IOrganizationServiceFactory> organizationServiceFactoryMock;
        private Mock<IOrganizationService> organizationServiceMock;
        private Mock<IServiceProvider> crmServiceProviderMock;
        private IPluginExecutorContext m_pluginContext;

        #region SetUp

        [SetUp]
        public void Init()
        {
            MockCrmServiceProvider();

            m_pluginContext = new PluginContext(crmServiceProviderMock.Object, typeof(Entity));
        }

        private void MockCrmServiceProvider()
        {
            tracingServiceMock = new Mock<ITracingService>();
            pluginExecutionContextMock = MockPluginExecutionContext();
            organizationServiceMock = new Mock<IOrganizationService>();
            organizationServiceFactoryMock = new Mock<IOrganizationServiceFactory>();
            organizationServiceFactoryMock.Setup(x => x.CreateOrganizationService(TestId))
                                          .Returns(organizationServiceMock.Object);
            organizationServiceFactoryMock.As<IProxyTypesAssemblyProvider>();

            crmServiceProviderMock = new Mock<IServiceProvider>();
            crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(ITracingService))))
                                  .Returns(tracingServiceMock.Object);
            crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IPluginExecutionContext))))
                                  .Returns(pluginExecutionContextMock.Object);
            crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IOrganizationServiceFactory))))
                                  .Returns(organizationServiceFactoryMock.Object);
        }

        private Mock<IPluginExecutionContext> MockPluginExecutionContext()
        {
            var mock = new Mock<IPluginExecutionContext>();

            mock.SetupGet(x => x.UserId).Returns(TestId);
            mock.SetupGet(x => x.InputParameters).Returns(TestParameterCollection);
            var testEntityImageCollection =
                new EntityImageCollection() { new KeyValuePair<string, Entity>(PluginContext.ImageEntityAlias, TestEntity) };
            mock.SetupGet(x => x.PreEntityImages).Returns(testEntityImageCollection);
            mock.SetupGet(x => x.PostEntityImages).Returns(testEntityImageCollection);

            return mock;
        }

        #endregion

        [Test]
        public void GetTracingService_ShouldCallGetServiceOfCrmServiceProviderForTracingService()
        {
            ITracingService service = m_pluginContext.TracingService;

            crmServiceProviderMock.Verify(x => x.GetService(typeof(ITracingService)), Times.AtLeastOnce);
            Assert.That(service, Is.SameAs(tracingServiceMock.Object));
        }

        [Test]
        public void GetPluginExecutionContext_ShouldCallGetServiceOfCrmServiceProviderForPluginExecutionContext()
        {
            IPluginExecutionContext context = m_pluginContext.PluginExecutionContext;

            crmServiceProviderMock.Verify(x => x.GetService(typeof(IPluginExecutionContext)), Times.AtLeastOnce);
            Assert.That(context, Is.SameAs(pluginExecutionContextMock.Object));
        }

        [Test]
        public void GetOrganizationService_ShouldCallGetServiceOfCrmServiceProviderForOrganizationServiceFactory()
        {
            IOrganizationService service = m_pluginContext.OrganizationService;

            crmServiceProviderMock.Verify(x => x.GetService(typeof(IOrganizationServiceFactory)), Times.AtLeastOnce);
            Assert.That(service, Is.Not.Null);
        }

        [Test]
        public void GetOrganizationService_ShouldCallCreateOrganizationServiceOfOrganizationServiceFactory()
        {
            IOrganizationService service = m_pluginContext.OrganizationService;

            organizationServiceFactoryMock.Verify(x => x.CreateOrganizationService(TestId), Times.AtLeastOnce);
            Assert.That(service, Is.SameAs(organizationServiceMock.Object));
        }

        [Test]
        public void GetTargetEntity_ShouldCallGetInputParametersOfPluginExecutionContextForTargetAlias()
        {
            Entity entity = m_pluginContext.GetTargetEntity<Entity>();

            Assert.That(entity, Is.Not.Null);
            pluginExecutionContextMock.VerifyGet(x => x.InputParameters);
        }

        [Test]
        public void GetTargetEntityReference_ShouldCallGetInputParametersOfPluginExecutionContextForTargetAlias()
        {
            TestParameterCollection[PluginContext.TargetEntityAlias] = new EntityReference();

            EntityReference entityRef = m_pluginContext.GetTargetEntityReference();

            pluginExecutionContextMock.VerifyGet(x => x.InputParameters);
            Assert.That(entityRef, Is.Not.Null);

            TestParameterCollection[PluginContext.TargetEntityAlias] = TestEntity;
        }

        [Test]
        public void GetPreEntityImage_ShouldCallPreEntityImagesOfPluginExecutionContextForEntityAlias()
        {
            Entity entity = m_pluginContext.GetPreEntityImage<Entity>();

            pluginExecutionContextMock.VerifyGet(x => x.PreEntityImages);
            Assert.That(entity, Is.Not.Null);
        }

        [Test]
        public void GetPostEntityImage_ShouldCallPostEntityImagesOfPluginExecutionContextForEntityAlias()
        {
            Entity entity = m_pluginContext.GetPostEntityImage<Entity>();

            pluginExecutionContextMock.VerifyGet(x => x.PostEntityImages);
            Assert.That(entity, Is.Not.Null);
        }
    }
}
