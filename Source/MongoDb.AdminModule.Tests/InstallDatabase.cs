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

namespace MongoDb.AdminModule.Tests
{
    public class InstallDatabase : IClassFixture<PowershellAdminModuleFixture>
    {
        private PowerShell _ps;
        private string _database;
        private IMongoClient _client;

        [Fact]
        public void CreateDatabase()
        {
            var defaults = StoreSettings.DefaultSettings();
            Assert.False(_client.DatabaseExistsAsync(_database).Result);
            _ps.Invoke();
            Assert.True(_client.DatabaseExistsAsync(_database).Result);
            var db = _client.GetDatabase(_database);
            Assert.True(db.CollectionExistsAsync(defaults.AuthorizationCodeCollection).Result);
            Assert.True(db.CollectionExistsAsync(defaults.ClientCollection).Result);
            Assert.True(db.CollectionExistsAsync(defaults.ConsentCollection).Result);
            Assert.True(db.CollectionExistsAsync(defaults.RefreshTokenCollection).Result);
            Assert.True(db.CollectionExistsAsync(defaults.ScopeCollection).Result);
            Assert.True(db.CollectionExistsAsync(defaults.TokenHandleCollection).Result);
            //TODO: verify indexes maybe?
            _client.DropDatabaseAsync(_database).Wait();
        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            _database = data.Database;
            _ps.AddScript(script).AddParameter("Database", _database);
            _client = data.Client;
        }
    }
}
