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

using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using IdentityServer.Admin.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using IdentityServer3.Admin.MongoDb.Powershell;
using IdentityServer3.MongoDb;
using MongoDB.Driver;

namespace MongoDb.AdminModule.Tests
{
    public class PowershellAdminModuleFixture : IDisposable
    {
        private readonly PowerShell _powerShell;
        private readonly string _database;
        private readonly IMongoClient _client;
        private readonly Factory _factory;

        public PowershellAdminModuleFixture()
        {
            _powerShell = PowerShell.Create();
            _powerShell.AddCommand("Import-Module").AddParameter("Name", typeof(CreateScope).Assembly.Location);
            _database = Guid.NewGuid().ToString("N");
            
            var settings = StoreSettings.DefaultSettings();
            settings.Database = _database;
            var config = new ServiceFactory(null, settings);
            _factory = new Factory(settings, config, 
                new AdminServiceRegistry());
            _client = new MongoClient(settings.ConnectionString);
        }

        public PowerShell PowerShell
        {
            get { return _powerShell; }
        }

        public string Database
        {
            get { return _database; }
        }

        public IMongoClient Client
        {
            get { return _client; }
        }

        public Factory Factory
        {
            get { return _factory; }
        }

        public void Dispose()
        {
            var failed = GetPowershellErrors();
            PowerShell.Dispose();
            if (Environment.GetEnvironmentVariable("idsvr_mongodb_no_teardown") == null)
            {
                if (_client.DatabaseExistsAsync(Database).Result)
                    _client.DropDatabaseAsync(Database).Wait();
            }
            //var dbns = _client.ListDatabasesAsync().Result.ToListAsync().Result.Select(x => x["name"].AsString);
            //foreach (var dbn in dbns)
            //{
            //    Guid ignored;
            //    if (Guid.TryParse(dbn, out ignored))
            //        _client.DropDatabaseAsync(dbn).Wait();
            //}

            if (failed != null) throw failed;
        }

        public AggregateException GetPowershellErrors()
        {
            AggregateException failed = null;
            if (PowerShell.Streams.Error.Count > 0)
            {
                var exceptions = PowerShell.Streams.Error.Select(x => x.Exception).ToArray();
                foreach (var exception in exceptions)
                {
                    Console.WriteLine(exception);
                }
                failed = new AggregateException(exceptions);
            }
            return failed;
        }

        public string LoadScript(object o)
        {
            var type = o.GetType();

            var scriptResource = string.Format("{0}.Scripts.{1}.ps1", type.Namespace, type.Name);

            using (var stream = type.Assembly.GetManifestResourceStream(scriptResource))
            {
                if (stream == null)
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat("Could not find resource '{0}'", scriptResource);
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("Available Resources :");
                    foreach (var resourceName in type.Assembly.GetManifestResourceNames())
                    {
                        sb.AppendLine(resourceName);
                    }
                    throw new InvalidOperationException(sb.ToString());
                }
                return new StreamReader(stream).ReadToEnd();
            }

        }
    }
}
