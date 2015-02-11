using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using MongoDB.Driver;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class InstallDatabase : IClassFixture<PowershellAdminModuleFixture>
    {
        private readonly PowerShell _ps;
        private readonly string _database;
        private readonly MongoServer _server;

        [Fact]
        public void CreateDatabase()
        {
            var defaults = ServiceFactory.DefaultStoreSettings();
            Assert.False(_server.DatabaseExists(_database));
            _ps.Invoke();
            Assert.True(_server.DatabaseExists(_database));
            var db = _server.GetDatabase(_database);
            Assert.True(db.CollectionExists(defaults.AuthorizationCodeCollection));
            Assert.True(db.CollectionExists(defaults.ClientCollection));
            Assert.True(db.CollectionExists(defaults.ConsentCollection));
            Assert.True(db.CollectionExists(defaults.RefreshTokenCollection));
            Assert.True(db.CollectionExists(defaults.ScopeCollection));
            Assert.True(db.CollectionExists(defaults.TokenHandleCollection));
            //TODO: verify indexes maybe?
            _server.DropDatabase(_database);
        }

        public InstallDatabase(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            _database = data.Database;
            _ps.AddScript(script).AddParameter("Database", _database);
            _server = data.Server;
        }
    }
}
