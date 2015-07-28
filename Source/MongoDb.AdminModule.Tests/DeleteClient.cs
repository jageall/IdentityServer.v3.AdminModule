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
using IdentityServer.Admin.MongoDb;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class DeleteClient : IClassFixture<PowershellAdminModuleFixture>
    {
        private PowershellAdminModuleFixture _data;
        private PowerShell _ps;
        
        private IClientStore _store;
        private string _clientId;

        [Fact]
        public void ClientIsRemoved()
        {
            Assert.NotNull(_store.FindClientByIdAsync(_clientId).Result);
            _ps.AddParameter("ClientId", _clientId);
            _ps.Invoke();
            
            Assert.Null(_data.GetPowershellErrors());
            Assert.Null(_store.FindClientByIdAsync(_clientId).Result);
        }

        public DeleteClient(PowershellAdminModuleFixture data)
        {
            _data = data;
            _ps = data.PowerShell;
            var database = data.Database;
            _ps.AddScript(data.LoadScript(this)).AddParameter("Database", database);
            _store = data.Factory.Resolve<IClientStore>();
            var am = data.Factory.Resolve<IAdminService>();

            var client = TestData.ClientAllProperties();
            _clientId = client.ClientId;
            am.Save(client);
        }
    }
}
