using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;
using ZeekoUtilsPack.ExpressionCache;
using static Test.ExpressionCacheTest.Utils;

namespace Test.ExpressionCacheTest
{
    public class ExpressionTreeComparerTests
    {

        public static TheoryData<Expression, Expression, Expression> GetDifferentExpressions()
        {
            return new TheoryData<Expression, Expression, Expression>
            {
                {
                    CreateFilter(p => p.IsActive),
                    CreateFilter(b => b.IsActive),
                    CreateFilter(p => p.Id == 21)
                },
                {
                    CreateSelector(p => p.Tags.Count),
                    CreateFilter(b => b.IsActive),
                    CreateFilter(p => p.Id == 21)
                },
                {
                    CreateFilter(p => p.IsActive),
                    CreateFilter(b =>
                        !b.IsActive && b.Tags.Any(t => String.CompareOrdinal("Hello", t.ToLowerInvariant()) == 0)),
                    CreateFilter(p => p.Id == 21)
                },
            };
        }

        public static TheoryData GetSameExpressions()
        {
            var data = GetDifferentExpressions();
            var result = new TheoryData<Expression, Expression, Expression>();
            foreach (var objects in data)
            {
                foreach (Expression expr in objects)
                {
                    result.Add(expr, expr, expr);
                }
            }

            return result;
        }

        [Theory, MemberData(nameof(GetDifferentExpressions))]
        public void CompareDifferentTest(Expression x, Expression y, Expression z)
        {
            var comparer = new ExpressionTreeComparer();
            var list = new List<Expression>
            {
                x, y, z
            };

            list.Sort(comparer);
            list.Select(expr => expr.GetHashCode()).Distinct().Should().HaveCount(3);
            comparer.Compare(list[0], list[1]).Should().BeLessThan(0);
            comparer.Compare(list[0], list[2]).Should().BeLessThan(0);
            comparer.Compare(list[1], list[2]).Should().BeLessThan(0);
            comparer.Compare(list[1], list[0]).Should().BeGreaterThan(0);
            comparer.Compare(list[2], list[0]).Should().BeGreaterThan(0);
            comparer.Compare(list[2], list[1]).Should().BeGreaterThan(0);
        }

        [Theory, MemberData(nameof(GetSameExpressions))]
        public void CompareSameTest(Expression x, Expression y, Expression z)
        {
            var comparer = new ExpressionTreeComparer();
            var xy = comparer.Compare(x, y);
            var xz = comparer.Compare(x, z);
            var yz = comparer.Compare(y, z);
            var yx = comparer.Compare(y, x);
            var zx = comparer.Compare(z, x);
            var zy = comparer.Compare(z, y);
            var results = new List<int>
            {
                xy, zx, yz, xz, yx, zy
            };
            var hashes = new List<int> { x.GetHashCode(), y.GetHashCode(), z.GetHashCode() };
            hashes.Distinct().Should().HaveCount(1);
            results.All(r => r == 0).Should().BeTrue();
        }
    }
}