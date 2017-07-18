using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace ApiSign.Core
{
    public class SignParse
    {

        #region 获取请求数据参数，并以“参数名=参数值”的形式组成数组
        /// <summary>
        /// 获取请求数据参数，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>
        /// request回来的信息组成的数组；
        /// 参数中key的值；
        /// 参数中sign的值
        /// </returns>
        public RequestData GetRequestParams(HttpContext context)
        {
            RequestData reqData = new RequestData();

            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();

            //获取请求类型，Content-Length ,uri,请求头参数及Url中的请求参数

            //请求类型
            string reqMethod = context.Request.Method;
            sArray.Add("method", reqMethod);
            //uri
            string reqUri = context.Request.Scheme;
            sArray.Add("uri", reqUri);
            //Content-Length
            long reqContLength = context.Request.ContentLength == null ? 0 : (long)context.Request.ContentLength;
            sArray.Add("contentlength", reqContLength.ToString());

            #region 请求头参数
            //请求头参数
            IHeaderDictionary headers = context.Request.Headers;
            foreach (var item in headers)
            {
                if (item.Key == "X-Auth-Sign")
                {
                    //sign值不参与签名计算
                    reqData.ReqSign = item.Value;
                }
                if (item.Key == "X-Auth-Key")
                {
                    sArray.Add("key", item.Value);
                    reqData.ReqKey = item.Value;
                }
                if (item.Key == "X-Auth-TimeStamp")
                {
                    sArray.Add("timestamp", item.Value);
                    reqData.ReqTimeStamp = item.Value;
                }
            }
            #endregion

            #region 当请求类型为 GET  DELETE 时，获取url中的请求参数
            //当请求类型为 GET  DELETE 时，获取url中的请求参数
            if (reqMethod == "GET" || reqMethod == "DELETE")
            {
                IQueryCollection forms = context.Request.Query;
                foreach (var item in forms)
                {
                    sArray.Add(item.Key, item.Value[0].ToString());
                }
            } 
            #endregion

            reqData.ReqDics = sArray;

            return reqData;
        }
        #endregion

        #region 对请求参数进行签名
        /// <summary>
        /// 对请求参数进行签名
        /// </summary>
        /// <param name="paramReqs"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public string SignRequest(SortedDictionary<string, string> paramReqs, string secret)
        {
            //待签名请求参数数组
            Dictionary<string, string> sPara = new Dictionary<string, string>();

            //过滤签名参数数组
            sPara = SignCore.FilterPara(paramReqs);

            //“参数=参数值”的模式用“&”字符拼接
            string temp = SignCore.CreateLinkString(sPara);

            //获得签名结果
            string mysign = SignCore.CreateSign(temp, secret);

            return mysign;
        }
        #endregion


        #region 验证签名是否正确
        /// <summary>
        /// 验证签名是否正确
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="sign">签名结果</param>
        /// <param name="key">密钥</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns>验证结果</returns>
        public bool VerifySign(string prestr, string sign, string secret, string _input_charset = "utf-8")
        {
            string mysign = SignCore.CreateSign(prestr, secret, _input_charset);
            if (mysign == sign)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 验证sign是否正确
        /// <summary>
        /// 验证sign是否正确
        /// </summary>
        /// <param name="paramReqs">请求的参数</param>
        /// <param name="sign">签名结果</param>
        /// <param name="secret">密钥</param>
        /// <param name="_input_charset">编码格式</param>
        /// <returns></returns>
        public bool VerifySign(SortedDictionary<string, string> paramReqs, string sign, string secret, string _input_charset = "utf-8")
        {
            //过滤签名参数数组
            var sPara = SignCore.FilterPara(paramReqs);

            //“参数=参数值”的模式用“&”字符拼接
            string temp = SignCore.CreateLinkString(sPara);

            //获得签名结果
            string mysign = SignCore.CreateSign(temp, secret, _input_charset);
            if (mysign == sign)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

    }

    /// <summary>
    /// 请求参数
    /// </summary>
    public class RequestData
    {
        /// <summary>
        /// 请求头中的key的值
        /// </summary>
        public string ReqKey { get; set; }
        /// <summary>
        /// 请求头中的sign的值
        /// </summary>
        public string ReqSign { get; set; }
        /// <summary>
        /// 请求头中的timestamp的值
        /// </summary>
        public string ReqTimeStamp { get; set; }
        /// <summary>
        /// 所有请求参数的键值对数组
        /// </summary>
        public SortedDictionary<string, string> ReqDics = new SortedDictionary<string, string>();
    }

}
