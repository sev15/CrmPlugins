using System;
using Moq;
using NUnit.Framework;

namespace SEV.Crm.Business.Agents.Tests
{
    [TestFixture]
    public class CrmBusinessAgentTests
    {
        private IBusinessAgent m_businessAgent;

        [SetUp]
        public void Init()
        {
            var businessAgentMock = new Mock<CrmBusinessAgent> { CallBase = true };
            m_businessAgent = businessAgentMock.Object;
        }

        [Test]
        public void CanSetAndGetExecutionOrder()
        {
            const int executionOrder = 11;

            m_businessAgent.ExecutionOrder = executionOrder;

            Assert.That(m_businessAgent.ExecutionOrder, Is.EqualTo(executionOrder));
        }

        [Test]
        public void CanSetAndGetContext()
        {
            const string testObj = "test object";

            m_businessAgent.Context = new object[] { testObj };

            Assert.That(m_businessAgent.Context, Is.Not.Null);
            Assert.That(m_businessAgent.Context[0], Is.SameAs(testObj));
        }

        [Test]
        public void Execute_ShouldThrowInvalidOperationException_WhenExecutionOrderIsNotInitialized()
        {
            var context = new Mock<IPluginExecutorContext>().Object;

            Assert.That(() => m_businessAgent.Execute(context), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Execute_ShouldNotThrowExceptions_WhenExecutionOrderIsInitialized()
        {
            m_businessAgent.ExecutionOrder = 1;
            var context = new Mock<IPluginExecutorContext>().Object;

            Assert.That(() => m_businessAgent.Execute(context), Throws.Nothing);
        }
    }
}
