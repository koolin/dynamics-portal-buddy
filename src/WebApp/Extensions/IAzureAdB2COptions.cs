using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Extensions
{
    public interface IAzureAdB2COptions
    {
        string PolicyAuthenticationProperty { get; }
        string ClientId { get; set; }
        string AzureAdB2CInstance { get; }
        string Tenant { get; set; }
        string SignUpSignInPolicyId { get; set; }
        string SignInPolicyId { get; set; }
        string SignUpPolicyId { get; set; }
        string ResetPasswordPolicyId { get; set; }
        string EditProfilePolicyId { get; set; }
        string RedirectUri { get; set; }
        string DefaultPolicy { get; }
        string Authority { get; }
        string Metadata { get; }

    }
}
