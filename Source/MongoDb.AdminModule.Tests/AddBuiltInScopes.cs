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
using System.Threading.Tasks;
using IdentityServer3.Admin.MongoDb;
using IdentityServer3.Admin.MongoDb.Powershell;
using IdentityServer3.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class AddBuiltInScopes : IClassFixture<PowershellAdminModuleFixture>, IAsyncLifetime
    {
        private readonly IScopeStore _scopeStore;
        private readonly PowerShell _ps;
        private readonly PowershellAdminModuleFixture _data;

        [Fact]
        public async Task VerifyAllBuiltInScopes()
        {
            _ps.Invoke();
            Assert.Null(_data.GetPowershellErrors());
            Assert.Equal(
                ReadScopes.BuiltInScopes()
                    .OrderBy(x => x.Name)
                    .Select(TestData.ToTestableString), 
                (await _scopeStore.GetScopesAsync(false))
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
            
        }

        public async Task InitializeAsync()
        {
            var adminService = _data.Factory.Resolve<IAdminService>();
            await adminService.CreateDatabase();
        }

        public Task DisposeAsync()
        {
            return Task.FromResult(0);
        }
    }
}
