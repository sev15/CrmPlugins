using System;
using Microsoft.Xrm.Client;
using NUnit.Framework;
using Sample.Crm.Entities;

namespace Sample.Crm.Plugins.Tests
{
    [TestFixture]
    public class AccountSysTests : SysTestBase
    {
        private static readonly Guid IncidentId = Guid.NewGuid();
        private const string IncidentTitle = "Test Incident Title";

        private Guid? NewContactId = null;

        [TearDown]
        public void Clean()
        {
            if (FindById<Incident>(IncidentId) != null)
            {
                ServiceContext.Delete(Incident.EntityLogicalName, IncidentId);
            }
            if (NewContactId.HasValue)
            {
                ServiceContext.Update(new Account { Id = AccountId, PrimaryContactId = null });
                ServiceContext.Delete(Contact.EntityLogicalName, NewContactId.Value);
                NewContactId = null;
            }
        }

        [Test]
        public void Sys_Account_ShouldBeCreated()
        {
            var account = FindById<Account>(AccountId);

            Assert.That(account, Is.Not.Null);
        }

        [Test]
        public void Sys_Contact_ShouldBeCreated()
        {
            var contact = FindById<Contact>(ContactId);

            Assert.That(contact, Is.Not.Null);
        }

        [Test]
        public void Sys_AccountUpdate_ShouldUpdateResponsibleContactOfIncidents()
        {
            ServiceContext.Create(new Incident
            {
                Id = IncidentId,
                Title = IncidentTitle,
                CustomerId = new CrmEntityReference(Account.EntityLogicalName, AccountId)
            });
            var incident = FindById<Incident>(IncidentId);
            Assert.That(incident, Is.Not.Null);

            NewContactId = Guid.NewGuid();
            ServiceContext.Create(new Contact
            {
                Id = NewContactId.Value,
                ParentCustomerId = new CrmEntityReference(Account.EntityLogicalName, AccountId)
            });
            var contact = FindById<Contact>(NewContactId.Value);
            Assert.That(contact, Is.Not.Null);
            ServiceContext.Update(new Account
            {
                Id = AccountId,
                PrimaryContactId = new CrmEntityReference(Contact.EntityLogicalName, NewContactId.Value)
            });
            ServiceContext.SaveChanges(); // To clean the cache

            incident = FindById<Incident>(IncidentId);
            Assert.That(incident, Is.Not.Null);
            Assert.That(incident.ResponsibleContactId, Is.Not.Null);
            Assert.That(incident.ResponsibleContactId.Id, Is.EqualTo(NewContactId));
        }
    }
}