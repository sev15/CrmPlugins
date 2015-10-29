using System;
using System.Linq;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Sample.Crm.Entities;

namespace Sample.Crm.Plugins.Tests
{
    public class SysTestBase
    {
        internal static readonly Guid AccountId = Guid.NewGuid();
        internal const string AccountName = "Test Account";
        internal static readonly Guid ContactId = Guid.NewGuid();
        internal const string ContactFirstName = "Test First Name";
        internal const string ContactLastName = "Test Last Name";

        private static XrmServiceContext m_serviceContext;
        protected static XrmServiceContext ServiceContext
        {
            get
            {
                return m_serviceContext ?? (m_serviceContext = new XrmServiceContext("Xrm"));
            }
        }

        [TestFixtureSetUp]
        public void Init()
        {
            ServiceContext.Create(new Account { Id = AccountId, Name = AccountName });
            ServiceContext.Create(new Contact
            {
                Id = ContactId,
                FirstName = ContactFirstName,
                LastName = ContactLastName,
                ParentCustomerId = new CrmEntityReference(Account.EntityLogicalName, AccountId)
            });
            ServiceContext.Update(new Account
            {
                Id = AccountId, PrimaryContactId = new CrmEntityReference(Contact.EntityLogicalName, ContactId)
            });
        }

        [TestFixtureTearDown]
        public void CleanUp()
        {
            ServiceContext.Update(new Account { Id = AccountId, PrimaryContactId = null });
            ServiceContext.Delete(Contact.EntityLogicalName, ContactId);
            ServiceContext.Delete(Account.EntityLogicalName, AccountId);

            ServiceContext.Dispose();
        }

        protected T FindById<T>(Guid entityId) where T : Entity
        {
            return ServiceContext.CreateQuery<T>().SingleOrDefault(x => x.Id == entityId);
        }
    }
}