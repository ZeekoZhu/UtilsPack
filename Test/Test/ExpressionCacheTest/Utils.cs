using System;
using System.Linq.Expressions;
using Test.ExpressionCacheTest.Models;

namespace Test.ExpressionCacheTest
{
    public static class Utils
    {
        
        public static Expression CreateFilter(Expression<Func<BlogPost, bool>> func) => func;

        public static Expression CreateSelector<T>(Expression<Func<BlogPost, T>> fn) => fn;
    }
}