using System.Linq.Expressions;

namespace ZeekoUtilsPack.ExpressionCache
{
    public class ExpressionHasher : ExpressionVisitor
    {
        public int HashExpression(Expression exp)
        {
            HashCode = 0;
            Visit(exp);
            return HashCode;
        }

        public int HashCode { get; protected set; }
        private static readonly object NullObj = new object();

        private void CombineHashCode(int value)
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + HashCode;
                hash = hash * 31 + value;
                HashCode = hash;
            }
        }

        protected virtual ExpressionHasher CombineHash<T>(in T value)
        {
            CombineHashCode(value is null ? NullObj.GetHashCode() : value.GetHashCode());
            return this;
        }


        public override Expression Visit(Expression node)
        {
            if (node == null) return null;
            CombineHash((int) node.NodeType)
                .CombineHash(node.Type);
            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            CombineHash(node.IsLifted).CombineHash(node.IsLiftedToNull).CombineHash(node.Method);
            return base.VisitBinary(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            CombineHash(node.Value);
            return base.VisitConstant(node);
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            CombineHash(node.Document)
                .CombineHash(node.StartColumn)
                .CombineHash(node.EndColumn)
                .CombineHash(node.StartLine)
                .CombineHash(node.EndLine);
            return base.VisitDebugInfo(node);
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            CombineHash(node.AddMethod);
            return base.VisitElementInit(node);
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            CombineHash(node.Kind);
            return base.VisitGoto(node);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            CombineHash(node.Indexer);
            return base.VisitIndex(node);
        }


        protected override Expression VisitLabel(LabelExpression node)
        {
            CombineHash(node.Target);
            return base.VisitLabel(node);
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            CombineHash(node.Name);
            return base.VisitLabelTarget(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            CombineHash(node.ReturnType);
            return base.VisitLambda(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            CombineHash(node.Member);
            return base.VisitMember(node);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            CombineHash(node.Member)
                .CombineHash(node.BindingType);
            return base.VisitMemberAssignment(node);
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            CombineHash(node.Member)
                .CombineHash(node.BindingType);
            return base.VisitMemberBinding(node);
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            CombineHash(node.Member)
                .CombineHash(node.BindingType);
            return base.VisitMemberListBinding(node);
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            CombineHash(node.Member)
                .CombineHash(node.BindingType);
            return base.VisitMemberMemberBinding(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            CombineHash(node.Method);
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            foreach (var member in node.Members)
            {
                CombineHash(member);
            }

            return base.VisitNew(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            CombineHash(node.IsByRef)
                .CombineHash(node.Name);
            return base.VisitParameter(node);
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            CombineHash(node.Comparison);
            return base.VisitSwitch(node);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            CombineHash(node.TypeOperand);
            return base.VisitTypeBinary(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            CombineHash(node.IsLifted).CombineHash(node.IsLiftedToNull).CombineHash(node.Method);
            return base.VisitUnary(node);
        }
    }
}