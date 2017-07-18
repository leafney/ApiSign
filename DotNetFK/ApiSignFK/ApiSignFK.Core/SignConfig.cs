using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiSignFK.Core
{
    public class SignConfig
    {
        private static string appkey = "";
        private static string appsecret = "";


        static SignConfig()
        {

            //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

            //合作身份者ID，10位数字
            appkey = "2088911242";

            //交易安全检验码，由数字和大小写字母组成的32位字符串
            appsecret = "cfvepv0kzm34epzn8bpcsct94357fz1j";


            //↑↑↑↑↑↑↑↑↑↑请在这里配置您的基本信息↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

        }

        #region 属性


        public static string AppKey
        {
            get { return appkey; }
            set { appkey = value; }
        }

        public static string AppSecret
        {
            get { return appsecret; }
            set { appsecret = value; }
        }

        #endregion


    }
}
