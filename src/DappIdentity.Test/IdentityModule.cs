using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DappIdentity.Dapper;
using DappIdentity.User;
using DappIdentity.UserGroup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Module = Autofac.Module;

namespace DappIdentity.Test
{
    public class IdentityModule : Module
    {
        private readonly Assembly[] _assemblies;

        public IdentityModule(params Assembly[] extraAssemblies)
        {
            _assemblies = new []{ Assembly.GetEntryAssembly() };
            if (extraAssemblies != null)
                _assemblies = _assemblies.Concat(extraAssemblies).ToArray();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new UserGroupManager(c.Resolve<IDapperConnection>())).AsSelf(); //Add Interface like IUserGroupManager?
            builder.Register(c => new AppUserStoreConfig()).AsImplementedInterfaces();
            builder.Register(c => new AppUserStore(c.Resolve<IAppUserStoreConfig>(), c.Resolve<IDapperConnection>())).AsImplementedInterfaces();

            builder.Register(c => new AppIdentityOptions(c.Resolve<UserGroupManager>())).As<IOptions<IdentityOptions>>();
            builder.Register(c => new PasswordHasher<AppUser>()).As<IPasswordHasher<AppUser>>();
            builder.Register(c => new AppUserValidator()).As<IUserValidator<AppUser>>();
            builder.Register(c => new AppPasswordValidator()).As<IPasswordValidator<AppUser>>();
            builder.Register(c => new AppUserManager(c.Resolve<IUserStore<AppUser>>(), 
                                                    c.Resolve<IOptions<IdentityOptions>>(),
                                                    c.Resolve<IPasswordHasher<AppUser>>(),  
                                                    c.Resolve<IEnumerable<IUserValidator<AppUser>>>(), 
                                                    c.Resolve<IEnumerable<IPasswordValidator<AppUser>>>(), 
                                                    new UpperInvariantLookupNormalizer(), 
                                                    new IdentityErrorDescriber(), 
                                                    new AutofacServiceProvider(c),
                                                    c.Resolve<ILoggerFactory>().CreateLogger<AppUserManager>()));
            
        }
    }
}
