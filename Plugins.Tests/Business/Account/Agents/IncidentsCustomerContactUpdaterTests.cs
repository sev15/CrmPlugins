using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Client;
using Moq;
using NUnit.Framework;
using Sample.Crm.Entities;

namespace Sample.Crm.Business.Agents.Tests
{
    [TestFixture]
    public class IncidentsCustomerContactUpdaterTests : CrmBusinessAgentTestBase
    {
        private Account m_account;
        private List<Incident> m_incidents;

        #region SetUp

        [SetUp]
        public void Init()
        {
            BusinessAgent = new IncidentsCustomerContactUpdater
            {
                ExecutionOrder = 1, Context = new object[] { CrmServiceContextMock.Object }
            };
        }

        protected override void InitializaMocks()
        {
            base.InitializaMocks();

            m_account = new Account
            {
                Id = Guid.NewGuid(), PrimaryContactId = new CrmEntityReference("", Guid.NewGuid())
            };
            ExecutorContextMock.Setup(x => x.GetTargetEntity<Account>()).Returns(m_account);

            MockIncidentQuery();
        }

        private void MockIncidentQuery()
        {
            m_incidents = new List<Incident>
            {
                new Incident { Id = Guid.NewGuid(), CustomerId = new CrmEntityReference("", Guid.NewGuid()) }, 
                new Incident { Id = Guid.NewGuid(), CustomerId = new CrmEntityReference("", Guid.NewGuid()) },
                new Incident { Id = Guid.NewGuid(), CustomerId = new CrmEntityReference("", Guid.NewGuid()) }
            };
            var queryable = m_incidents.AsQueryable();
            var queryableMock = new Mock<IQueryable<Incident>>();
            queryableMock.Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator());
            queryableMock.Setup(x => x.Provider).Returns(queryable.Provider);
            queryableMock.Setup(x => x.ElementType).Returns(queryable.ElementType);
            queryableMock.Setup(x => x.Expression).Returns(queryable.Expression);
            CrmServiceContextMock.Setup(x => x.CreateQuery<Incident>()).Returns(queryableMock.Object);
        }

        #endregion

        [Test]
        public void Execute_ShouldCallGetTargetEntityOfPluginContext()
        {
            BusinessAgent.Execute(ExecutorContextMock.Object);

            ExecutorContextMock.Verify(x => x.GetTargetEntity<Account>(), Times.Once);
        }

        [Test]
        public void Execute_ShouldQueryCrmServiceContextForIncidents()
        {
            BusinessAgent.Execute(ExecutorContextMock.Object);

            CrmServiceContextMock.Verify(x => x.CreateQuery<Incident>(), Times.Once);
        }

        [Test]
        public void Execute_ShouldCallUpdateObjectOfCrmServiceContext_WhenThereAreIncidentsToUpdate()
        {
            m_incidents[0].CustomerId = m_account.ToEntityReference();
            m_incidents[1].CustomerId = m_account.ToEntityReference();

            BusinessAgent.Execute(ExecutorContextMock.Object);

            CrmServiceContextMock.Verify(x => x.UpdateObject(
                It.Is<Incident>(y => y.ResponsibleContactId.Id == m_account.PrimaryContactId.Id)), Times.Exactly(2));
        }

        [Test]
        public void Execute_ShouldCallSaveChangesOfCrmServiceContext_WhenThereAreIncidentsToUpdate()
        {
            m_incidents[0].CustomerId = m_account.ToEntityReference();

            BusinessAgent.Execute(ExecutorContextMock.Object);

            CrmServiceContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
        public void Execute_ShouldNotCallSaveChangesOfCrmServiceContext_WhenThereAreNotIncidentsToUpdate()
        {
            BusinessAgent.Execute(ExecutorContextMock.Object);

            CrmServiceContextMock.Verify(x => x.SaveChanges(), Times.Never);
        }
    }
}