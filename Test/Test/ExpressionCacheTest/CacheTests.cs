using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Xunit;
using ZeekoUtilsPack.ExpressionCache;

namespace Test.ExpressionCacheTest
{
    public class CacheTests
    {
        static Func<Expression, (Expression, int, Mock<Func<Expression, int>>)> CreateMockCreator(int value)
        {
            var mock = new Mock<Func<Expression, int>>();
            mock.Setup(fn => fn(It.IsAny<Expression>())).Returns(value);
            return expr => (expr, value, mock);
        }
        public static TheoryData GetCacheTestData()
        {
            var five = CreateMockCreator(5);
            var six = CreateMockCreator(6);
            var eight = CreateMockCreator(8);
            var nine = CreateMockCreator(9);
            var ten = CreateMockCreator(10);
            return new TheoryData<IEnumerable<(Expression, int, Mock<Func<Expression, int>>)>>
            {
                new []
                {
                    five(Utils.CreateSelector(p => p.Id)),
                    six(Utils.CreateSelector(p => p.Tags.Count)),
                    five(Utils.CreateSelector(p => p.Id)),
                    six(Utils.CreateSelector(p => p.Tags.Count)),
                },
                new []
                {
                    eight(Utils.CreateFilter(p => p.Id > 10)),
                    nine(Utils.CreateSelector(p => p.IsActive && p.Tags.Any() || p.Categories.Count == p.Tags.Count && p.Categories.Any(c => c.Name == "Root"))),
                    ten(Utils.CreateSelector(p => p.Tags.Count)),
                    nine(Utils.CreateSelector(p => p.IsActive && p.Tags.Any() || p.Categories.Count == p.Tags.Count && p.Categories.Any(c => c.Name == "Root"))),
                    ten(Utils.CreateSelector(p => p.Tags.Count)),
                    eight(Utils.CreateFilter(p => p.Id > 10)),
                }
            };
        }
        
        [Theory, MemberData(nameof(GetCacheTestData))]
        public void CacheTest(IEnumerable<(Expression Expr, int Value, Mock<Func<Expression, int>> Creator)> tests)
        {
            var cache = new DictionaryExpressionCache<Expression, int>();
                foreach (var (expr, value, mockCreator) in tests)
                {
                    var cached = cache.GetOrAdd(expr, mockCreator.Object);
                    cached.Should().Be(value);
                    mockCreator.Invocations.Count.Should().Be(1);
                }
        }
    }
}