using System;
using System.Security.Cryptography;
using System.Text;

namespace ZeekoUtilsPack.BCLExt
{
    public static class StringExt
    {
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>密文</returns>
        public static string GetMd5(this string str)
        {
            MD5 m = MD5.Create();
            var bytes = m.ComputeHash(Encoding.UTF8.GetBytes(str));
            string res = BitConverter.ToString(bytes);
            res = res.ToLower().Replace("-", "");
            return res;
        }
    }
}
