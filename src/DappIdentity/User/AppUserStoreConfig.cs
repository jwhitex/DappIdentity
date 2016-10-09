using System;
using System.Collections.Generic;

namespace DappIdentity.User
{
    public class AppUserStoreConfig : IAppUserStoreConfig
    {
        public AppUserStoreConfig()
        {
            UserTableDataKey = new Tuple<string, string, string, string>("AspNetUsers", "Id", "UserName", "Email");
            NullableFields = new List<string> { "LockoutEndDateUtc"};
            UserTableJoins = new List<Tuple<string, string>> {new Tuple<string, string>("UserGroups", "UserGroupId")}; //todo: Implement
        }
        public Tuple<string, string, string, string> UserTableDataKey { get; }
        public List<Tuple<string, string>> UserTableJoins { get; }
        public List<string> NullableFields { get; }
    }

    public interface IAppUserStoreConfig
    {
        Tuple<string, string, string, string> UserTableDataKey { get; }
        List<Tuple<string, string>> UserTableJoins { get; }
        List<string> NullableFields { get; }
    }
}