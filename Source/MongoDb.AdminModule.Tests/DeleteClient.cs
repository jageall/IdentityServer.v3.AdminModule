using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class DeleteClient : IClassFixture<PowershellAdminModuleFixture>
    {
        private readonly PowershellAdminModuleFixture _data;
        private readonly PowerShell _ps;
        
        private readonly IClientStore _store;
        private readonly string _clientId;

        [Fact]
        public void RemoveClient()
        {
            Assert.NotNull(_store.FindClientByIdAsync(_clientId).Result);
            _ps.AddParameter("ClientId", _clientId);
            _ps.Invoke();
            
            Assert.Null(_data.GetPowershellErrors());
            Assert.Null(_store.FindClientByIdAsync(_clientId).Result);
        }

        public DeleteClient(PowershellAdminModuleFixture data)
        {
            _data = data;
            _ps = data.PowerShell;
            var database = data.Database;
            _ps.AddScript(data.LoadScript(this)).AddParameter("Database", database);
            _store = data.Factory.Resolve<IClientStore>();
            var am = data.Factory.Resolve<IAdminService>();

            var client = TestData.ClientAllProperties();
            _clientId = client.ClientId;
            am.Save(client);
        }
    }
}
