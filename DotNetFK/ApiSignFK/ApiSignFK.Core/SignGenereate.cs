using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSignFK.Core
{
    public class SignGenereate
    {


        #region 生成请求的参数字符串
        /// <summary>
        /// 生成请求的参数字符串
        /// </summary>
        /// <param name="sParaTemp">请求参数</param>
        /// <returns></returns>
        public static string CreateRequestParam(SortedDictionary<string, string> sParaTemp)
        {
            //待签名请求参数数组
            Dictionary<string, string> sPara = new Dictionary<string, string>();

            //过滤签名参数数组
            sPara = SignCore.FilterPara(sParaTemp);

            //“参数=参数值”的模式用“&”字符拼接
            string temp = SignCore.CreateLinkString(sPara);

            //获得签名结果
            string mysign = SignCore.CreateSign(temp, SignConfig.AppSecret);

            //生成请求的参数字符串
            StringBuilder prestr = new StringBuilder();

            //other param
            foreach (KeyValuePair<string, string> par in sParaTemp)
            {
                prestr.Append(par.Key + "=" + par.Value + "&");
            }

            //sign
            prestr.Append("sign=" + mysign);

            return prestr.ToString();
        }
        #endregion


        public static string CreateRequestMySign(SortedDictionary<string, string> sParaTemp)
        {
            //过滤签名参数数组
            var sPara = SignCore.FilterPara(sParaTemp);

            //“参数=参数值”的模式用“&”字符拼接
            string temp = SignCore.CreateLinkString(sPara);

            //获得签名结果
            string mysign = SignCore.CreateSign(temp, SignConfig.AppSecret);

            return mysign;
        }
    }
}
