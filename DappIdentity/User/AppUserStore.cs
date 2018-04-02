using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DappIdentity.Dapper;
using DappIdentity.Exception;
using Microsoft.AspNetCore.Identity;

namespace DappIdentity.User
{
    public class AppUserStore : IQueryableUserStore<AppUser>, IUserPasswordStore<AppUser>, IUserSecurityStampStore<AppUser>, IUserEmailStore<AppUser>, IUserLockoutStore<AppUser>
    {
        private readonly IDapperConnection _connection;
        private bool _disposed;
        private IQueryable<AppUser> _users;
        private readonly IAppUserStoreConfig _configuration; //move to Generic..
        private readonly PropertyInfo[] _userPropertyInfos;
        private readonly PropertyInfo _userIdPropertyInfo;

        public AppUserStore(IAppUserStoreConfig configuration, IDapperConnection connection)
        {
            _connection = connection;
            _configuration = configuration;
            _userPropertyInfos = typeof(AppUser).GetProperties();
            _userIdPropertyInfo = _userPropertyInfos.FirstOrDefault(x => x.Name == _configuration.UserTableDataKey.Item2);
            _userPropertyInfos = _userPropertyInfos.Where(x => x.Name != _configuration.UserTableDataKey.Item2).ToArray();
            if (_userIdPropertyInfo == null)
                throw new InvalidDbModelCastException();
            if (_userPropertyInfos.Length == 0)
                throw new InvalidDbModelCastException();
        }

        public IQueryable<AppUser> Users
        {
            get
            {
                ThrowIfDisposed();
                _users = LoadUsers();
                return _users;
            }
        }

        public class UsersPoco
        {
            public string UserTableDataKey { get; set; }
            public int UserIdPropertyInfo { get; set; }
        }
        
        private IQueryable<AppUser> LoadUsers()
        {
            ThrowIfDisposed();
            return _connection.ToEnumerable<AppUser>($"SELECT * FROM @UserTableDataKey", new UsersPoco
            {
                UserTableDataKey = _configuration.UserTableDataKey.Item1
            }).ToList().AsQueryable();
        }

        public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            string sql = $"INSERT INTO {_configuration.UserTableDataKey.Item1}({_userIdPropertyInfo.Name}, ";
            foreach (var propertyInfo in _userPropertyInfos)
            {
                sql += $"{propertyInfo.Name}, ";
            }
            var idValue = _userIdPropertyInfo.GetValue(user);
            if (idValue == null)
                throw new InvalidDbModelCastException();
            sql = sql.Substring(0, sql.Length - 2) + $") VALUES ('{idValue}', ";
            foreach (var propertyInfo in _userPropertyInfos)
            {
                var propValue = propertyInfo.GetValue(user);
                if (propValue == null && !_configuration.NullableFields.Contains(propertyInfo.Name))
                    throw new InvalidDbModelCastException();
                sql += $"'{propValue}', ";
            }
            sql = sql.Substring(0, sql.Length - 2) + ")";
            await _connection.Execute(sql);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            string sql = $"UPDATE {_configuration.UserTableDataKey.Item1} SET ";
            foreach (var propertyInfo in _userPropertyInfos)
            {
                var propValue = propertyInfo.GetValue(user);
                if (propValue == null && !_configuration.NullableFields.Contains(propertyInfo.Name))
                    throw new InvalidDbModelCastException();
                sql += $"{propertyInfo.Name} = '{propValue}', ";
            }
            sql = sql.Substring(0, sql.Length - 2);
            var idValue = _userIdPropertyInfo.GetValue(user);
            if (idValue == null) throw new InvalidDbModelCastException();
            sql += $" WHERE {_configuration.UserTableDataKey.Item2} = '{idValue}'";
            await _connection.Execute(sql);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            var idValue = _userIdPropertyInfo.GetValue(user);
            string sql = $"DELETE {_configuration.UserTableDataKey.Item1} WHERE {_configuration.UserTableDataKey.Item2} = '{idValue}'";
            await _connection.Execute(sql);
            return IdentityResult.Success;
        }

        public Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if ((object)userId == null)
                throw new ArgumentNullException(nameof(userId));
            return _connection.FirstOrDefault<AppUser>($"SELECT * FROM {_configuration.UserTableDataKey.Item1} WHERE {_configuration.UserTableDataKey.Item2} = '{userId}'");
        }

        public Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if ((object)normalizedUserName == null)
                throw new ArgumentNullException(nameof(normalizedUserName));
            return _connection.FirstOrDefault<AppUser>($"SELECT * FROM {_configuration.UserTableDataKey.Item1} WHERE {_configuration.UserTableDataKey.Item3} = '{normalizedUserName}'");
        }

        public Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (normalizedEmail == null)
                throw new ArgumentNullException(nameof(normalizedEmail));
            return _connection.FirstOrDefault<AppUser>($"SELECT * FROM {_configuration.UserTableDataKey.Item1} WHERE {_configuration.UserTableDataKey.Item4} = '{normalizedEmail}'");
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (normalizedName == null)
                throw new ArgumentNullException(nameof(normalizedName));
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (passwordHash == null)
                throw new ArgumentNullException(nameof(passwordHash));
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetSecurityStampAsync(AppUser user, string stamp, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (stamp == null)
                throw new ArgumentNullException(nameof(stamp));
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (email == null)
                throw new ArgumentNullException(nameof(email));
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            user.NormalizedEmail = normalizedEmail ?? throw new ArgumentNullException(nameof(normalizedEmail));
            return Task.FromResult(0);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.LockoutEndDateUtc.HasValue ? new DateTimeOffset?(user.LockoutEndDateUtc.Value) : null);
        }

        public Task SetLockoutEndDateAsync(AppUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            user.LockoutEndDateUtc = lockoutEnd?.UtcDateTime;
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(AppUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(AppUser user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _connection.Dispose();
            _disposed = true;
            _users = null;
        }
    }
}
