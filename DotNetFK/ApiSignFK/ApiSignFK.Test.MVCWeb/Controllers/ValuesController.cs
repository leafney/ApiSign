using ApiSignFK.Test.MVCWeb.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiSignFK.Test.MVCWeb.Controllers
{
    public class ValuesController : ApiController
    {
        [APISignFilter]
        public string Get()
        {
            return "Hello World";
        }

    }
}
