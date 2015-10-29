using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Client;
using Moq;
using NUnit.Framework;
using Sample.Crm.Business.Agents;
using Sample.Crm.Entities;
using SEV.Crm.Business.Agents;

namespace Sample.Crm.Business.Configurators.Tests
{
    [TestFixture]
    public class PostUpdateAccountBusinessConfiguratorTests : BusinessConfiguratorTestBase
    {
        private Account m_account;

        #region SetUp

        [SetUp]
        public void Init()
        {
            Configurator = new PostUpdateAccountBusinessConfigurator();
        }

        protected override void MockServiceProviders()
        {
            base.MockServiceProviders();

            m_account = new Account
            {
                Id = Guid.NewGuid()
            };
            ExecutorContextMock.Setup(x => x.GetTargetEntity<Account>()).Returns(m_account);
        }

        #endregion

        [Test]
        public void Configure_ShouldCallGetTargetEntityOfPluginContext()
        {
            Configurator.Configure(ExecutorContextMock.Object);

            ExecutorContextMock.Verify(x => x.GetTargetEntity<Account>(), Times.Once);
        }

        [Test]
        public void Configure_ShouldNotReturnAnyBusinessAgent_WhenPrimaryContactIsNotModified()
        {
            IEnumerable<IBusinessAgent> businessAgents = Configurator.Configure(ExecutorContextMock.Object);

            Assert.That(businessAgents.Any(), Is.False);
        }

        [Test]
        public void Configure_ShouldReturnOnlyIncidentsCustomerContactUpdater_WhenPrimaryContactIsModified()
        {
            m_account.PrimaryContactId = new CrmEntityReference("", Guid.NewGuid());

            IEnumerable<IBusinessAgent> businessAgents = Configurator.Configure(ExecutorContextMock.Object).ToArray();

            var businessAgent = businessAgents.Single();
            Assert.That(businessAgent, Is.InstanceOf<IncidentsCustomerContactUpdater>());
            Assert.That(businessAgent.ExecutionOrder, Is.EqualTo(1));
            Assert.That(businessAgent.Context, Is.Not.Null);
            Assert.That(businessAgent.Context[0], Is.SameAs(CrmServiceContextMock.Object));
        }
    }
}
