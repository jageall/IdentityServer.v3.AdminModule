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
    public class CleanUpAuthorizationCodes : IClassFixture<PowershellAdminModuleFixture>, IAsyncLifetime
    {
        private readonly PowershellAdminModuleFixture _data;
        private readonly PowerShell _ps;
        private readonly string _script;
        private readonly string _database;
        private readonly IAuthorizationCodeStore _acStore;
        private const string Subject = "expired";
        
        [Fact]
        public async Task AuthorizationCodesAreDeleted()
        {
            Assert.NotEmpty(await _acStore.GetAllAsync(Subject));
            _ps.Invoke();

            Assert.Equal(
                new string[] { },
                _acStore.GetAllAsync(Subject).Result.Select(TestData.ToTestableString));
            
        }

        public CleanUpAuthorizationCodes(PowershellAdminModuleFixture data)
        {
            _data = data;
            _ps = data.PowerShell;
            _script = data.LoadScript(this);
            _database = data.Database;
            _ps.AddScript(_script).AddParameter("Database", _database);
            _acStore = data.Factory.Resolve<IAuthorizationCodeStore>();
            
        }

        public async Task InitializeAsync()
        {
            var factory = _data.Factory;
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

        public Task DisposeAsync()
        {
            return Task.FromResult(0);
        }
    }
}
