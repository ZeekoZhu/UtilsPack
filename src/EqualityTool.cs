using System;
using System.Collections.Generic;

namespace Zeeko.UtilsPack
{
    public static class Equality<T>
    {
        /// <summary>
        /// 使用属性选择器创建一个比较器
        /// </summary>
        /// <typeparam name="V">属性类型</typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector)
        {
            return new CommonEqualityComparer<V>(keySelector);
        }
        public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector, IEqualityComparer<V> comparer)
        {
            return new CommonEqualityComparer<V>(keySelector, comparer);
        }

        class CommonEqualityComparer<V> : IEqualityComparer<T>
        {
            private readonly Func<T, V> _keySelector;
            private readonly IEqualityComparer<V> _comparer;

            public CommonEqualityComparer(Func<T, V> keySelector, IEqualityComparer<V> comparer)
            {
                _keySelector = keySelector;
                _comparer = comparer;
            }
            public CommonEqualityComparer(Func<T, V> keySelector)
                : this(keySelector, EqualityComparer<V>.Default)
            { }

            public bool Equals(T x, T y)
            {
                return _comparer.Equals(_keySelector(x), _keySelector(y));
            }
            public int GetHashCode(T obj)
            {
                return _comparer.GetHashCode(_keySelector(obj));
            }
        }
    }

    public static class Comparison<T>
    {
        public static IComparer<T> CreateComparer<V>(Func<T, V> keySelector)
        {
            return new CommonComparer<V>(keySelector);
        }
        public static IComparer<T> CreateComparer<V>(Func<T, V> keySelector, IComparer<V> comparer)
        {
            return new CommonComparer<V>(keySelector, comparer);
        }

        class CommonComparer<V> : IComparer<T>
        {
            private readonly Func<T, V> _keySelector;
            private readonly IComparer<V> _comparer;

            public CommonComparer(Func<T, V> keySelector, IComparer<V> comparer)
            {
                _keySelector = keySelector;
                _comparer = comparer;
            }
            public CommonComparer(Func<T, V> keySelector)
                : this(keySelector, Comparer<V>.Default)
            { }

            public int Compare(T x, T y)
            {
                return _comparer.Compare(_keySelector(x), _keySelector(y));
            }
        }
    }
}
