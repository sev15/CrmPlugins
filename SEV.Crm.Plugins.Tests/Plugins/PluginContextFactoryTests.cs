using System;
using Microsoft.Xrm.Sdk;
using Moq;
using NUnit.Framework;
using SEV.Crm.Business;

namespace SEV.Crm.Plugins.Tests
{
    [TestFixture]
    public class PluginContextFactoryTests
    {
        [Test]
        public void CreateContext_ShouldReturnNewPluginContext()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            IServiceProvider serviceProvider = serviceProviderMock.Object;
            IPluginContextFactory contextFactory = new PluginContextFactory();

            IPluginExecutorContext context = contextFactory.CreateContext(serviceProvider, typeof(Entity));

            Assert.That(context, Is.Not.Null);
        }
    }
}
