using System.Linq;
using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class CleanUpRefreshTokens : IClassFixture<PowershellAdminModuleFixture>
    {
        private readonly PowerShell _ps;
        private readonly IRefreshTokenStore _rtStore;
        private const string Subject = "expired";


        [Fact]
        public void RefreshTokensAreDeleted()
        {
            Assert.NotEmpty(_rtStore.GetAllAsync(Subject).Result);
            _ps.Invoke();

            Assert.Equal(
                new string[] { },
                _rtStore.GetAllAsync(Subject).Result.Select(TestData.ToTestableString));
            
        }

        public CleanUpRefreshTokens(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            var database = data.Database;
            _ps.AddScript(script).AddParameter("Database", database);
            var adminService = data.Factory.Resolve<IAdminService>();
            adminService.CreateDatabase(expireUsingIndex: false);

            _rtStore = data.Factory.Resolve<IRefreshTokenStore>();
            AddExpiredTokens(data.Factory);
        }

        private void AddExpiredTokens(Factory factory)
        {
            var admin = factory.Resolve<IAdminService>();
            var token = TestData.RefreshToken(Subject);
            admin.Save(token.AccessToken.Client);
            _rtStore.StoreAsync("ac", token).Wait();
        }
    }
}
