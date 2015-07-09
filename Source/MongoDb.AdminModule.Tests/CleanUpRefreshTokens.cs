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
    public class CleanUpRefreshTokens : IClassFixture<PowershellAdminModuleFixture>
    {
        private PowerShell _ps;
        private IRefreshTokenStore _rtStore;
        private const string Subject = "expired";


        [Fact]
        public void RefreshTokensAreDeleted()
        {
            Assert.NotEmpty(_rtStore.GetAllAsync(Subject).Result);
            _ps.Invoke();

            Assert.Equal(
                new string[] { },
                _rtStore.GetAllAsync(Subject).Result.Select(TestData.ToTestableString));
            
        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            var database = data.Database;
            _ps.AddScript(script).AddParameter("Database", database);
            var adminService = data.Factory.Resolve<IAdminService>();
            adminService.CreateDatabase(expireUsingIndex: false);

            _rtStore = data.Factory.Resolve<IRefreshTokenStore>();
            AddExpiredTokens(data.Factory);
        }

        private void AddExpiredTokens(Factory factory)
        {
            var admin = factory.Resolve<IAdminService>();
            var token = TestData.RefreshToken(Subject);
            admin.Save(token.AccessToken.Client);
            _rtStore.StoreAsync("ac", token).Wait();
        }
    }
}
