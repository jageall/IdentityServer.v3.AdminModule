using System.Linq;
using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class CleanUpTokenHandles : IClassFixture<PowershellAdminModuleFixture>
    {
        private readonly PowerShell _ps;
        private readonly ITokenHandleStore _thStore;
        private const string Subject = "expired";


        [Fact]
        public void RefreshTokensAreDeleted()
        {
            Assert.NotEmpty(_thStore.GetAllAsync(Subject).Result);
            _ps.Invoke();

            Assert.Equal(
                new string[] { },
                _thStore.GetAllAsync(Subject).Result.Select(TestData.ToTestableString));

        }

        public CleanUpTokenHandles(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            var database = data.Database;
            _ps.AddScript(script).AddParameter("Database", database);
            var adminService = data.Factory.Resolve<IAdminService>();
            adminService.CreateDatabase(expireUsingIndex: false);
            _thStore = data.Factory.Resolve<ITokenHandleStore>();
            AddExpiredTokens(data.Factory);
        }

        private void AddExpiredTokens(Factory factory)
        {
            
            var admin = factory.Resolve<IAdminService>();
            var token = TestData.Token(Subject);
            admin.Save(token.Client);
            _thStore.StoreAsync("ac", token).Wait();
        }
    }
}