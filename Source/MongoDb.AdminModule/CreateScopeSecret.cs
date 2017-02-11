using System;
using System.Management.Automation;
using IdentityServer3.Core.Models;

namespace IdentityServer3.Admin.MongoDb.Powershell
{
    [Cmdlet(VerbsCommon.New, "ScopeSecret")]
    public class CreateScopeSecret : PSCmdlet
    {
        [Parameter(Mandatory = true),
         ValidateNotNullOrEmpty]
        public string Value { get; set; }
        
        [Parameter]
        public string Description { get; set; }

        [Parameter]
        public string Type { get; set; }
        
        [Parameter]
        public DateTimeOffset? Expiration { get; set; }

        protected override void ProcessRecord()
        {
            var secret = new Secret();
            secret.Type = Type ?? secret.Type;
            secret.Value = Value;
            secret.Description = Description;
            secret.Expiration = Expiration;
            base.WriteObject(secret);
        }
    }
}