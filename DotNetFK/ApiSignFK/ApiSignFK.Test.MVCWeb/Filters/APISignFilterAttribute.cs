using ApiSignFK.Test.MVCWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Net;
using ApiSignFK.Core;

namespace ApiSignFK.Test.MVCWeb.Filters
{
    public class APISignFilterAttribute: ActionFilterAttribute
    {

        List<Showkey> showKeys = new List<Showkey> {
            new Showkey {key="6800168364",secret="Fr2rsAJYtqlolNZVwNuTqMoBU8sFCdMF" },
            new Showkey {key="2088911242",secret="cfvepv0kzm34epzn8bpcsct94357fz1j" }
        };

        private int delayMinits = 10;//请求延迟时间2分钟


        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                return;
            }

            SignParse parse = new SignParse();
            RequestData reqData = parse.GetRequestParams();

            string Ckey = reqData.ReqKey;
            string Csign = reqData.ReqSign;
            string Ctime = reqData.ReqTimeStamp;
            var Cdis = reqData.ReqDics;

            #region 验证是否超时

            long deM = TimeHelper.dtTimeSpan(Convert.ToInt64(Ctime), DateTime.Now, "M");
            if (deM > delayMinits)
            {
                //请求超时，无效
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, new { code = 4002, msg = "Request Timeout" });

                return;
            }
            #endregion

            #region 验证appKey
            if (string.IsNullOrEmpty(Ckey))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, new { code = 4003, msg = "key is Empty" });
                return;
            }

            //从数据库中查询AppKey和AppSecret
            Showkey ishavekey = showKeys.Where(p => p.key == Ckey).FirstOrDefault();

            if (ishavekey == null)
            {
                //key不存在

                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, new { code = 4004, msg = "key is Error" });

                return;
            }
            #endregion

            #region 验证Sign

            string thisSecrret = ishavekey.secret;
            bool isSign = parse.VerifySign(Cdis, Csign, thisSecrret);
            if (!isSign)
            {
                //sign验证失败
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, new { code = 4005, msg = "Sign is Error" });
                return;
            }
            #endregion




            base.OnActionExecuting(actionContext);
        }

    }
}