using System;
using Microsoft.Xrm.Sdk;
using Moq;
using NUnit.Framework;
using SEV.Crm.Business;

namespace SEV.Crm.Services.Tests
{
    [TestFixture]
    public class CrmPluginEventExtractorTests
    {
        private const string CreateMessage = "Create";
        private const string UpdateMessage = "Update";
        private const string DeleteMessage = "Delete";
        private const int PreValidate = 10;
        private const int PreOperation = 20;
        private const int PostOperation = 40;

        private Mock<IPluginExecutionContext> m_pluginExecutionContextMock;
        private Mock<IPluginExecutorContext> m_pluginContextMock;
        private CrmPluginEvent pluginEvent;
        private ICrmPluginEventExtractor m_extractor;

        #region SetUp

        [SetUp]
        public void Init()
        {
            InitializeMocks();

            m_extractor = new CrmPluginEventExtractor();
        }

        private void InitializeMocks()
        {
            var tracingServiceMock = new Mock<ITracingService>();
            m_pluginExecutionContextMock = new Mock<IPluginExecutionContext>();
            m_pluginExecutionContextMock.SetupGet(x => x.Stage).Returns(PreValidate);
            m_pluginExecutionContextMock.SetupGet(x => x.MessageName).Returns(CreateMessage);
            var organizationServiceFactoryMock = new Mock<IOrganizationServiceFactory>() { DefaultValue = DefaultValue.Mock };
            var crmServiceProviderMock = new Mock<IServiceProvider>();
            crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(ITracingService))))
                                  .Returns(tracingServiceMock.Object);
            crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IPluginExecutionContext))))
                                  .Returns(m_pluginExecutionContextMock.Object);
            crmServiceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IOrganizationServiceFactory))))
                                  .Returns(organizationServiceFactoryMock.Object);
            m_pluginContextMock = new Mock<IPluginExecutorContext>();
            m_pluginContextMock.SetupGet(x => x.PluginExecutionContext).Returns(m_pluginExecutionContextMock.Object);
        }

        #endregion

        [Test]
        public void GetPluginEvent_ShouldCallGetPluginExecutionContextOfPluginContext()
        {
            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            m_pluginContextMock.VerifyGet(x => x.PluginExecutionContext, Times.AtLeastOnce);
        }

        [Test]
        public void GetPluginEvent_ShouldCallGetStageOfPluginExecutionContext()
        {
            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            m_pluginExecutionContextMock.VerifyGet(x => x.Stage, Times.AtLeastOnce);
        }

        [Test]
        public void GetPluginEvent_ShouldCallGetMessageNameOfPluginExecutionContext()
        {
            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            m_pluginExecutionContextMock.VerifyGet(x => x.MessageName, Times.AtLeastOnce);
        }

        [Test]
        public void GetPluginEvent_ShouldReturnPreValidateCreatePluginEvent_WhenPluginStageIsPreValidateAndCrmMessageNameIsCreate()
        {
            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            Assert.That(pluginEvent, Is.EqualTo(CrmPluginEvent.PreValidateCreate));
        }

        [Test]
        public void GetPluginEvent_ShouldReturnPreCreatePluginEvent_WhenPluginStageIsPreOperationAndCrmMessageNameIsCreate()
        {
            m_pluginExecutionContextMock.SetupGet(x => x.Stage).Returns(PreOperation);
            m_pluginExecutionContextMock.SetupGet(x => x.MessageName).Returns(CreateMessage);

            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            Assert.That(pluginEvent, Is.EqualTo(CrmPluginEvent.PreCreate));
        }

        [Test]
        public void GetPluginEvent_ShouldReturnPostCreatePluginEvent_WhenPluginStageIsPostOperationAndCrmMessageNameIsCreate()
        {
            m_pluginExecutionContextMock.SetupGet(x => x.Stage).Returns(PostOperation);
            m_pluginExecutionContextMock.SetupGet(x => x.MessageName).Returns(CreateMessage);

            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            Assert.That(pluginEvent, Is.EqualTo(CrmPluginEvent.PostCreate));
        }

        [Test]
        public void GetPluginEvent_ShouldReturnPreValidateUpdatePluginEvent_WhenPluginStageIsPreValidateAndCrmMessageNameIsUpdate()
        {
            m_pluginExecutionContextMock.SetupGet(x => x.Stage).Returns(PreValidate);
            m_pluginExecutionContextMock.SetupGet(x => x.MessageName).Returns(UpdateMessage);

            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            Assert.That(pluginEvent, Is.EqualTo(CrmPluginEvent.PreValidateUpdate));
        }

        [Test]
        public void GetPluginEvent_ShouldReturnPreUpdatePluginEvent_WhenPluginStageIsPreOperationAndCrmMessageNameIsUpdate()
        {
            m_pluginExecutionContextMock.SetupGet(x => x.Stage).Returns(PreOperation);
            m_pluginExecutionContextMock.SetupGet(x => x.MessageName).Returns(UpdateMessage);

            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            Assert.That(pluginEvent, Is.EqualTo(CrmPluginEvent.PreUpdate));
        }

        [Test]
        public void GetPluginEvent_ShouldReturnPostUpdatePluginEvent_WhenPluginStageIsPostOperationAndCrmMessageNameIsUpdate()
        {
            m_pluginExecutionContextMock.SetupGet(x => x.Stage).Returns(PostOperation);
            m_pluginExecutionContextMock.SetupGet(x => x.MessageName).Returns(UpdateMessage);

            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            Assert.That(pluginEvent, Is.EqualTo(CrmPluginEvent.PostUpdate));
        }

        [Test]
        public void GetPluginEvent_ShouldReturnPreValidateDeletePluginEvent_WhenPluginStageIsPreValidateAndCrmMessageNameIsDelete()
        {
            m_pluginExecutionContextMock.SetupGet(x => x.Stage).Returns(PreValidate);
            m_pluginExecutionContextMock.SetupGet(x => x.MessageName).Returns(DeleteMessage);

            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            Assert.That(pluginEvent, Is.EqualTo(CrmPluginEvent.PreValidateDelete));
        }

        [Test]
        public void GetPluginEvent_ShouldReturnPreDeletePluginEvent_WhenPluginStageIsPreOperationAndCrmMessageNameIsDelete()
        {
            m_pluginExecutionContextMock.SetupGet(x => x.Stage).Returns(PreOperation);
            m_pluginExecutionContextMock.SetupGet(x => x.MessageName).Returns(DeleteMessage);

            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            Assert.That(pluginEvent, Is.EqualTo(CrmPluginEvent.PreDelete));
        }

        [Test]
        public void GetPluginEvent_ShouldReturnPostDeletePluginEvent_WhenPluginStageIsPostOperationAndCrmMessageNameIsDelete()
        {
            m_pluginExecutionContextMock.SetupGet(x => x.Stage).Returns(PostOperation);
            m_pluginExecutionContextMock.SetupGet(x => x.MessageName).Returns(DeleteMessage);

            pluginEvent = m_extractor.GetPluginEvent(m_pluginContextMock.Object);

            Assert.That(pluginEvent, Is.EqualTo(CrmPluginEvent.PostDelete));
        }

        [Test]
        public void GetPluginEvent_ShouldThrowInvalidCastException_WhenPluginContextInfoIsInvalid()
        {
            const int stage = 1;
            const string eventText = "test";
            string message = String.Format(Plugins.Properties.Resources.PluginEventExtractorError, stage + eventText);
            m_pluginExecutionContextMock.SetupGet(x => x.Stage).Returns(stage);
            m_pluginExecutionContextMock.SetupGet(x => x.MessageName).Returns(eventText);

            Assert.That(() => m_extractor.GetPluginEvent(m_pluginContextMock.Object),
                                Throws.InstanceOf<InvalidCastException>().With.Message.EqualTo(message));
        }
    }
}
