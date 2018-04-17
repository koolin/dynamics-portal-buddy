using Microsoft.AspNetCore.Mvc;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using WebApp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class XrmController : Controller
    {
        public CrmServiceClient CrmServiceClient;
        public OrganizationServiceContext ServiceContext;
        public IOrganizationService OrgService;

        public XrmController(CrmCoreServiceClient crmCoreClient)
        {
            CrmServiceClient = crmCoreClient.CrmServiceClient;
            ServiceContext = crmCoreClient.ServiceContext;
            OrgService = crmCoreClient.OrgService;
        }

        public XrmController(CrmServiceClient crmClient, OrganizationServiceContext context, IOrganizationService orgService)
        {
            CrmServiceClient = crmClient;
            ServiceContext = context;
            OrgService = orgService;
        }
    }
}