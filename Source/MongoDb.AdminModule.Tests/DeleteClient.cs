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
    public class DeleteClient : IClassFixture<PowershellAdminModuleFixture>, IAsyncLifetime
    {
        private PowershellAdminModuleFixture _data;
        private PowerShell _ps;
        
        private IClientStore _store;

        private Client _client;

        [Fact]
        public async Task ClientIsRemoved()
        {
            string clientId = _client.ClientId;
            Assert.NotNull(await _store.FindClientByIdAsync(clientId));
            _ps.AddParameter("ClientId", clientId);
            _ps.Invoke();
            
            Assert.Null(_data.GetPowershellErrors());
            Assert.Null(await _store.FindClientByIdAsync(clientId));
        }

        public DeleteClient(PowershellAdminModuleFixture data)
        {
            _data = data;
            _ps = data.PowerShell;
            var database = data.Database;
            _ps.AddScript(data.LoadScript(this)).AddParameter("Database", database);
            _store = data.Factory.Resolve<IClientStore>();

            _client = TestData.ClientAllProperties();
            
        }

        public Task InitializeAsync()
        {
            var am = _data.Factory.Resolve<IAdminService>();
            return am.Save(_client);
        }

        public Task DisposeAsync()
        {
            return Task.FromResult(0);
        }
    }
}
