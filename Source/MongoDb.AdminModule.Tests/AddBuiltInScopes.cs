using System.Linq;
using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class AddBuiltInScopes : IClassFixture<PowershellAdminModuleFixture>
    {
        private readonly IScopeStore _scopeStore;
        private readonly PowerShell _ps;
        private readonly PowershellAdminModuleFixture _data;

        [Fact]
        public void VerifyAllBuiltInScopes()
        {
            _ps.Invoke();
            Assert.Null(_data.GetPowershellErrors());
            Assert.Equal(
                ReadScopes.BuiltInScopes()
                    .OrderBy(x => x.Name)
                    .Select(TestData.ToTestableString), 
                _scopeStore.GetScopesAsync(false).Result
                    .OrderBy(x=>x.Name)
                    .Select(TestData.ToTestableString)
                );
        }

        public AddBuiltInScopes(PowershellAdminModuleFixture data)
        {
            _data = data;
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            var database = data.Database;
            _ps.AddScript(script).AddParameter("Database", database);
            _scopeStore = data.Factory.Resolve<IScopeStore>();
            var adminService = data.Factory.Resolve<IAdminService>();
            adminService.CreateDatabase();
        }
    }
}
