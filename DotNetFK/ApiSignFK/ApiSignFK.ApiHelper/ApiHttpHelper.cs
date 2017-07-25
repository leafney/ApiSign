using ApiSignFK.Core;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiSignFK.ApiHelper
{
    public class ApiHttpHelper
    {

        private static string _baseUri = "";
        private static string _appKey = "";
        private static string _appSecret = "";

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="baseUri">请求基地址</param>
        /// <param name="appKey">appKeyparam>
        /// <param name="appSecret">appSecret</param>
        public ApiHttpHelper(string baseUri, string appKey, string appSecret)
        {
            _baseUri = baseUri;
            _appKey = appKey;
            _appSecret = appSecret;
        }
        #endregion


        #region 基于RestSharp封装
        /// <summary>
        /// 基于RestSharp封装
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="method"></param>
        /// <param name="paramData"></param>
        /// <param name="bodyData"></param>
        /// <returns></returns>
        private RestRequest BaseApiRequest(RestClient client, RestRequest request, Method method, Dictionary<string, string> paramData = null, object bodyData = null)
        {
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
                        //string json_Data = JsonConvert.SerializeObject(bodyData);
                        string json_Data = SimpleJson.SerializeObject(bodyData);
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

            return request;
        } 
        #endregion


        #region 基于RestSharp封装接口请求方法
        /// <summary>
        /// 基于RestSharp封装接口请求方法
        /// </summary>
        /// <param name="resource">请求资源</param>
        /// <param name="method">请求方法</param>
        /// <param name="paramData">Url请求参数</param>
        /// <param name="bodyData">Body请求参数</param>
        /// <returns></returns>
        private IRestResponse ApiRequest(string resource, Method method, Dictionary<string, string> paramData = null, object bodyData = null)
        {
            var client = new RestClient(_baseUri);
            var request = new RestRequest(resource, method);

            request= BaseApiRequest(client, request, method, paramData, bodyData);

            //发起请求
            IRestResponse queryResult = client.Execute(request);
            return queryResult;
        }

        /// <summary>
        /// 基于RestSharp封装接口请求方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="method"></param>
        /// <param name="paramData"></param>
        /// <param name="bodyData"></param>
        /// <returns></returns>
        private IRestResponse<T> ApiRequest<T>(string resource, Method method, Dictionary<string, string> paramData = null, object bodyData = null) where T : new()
        {
            var client = new RestClient(_baseUri);
            var request = new RestRequest(resource, method);

            request = BaseApiRequest(client, request, method, paramData, bodyData);

            //发起请求
            IRestResponse<T> queryResult = client.Execute<T>(request);
            return queryResult;// T =queryResult.Data;
        }

        #endregion


        #region 基础方式请求

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public IRestResponse ApiGet(string resource, Dictionary<string, string> paramData)
        {
            return ApiRequest(resource, Method.GET, paramData);
        }

        public IRestResponse ApiPost(string resource, object postData)
        {
            return ApiRequest(resource, Method.POST, bodyData: postData);
        }

        public IRestResponse ApiPut(string resource, object postData)
        {
            return ApiRequest(resource, Method.PUT, bodyData: postData);
        }

        public IRestResponse ApiDelete(string resource, Dictionary<string, string> paramData)
        {
            return ApiRequest(resource, Method.DELETE, paramData: paramData);
        }

        #endregion

        #region 泛型方式请求

        public IRestResponse<T> ApiGet<T>(string resource, Dictionary<string, string> paramData) where T : new()
        {
            return ApiRequest<T>(resource, Method.GET, paramData);
        }

        public IRestResponse<T> ApiPost<T>(string resource, object postData) where T : new()
        {
            return ApiRequest<T>(resource, Method.POST, bodyData: postData);
        }

        public IRestResponse<T> ApiPut<T>(string resource, object postData) where T : new()
        {
            return ApiRequest<T>(resource, Method.PUT, bodyData: postData);
        }

        public IRestResponse<T> ApiDelete<T>(string resource, Dictionary<string, string> paramData) where T : new()
        {
            return ApiRequest<T>(resource, Method.DELETE, paramData: paramData);
        }

        #endregion



        #region V1

        /*
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
        */
        #endregion

    }
}
