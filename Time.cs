using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Time
{
    class TimeFormat
    {
        #region 格式化时间表示
        //转化为北京时间
        public static string FormatTime(string sec)
        {
            long seconds = Convert.ToInt64(sec);
            return (new DateTime(1970, 1, 1, 8, 0, 0)).AddSeconds(seconds).ToString();
        }
        //转化为totalseconds
        public static long FormatTime(DateTime date)
        {
            return (long)date.Subtract(new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds;
        }
        //sec转化为*小时*分*秒
        public static string FormatTimeToHHMMSS(long seconds)
        {
            int hour = 0, min = 0, sec = 0;
            hour = (int)seconds / 3600;
            min = (int)seconds % 3600 / 60;
            sec = (int)seconds % 60;
            return hour.ToString() + "小时" + min.ToString() + "分钟" + sec.ToString() + "秒";
        }
        #endregion 
    }
}
