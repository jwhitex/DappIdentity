using System.Threading.Tasks;
using DappIdentity.UserGroup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DappIdentity.User
{
    public class AppIdentityOptions : IOptions<IdentityOptions>
    {
        private readonly UserGroupManager _userGroupManager;
        private IdentityOptions IdentityOptions { get; set; } = new IdentityOptions();
        public AppIdentityOptions(UserGroupManager userGroupManager)
        {
            _userGroupManager = userGroupManager;
        }

        public async Task LoadUserGroup(int id)
        {
            await _userGroupManager.Initialize(id);
            IdentityOptions = new IdentityOptions
            {
                Password = _userGroupManager.PasswordOptions
            };
        }

        public IdentityOptions Value => IdentityOptions;
    }
}