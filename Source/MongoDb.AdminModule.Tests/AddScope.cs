﻿/*
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
using System.Threading.Tasks;
using IdentityServer3.Admin.MongoDb;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Xunit;

namespace MongoDb.AdminModule.Tests
{
    public class AddScope : IClassFixture<PowershellAdminModuleFixture>
    {
        private readonly PowerShell _ps;
        private readonly IScopeStore _scopeStore;

        [Fact]
        public async Task VerifyAdd()
        {
            
            Assert.Empty(await _scopeStore.GetScopesAsync(false));
            _ps.Invoke();

            var result = (await _scopeStore.GetScopesAsync(false)).ToArray();
            Assert.Equal(1, result.Length);
            var scope = result.Single();
            Assert.Equal("unit_test_scope", scope.Name);
            Assert.Equal("displayName", scope.DisplayName);
            Assert.Equal("claim description", scope.Description);
            Assert.Equal("customRuleName", scope.ClaimsRule);
            Assert.True(scope.Emphasize);
            Assert.True(scope.Enabled);
            Assert.True(scope.IncludeAllClaimsForUser);
            Assert.True(scope.Required);
            Assert.True(scope.ShowInDiscoveryDocument);
            Assert.Equal(ScopeType.Identity, scope.Type);
            Assert.Equal(2, scope.Claims.Count());
            var first = scope.Claims.OrderBy(x => x.Name).First();
            Assert.Equal("unit_test_claim1", first.Name);
            Assert.Equal("Sample description for unit test", first.Description);
            Assert.True(first.AlwaysIncludeInIdToken);

            var second = scope.Claims.OrderBy(x => x.Name).Skip(1).First();
            Assert.Equal("unit_test_claim2", second.Name);
            Assert.Equal("Sample description", second.Description);
            Assert.False(second.AlwaysIncludeInIdToken);
            Assert.True(scope.AllowUnrestrictedIntrospection);
            Assert.Equal(new List<Secret> {
                new Secret("secret1"),
                new Secret("secret2", "description", new DateTimeOffset(2000, 1, 1, 1, 1, 1, 1, TimeSpan.Zero)) {Type = "SomeOtherType"} }.Select(TestData.ToTestableString), 
                scope.ScopeSecrets.Select(TestData.ToTestableString));
        }

        public AddScope(PowershellAdminModuleFixture data)
        {
            _ps = data.PowerShell;
            var script = data.LoadScript(this);
            var database = data.Database;
            _ps.AddScript(script).AddParameter("Database", database);
            var adminService = data.Factory.Resolve<IAdminService>();
            adminService.CreateDatabase();
            _scopeStore = data.Factory.Resolve<IScopeStore>();
        }
    }
}
