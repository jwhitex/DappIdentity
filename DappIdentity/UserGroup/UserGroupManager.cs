using System.Threading.Tasks;
using DappIdentity.Dapper;
using DappIdentity.Exception;
using Microsoft.AspNetCore.Identity;

namespace DappIdentity.UserGroup
{
    public class UserGroupManager
    {
        private readonly IDapperConnection _connection;
        public UserGroupManager()
        {
        }
        public UserGroupManager(IDapperConnection connection)
        {
            _connection = connection;
        }
        public async Task Initialize(int userGroupId)
        {
            string sql = $"SELECT * FROM UserGroups WHERE UserGroupId = '{userGroupId}'";
            var queryResult = (await _connection.FirstOrDefault<UserGroupManager>(sql));
            if (queryResult == null)
                throw new InvalidDbModelCastException();

            UserGroupId = queryResult.UserGroupId;
            UserGroupName = queryResult.UserGroupName;

            PasswordOptions = new PasswordOptions
            {
                RequiredLength = queryResult.PasswordRequiredLength,
                RequireNonAlphanumeric = queryResult.PasswordRequireNonAlphanumeric,
                RequireUppercase = queryResult.PasswordRequireUppercase,
                RequireLowercase = queryResult.PasswordRequireLowercase,
                RequireDigit = queryResult.PasswordRequireDigit,
            };

            _connection.Dispose();
        }
        public PasswordOptions PasswordOptions { get; set; }
        public int UserGroupId { get; set; }
        public string UserGroupName { get; set; }

        public int PasswordRequiredLength { get; set; }
        /// <summary>Require a non letter or digit character</summary>
        public bool PasswordRequireNonAlphanumeric { get; set; }
        /// <summary>Require a lower case letter ('a' - 'z')</summary>
        public bool PasswordRequireLowercase { get; set; }
        /// <summary>Require an upper case letter ('A' - 'Z')</summary>
        public bool PasswordRequireUppercase { get; set; }
        /// <summary>Require a digit ('0' - '9')</summary>
        public bool PasswordRequireDigit { get; set; }
    }
}