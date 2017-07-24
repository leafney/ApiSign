using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using ApiSignFK.Core;

namespace ApiSignFK.Test.Win
{
    public class ApiHttpHelper
    {

        private static string _baseUri = "";

        public static string App_Key = "6800168364";
        public static string App_Secret = "Fr2rsAJYtqlolNZVwNuTqMoBU8sFCdMF";

        public ApiHttpHelper(string baseUri)
        {
            _baseUri = baseUri;
        }

        #region V1

        public IRestResponse HttpGet(string resource, Dictionary<string, string> paramData)
        {
            var client = new RestClient(_baseUri);
            var request = new RestRequest(resource, Method.GET);

            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            //获取所有请求参数
            SortedDictionary<string, string> sortParam = new SortedDictionary<string, string>();
            foreach (var pair in paramData)
            {
                request.AddParameter(pair.Key, pair.Value);
                sortParam.Add(pair.Key, pair.Value);
            }

            //获取请求方式相关参数
            long unixTime = TimeHelper.dtDateTime2Unix();
            sortParam.Add("timestamp", unixTime.ToString());
            sortParam.Add("key", App_Key);
            sortParam.Add("method", request.Method.ToString());
            Uri uri = client.BuildUri(request);
            sortParam.Add("uri", uri.PathAndQuery);
            sortParam.Add("contentlength", "0");

            //计算sign值
            string mySign = SignParse.CreateRequestMySign(sortParam, App_Secret);

            //请求头中添加三个必要参数 key sign timestamp
            request.AddHeader("X-Auth-Sign", mySign);
            request.AddHeader("X-Auth-Key", App_Key);
            request.AddHeader("X-Auth-TimeStamp", unixTime.ToString());

            //发起请求
            IRestResponse queryResult = client.Execute(request);
            return queryResult;
        }



        public IRestResponse HttpPost(string resource, object postData)
        {
            var client = new RestClient(_baseUri);
            var request = new RestRequest(resource, Method.POST);

            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            //将请求内容作为json数据存入Body中
            request.AddJsonBody(postData);
            //获取请求数据的字节长度
            string json_Data = JsonConvert.SerializeObject(postData);
            var bytes = Encoding.UTF8.GetBytes(json_Data);

            //获取所有请求参数
            SortedDictionary<string, string> sortParam = new SortedDictionary<string, string>();

            //获取请求方式相关参数
            long unixTime = TimeHelper.dtDateTime2Unix();
            sortParam.Add("timestamp", unixTime.ToString());
            sortParam.Add("key", App_Key);
            sortParam.Add("method", request.Method.ToString());
            Uri uri = client.BuildUri(request);
            sortParam.Add("uri", uri.PathAndQuery);
            sortParam.Add("contentlength", bytes.Length.ToString());

            //计算sign值
            string mySign = SignParse.CreateRequestMySign(sortParam, App_Secret);

            //请求头中添加三个必要参数 key sign timestamp
            request.AddHeader("X-Auth-Sign", mySign);
            request.AddHeader("X-Auth-Key", App_Key);
            request.AddHeader("X-Auth-TimeStamp", unixTime.ToString());

            IRestResponse queryResult = client.Execute(request);
            return queryResult;
        }

        #endregion


        #region 基于RestSharp封装接口请求方法
        /// <summary>
        /// 基于RestSharp封装接口请求方法
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="method"></param>
        /// <param name="paramData"></param>
        /// <param name="bodyData"></param>
        /// <returns></returns>
        private IRestResponse HttpRequest(string resource, Method method, Dictionary<string, string> paramData = null, object bodyData = null)
        {
            var client = new RestClient(_baseUri);
            var request = new RestRequest(resource, method);
            int contentLength = 0;

            //获取所有请求参数
            SortedDictionary<string, string> sortParam = new SortedDictionary<string, string>();
            switch (method)
            {
                case Method.GET:
                case Method.DELETE:
                    if (paramData != null)
                    {
                        foreach (var pair in paramData)
                        {
                            request.AddParameter(pair.Key, pair.Value);
                            sortParam.Add(pair.Key, pair.Value);
                        }
                    }
                    contentLength = 0;
                    break;
                case Method.POST:
                case Method.PUT:
                    if (bodyData != null)
                    {
                        //将请求内容作为json数据存入Body中
                        request.AddJsonBody(bodyData);
                        //获取请求数据的字节长度
                        string json_Data = JsonConvert.SerializeObject(bodyData);
                        var bytes = Encoding.UTF8.GetBytes(json_Data);
                        contentLength = bytes.Length;
                    }
                    break;
                case Method.HEAD:
                    break;
                case Method.OPTIONS:
                    break;
                case Method.PATCH:
                    break;
                case Method.MERGE:
                    break;
                default:
                    break;
            }

            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            //获取请求方式相关参数
            long unixTime = TimeHelper.dtDateTime2Unix();
            sortParam.Add("timestamp", unixTime.ToString());
            sortParam.Add("key", App_Key);
            sortParam.Add("method", request.Method.ToString());
            Uri uri = client.BuildUri(request);
            sortParam.Add("uri", uri.PathAndQuery);
            sortParam.Add("contentlength", contentLength.ToString());

            //计算sign值
            string mySign = SignParse.CreateRequestMySign(sortParam, App_Secret);

            //请求头中添加三个必要参数 key sign timestamp
            request.AddHeader("X-Auth-Sign", mySign);
            request.AddHeader("X-Auth-Key", App_Key);
            request.AddHeader("X-Auth-TimeStamp", unixTime.ToString());

            //发起请求
            IRestResponse queryResult = client.Execute(request);
            return queryResult;
        } 
        #endregion

        #region V2

        public IRestResponse HttpGetV2(string resource, Dictionary<string, string> paramData)
        {
            return HttpRequest(resource,Method.GET, paramData);
        }

        public IRestResponse HttpPostV2(string resource, object postData)
        {
            return HttpRequest(resource, Method.POST, bodyData: postData);
        }

        public IRestResponse HttpPutV2(string resource, object postData)
        {
            return HttpRequest(resource, Method.PUT, bodyData: postData);
        }

        public IRestResponse HttpDeleteV2(string resource, Dictionary<string, string> paramData)
        {
            return HttpRequest(resource, Method.DELETE, paramData: paramData);
        }
        #endregion
    }
}
