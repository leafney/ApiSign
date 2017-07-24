using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApiSignFK.Core
{
    public class SignCore
    {
        #region 除去数组中的空值和签名参数并以字母a到z的顺序排序
        /// <summary>
        /// 除去数组中的空值和签名参数并以字母a到z的顺序排序
        /// </summary>
        /// <param name="dicArrayPre">过滤前的参数组</param>
        /// <returns>过滤后的参数组</returns>
        public static Dictionary<string, string> FilterPara(SortedDictionary<string, string> dicArrayPre)
        {
            Dictionary<string, string> dicArray = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                if (temp.Key.ToLower() != "sign" && temp.Value != "" && temp.Value != null)
                {
                    dicArray.Add(temp.Key, temp.Value);
                }
            }

            return dicArray;
        }
        #endregion

        #region 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="sArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string CreateLinkString(Dictionary<string, string> dicArray)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                // Uri.EscapeDataString 对请求参数中的中文进行编码处理
                prestr.Append(temp.Key + "=" + Uri.EscapeDataString(temp.Value) + "&");
            }

            #region 备用
            ////去掉最後一個&字符
            //int nLen = prestr.Length;
            //prestr.Remove(nLen - 1, 1); 
            #endregion

            //最后的一个 & 用于拼接后面的 secret=

            return prestr.ToString();
        }
        #endregion

        #region 签名字符串
        /// <summary>
        /// 签名字符串
        /// </summary>
        /// <param name="prestr">需要签名的字符串</param>
        /// <param name="appSecret">密钥</param>
        /// <param name="charset">编码格式</param>
        /// <returns>签名结果</returns>
        public static string CreateSign(string prestr, string appSecret, string charset = "utf-8")
        {
            //将secret=appsecret拼接在所有参数字符串后面
            prestr = prestr + "secret=" + GetMd5Hash(appSecret, charset);
            return GetMd5Hash(prestr, charset);
        }
        #endregion


        #region 获取字符串的MD5值

        public static string GetMd5Hash(string text, string charset = "utf-8")
        {
            using (var md5 = MD5.Create())
            {
                byte[] bt = md5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(text));
                var strResult = BitConverter.ToString(bt);
                string str = strResult.Replace("-", "");
                return str.ToUpper();
            }
        } 
        #endregion

    }
}
