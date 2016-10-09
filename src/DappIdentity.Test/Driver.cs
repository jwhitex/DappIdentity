using System.Threading.Tasks;
using Autofac;
using DappIdentity.User;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;

namespace DappIdentity.Test
{
    [TestFixture]
    public class Driver
    {
        public ILifetimeScope LifetimeScope { get; set; }

        [SetUp]
        public void Setup()
        {
            LifetimeScope = DiFixture.Container.BeginLifetimeScope();
        }

        [Test]
        public async Task CreateUserDriver()
        {
            var userManager = LifetimeScope.Resolve<AppUserManager>();

            var password = "Password@1";
            var user = new AppUser
            {
                UserName = "Test",
                Email = "test@test.com",
                UserGroupId = 1
            };
            var result = await userManager.CreateAsync(user, password);
            Assert.That(result, Is.EqualTo(IdentityResult.Success));
        }


        [TearDown]
        public void Teardown()
        {
            LifetimeScope.Dispose();
        }
    }
}
