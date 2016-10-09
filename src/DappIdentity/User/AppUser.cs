using System;

namespace DappIdentity.User
{
    public class AppUser
    {
        public AppUser()
        {
            Id = Guid.NewGuid().ToString();
            CreateDate = DateTime.UtcNow;
            LockoutEnabled = true;
            
        }
        public string Id { get; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public int UserGroupId { get; set; }
    }
}