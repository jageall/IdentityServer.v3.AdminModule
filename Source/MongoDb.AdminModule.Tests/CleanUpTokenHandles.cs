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
using System.Threading.Tasks;

namespace MongoDb.AdminModule.Tests
{
    public class CleanUpTokenHandles : IClassFixture<PowershellAdminModuleFixture>
    {
        private PowerShell _ps;
        private ITokenHandleStore _thStore;
        private const string Subject = "expired";
        Task _setup;

        [Fact]
        public async Task RefreshTokensAreDeleted()
        {
            await _setup;
            Assert.NotEmpty(_thStore.GetAllAsync(Subject).Result);
            _ps.Invoke();

            Assert.Equal(
                new string[] { },
                (await _thStore.GetAllAsync(Subject)).Select(TestData.ToTestableString));

        }

        public CleanUpTokenHandles(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            var database = data.Database;
            _ps.AddScript(script).AddParameter("Database", database);
            _thStore = data.Factory.Resolve<ITokenHandleStore>();
            _setup = Setup(data.Factory);
        }

        private async Task Setup(Factory factory)
        {
            
            var admin = factory.Resolve<IAdminService>();
            await admin.CreateDatabase(expireUsingIndex: false);
            
            var token = TestData.Token(Subject);
            await admin.Save(token.Client);
            await _thStore.StoreAsync("ac", token);
        }
    }
}