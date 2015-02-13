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
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Claims;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class AddClient : IUseFixture<PowershellAdminModuleFixture>
    {
        private PowershellAdminModuleFixture _data;
        private PowerShell _ps;
        private IClientStore _store;

        [Fact]
        public void CheckClient()
        {
            _ps.Invoke();
            Assert.Null(_data.GetPowershellErrors());
            var client = _store.FindClientByIdAsync("test").Result;
            Assert.NotNull(client);
            Assert.Equal(10, client.AbsoluteRefreshTokenLifetime);
            Assert.Equal(20, client.AccessTokenLifetime);
            Assert.Equal(AccessTokenType.Reference, client.AccessTokenType);
            Assert.Equal(false, client.EnableLocalLogin);
            Assert.Equal(true, client.AllowRememberConsent);
            Assert.Equal(30, client.AuthorizationCodeLifetime);
            Assert.Equal("unittest", client.ClientName);
            Assert.NotNull(client.ClientSecrets);
            Assert.NotEmpty(client.ClientSecrets);
            Assert.Equal(2, client.ClientSecrets.Count);
            Assert.Equal("WxFhjC5EAnh30M0JIe0Wa58Xb1BYf8kedTTdKUbbd9Y=", client.ClientSecrets[0].Value);
            Assert.Equal("testsecret", client.ClientSecrets[0].Description);
            Assert.Equal(new DateTimeOffset(2000, 1,1,1,1,1,0,TimeSpan.Zero), client.ClientSecrets[0].Expiration);
            Assert.Equal("cckjYsokygyEIiAV5Y/sIStXfM1W7qwGTo3K9VxmT4xXfeRFWekmMbr2wKZy2T1HSNXW6vjNyRxajG46oEBrAw==", client.ClientSecrets[1].Value);
            Assert.Null(client.ClientSecrets[1].Description);
            Assert.Null(client.ClientSecrets[1].Expiration);
            Assert.Equal(true, client.Enabled);
            Assert.Equal(Flows.AuthorizationCode, client.Flow);
            Assert.Equal(new List<string>{"restriction1", "restriction2"}, client.IdentityProviderRestrictions);
            Assert.Equal(40, client.IdentityTokenLifetime);
            Assert.Equal("uri:logo", client.LogoUri);
            Assert.Equal(new List<string>{"uri:logout1", "uri:logout2"}, client.PostLogoutRedirectUris);
            Assert.Equal(new List<string>{"uri:redirect1", "uri:redirect2"}, client.RedirectUris);
            Assert.Equal(TokenExpiration.Sliding, client.RefreshTokenExpiration);
            Assert.Equal(TokenUsage.ReUse, client.RefreshTokenUsage);
            Assert.Equal(true, client.RequireConsent);
            Assert.Equal(new List<string> { "openid", "email", "roles" }, client.ScopeRestrictions);         
            Assert.Equal(new List<string> { "grantrestriction1", "grantrestriction2", "grantrestriction3" }, client.CustomGrantTypeRestrictions);
            Assert.Equal(50, client.SlidingRefreshTokenLifetime);
            Assert.True(client.AlwaysSendClientClaims);         
            Assert.True(client.PrefixClientClaims);         
            Assert.True(client.IncludeJwtId);
            Assert.Equal(new List<Claim> { new Claim("claimtype1", "claimvalue1"), new Claim("claimtype2", "claimvalue2"), }.Select(TestData.ToTestableString), client.Claims.Select(TestData.ToTestableString));
        }

        public void SetFixture(PowershellAdminModuleFixture data)
        {
            _data = data;
            _ps = data.PowerShell;
            var database = data.Database;
            _ps.AddScript(data.LoadScript(this)).AddParameter("Database", database);
            _store = data.Factory.Resolve<IClientStore>();
            var adminService = data.Factory.Resolve<IAdminService>();
            adminService.CreateDatabase();
        }
    }
}
