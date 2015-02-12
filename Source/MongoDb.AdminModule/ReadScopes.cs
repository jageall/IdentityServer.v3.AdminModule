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
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.MongoDb.AdminModule
{
    [Cmdlet(VerbsCommon.Get, "Scopes")]
    public class ReadScopes : MongoCmdlet
    {
        [Parameter(HelpMessage = "Gets the predefined standard scopes from identity server. These need to be persisted into the database using Set-Scope if you want them available to the application at runtime")]
        public SwitchParameter Predefined { get; set; }

        protected override void BeginProcessing()
        {
            if(!Predefined)
                base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            IEnumerable<Scope> scopes;
            if (Predefined)
            {
                var builtin = BuiltInScopes();
                scopes = builtin;
            }
            else
            {
                scopes = ScopeStore.GetScopesAsync().Result;
            }

            foreach (var scope in scopes)
            {
                WriteObject(scope);
            }
        }

        public static IEnumerable<Scope> BuiltInScopes()
        {
            foreach (var scope in StandardScopes.All)
            {
                yield return scope;
            }
            yield return StandardScopes.AllClaims;
            yield return StandardScopes.OfflineAccess;
            yield return StandardScopes.Roles;
        }
    }
}
