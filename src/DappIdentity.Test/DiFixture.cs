using System.Reflection;
using Autofac;
using DappIdentity.User;
using NUnit.Framework;

namespace DappIdentity.Test
{
    [SetUpFixture]
    public class DiFixture
    {
        public static IContainer Container { get; set; }

        [OneTimeSetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new IdentityModule(typeof(AppUser).GetTypeInfo().Assembly));
            Container = builder.Build();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Container.Dispose();
        }

    }
}
