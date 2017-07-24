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
            string mySign = SignParse.CreateRequestMySign(sortParam,App_Secret);

            //请求头中添加三个必要参数 key sign timestamp
            request.AddHeader("X-Auth-Sign", mySign);
            request.AddHeader("X-Auth-Key", App_Key);
            request.AddHeader("X-Auth-TimeStamp", unixTime.ToString());

            //发起请求
            IRestResponse queryResult = client.Execute(request);
            return queryResult;
        }



        public IRestResponse HttpPost(string resource,object postData)
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

    }
}
