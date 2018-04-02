using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DappIdentity.User
{
    public class AppUserManager : UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store,
                            IOptions<IdentityOptions> options,
                            IPasswordHasher<AppUser> passwordHasher,
                            IEnumerable<IUserValidator<AppUser>> userValidators,
                            IEnumerable<IPasswordValidator<AppUser>> passwordValidators,
                            ILookupNormalizer lookupNormalizer,
                            IdentityErrorDescriber identityErrorDescriber,
                            IServiceProvider serviceProvider,
                            ILogger<AppUserManager> logger)
            : base(store, options, passwordHasher, userValidators, passwordValidators, lookupNormalizer, identityErrorDescriber, serviceProvider, logger)
        {
        }

        //todo: Override neccessary methods to load Group options...
    }
}