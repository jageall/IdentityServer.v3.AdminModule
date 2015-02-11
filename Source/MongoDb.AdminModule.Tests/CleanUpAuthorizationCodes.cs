using System.Linq;
using System.Management.Automation;
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class CleanUpAuthorizationCodes : IClassFixture<PowershellAdminModuleFixture>
    {
        private PowerShell _ps;
        private string _script;
        private string _database;
        private IAuthorizationCodeStore _acStore;
        private const string Subject = "expired";


        [Fact]
        public void AuthorizationCodesAreDeleted()
        {
            Assert.NotEmpty(_acStore.GetAllAsync(Subject).Result);
            _ps.Invoke();

            Assert.Equal(
                new string[] { },
                _acStore.GetAllAsync(Subject).Result.Select(TestData.ToTestableString));
            
        }

        public CleanUpAuthorizationCodes(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            _script = data.LoadScript(this);
            _database = data.Database;
            _ps.AddScript(_script).AddParameter("Database", _database);
            var adminService = data.Factory.Resolve<IAdminService>();
            adminService.CreateDatabase(expireUsingIndex: false);
            _acStore = data.Factory.Resolve<IAuthorizationCodeStore>();
            AddExpiredTokens(data.Factory);
        }

        private void AddExpiredTokens(Factory factory)
        {
            
            var admin = factory.Resolve<IAdminService>();
            var code = TestData.AuthorizationCode(Subject);
            admin.Save(code.Client);
            foreach (var scope in code.RequestedScopes)
            {
                admin.Save(scope);
            }
            _acStore.StoreAsync("ac", code).Wait();


        }
    }
}
