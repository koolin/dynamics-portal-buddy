using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Extensions
{
    public class AzureAdB2CJwtOptions : IAzureAdB2COptions
    {
        public string PolicyAuthenticationProperty => "Policy";
        public string ClientId { get; set; }
        public string AzureAdB2CInstance => "https://login.microsoftonline.com/tfp";
        public string Tenant { get; set; }
        public string SignUpSignInPolicyId { get; set; }
        public string SignInPolicyId { get; set; }
        public string SignUpPolicyId { get; set; }
        public string ResetPasswordPolicyId { get; set; }
        public string EditProfilePolicyId { get; set; }
        public string RedirectUri { get; set; }
        public string DefaultPolicy => SignUpSignInPolicyId;
        public string Authority => $"{AzureAdB2CInstance}/{Tenant}/{DefaultPolicy}/v2.0";
        public string Metadata => $"{Authority}/.well-known/openid-configuration";
        public string ClientSecret { get; set; }
        public string ApiUrl { get; set; }
        public string ApiScopes { get; set; }

    }
}
