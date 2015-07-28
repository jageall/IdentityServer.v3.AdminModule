/*
 * Copyright 2014, 2015 James Geall
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.Linq;
using System.Management.Automation;
using IdentityServer.Admin.MongoDb;
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
