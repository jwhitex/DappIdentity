using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DappIdentity.Dapper;
using DappIdentity.User;
using NSubstitute;
using NUnit.Framework;

namespace DappIdentity.Test
{
    [TestFixture]
    public class TestAppUserStore
    {
        public ILifetimeScope LifetimeScope { get; set; }
        public IDapperConnection Connection { get; set; }

        public Tuple<string, string, string, string> UserTableDataKey => new Tuple<string, string, string, string>("UserTable", "Id", "UserName", "Email");
        public List<string> NullableUserTableFields => new List<string> { "LockoutEndDateUtc" };
        public List<Tuple<string, string>> UserTableJoins => new List<Tuple<string, string>> { new Tuple<string, string>("UserGroupTable", "UserGroupId") };

        [SetUp]
        public void Setup()
        {
            var configuration = Substitute.For<IAppUserStoreConfig>();
            configuration.UserTableDataKey.Returns(UserTableDataKey);
            configuration.NullableFields.Returns(NullableUserTableFields);
            configuration.UserTableJoins.Returns(UserTableJoins);
            Connection = Substitute.For<IDapperConnection>();
            LifetimeScope = DiFixture.Container.BeginLifetimeScope(builder =>
            {
                builder.Register(c => configuration).As<IAppUserStoreConfig>().SingleInstance();
                builder.Register(c => Connection).As<IDapperConnection>().SingleInstance();
                builder.Register(c => new AppUserStore(c.Resolve<IAppUserStoreConfig>(), c.Resolve<IDapperConnection>())).SingleInstance();
            });
        }

        [Test]
        public void TestLoadUsers()
        {
            var userStore = LifetimeScope.Resolve<AppUserStore>();
            var users = userStore.Users;
            Connection.Received(1).ToEnumerable<AppUser>(Arg.Is<string>(x => x == "SELECT * FROM UserTable"));
        }

        [Test]
        public async Task TestFindByName()
        {
            var userName = "John";
            var appUser = new AppUser { UserName = userName };
            Connection.FirstOrDefault<AppUser>(null).ReturnsForAnyArgs(appUser);

            var userStore = LifetimeScope.Resolve<AppUserStore>();
            var result = await userStore.FindByNameAsync(userName, CancellationToken.None);

            Assert.That(result.UserName, Is.EqualTo(userName));
            await Connection.Received(1).FirstOrDefault<AppUser>(Arg.Is<string>(x => x == $"SELECT * FROM UserTable WHERE UserName = '{userName}'"));
        }

        [Test]
        public async Task TestFindById()
        {
            var userName = "John";
            var appUser = new AppUser { UserName = userName };
            Connection.FirstOrDefault<AppUser>(null).ReturnsForAnyArgs(appUser);

            var userStore = LifetimeScope.Resolve<AppUserStore>();
            var result = await userStore.FindByIdAsync(appUser.Id, CancellationToken.None);

            Assert.That(result.Id, Is.EqualTo(appUser.Id));
            Assert.That(result.UserName, Is.EqualTo(userName));
            await Connection.Received(1).FirstOrDefault<AppUser>(Arg.Is<string>(x => x == $"SELECT * FROM UserTable WHERE Id = '{appUser.Id}'"));

        }

        [Test]
        public async Task TestUpdateAsync()
        {
            var appUser = new AppUser
            {
                UserName = "Test",
                PasswordHash = "a",
                SecurityStamp = "b",
                UserGroupId = 1
            };
            Connection.Execute(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult);

            var userStore = LifetimeScope.Resolve<AppUserStore>();
            await userStore.UpdateAsync(appUser, CancellationToken.None);

            await Connection.Received(1).Execute(Arg.Is<string>(x => x == $"UPDATE UserTable SET UserName = '{appUser.UserName}', CreateDate = '{appUser.CreateDate}', PasswordHash = '{appUser.PasswordHash}', SecurityStamp = '{appUser.SecurityStamp}', UserGroupId = '{appUser.UserGroupId}' WHERE Id = '{appUser.Id}'"));
        }

        [Test]
        public async Task TestCreateAsync()
        {
            var appUser = new AppUser
            {
                UserName = "Test",
                PasswordHash = "a",
                SecurityStamp = "b",
                UserGroupId = 1
            };
            Connection.Execute(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult);

            var userStore = LifetimeScope.Resolve<AppUserStore>();
            await userStore.CreateAsync(appUser, CancellationToken.None);

            await Connection.Received(1).Execute(Arg.Is<string>(x => x == $"INSERT INTO UserTable(Id, UserName, CreateDate, PasswordHash, SecurityStamp, UserGroupId) VALUES ('{appUser.Id}', '{appUser.UserName}', '{appUser.CreateDate}', '{appUser.PasswordHash}', '{appUser.SecurityStamp}', '{appUser.UserGroupId}')"));
        }


        [Test]
        public async Task TestDeleteAsync()
        {
            var appUser = new AppUser();
            Connection.Execute(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult);

            var userStore = LifetimeScope.Resolve<AppUserStore>();
            await userStore.DeleteAsync(appUser, CancellationToken.None);

            await Connection.Received(1).Execute(Arg.Is<string>(x => x == $"DELETE UserTable WHERE Id = '{appUser.Id}'"));
        }

        [TearDown]
        public void Teardown()
        {
            LifetimeScope.Dispose();
        }
    }
}
