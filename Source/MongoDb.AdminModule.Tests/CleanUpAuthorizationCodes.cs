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
using Xunit;
using System.Threading.Tasks;
using IdentityServer3.Admin.MongoDb;
using IdentityServer3.Admin.MongoDb.Powershell;
using IdentityServer3.Core.Services;

namespace MongoDb.AdminModule.Tests
{
    public class CleanUpAuthorizationCodes : IClassFixture<PowershellAdminModuleFixture>
    {
        private PowerShell _ps;
        private string _script;
        private string _database;
        private IAuthorizationCodeStore _acStore;
        private const string Subject = "expired";
        Task _setup;

        [Fact]
        public async Task AuthorizationCodesAreDeleted()
        {
            await _setup;
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
            _acStore = data.Factory.Resolve<IAuthorizationCodeStore>();
            _setup = Setup(data.Factory);
        }

        private async Task Setup(Factory factory)
        {
            
            var admin = factory.Resolve<IAdminService>();
            var code = TestData.AuthorizationCode(Subject);
            
            await admin.CreateDatabase(expireUsingIndex: false);
            
            await admin.Save(code.Client);
            foreach (var scope in code.RequestedScopes)
            {
                await admin.Save(scope);
            }
            await _acStore.StoreAsync("ac", code);


        }
    }
}
