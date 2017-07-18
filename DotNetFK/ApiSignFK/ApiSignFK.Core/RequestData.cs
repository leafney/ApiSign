using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSignFK.Core
{
    public class RequestData
    {
        /// <summary>
        /// 请求参数中的key的值
        /// </summary>
        public string ReqKey { get; set; }
        /// <summary>
        /// 请求参数中的sign的值
        /// </summary>
        public string ReqSign { get; set; }
        /// <summary>
        /// 请求参数中的timestamp的值
        /// </summary>
        public string ReqTimeStamp { get; set; }
        /// <summary>
        /// 所有请求参数的键值对数组
        /// </summary>
        public SortedDictionary<string, string> ReqDics = new SortedDictionary<string, string>();
    }
}
