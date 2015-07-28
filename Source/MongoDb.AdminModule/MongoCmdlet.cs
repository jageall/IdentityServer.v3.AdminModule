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
using System.Management.Automation;
using IdentityServer.Admin.MongoDb;
using IdentityServer.Core.MongoDb;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.MongoDb.AdminModule
{
    public abstract class MongoCmdlet : PSCmdlet
    {
        private readonly bool _createDb;
        private IAdminService _adminService;
        private IScopeStore _scopeStore;
        private ICleanupExpiredTokens _tokenCleanupService;

        protected MongoCmdlet(bool createDb = false)
        {
            _createDb = createDb;
        }

        [Parameter]
        public string ConnectionString { get; set; }

        [Parameter]
        public string Database { get; set; }

        [Parameter]
        public string ClientCollection { get; set; }
        [Parameter]
        public string ScopeCollection { get; set; }
        [Parameter]
        public string TokenHandleCollection { get; set; }
        [Parameter]
        public string RefreshTokenCollection { get; set; }
        [Parameter]
        public string ConsentCollection { get; set; }
        [Parameter]
        public string AuthorizationCodeCollection { get; set; }

        public IAdminService AdminService
        {
            get { return _adminService; }
        }

        public IScopeStore ScopeStore
        {
            get { return _scopeStore; }
        }

        public ICleanupExpiredTokens TokenCleanupService
        {
            get { return _tokenCleanupService; }
        }

        protected override void BeginProcessing()
        {
            var storeSettings = StoreSettings.DefaultSettings();
            storeSettings.ConnectionString = ConnectionString ?? storeSettings.ConnectionString;
            storeSettings.Database = Database ?? storeSettings.Database;
            storeSettings.ClientCollection = ClientCollection ?? storeSettings.ClientCollection;
            storeSettings.ScopeCollection = ScopeCollection ?? storeSettings.ScopeCollection;
            storeSettings.ConsentCollection = ConsentCollection ?? storeSettings.ConsentCollection;
            storeSettings.AuthorizationCodeCollection = AuthorizationCodeCollection ?? storeSettings.AuthorizationCodeCollection;
            storeSettings.RefreshTokenCollection = RefreshTokenCollection ?? storeSettings.RefreshTokenCollection;
            storeSettings.TokenHandleCollection = TokenHandleCollection ?? storeSettings.TokenHandleCollection;
            CanCreateDatabase(storeSettings);
            
            var serviceFactory = new ServiceFactory(null, storeSettings);
            serviceFactory.Register(new Registration<IMongoClient>(new MongoClient(storeSettings.ConnectionString)));
            var factory = new Factory(storeSettings,serviceFactory, new AdminServiceRegistry());
            _adminService = factory.Resolve<IAdminService>();
            _tokenCleanupService = factory.Resolve<ICleanupExpiredTokens>();
            _scopeStore = factory.Resolve<IScopeStore>();
            base.BeginProcessing();
        }

        void CanCreateDatabase(StoreSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);

            if (client.DatabaseExistsAsync(settings.Database).Result || _createDb)
                return;
            throw new InvalidOperationException("Database does not exist");
        }
    }
}