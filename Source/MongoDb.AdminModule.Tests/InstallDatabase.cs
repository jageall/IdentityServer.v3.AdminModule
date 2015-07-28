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
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using MongoDB.Driver;
using Xunit;
using System.Threading.Tasks;

namespace MongoDb.AdminModule.Tests
{
    public class InstallDatabase : IClassFixture<PowershellAdminModuleFixture>
    {
        private readonly PowerShell _ps;
        private readonly string _database;
        private readonly IMongoClient _client;

        [Fact]
        public async Task CreateDatabase()
        {
            var defaults = StoreSettings.DefaultSettings();
            Assert.False(await _client.DatabaseExistsAsync(_database));
            _ps.Invoke();
            Assert.True(await _client.DatabaseExistsAsync(_database));
            var db = _client.GetDatabase(_database);
            Assert.True(await db.CollectionExistsAsync(defaults.AuthorizationCodeCollection),"Authoriz");
            Assert.True(await db.CollectionExistsAsync(defaults.ClientCollection));
            Assert.True(await db.CollectionExistsAsync(defaults.ConsentCollection));
            Assert.True(await db.CollectionExistsAsync(defaults.RefreshTokenCollection));
            Assert.True(await db.CollectionExistsAsync(defaults.ScopeCollection));
            Assert.True(await db.CollectionExistsAsync(defaults.TokenHandleCollection));
            
            await _client.DropDatabaseAsync(_database);
        }

        public InstallDatabase(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            _database = data.Database;
            _ps.AddScript(script).AddParameter("Database", _database);
            _client = data.Client;
        }
    }
}
