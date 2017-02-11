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
using System.Management.Automation;
using System.Threading.Tasks;
using IdentityServer.MongoDb.AdminModule;

namespace IdentityServer3.Admin.MongoDb.Powershell
{
    [Cmdlet(VerbsCommon.Remove, "Tokens")]
    public class DeleteExpiredTokens : MongoCmdlet
    {
        [Parameter]
        public TokenTypes Types { get; set; }

        [Parameter]
        [ValidateNotNull]
        public DateTimeOffset? ExpiredBefore { get; set; }

        protected override void ProcessRecord()
        {
            var service = TokenCleanupService;
            var expiredBefore = (ExpiredBefore ?? DateTimeOffset.UtcNow).ToUniversalTime();
            var expiry = new DateTime(
                expiredBefore.Year,
                expiredBefore.Month,
                expiredBefore.Day,
                expiredBefore.Hour,
                expiredBefore.Minute,
                expiredBefore.Second,
                expiredBefore.Millisecond,
                DateTimeKind.Utc);
            List<Task> cleanupTasks = new List<Task>();
            if ((Types & TokenTypes.AuthorizationCode) == TokenTypes.AuthorizationCode)
            {
                cleanupTasks.Add(service.CleanupAuthorizationCodes(expiry));
            }

            if ((Types & TokenTypes.Refresh) == TokenTypes.Refresh)
            {
                cleanupTasks.Add(service.CleanupRefreshTokens(expiry));
            }

            if ((Types & TokenTypes.Handle) == TokenTypes.Handle)
            {
                cleanupTasks.Add(service.CleanupTokenHandles(expiry));
            }

            Task.WaitAll(cleanupTasks.ToArray());
        }
    }
}
