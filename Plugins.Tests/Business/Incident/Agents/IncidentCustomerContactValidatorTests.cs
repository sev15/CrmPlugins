using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using Moq;
using NUnit.Framework;
using Sample.Crm.Business.Agents;
using Sample.Crm.Business.Agents.Tests;
using Sample.Crm.Entities;

namespace Sample.Crm.Business.Tests.Configurators
{
    public class IncidentCustomerContactValidatorTests : CrmBusinessAgentTestBase
    {
// ReSharper disable InconsistentNaming
        private static readonly Guid TEST_ID = Guid.NewGuid();
// ReSharper restore InconsistentNaming

        private Incident m_incident;
        private Account m_account;

        #region SetUp

        [SetUp]
        public void Init()
        {
            BusinessAgent = new IncidentCustomerContactValidator
            {
                ExecutionOrder = 1, Context = new object[] { CrmServiceContextMock.Object }
            };
        }

        protected override void InitializaMocks()
        {
            base.InitializaMocks();

            m_incident = new Incident
            {
                Id = Guid.NewGuid(),
                ResponsibleContactId = new CrmEntityReference("", Guid.NewGuid())
            };
            ExecutorContextMock.Setup(x => x.GetTargetEntity<Incident>()).Returns(m_incident);

            MockAccountQuery();
        }

        private void MockAccountQuery()
        {
            m_account = new Account { Id = TEST_ID, PrimaryContactId = m_incident.ResponsibleContactId };
            var accounts = new List<Account>
            {
                m_account, new Account { Id = Guid.NewGuid() }, new Account { Id = Guid.NewGuid() }
            };
            var queryable = accounts.AsQueryable();
            var queryableMock = new Mock<IQueryable<Account>>();
            queryableMock.Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator());
            queryableMock.Setup(x => x.Provider).Returns(queryable.Provider);
            queryableMock.Setup(x => x.ElementType).Returns(queryable.ElementType);
            queryableMock.Setup(x => x.Expression).Returns(queryable.Expression);
            CrmServiceContextMock.Setup(x => x.CreateQuery<Account>()).Returns(queryableMock.Object);
        }

        #endregion

        [Test]
        public void Execute_ShouldCallGetTargetEntityOfPluginContext()
        {
            m_incident.CustomerId = new CrmEntityReference("", TEST_ID);

            BusinessAgent.Execute(ExecutorContextMock.Object);

            ExecutorContextMock.Verify(x => x.GetTargetEntity<Incident>(), Times.Once);
        }

        [Test]
        public void Execute_ShouldCallGetPreEntityImageOfPluginContext_WhenCustomerIdOfTargetEntityIsNotSpecified()
        {
            var incident = new Incident
            {
                Id = Guid.NewGuid(), CustomerId = new CrmEntityReference("", TEST_ID)
            };
            ExecutorContextMock.Setup(x => x.GetPreEntityImage<Incident>()).Returns(incident);

            BusinessAgent.Execute(ExecutorContextMock.Object);

            ExecutorContextMock.Verify(x => x.GetPreEntityImage<Incident>(), Times.Once);
        }

        [Test]
        public void Execute_ShouldQueryCrmServiceContextForAccounts()
        {
            m_incident.CustomerId = new CrmEntityReference("", TEST_ID);

            BusinessAgent.Execute(ExecutorContextMock.Object);

            CrmServiceContextMock.Verify(x => x.CreateQuery<Account>(), Times.Once);
        }

        [Test]
        public void Execute_ShouldThrowInvalidPluginExecution_WhenResponsibleContactOfIncidentIsNotEqualToPrimaryContactOfItsCustomerAccount()
        {
            m_incident.CustomerId = new CrmEntityReference("", TEST_ID);
            m_account.PrimaryContactId = new CrmEntityReference("", Guid.NewGuid());

            Assert.That(() => BusinessAgent.Execute(ExecutorContextMock.Object),
                                                            Throws.InstanceOf<InvalidPluginExecutionException>());
        }
    }
}