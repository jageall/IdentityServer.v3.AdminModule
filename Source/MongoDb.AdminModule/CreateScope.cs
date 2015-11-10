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

using System.Linq;
using System.Management.Automation;
using IdentityServer3.Core.Models;

namespace IdentityServer3.Admin.MongoDb.Powershell
{
    [Cmdlet(VerbsCommon.New, "Scope")]
    public class CreateScope : PSCmdlet
    {
        static readonly Scope DefaultValues = new Scope();
        [Parameter]
        public string Name { get; set; }
        [Parameter]
        public ScopeClaim[] Claims { get; set; }

        [Parameter]
        public string ClaimsRule { get; set; }
        [Parameter]
        public string Description { get; set; }
        [Parameter]
        public string DisplayName { get; set; }
        [Parameter]
        public bool? Emphasize { get; set; }
        [Parameter]
        public bool? Enabled { get; set; }
        [Parameter]
        public bool? IncludeAllClaimsForUser { get; set; }
        [Parameter]
        public bool? ShowInDiscoveryDocument { get; set; }
        [Parameter]
        public bool? Required { get; set; }
        [Parameter]
        public ScopeType? Type { get; set; }

        protected override void ProcessRecord()
        {
            var scope = new Scope()
            {
                Claims = (Claims ?? new ScopeClaim[] {}).ToList(),
                ClaimsRule = ClaimsRule,
                Description = Description,
                DisplayName = DisplayName,
                Emphasize = Emphasize.GetValueOrDefault(DefaultValues.Emphasize),
                Enabled = Enabled.GetValueOrDefault(DefaultValues.Enabled),
                IncludeAllClaimsForUser =
                    IncludeAllClaimsForUser.GetValueOrDefault(DefaultValues.IncludeAllClaimsForUser),
                Name = Name,
                Required = Required.GetValueOrDefault(DefaultValues.Required),
                ShowInDiscoveryDocument =
                    ShowInDiscoveryDocument.GetValueOrDefault(DefaultValues.ShowInDiscoveryDocument),
                Type = Type.GetValueOrDefault(DefaultValues.Type)
            };
            
            WriteObject(scope);
        }
    }
}
