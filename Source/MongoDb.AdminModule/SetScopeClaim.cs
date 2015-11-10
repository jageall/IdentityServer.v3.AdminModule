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
using IdentityServer3.Core.Models;

namespace IdentityServer3.Admin.MongoDb.Powershell
{
    [Cmdlet(VerbsCommon.Set, "ScopeClaim")]
    public class SetScopeClaim : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public Scope Scope { get; set; }

        [Parameter]
        public ScopeClaim[] Claims { get; set; }

        [Parameter]
        public SwitchParameter ReplaceExisting { get; set; }

        protected override void ProcessRecord()
        {
            var existing = Scope.Claims.ToList();
            foreach (var scopeClaim in Claims)
            {
                if (Claims.Any(x => String.Equals(x.Name, scopeClaim.Name, StringComparison.Ordinal) && x != scopeClaim))
                {
                    throw new ArgumentException("Claims cannot be specified more than once");
                }
            }
            var updated = new List<ScopeClaim>();
            if (!ReplaceExisting)
            {
                updated.AddRange(existing.Where(scopeClaim => !Claims.Any(x => String.Equals(x.Name, scopeClaim.Name, StringComparison.Ordinal))));
            }
            updated.AddRange(Claims);

            Scope.Claims = updated;

            WriteObject(Scope);
        }
    }
}