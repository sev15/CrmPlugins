using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SEV.Crm.Business;
using SEV.Crm.Business.Agents;
using SEV.Crm.ServiceContext;
using SEV.Crm.Services;

namespace SEV.Crm.Plugins.Tests
{
    [TestFixture]
    public class PluginBaseTests
    {
        private IServiceProvider m_serviceProvider;
        private Mock<ITracingService> m_tracingServiceMock;
        private Mock<ICrmServiceProvider> m_crmServiceProviderMock;
        private Mock<IPluginContextFactory> m_pluginContextFactoryMock;
        private Mock<IPluginExecutorContext> m_pluginContextMock;
        private Mock<IPluginExecutorFactory> m_pluginExecutorFactoryMock;
        private Mock<IBusinessConfiguratorAbsractFactory> m_businessConfiguratorAbsractFactoryMock;
        private Mock<IPluginExecutor> m_pluginExecutorMock;
        private IEnumerable<IBusinessAgent> m_businessAgents;
        private PluginBase m_plugin;

        #region SetUp

        [TestFixtureSetUp]
        public void InitFixure()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            m_serviceProvider = serviceProviderMock.Object;
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
            m_pluginContextFactoryMock = new Mock<IPluginContextFactory>();
            m_pluginContextMock = new Mock<IPluginExecutorContext>();
            m_tracingServiceMock = new Mock<ITracingService>();
            m_pluginContextMock.SetupGet(x => x.TracingService)
                               .Returns(m_tracingServiceMock.Object);
            m_pluginContextFactoryMock.Setup(x => x.CreateContext(m_serviceProvider, typeof(Entity)))
                                      .Returns(m_pluginContextMock.Object);
            m_pluginExecutorFactoryMock = new Mock<IPluginExecutorFactory>();
            m_pluginExecutorMock = new Mock<IPluginExecutor>();
            m_businessAgents = new IBusinessAgent[0];
            m_pluginExecutorMock.Setup(x => x.Configure(m_pluginContextMock.Object)).Returns(m_businessAgents);
            m_pluginExecutorMock.Setup(x => x.Execute(m_pluginContextMock.Object, m_businessAgents))
                                .Callback(PluginExecutorCallback);
            m_pluginExecutorFactoryMock.Setup(x => x.GetExecutor(typeof(Entity)))
                                       .Returns(m_pluginExecutorMock.Object);
            m_businessConfiguratorAbsractFactoryMock = new Mock<IBusinessConfiguratorAbsractFactory>();
            m_crmServiceProviderMock = new Mock<ICrmServiceProvider>();
            m_crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IPluginContextFactory))))
                                    .Returns(m_pluginContextFactoryMock.Object);
            m_crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IPluginExecutorFactory))))
                                    .Returns(m_pluginExecutorFactoryMock.Object);
            m_crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IBusinessConfiguratorAbsractFactory))))
                                    .Returns(m_businessConfiguratorAbsractFactoryMock.Object);

            return m_crmServiceProviderMock.Object;
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
            m_plugin.Execute(m_serviceProvider);

            m_crmServiceProviderMock.Verify(x => x.GetService(typeof(IPluginContextFactory)), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallCreateContextOfPluginContextFactory()
        {
            m_plugin.Execute(m_serviceProvider);

            m_pluginContextFactoryMock.Verify(x => x.CreateContext(m_serviceProvider, typeof(Entity)), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallGetServiceOfCrmServiceProviderForPluginExecutorFactory()
        {
            m_plugin.Execute(m_serviceProvider);

            m_crmServiceProviderMock.Verify(x => x.GetService(typeof(IPluginExecutorFactory)), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallGetExecutorOfPluginExecutorFactory()
        {
            m_plugin.Execute(m_serviceProvider);

            m_pluginExecutorFactoryMock.Verify(x => x.GetExecutor(typeof(Entity)), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallConfigureOfPluginExecutor()
        {
            m_plugin.Execute(m_serviceProvider);

            m_pluginExecutorMock.Verify(x => x.Configure(m_pluginContextMock.Object), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldCallExecuteOfPluginExecutor()
        {
            m_plugin.Execute(m_serviceProvider);

            m_pluginExecutorMock.Verify(x => x.Execute(m_pluginContextMock.Object, m_businessAgents), Times.Once);
        }

        [Test]
        public void Execute_ShouldHandleException_WhenPluginExecutorTrowsExceptionAndTheExceptionIsNotInvalidPluginExecutionException()
        {
            m_throwException = true;
            pluginExecutorCallbackAction = () => { throw new Exception(); };

            Assert.That(() => m_plugin.Execute(m_serviceProvider),
                Throws.Exception.TypeOf<FaultException<OrganizationServiceFault>>());
            m_tracingServiceMock.Verify(x => x.Trace(It.IsAny<string>()), Times.Exactly(2));

            m_throwException = false;
            pluginExecutorCallbackAction = null;
        }

        [Test]
        public void Execute_ShouldReThrowException_WhenPluginExecutorTrowsExceptionAndTheExceptionIsInvalidPluginExecutionException()
        {
            m_throwException = true;
            pluginExecutorCallbackAction = () => { throw new InvalidPluginExecutionException(); };

            Assert.That(() => m_plugin.Execute(m_serviceProvider), Throws.Exception.TypeOf<InvalidPluginExecutionException>());

            m_throwException = false;
            pluginExecutorCallbackAction = null;
        }

        [Test]
        public void PluginCreation_ShouldInitializeCrmServiceProvider()
        {
            m_plugin = new Mock<PluginBase>(typeof(Entity)) { CallBase = true }.Object;

            Assert.That(CrmServiceProvider.Current.GetService(typeof(ICrmServiceContextFactory)),
                                                                Is.InstanceOf<CrmServiceContextFactory>());
            Assert.That(CrmServiceProvider.Current.GetService(typeof(IPluginContextFactory)),
                                                                Is.InstanceOf<PluginContextFactory>());
            Assert.That(CrmServiceProvider.Current.GetService(typeof(IPluginExecutorFactory)),
                                                                Is.InstanceOf<PluginExecutorFactory>());
            Assert.That(CrmServiceProvider.Current.GetService(typeof(IBusinessConfiguratorAbsractFactory)),
                                                                Is.InstanceOf<BusinessConfiguratorAbsractFactory>());
            Assert.That(CrmServiceProvider.Current.GetService(typeof(ICrmPluginEventExtractor)),
                                                                Is.InstanceOf<CrmPluginEventExtractor>());
            Assert.That(CrmServiceProvider.Current.GetService(typeof(ICrmCache)), Is.InstanceOf<CrmDeleteCache>());
        }
    }
}
