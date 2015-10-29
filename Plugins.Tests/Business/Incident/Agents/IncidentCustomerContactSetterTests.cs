using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Client;
using Moq;
using NUnit.Framework;
using Sample.Crm.Business.Agents;
using Sample.Crm.Business.Agents.Tests;
using Sample.Crm.Entities;

namespace Sample.Crm.Business.Tests.Configurators
{
    public class IncidentCustomerContactSetterTests : CrmBusinessAgentTestBase
    {
        private Incident m_incident;
        private Account m_account;

        #region SetUp

        [SetUp]
        public void Init()
        {
            BusinessAgent = new IncidentCustomerContactSetter
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
                CustomerId = new CrmEntityReference("", Guid.NewGuid())
            };
            ExecutorContextMock.Setup(x => x.GetTargetEntity<Incident>()).Returns(m_incident);

            MockAccountQuery();
        }

        private void MockAccountQuery()
        {
            m_account = new Account
            {
                Id = m_incident.CustomerId.Id, PrimaryContactId = new CrmEntityReference("", Guid.NewGuid())
            };
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
            BusinessAgent.Execute(ExecutorContextMock.Object);

            ExecutorContextMock.Verify(x => x.GetTargetEntity<Incident>(), Times.Once);
        }

        [Test]
        public void Execute_ShouldQueryCrmServiceContextForAccounts()
        {
            BusinessAgent.Execute(ExecutorContextMock.Object);

            CrmServiceContextMock.Verify(x => x.CreateQuery<Account>(), Times.Once);
        }

        [Test]
        public void Execute_ShouldSetResponsibleContactValue()
        {
            BusinessAgent.Execute(ExecutorContextMock.Object);

            Assert.That(m_incident.ResponsibleContactId, Is.Not.Null);
            Assert.That(m_incident.ResponsibleContactId.Id, Is.EqualTo(m_account.PrimaryContactId.Id));
        }
    }
}