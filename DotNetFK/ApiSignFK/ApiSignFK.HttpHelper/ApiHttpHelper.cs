using ApiSignFK.Core;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiSignFK.HttpHelper
{
    public class ApiHttpHelper
    {

        private static string _baseUri = "";
        private static string _appKey = "";
        private static string _appSecret = "";

        //public static string App_Key = "6800168364";
        //public static string App_Secret = "Fr2rsAJYtqlolNZVwNuTqMoBU8sFCdMF";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUri">请求地址</param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        public ApiHttpHelper(string baseUri, string appKey, string appSecret)
        {
            _baseUri = baseUri;
            _appKey = appKey;
            _appSecret = appSecret;
        }

        #region 基于RestSharp封装接口请求方法
        /// <summary>
        /// 基于RestSharp封装接口请求方法
        /// </summary>
        /// <param name="resource">请求资源</param>
        /// <param name="method">请求方法</param>
        /// <param name="paramData">Url请求参数</param>
        /// <param name="bodyData">Body请求参数</param>
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
            sortParam.Add("key", _appKey);
            sortParam.Add("method", request.Method.ToString());
            Uri uri = client.BuildUri(request);
            sortParam.Add("uri", uri.PathAndQuery);
            sortParam.Add("contentlength", contentLength.ToString());

            //计算sign值
            string mySign = SignParse.CreateRequestMySign(sortParam, _appSecret);

            //请求头中添加三个必要参数 key sign timestamp
            request.AddHeader("X-Auth-Sign", mySign);
            request.AddHeader("X-Auth-Key", _appKey);
            request.AddHeader("X-Auth-TimeStamp", unixTime.ToString());

            //发起请求
            IRestResponse queryResult = client.Execute(request);
            return queryResult;
        }
        #endregion



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
            sortParam.Add("key", _appKey);
            sortParam.Add("method", request.Method.ToString());
            Uri uri = client.BuildUri(request);
            sortParam.Add("uri", uri.PathAndQuery);
            sortParam.Add("contentlength", "0");

            //计算sign值
            string mySign = SignParse.CreateRequestMySign(sortParam, _appSecret);

            //请求头中添加三个必要参数 key sign timestamp
            request.AddHeader("X-Auth-Sign", mySign);
            request.AddHeader("X-Auth-Key", _appKey);
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
            sortParam.Add("key", _appKey);
            sortParam.Add("method", request.Method.ToString());
            Uri uri = client.BuildUri(request);
            sortParam.Add("uri", uri.PathAndQuery);
            sortParam.Add("contentlength", bytes.Length.ToString());

            //计算sign值
            string mySign = SignParse.CreateRequestMySign(sortParam, _appSecret);

            //请求头中添加三个必要参数 key sign timestamp
            request.AddHeader("X-Auth-Sign", mySign);
            request.AddHeader("X-Auth-Key", _appKey);
            request.AddHeader("X-Auth-TimeStamp", unixTime.ToString());

            IRestResponse queryResult = client.Execute(request);
            return queryResult;
        }

        #endregion


        #region V2

        public IRestResponse HttpGetV2(string resource, Dictionary<string, string> paramData)
        {
            return HttpRequest(resource, Method.GET, paramData);
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
