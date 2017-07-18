using ApiSign.Test.MVCWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net;
using System.Net.Http;
using ApiSign.Core;

namespace ApiSign.Test.MVCWeb.Filters
{
    public class APISignFilterAttribute:ActionFilterAttribute
    {
        List<ShowKey> showKeys = new List<ShowKey> {
            new ShowKey {key="6800168364",secret="Fr2rsAJYtqlolNZVwNuTqMoBU8sFCdMF" },
            new ShowKey {key="2088911242",secret="cfvepv0kzm34epzn8bpcsct94357fz1j" }
        };

        private int delayMinits = 10;//请求延迟时间10分钟


        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                return;
            }

            SignParse parse = new SignParse();
           // RequestData reqData = parse.GetRequestParams((Microsoft.AspNetCore.Http.HttpContext)actionContext);




            base.OnActionExecuting(actionContext);
        }

    }
}