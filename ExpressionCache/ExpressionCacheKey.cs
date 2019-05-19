using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;

// ReSharper disable JoinDeclarationAndInitializer

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace ZeekoUtilsPack.ExpressionCache
{
    public class ExpressionCacheKey : IComparable<ExpressionCacheKey>
    {
        private readonly Expression _expr;
        private int _hashCode;
        private bool _hashCodeInitialized = false;
        private readonly ExpressionTreeComparer _comparer = new ExpressionTreeComparer();

        public ExpressionCacheKey(Expression expr)
        {
            _expr = expr;
        }

        public int CompareTo(ExpressionCacheKey other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var hashCodeComparison = GetHashCode().CompareTo(other.GetHashCode());
            if (hashCodeComparison != 0) return hashCodeComparison;
            return _comparer.Compare(_expr, other._expr);
        }

        protected bool Equals(ExpressionCacheKey other)
        {
            return _comparer.Compare(_expr, other._expr) == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ExpressionCacheKey) obj);
        }

        public override int GetHashCode()
        {
            if (_hashCodeInitialized == false)
            {
                _hashCode = new ExpressionHasher().HashExpression(_expr);
                _hashCodeInitialized = true;
            }

            return _hashCode;
        }
    }
}