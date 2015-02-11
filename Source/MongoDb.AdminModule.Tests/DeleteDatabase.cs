using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using MongoDB.Driver;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class DeleteDatabase : IClassFixture<PowershellAdminModuleFixture>
    {
        private readonly string _database;
        private readonly MongoServer _server;
        private readonly PowerShell _ps;

        [Fact]
        public void DatabaseShouldBeDeleted()
        {
            Assert.True(_server.DatabaseExists(_database));
            _ps.Invoke();
            Assert.False(_server.DatabaseExists(_database));
        }

        public DeleteDatabase(PowershellAdminModuleFixture data)
        {
            var admin = data.Factory.Resolve<IAdminService>();
            admin.CreateDatabase();
            _ps = data.PowerShell;
            _database = data.Database;
            _server = data.Server; 
            var script = data.LoadScript(this);
            _ps.AddScript(script).AddParameter("Database", _database);
           
        }
    }
}
