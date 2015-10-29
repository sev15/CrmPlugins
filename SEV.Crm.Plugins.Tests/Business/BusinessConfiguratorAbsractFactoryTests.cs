using System;
using Microsoft.Xrm.Sdk;
using Moq;
using NUnit.Framework;
using SEV.Crm.Services;

namespace SEV.Crm.Business.Tests
{
    [TestFixture]
    public class BusinessConfiguratorAbsractFactoryTests
    {
        private IBusinessConfiguratorAbsractFactory m_abstractFactory;

        [SetUp]
        public void Init()
        {
            m_abstractFactory = new BusinessConfiguratorAbsractFactory();
        }

        [Test]
        public void AddFactory_ShouldAddNewBusinessConfiguratorFactoryInBusinessConfiguratorAbsractFactory()
        {
            m_abstractFactory.AddFactory(typeof(Entity), new Mock<IBusinessConfiguratorFactory>().Object);

            Assert.That(m_abstractFactory.GetFactory<Entity>(), Is.Not.Null);
        }

        [Test]
        public void AddFactory_ShouldNotAddNewBusinessConfiguratorFactoryInBusinessConfiguratorAbsractFactory_WhenBusinessConfiguratorFactoryForTheSameEntityTypeIsAlreadyAdded()
        {
            m_abstractFactory.AddFactory(typeof(Entity), new Mock<IBusinessConfiguratorFactory>().Object);
            var factory = new Mock<IBusinessConfiguratorFactory>().Object;

            m_abstractFactory.AddFactory(typeof(Entity), factory);

            Assert.That(m_abstractFactory.GetFactory<Entity>(), Is.Not.SameAs(factory));
        }

        [Test]
        public void GetFactory_ShouldThrowInvalidOperationException_WhenThereAreNoAddedBusinessConfiguratorFactories()
        {
            Assert.That(() => m_abstractFactory.GetFactory<Entity>(), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void GetFactory_ShouldReturnBusinessConfiguratorFactory_WhenBusinessConfiguratorFactoryIsAddedForSpecifiedType()
        {
            var factory = new Mock<IBusinessConfiguratorFactory>().Object;
            m_abstractFactory.AddFactory(typeof(Entity), factory);

            var result = m_abstractFactory.GetFactory<Entity>();

            Assert.That(result, Is.SameAs(factory));
        }

        [Test]
        public void GetFactory_ShouldReturnTheSameBusinessConfiguratorFactory_WhenCalledForTheSameEntityType()
        {
            m_abstractFactory.AddFactory(typeof(Entity), new Mock<IBusinessConfiguratorFactory>().Object);
            var factory = m_abstractFactory.GetFactory<Entity>();

            var anotherFactory = m_abstractFactory.GetFactory<Entity>();

            Assert.That(anotherFactory, Is.SameAs(factory));
        }
    }
}
