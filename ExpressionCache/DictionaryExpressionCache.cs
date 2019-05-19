using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace ZeekoUtilsPack.ExpressionCache
{
    public class DictionaryExpressionCache<TK, T> : IExpressionCache<TK, T> where TK : Expression
    {
        private readonly ConcurrentDictionary<ExpressionCacheKey, T> _dictionary =
            new ConcurrentDictionary<ExpressionCacheKey, T>();

        public T GetOrAdd(TK key, Func<TK, T> creator)
        {
            var cacheKey = new ExpressionCacheKey(key);
            return _dictionary.GetOrAdd(cacheKey, _ => creator(key));
        }

        public bool Contains(TK key)
        {
            return _dictionary.ContainsKey(new ExpressionCacheKey(key));
        }


        public bool TryRemove(TK key, out T value)
        {
            return _dictionary.TryRemove(new ExpressionCacheKey(key), out value);
        }

        public T AddOrUpdate(TK key, Func<TK, T> creator, Func<TK, T, T> updater)
        {
            return _dictionary.AddOrUpdate(new ExpressionCacheKey(key), _ => creator(key), (_, v) => updater(key, v));
        }
    }
}