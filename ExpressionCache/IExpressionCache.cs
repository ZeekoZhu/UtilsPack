using System;
using System.Linq.Expressions;

namespace ZeekoUtilsPack.ExpressionCache
{
    public interface IExpressionCache<TK,T> where TK : Expression
    {
        T GetOrAdd(TK key, Func<TK, T> creator);
        bool Contains(TK key);
        bool TryRemove(TK key, out T value);
        T AddOrUpdate(TK key, Func<TK, T> creator, Func<TK, T, T> updater);
    }
}