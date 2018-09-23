using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class LoaderController : Controller
    {
        public AzureAdB2CJwtOptions B2COptions;
        public Dyn365portalAuthOptions Dyn365portalOptions;

        public LoaderController(IOptions<AzureAdB2CJwtOptions> b2cOptions, IOptions<Dyn365portalAuthOptions> dyn365portalOptions)
        {
            B2COptions = b2cOptions.Value;
            Dyn365portalOptions = dyn365portalOptions.Value;
        }

        [ResponseCache(Duration = 100)]
        public IActionResult Index(string authType = "aadb2c")
        {
            if (authType == "dyn365portal")
            {
                return View(new AzureAdB2CJwtOptions { RedirectUri = Dyn365portalOptions.RedirectUri });
            }
            return View(B2COptions);
        }
    }
}
