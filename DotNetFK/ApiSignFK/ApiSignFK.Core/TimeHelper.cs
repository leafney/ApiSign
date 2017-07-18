using System;

namespace ApiSignFK.Core
{
    /// <summary>
    /// Unix时间戳帮助类
    /// </summary>
    public class TimeHelper
    {
        #region 将Unix时间戳转换为DateTime类型时间
        /// <summary>
        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="dtUnix">10位或13位的unix时间戳</param>
        /// <returns></returns>
        public static DateTime dtUnix2DateTime(long dtUnix)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            string dtStr = dtUnix.ToString();
            if (dtStr.Length == 10)
            {
                time = startTime.AddSeconds(dtUnix);
            }
            else if (dtStr.Length == 13)
            {
                time = startTime.AddMilliseconds(dtUnix);
            }
            else
            { }
            return time;
        }
        #endregion

        #region 将c# DateTime时间格式转换为Unix时间戳格式
        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">要转换的时间</param>
        /// <param name="len">转换后unix时间戳的精度10位或13位</param>
        /// <returns>转换后的unix时间戳</returns>
        public static long dtDateTime2Unix(DateTime time, int len = 10)
        {
            //double intResult = 0;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));

            long result = 0;
            if (len == 10)
            {
                result = (int)(time - startTime).TotalSeconds;
            }
            else
            {
                result = (long)(time - startTime).TotalMilliseconds;
            }
            return result;
        }

        #region 获取当前时间的UNIX时间戳
        /// <summary>
        /// 获取当前时间的UNIX时间戳
        /// </summary>
        /// <param name="len">unix时间戳的精度10位 13位</param>
        /// <returns></returns>
        public static long dtDateTime2Unix(int len = 10)
        {
            DateTime time = DateTime.Now;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long result = 0;
            if (len == 10)
            {
                result = (int)(time - startTime).TotalSeconds;
            }
            else
            {
                result = (long)(time - startTime).TotalMilliseconds;
            }
            return result;
        }
        #endregion


        #endregion

        #region 计算两个时间戳的差值
        /// <summary>
        /// 计算两个时间戳的差值
        /// </summary>
        /// <param name="firUnix"></param>
        /// <param name="secUnix"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static long dtTimeSpan(long firUnix, long secUnix, string f = "M")
        {
            //将时间戳转换为时间
            DateTime firtime = dtUnix2DateTime(firUnix);
            DateTime sectime = dtUnix2DateTime(secUnix);
            return dtTimeSpan(firtime, sectime, f);
        }
        #endregion

        #region 计算时间戳和时间的差值
        /// <summary>
        /// 计算时间戳和时间的差值
        /// </summary>
        /// <param name="dtUnix"></param>
        /// <param name="secTime"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static long dtTimeSpan(long dtUnix, DateTime secTime, string f = "M")
        {
            //将时间戳转换为时间
            DateTime firtime = dtUnix2DateTime(dtUnix);
            return dtTimeSpan(firtime, secTime, f);
        }
        #endregion

        #region 计算两个时间的差值
        /// <summary>
        /// 计算两个时间的差值
        /// </summary>
        /// <param name="firTime">第一个时间</param>
        /// <param name="dtime">第二个时间</param>
        /// <param name="f">差值的显示格式 D:天 H:小时 M:分钟 S:秒 默认为M:分钟</param>
        /// <returns>差值</returns>
        public static long dtTimeSpan(DateTime firTime, DateTime secTime, string f = "M")
        {

            TimeSpan ts1 = new TimeSpan(firTime.Ticks);
            TimeSpan ts2 = new TimeSpan(secTime.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();//差值的绝对值

            double show = 0.0D;
            switch (f.ToUpper())
            {
                case "D":
                    show = ts.TotalDays;
                    break;
                case "M":
                    show = ts.TotalMinutes;
                    break;
                case "H":
                    show = ts.TotalHours;
                    break;
                case "S":
                    show = ts.TotalSeconds;
                    break;
                default:
                    show = ts.TotalMinutes;
                    break;
            }

            return Convert.ToInt64(show);
        }
        #endregion

    }
}
