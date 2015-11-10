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

using System.Management.Automation;
using System.Threading.Tasks;
using IdentityServer3.Admin.MongoDb;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class DeleteScope : IClassFixture<PowershellAdminModuleFixture>, IAsyncLifetime 
    {
        private readonly PowershellAdminModuleFixture _data;
        private IScopeStore _scopeStore;
        private PowerShell _ps;
        private const string ScopeName = "removethisscope";

        [Fact]
        public async Task ScopeIsRemoved()
        {
            Assert.NotEmpty(await _scopeStore.FindScopesAsync(new[] { ScopeName }));
            _ps.Invoke();
            Assert.Empty(await _scopeStore.FindScopesAsync(new[] { ScopeName }));
        }

        public DeleteScope(PowershellAdminModuleFixture data)
        {
            _data = data;

            _ps = data.PowerShell;
            _ps.AddScript(data.LoadScript(this))
                .AddParameter("Database", data.Database)
                .AddParameter("Name", ScopeName);
            _scopeStore = data.Factory.Resolve<IScopeStore>();
        }

        public Task InitializeAsync()
        {
            var admin = _data.Factory.Resolve<IAdminService>();
            Scope scope = TestData.ScopeMandatoryProperties();
            scope.Name = ScopeName;
            return admin.Save(scope);
        }

        public Task DisposeAsync()
        {
            return Task.FromResult(0);
        }
    }
}
