using System;
using System.Collections.Generic;

namespace DappIdentity.User
{
    public class AppUserStoreConfig : IAppUserStoreConfig
    {
        public AppUserStoreConfig()
        {
            UserTableDataKey = new Tuple<string, string, string, string>("AspNetUsers", "Id", "UserName", "Email"); ;
            UserTableJoins = new List<Tuple<string, string>> {new Tuple<string, string>("UserGroups", "UserGroupId")};
        }
        public Tuple<string, string, string, string> UserTableDataKey { get; }
        public List<Tuple<string, string>> UserTableJoins { get; }
    }

    public interface IAppUserStoreConfig
    {
        Tuple<string, string, string, string> UserTableDataKey { get; }
        List<Tuple<string, string>> UserTableJoins { get; }
    }
}