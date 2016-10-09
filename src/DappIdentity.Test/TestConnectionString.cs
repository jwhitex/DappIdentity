using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using DappIdentity.User;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace DappIdentity.Test
{
    public class TestConnectionString
    {
        [Test]
        public async Task Test()
        {
            var connectionString = Configuration.Root.GetConnectionString("DefaultConnection");
            var connection = new SqlConnection(connectionString);
            connection.Open();

            var queryResult = await connection.QueryFirstOrDefaultAsync<AppUser>("SELECT * FROM AspNetUsers");
            Assert.That(queryResult, Is.Not.Null);
            Assert.That(queryResult.PasswordHash, Is.Not.Empty);
            connection.Close();
            connection.Dispose();
        }
    }
}
