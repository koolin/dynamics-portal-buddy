using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Extensions
{
    public class Dyn365portalAuthOptions
    {
        public string Uri { get; set; }

        public string PublicKey { get; set; }

        public string Authorize => $"{Uri}/_services/Auth/Authorize";

        public string Token => $"{Uri}/_services/Auth/Token";

    }
}
