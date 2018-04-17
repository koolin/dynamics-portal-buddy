using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Extensions
{
    public class DynS2SOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string Resource { get; set; }

        public DynS2SOptions()
        { }

        public DynS2SOptions(string clientId, string clientSecret, string resource, string tenantId)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Resource = resource;
            TenantId = tenantId;

            Assert();
        }

        public void Assert()
        {
            if (string.IsNullOrEmpty(ClientId) || !Guid.TryParse(ClientId, out Guid tempGuid))
            {
                throw new ArgumentException($"{nameof(ClientId)} is null or not valid format");
            }
            if (string.IsNullOrEmpty(ClientSecret))
            {
                throw new ArgumentNullException($"{nameof(ClientSecret)} is null");
            }
            if (!Uri.IsWellFormedUriString(Resource, UriKind.Absolute))
            {
                throw new UriFormatException($"{nameof(Resource)} is not well formed URI");
            }
            if (string.IsNullOrEmpty(TenantId) || !Guid.TryParse(TenantId, out tempGuid))
            {
                throw new ArgumentException($"{nameof(TenantId)} is null or not valid format");
            }
            Trace.TraceInformation("Successfully asserted Dynamics S2S Options");
        }

        public bool Validate()
        {
            try
            {
                Assert();
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Validate of S2S Options failed: {ex.Message}");
                return false;
            }
            return true;
        }

        public virtual bool Equals(DynS2SOptions settings)
        {
            if (!ClientId.Equals(settings.ClientId))
                return false;
            if (!ClientSecret.Equals(settings.ClientSecret))
                return false;
            if (!Resource.Equals(settings.Resource))
                return false;
            if (!TenantId.Equals(settings.TenantId))
                return false;
            return true;
        }
    }
}
