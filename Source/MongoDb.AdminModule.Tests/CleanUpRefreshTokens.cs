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
using IdentityServer3.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class CleanUpRefreshTokens : IClassFixture<PowershellAdminModuleFixture>, IAsyncLifetime
    {
        private readonly PowershellAdminModuleFixture _data;
        private PowerShell _ps;
        private IRefreshTokenStore _rtStore;
        private const string Subject = "expired";

        [Fact]
        public async Task RefreshTokensAreDeleted()
        {
            Assert.NotEmpty(await _rtStore.GetAllAsync(Subject));
            _ps.Invoke();

            Assert.Equal(
                new string[] { },
                (await _rtStore.GetAllAsync(Subject)).Select(TestData.ToTestableString));
            
        }

        public CleanUpRefreshTokens(PowershellAdminModuleFixture data)
        {
            _data = data;
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            var database = data.Database;
            _ps.AddScript(script).AddParameter("Database", database);

            _rtStore = data.Factory.Resolve<IRefreshTokenStore>();
        }


        public async Task InitializeAsync()
        {
            var factory = _data.Factory;
            var admin = factory.Resolve<IAdminService>();
            await admin.CreateDatabase(expireUsingIndex: false);
            var token = TestData.RefreshToken(Subject);
            await admin.Save(token.AccessToken.Client);
            await _rtStore.StoreAsync("ac", token);
        }

        public Task DisposeAsync()
        {
            return Task.FromResult(0);
        }
    }
}
