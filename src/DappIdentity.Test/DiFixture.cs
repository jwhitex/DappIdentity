using System.Reflection;
using Autofac;
using DappIdentity.Dapper;
using DappIdentity.User;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using Microsoft.Extensions.Configuration;

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

            var connection = Configuration.Root.GetConnectionString("DefaultConnection");

            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole()
                .CreateLogger();
            Log.Logger = logger;
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog();

            builder.Register(c => loggerFactory).As<ILoggerFactory>();
            builder.Register(c => new DapperConnection(connection)).AsImplementedInterfaces();

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
