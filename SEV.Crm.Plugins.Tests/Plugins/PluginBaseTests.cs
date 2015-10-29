using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SEV.Crm.Business;
using SEV.Crm.Services;

namespace SEV.Crm.Plugins.Tests
{
    [TestFixture]
    public class PluginBaseTests
    {
        private IServiceProvider serviceProvider;
        private Mock<ITracingService> tracingServiceMock;
        private Mock<ICrmServiceProvider> crmServiceProviderMock;
        private Mock<IPluginContextFactory> pluginContextFactoryMock;
        private Mock<IPluginExecutorContext> pluginContextMock;
        private Mock<IPluginExecutorFactory> pluginExecutorFactoryMock;
        private Mock<IBusinessConfiguratorAbsractFactory> businessConfiguratorAbsractFactoryMock;
        private Mock<IPluginExecutor> pluginExecutorMock;
        private PluginBase m_plugin;

        #region SetUp

        [TestFixtureSetUp]
        public void InitFixure()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProvider = serviceProviderMock.Object;
        }

        [SetUp]
        public void Init()
        {
            var pluginMock = new Mock<PluginBase>(typeof(Entity)) { CallBase = true };
            pluginMock.Protected().Setup<ICrmServiceProvider>("GetServiceProvider")
                                  .Returns(CreateCrmServiceProviderMock());
            m_plugin = pluginMock.Object;
        }

        private ICrmServiceProvider CreateCrmServiceProviderMock()
        {
            pluginContextFactoryMock = new Mock<IPluginContextFactory>();
            pluginContextMock = new Mock<IPluginExecutorContext>();
            tracingServiceMock = new Mock<ITracingService>();
            pluginContextMock.SetupGet(x => x.TracingService)
                             .Returns(tracingServiceMock.Object);
            pluginContextFactoryMock.Setup(x => x.CreateContext(serviceProvider, typeof(Entity)))
                                    .Returns(pluginContextMock.Object);
            pluginExecutorFactoryMock = new Mock<IPluginExecutorFactory>();
            pluginExecutorMock = new Mock<IPluginExecutor>();
            pluginExecutorMock.Setup(x => x.Execute(pluginContextMock.Object)).Callback(PluginExecutorCallback);
            pluginExecutorFactoryMock.Setup(x => x.GetExecutor(typeof(Entity)))
                                     .Returns(pluginExecutorMock.Object);
            businessConfiguratorAbsractFactoryMock = new Mock<IBusinessConfiguratorAbsractFactory>();
            crmServiceProviderMock = new Mock<ICrmServiceProvider>();
            crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IPluginContextFactory))))
                               .Returns(pluginContextFactoryMock.Object);
            crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IPluginExecutorFactory))))
                               .Returns(pluginExecutorFactoryMock.Object);
            crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IBusinessConfiguratorAbsractFactory))))
                                  .Returns(businessConfiguratorAbsractFactoryMock.Object);

            return crmServiceProviderMock.Object;
        }

        private bool m_throwException;
        private Action pluginExecutorCallbackAction;

        private void PluginExecutorCallback()
        {
            if (m_throwException)
            {
                pluginExecutorCallbackAction.Invoke();
            }
        }

        #endregion

        [Test]
        public void Execute_ShouldCallGetServiceOfCrmServiceProviderForPluginContextFactory()
        {
            m_plugin.Execute(serviceProvider);

            crmServiceProviderMock.Verify(x => x.GetService(typeof(IPluginContextFactory)), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallCreateContextOfPluginContextFactory()
        {
            m_plugin.Execute(serviceProvider);

            pluginContextFactoryMock.Verify(x => x.CreateContext(serviceProvider, typeof(Entity)), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallGetServiceOfCrmServiceProviderForPluginExecutorFactory()
        {
            m_plugin.Execute(serviceProvider);

            crmServiceProviderMock.Verify(x => x.GetService(typeof(IPluginExecutorFactory)), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallGetExecutorOfPluginExecutorFactory()
        {
            m_plugin.Execute(serviceProvider);

            pluginExecutorFactoryMock.Verify(x => x.GetExecutor(typeof(Entity)), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallConfigureOfPluginExecutor()
        {
            m_plugin.Execute(serviceProvider);

            pluginExecutorMock.Verify(x => x.Configure(pluginContextMock.Object), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallExecuteOfPluginExecutor()
        {
            m_plugin.Execute(serviceProvider);

            pluginExecutorMock.Verify(x => x.Execute(pluginContextMock.Object), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallDisposeOfPluginExecutor()
        {
            m_plugin.Execute(serviceProvider);

            pluginExecutorMock.Verify(x => x.Dispose(), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldHandleException_WhenPluginExecutorTrowsExceptionAndTheExceptionIsNotInvalidPluginExecutionException()
        {
            m_throwException = true;
            pluginExecutorCallbackAction = () => { throw new Exception(); };

            Assert.That(() => m_plugin.Execute(serviceProvider),
                Throws.Exception.TypeOf<FaultException<OrganizationServiceFault>>());
            tracingServiceMock.Verify(x => x.Trace(It.IsAny<string>()), Times.Exactly(2));

            m_throwException = false;
            pluginExecutorCallbackAction = null;
        }

        [Test]
        public void Execute_ShouldReThrowException_WhenPluginExecutorTrowsExceptionAndTheExceptionIsInvalidPluginExecutionException()
        {
            m_throwException = true;
            pluginExecutorCallbackAction = () => { throw new InvalidPluginExecutionException(); };

            Assert.That(() => m_plugin.Execute(serviceProvider), Throws.Exception.TypeOf<InvalidPluginExecutionException>());

            m_throwException = false;
            pluginExecutorCallbackAction = null;
        }

        [Test]
        public void PluginCreation_ShouldInitializeCrmServiceProvider()
        {
            m_plugin = new Mock<PluginBase>(typeof(Entity)) { CallBase = true }.Object;

            Assert.That(CrmServiceProvider.Current.GetService(typeof(IPluginContextFactory)),
                        Is.InstanceOf<PluginContextFactory>());
            Assert.That(CrmServiceProvider.Current.GetService(typeof(IPluginExecutorFactory)),
                       Is.InstanceOf<PluginExecutorFactory>());
            Assert.That(CrmServiceProvider.Current.GetService(typeof(IBusinessConfiguratorAbsractFactory)),
                       Is.InstanceOf<BusinessConfiguratorAbsractFactory>());
            Assert.That(CrmServiceProvider.Current.GetService(typeof(ICrmPluginEventExtractor)),
                       Is.InstanceOf<CrmPluginEventExtractor>());
            Assert.That(CrmServiceProvider.Current.GetService(typeof(ICrmCache)),
                       Is.InstanceOf<CrmDeleteCache>());
        }
    }
}
