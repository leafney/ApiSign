using System.Collections.Generic;

namespace ApiSignFK.Core
{
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
