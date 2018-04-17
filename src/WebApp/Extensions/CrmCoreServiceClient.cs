using Adoxio.Dynamics.Connect;
using Microsoft.Extensions.Options;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Diagnostics;

namespace WebApp.Extensions
{
    public class CrmCoreServiceClient
    {
        private const string ServiceEndpoint = @"/xrmservices/2011/organization.svc/web?SdkClientVersion=";
        private const string AadInstance = "https://login.microsoftonline.com/";

        private OrganizationServiceContext _organizationServiceContext;
        private OrganizationServiceProxy _organizationServiceProxy;
        private CrmContext _crmContext;

        public CrmCoreServiceClient(IOptions<DynS2SOptions> s2sOptions, IOptions<DynConnStringOptions> connStringOptions) 
        {
            if (s2sOptions.Value.Validate())
            {
                var crmContext = new CrmContext(new S2SAppSettings(s2sOptions.Value.ClientId, s2sOptions.Value.ClientSecret, s2sOptions.Value.Resource, s2sOptions.Value.TenantId));
                var crmServiceClientWeb = new CrmServiceClient(crmContext.WebProxyClient);
                if (crmServiceClientWeb.IsReady)
                {
                    Trace.TraceInformation("Setting CrmContext with Adoxio Dynamics Connect");
                    _crmContext = crmContext;
                }
                else
                {
                    Trace.TraceWarning("unable to create CrmServiceClient based on S2S settings from Adoxio Dynamics Connect");
                }
            }

            if (!string.IsNullOrEmpty(connStringOptions?.Value.ConnString))
            {
                Trace.TraceInformation("Setting with Conn String");
                var crmServiceClientService = new CrmServiceClient(connStringOptions.Value.ConnString);
                if (crmServiceClientService.IsReady)
                {
                    Trace.TraceInformation("Setting ServiceProxy with CrmServiceClient");
                    _organizationServiceProxy = crmServiceClientService.OrganizationServiceProxy;
                }
                else
                {
                    Trace.TraceWarning("unable to create CrmServiceClient based on connection string");
                }
            }

            if (OrgService == null)
            {
                throw new Exception("unable to create CrmServiceClient - both S2S and ConnString failed");
            }
        }

        public CrmServiceClient CrmServiceClient
        {
            get { return ServiceProxy != null ? new CrmServiceClient(ServiceProxy) : new CrmServiceClient(WebProxyClient); }
        }

        public OrganizationWebProxyClient WebProxyClient
        {
            get { return _crmContext?.WebProxyClient; }
        }

        public OrganizationServiceProxy ServiceProxy
        {
            get { return _organizationServiceProxy; }
        }

        public OrganizationServiceContext ServiceContext
        {
            get
            {
                Trace.TraceInformation("Get ServiceContext");
                if (_organizationServiceContext == null)
                {
                    Trace.TraceInformation("ServiceContext is null, creating new context with OrgService");
                    _organizationServiceContext = new OrganizationServiceContext(OrgService);
                }

                return _organizationServiceContext;
            }
        }

        public IOrganizationService OrgService
        {
            get { return (IOrganizationService)ServiceProxy ?? WebProxyClient; }
        }
    }
}
