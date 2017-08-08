using System;
using System.Collections.Generic;
using System.Text;

namespace Zeeko.UtilsPack
{
    public static class StringTools
    {
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
    }
}
