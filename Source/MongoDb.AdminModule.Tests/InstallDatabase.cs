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
using MongoDB.Driver;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class InstallDatabase : IUseFixture<PowershellAdminModuleFixture>
    {
        private PowerShell _ps;
        private string _database;
        private MongoServer _server;

        [Fact]
        public void CreateDatabase()
        {
            var defaults = ServiceFactory.DefaultStoreSettings();
            Assert.False(_server.DatabaseExists(_database));
            _ps.Invoke();
            Assert.True(_server.DatabaseExists(_database));
            var db = _server.GetDatabase(_database);
            Assert.True(db.CollectionExists(defaults.AuthorizationCodeCollection));
            Assert.True(db.CollectionExists(defaults.ClientCollection));
            Assert.True(db.CollectionExists(defaults.ConsentCollection));
            Assert.True(db.CollectionExists(defaults.RefreshTokenCollection));
            Assert.True(db.CollectionExists(defaults.ScopeCollection));
            Assert.True(db.CollectionExists(defaults.TokenHandleCollection));
            //TODO: verify indexes maybe?
            _server.DropDatabase(_database);
        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            _database = data.Database;
            _ps.AddScript(script).AddParameter("Database", _database);
            _server = data.Server;
        }
    }
}
