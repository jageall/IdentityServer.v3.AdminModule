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
using System.Security.Claims;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MongoDb.AdminModule.Tests
{
    public static class TestData
    {
        public static Client ClientAllProperties()
        {
            return new Client
            {
                AbsoluteRefreshTokenLifetime = 10,
                AccessTokenLifetime = 20,
                AccessTokenType = AccessTokenType.Reference,
                EnableLocalLogin = false,
                AllowRememberConsent = true,
                AlwaysSendClientClaims = true,
                AuthorizationCodeLifetime = 30,
                ClientId = "123",
                ClientName = "TEST",
                ClientSecrets = new List<Secret>()
                {
                    new Secret("secret","secret", WellKnownTime){Type = Constants.SecretTypes.SharedSecret},
                    new Secret("newsecret"),
                },
                ClientUri = "clientUri",
                AllowedCustomGrantTypes = new List<string>()
                {
                    "Restriction1",
                    "Restriction2"
                },
                Enabled = true,
                Flow = Flows.AuthorizationCode,
                IdentityProviderRestrictions = new[] { "idpr" }.ToList(),
                IdentityTokenLifetime = 40,
                LogoUri = "uri:logo",
                PostLogoutRedirectUris = { "uri:logout" },
                RedirectUris = { "uri:redirect" },
                RefreshTokenExpiration = TokenExpiration.Sliding,
                RefreshTokenUsage = TokenUsage.ReUse,
                RequireConsent = true,
                AllowedScopes = { "restriction1", "restriction2", "restriction3" },
                SlidingRefreshTokenLifetime = 50,
                IncludeJwtId = true,
                PrefixClientClaims = true,
                Claims = new List<Claim>()
                {
                    new Claim("client1", "value1"),
                    new Claim("client2", "value2"),
                    new Claim("client3", "value3"),
                }
            };
        }

        public static Scope ScopeAllProperties()
        {
            return new Scope
            {
                Name = "all",
                DisplayName = "displayName",
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim
                    {
                        Name = "claim1",
                        AlwaysIncludeInIdToken = false,
                        Description = "claim1 description"
                    },
                    new ScopeClaim
                    {
                        Name = "claim2",
                        AlwaysIncludeInIdToken = true,
                        Description = "claim2 description"
                    },
                },
                ClaimsRule = "claimsRule",
                Description = "Description",
                Emphasize = true,
                Enabled = false,
                IncludeAllClaimsForUser = true,
                Required = true,
                ShowInDiscoveryDocument = false,
                Type = ScopeType.Identity
            };
        }

        public static Scope ScopeMandatoryProperties()
        {
            return new Scope
            {
                Name = "mandatory",
                DisplayName = "displayName"
            };
        }

        public static AuthorizationCode AuthorizationCode(string subjectId = null)
        {
            return new AuthorizationCode
            {
                IsOpenId = true,
                CreationTime = WellKnownTime,
                Client = Client(),
                RedirectUri = "uri:redirect",
                RequestedScopes = Scopes(),
                Subject = Subject(subjectId),
                WasConsentShown = true,
                Nonce = "test"
            };
        }

        private static DateTimeOffset WellKnownTime
        {
            get { return new DateTimeOffset(2000, 1, 1, 1, 1, 1, 0, TimeSpan.Zero); }
        }

        private static Client Client()
        {
            return ClientAllProperties();
        }

        private static IEnumerable<Scope> Scopes()
        {
            yield return ScopeAllProperties();
            yield return ScopeMandatoryProperties();
        }

        private static ClaimsPrincipal Subject(string subjectId)
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    Claims(subjectId), "authtype"
                    ));
        }

        private static List<Claim> Claims(string subjectId)
        {
            return new List<Claim>
            {
                new Claim("sub", subjectId ?? "foo"),
                new Claim("name", "bar"),
                new Claim("email", "baz@qux.com"),
                new Claim("scope", "scope1"),
                new Claim("scope", "scope2"),
            };
        }

        public static RefreshToken RefreshToken(string subject = null)
        {
            return new RefreshToken
            {
                AccessToken = Token(subject),
                CreationTime = new DateTimeOffset(2000, 1, 1, 1, 1, 1, 0, TimeSpan.Zero),
                LifeTime = 100,
                Version = 10
            };
        }

        public static Token Token(string subject = null)
        {
            return new Token
            {
                Audience = "audience",
                Claims = Claims(subject),
                Client = ClientAllProperties(),
                CreationTime = new DateTimeOffset(2000, 1, 1, 1, 1, 1, 0, TimeSpan.Zero),
                Issuer = "issuer",
                Lifetime = 200,
                Type = "tokenType",
                Version = 10
            };
        }

        private static readonly JsonSerializer Serializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        
        public static string ToTestableString<T>(T subject)
        {
            return JObject.FromObject(subject, Serializer).ToString();
        }
    }
}