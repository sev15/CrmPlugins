using System;
using NUnit.Framework;

namespace SEV.Crm.Services.Tests
{
    [TestFixture]
    public class CrmServiceProviderTests
    {
        private ICrmServiceProvider m_serviceProvider;

        [SetUp]
        public void Init()
        {
            m_serviceProvider = new CrmServiceProvider();
        }

        [Test]
        public void Current_ShouldGetStatiqueInstanceOfCrmServiceProvider()
        {
            ICrmServiceProvider serviceProvider = CrmServiceProvider.Current;

            Assert.That(serviceProvider, Is.Not.Null);
        }

        [Test]
        public void Current_ShouldCreateNewInstanceOfCrmServiceProvider_WhenCrmServiceProviderIsNotInitialized()
        {
            CrmServiceProvider.Load(null);

            ICrmServiceProvider serviceProvider = CrmServiceProvider.Current;

            Assert.That(serviceProvider, Is.Not.Null);
        }

        [Test]
        public void Load_ShouldInitializeCrmServiceProviderWithSpecifiqueInstance()
        {
            var serviceProvider = new CrmServiceProvider();

            CrmServiceProvider.Load(serviceProvider);

            Assert.That(CrmServiceProvider.Current, Is.SameAs(serviceProvider));
        }

        [Test]
        public void LoadService_ShouldAddNewServiceInCrmServiceProvider()
        {
            var service = new UriBuilder();

            m_serviceProvider.LoadService(service.GetType(), service);

            Assert.That(m_serviceProvider.GetService(typeof(UriBuilder)), Is.SameAs(service));
        }

        [Test]
        public void LoadService_ShouldNotAddNewServiceInCrmServiceProvider_WhenServiceForTheSameKeyIsAlreadyAdded()
        {
            m_serviceProvider.LoadService(typeof(UriBuilder), new UriBuilder());
            var service = new UriBuilder();

            m_serviceProvider.LoadService(typeof(UriBuilder), service);

            Assert.That(m_serviceProvider.GetService(typeof(UriBuilder)), Is.Not.SameAs(service));
        }

        [Test]
        public void GetService_ShouldThrowInvalidOperationException_WhenTryToGetServiceWhichIsNotLoaded()
        {
            Assert.That(() => CrmServiceProvider.Current.GetService(typeof(string)),
                                            Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void GetService_ShouldReturnServiceByItsType_WhenTheServiceWasLoaded()
        {
            m_serviceProvider.LoadService(typeof(StringAssert), new StringAssert());

            var result = m_serviceProvider.GetService(typeof(StringAssert));

            Assert.That(result, Is.InstanceOf<StringAssert>());
        }
    }
}