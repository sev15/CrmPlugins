using System;
using Microsoft.Xrm.Client;
using NUnit.Framework;
using Sample.Crm.Entities;

namespace Sample.Crm.Plugins.Tests
{
    [TestFixture]
    public class IncidentSysTests : SysTestBase
    {
        private static readonly Guid m_incidentId = Guid.NewGuid();
        private const string IncidentTitle = "Test Incident Title";

        [TearDown]
        public void Clean()
        {
            if (FindById<Incident>(m_incidentId) != null)
            {
                ServiceContext.Delete(Incident.EntityLogicalName, m_incidentId);
            }
        }

        [Test]
        public void Sys_Account_ShouldBeCreated()
        {
            var account = FindById<Account>(AccountId);

            Assert.That(account, Is.Not.Null);
            Assert.That(account.Id, Is.EqualTo(AccountId));
            Assert.That(account.Name, Is.EqualTo(AccountName));
            Assert.That(account.PrimaryContactId, Is.Not.Null);
            Assert.That(account.PrimaryContactId.Id, Is.EqualTo(ContactId));
        }

        [Test]
        public void Sys_Contact_ShouldBeCreated()
        {
            var contact = FindById<Contact>(ContactId);

            Assert.That(contact, Is.Not.Null);
            Assert.That(contact.Id, Is.EqualTo(ContactId));
            Assert.That(contact.FirstName, Is.EqualTo(ContactFirstName));
            Assert.That(contact.LastName, Is.EqualTo(ContactLastName));
            Assert.That(contact.ParentCustomerId, Is.Not.Null);
            Assert.That(contact.ParentCustomerId.Id, Is.EqualTo(AccountId));
        }

        [Test]
        public void Sys_IncidentCreation_ShouldSetResponsibleContact_WhenResponsibleContactIsNotDefined()
        {
            ServiceContext.Create(new Incident
            {
                Id = m_incidentId,
                Title = IncidentTitle,
                CustomerId = new CrmEntityReference(Account.EntityLogicalName, AccountId)
            });
            var incident = FindById<Incident>(m_incidentId);

            Assert.That(incident, Is.Not.Null);
            Assert.That(incident.Id, Is.EqualTo(m_incidentId));
            Assert.That(incident.Title, Is.EqualTo(IncidentTitle));
            Assert.That(incident.ResponsibleContactId, Is.Not.Null);
            Assert.That(incident.ResponsibleContactId.Id, Is.EqualTo(ContactId));
        }

        [Test]
        public void Sys_IncidentCreation_ShouldValidateResponsibleContact_WhenResponsibleContactIsDefined()
        {
            var invalidIncident = new Incident
            {
                Id = m_incidentId,
                Title = IncidentTitle,
                CustomerId = new CrmEntityReference(Account.EntityLogicalName, AccountId),
                ResponsibleContactId = new CrmEntityReference(Contact.EntityLogicalName, Guid.NewGuid())
            };

            Assert.That(() => ServiceContext.Create(invalidIncident),
                                Throws.InstanceOf<Exception>().With.Message.EqualTo("Invalid Responsible Contact"));
        }

        [Test]
        public void Sys_IncidentUpdate_ShouldValidateResponsibleContact_WhenResponsibleContactIsDefined()
        {
            ServiceContext.Create(new Incident
            {
                Id = m_incidentId,
                Title = IncidentTitle,
                CustomerId = new CrmEntityReference(Account.EntityLogicalName, AccountId)
            });
            var incident = FindById<Incident>(m_incidentId);
            Assert.That(incident, Is.Not.Null);

            var invalidIncidentToUpdate = new Incident
            {
                Id = m_incidentId,
                ResponsibleContactId = new CrmEntityReference(Contact.EntityLogicalName, Guid.NewGuid())
            };

            Assert.That(() => ServiceContext.Update(invalidIncidentToUpdate),
                                Throws.InstanceOf<Exception>().With.Message.EqualTo("Invalid Responsible Contact"));
        }
    }
}