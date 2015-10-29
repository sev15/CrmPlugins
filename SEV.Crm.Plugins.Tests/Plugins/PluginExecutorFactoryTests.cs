using System;
using Microsoft.Xrm.Sdk;
using Moq;
using NUnit.Framework;
using SEV.Crm.Business;

namespace SEV.Crm.Plugins.Tests
{
    [TestFixture]
    public class PluginExecutorFactoryTests
    {
        private IPluginExecutorFactory m_executorFactory;

        [SetUp]
        public void Init()
        {
            m_executorFactory = new PluginExecutorFactory();
        }

        [Test]
        public void AddExecutor_ShouldAddNewPluginExecutorInPluginExecutorFactory()
        {
            var executor = new Mock<IPluginExecutor>().Object;

            m_executorFactory.AddExecutor(typeof(Entity), executor);

            Assert.That(m_executorFactory.GetExecutor(typeof(Entity)), Is.Not.Null);
        }

        [Test]
        public void AddExecutor_ShouldNotAddNewPluginExecutorInPluginExecutorFactory_WhenPluginExecutorForTheSameEntityTypeIsAlreadyAdded()
        {
            m_executorFactory.AddExecutor(typeof(Entity), new Mock<IPluginExecutor>().Object);
            var executor = new Mock<IPluginExecutor>().Object;

            m_executorFactory.AddExecutor(typeof(Entity), executor);

            Assert.That(m_executorFactory.GetExecutor(typeof(Entity)), Is.Not.SameAs(executor));
        }

        [Test]
        public void GetExecutor_ShouldThrowInvalidOperationException_WhenThereAreNoAddedPluginExecutors()
        {
            Assert.That(() => m_executorFactory.GetExecutor(typeof(Entity)), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void GetExecutor_ShouldReturnTheSamePluginExecutor_WhenCalledForTheSameEntityType()
        {
            m_executorFactory.AddExecutor(typeof(Entity), new Mock<IPluginExecutor>().Object);
            var executor = m_executorFactory.GetExecutor(typeof(Entity));

            IPluginExecutor anotherExecutor = m_executorFactory.GetExecutor(typeof(Entity));

            Assert.That(anotherExecutor, Is.SameAs(executor));
        }
    }
}
