using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Security.Claims;

namespace WebApp.Extensions
{
    public static class OrganizationServiceExtensions
    {
        public static Entity GetContact(this IOrganizationService service, ClaimsIdentity identity)
        {
            // Get by Azure AD B2C oid claim
            var oid = identity.FindFirst("oid")?.Value;
            var objId = identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            if (oid != null || objId != null)
            {
                return service.GetContactByExternalIdentityUsername(oid ?? objId);
            }

            // Get by Dyn365 portal contactid nameidentifier claim
            var contactid = identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (contactid != null)
            {
                return service.GetContactById(new Guid(contactid));
            }

            return null;
        }

        public static Entity GetContactById(this IOrganizationService service, Guid contactid)
        {
            var fetchxml =
                $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' nolock='true'>
                      <entity name='contact'>
                        <all-attributes />
                        <filter type='and'>
                            <condition attribute='contactid' operator='eq' value='{contactid}' />
                        </filter>
                      </entity>
                    </fetch>";

            var contactResponse = service.RetrieveMultiple(new FetchExpression(fetchxml));

            return contactResponse.Entities.SingleOrDefault();
        }

        public static Entity GetContactByExternalIdentityUsername(this IOrganizationService service, string username)
        {
            var fetchxml =
                $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' nolock='true'>
                      <entity name='contact'>
                        <all-attributes />
                        <link-entity name='adx_externalidentity' from='adx_contactid' to='contactid' alias='ab'>
                          <filter type='and'>
                            <condition attribute='adx_username' operator='eq' value='{username}' />
                          </filter>
                        </link-entity>
                      </entity>
                    </fetch>";

            var contactResponse = service.RetrieveMultiple(new FetchExpression(fetchxml));

            return contactResponse.Entities.SingleOrDefault();
        }
    }
}
