using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Client;
using Moq;
using NUnit.Framework;
using Sample.Crm.Business.Agents;
using Sample.Crm.Business.Configurators;
using Sample.Crm.Business.Configurators.Tests;
using Sample.Crm.Entities;
using SEV.Crm.Business.Agents;

namespace Sample.Crm.Business.Tests.Configurators
{
    public class PreCreateIncidentBusinessConfiguratorTests : BusinessConfiguratorTestBase
    {
        private Incident m_incident;

        #region SetUp

        [SetUp]
        public void Init()
        {
            Configurator = new PreCreateIncidentBusinessConfigurator();
        }

        protected override void MockServiceProviders()
        {
            base.MockServiceProviders();

            m_incident = new Incident
            {
                Id = Guid.NewGuid()
            };
            ExecutorContextMock.Setup(x => x.GetTargetEntity<Incident>()).Returns(m_incident);
        }

        #endregion

        [Test]
        public void Configure_ShouldCallGetTargetEntityOfPluginContext()
        {
            Configurator.Configure(ExecutorContextMock.Object);

            ExecutorContextMock.Verify(x => x.GetTargetEntity<Incident>(), Times.Once);
        }

        [Test]
        public void Configure_ShouldNotReturnAnyBusinessAgent_WhenCustomerIsNotDefined()
        {
            IEnumerable<IBusinessAgent> businessAgents = Configurator.Configure(ExecutorContextMock.Object);

            Assert.That(businessAgents.Any(), Is.False);
        }

        [Test]
        public void Configure_ShouldReturnOnlyIncidentCustomerContactSetter_WhenCustomerIsDefined()
        {
            m_incident.CustomerId = new CrmEntityReference("", Guid.NewGuid());

            IEnumerable<IBusinessAgent> businessAgents = Configurator.Configure(ExecutorContextMock.Object).ToArray();

            var businessAgent = businessAgents.Single();
            Assert.That(businessAgent, Is.InstanceOf<IncidentCustomerContactSetter>());
            Assert.That(businessAgent.ExecutionOrder, Is.EqualTo(1));
            Assert.That(businessAgent.Context, Is.Not.Null);
            Assert.That(businessAgent.Context[0], Is.SameAs(CrmServiceContextMock.Object));
        }
    }
}