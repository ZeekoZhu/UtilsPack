using System;

namespace Zeeko.UtilsPack
{
    public static class DateTimeTools
    {
        /// <summary>
        /// 获取到 1970/1/1 之间的毫秒数
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>毫秒数</returns>
        public static long GetMilliseconds(this DateTime dateTime) =>
                    Convert.ToInt64(dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
    }
}
