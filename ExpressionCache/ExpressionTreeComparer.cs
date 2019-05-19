using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ZeekoUtilsPack.ExpressionCache
{
    /// <summary>
    /// Compare expression tree
    /// </summary>
    public class ExpressionTreeComparer : IComparer<Expression>
    {
        public int Compare(Expression x, Expression y)
        {
            int result;
            var comparer = new ExpressionNodeInformationComparer();
            var xVisitor = new ControlledVisitor();
            var yVisitor = new ControlledVisitor();
            xVisitor.Visit(x);
            yVisitor.Visit(y);
            do
            {
                // Compare expression nodes
                result = xVisitor.Visited.Count - yVisitor.Visited.Count;
                if (result != 0) break;
                result = xVisitor.Visited
                    .Zip(yVisitor.Visited, (a, b) => (a, b))
                    .Select(pair => comparer.Compare(pair.a, pair.b))
                    .FirstOrDefault(r => r != 0);
                if (result != 0) break;
                // compare not expression nodes
                result = xVisitor.VisitedNotExpressionNodes.Count - yVisitor.VisitedNotExpressionNodes.Count;
                if (result != 0) break;
                result = xVisitor.VisitedNotExpressionNodes
                    .Zip(yVisitor.VisitedNotExpressionNodes, (a, b) => (a, b))
                    .Select(pair => comparer.CompareNotExpressionNode(pair.a, pair.b))
                    .FirstOrDefault(r => r != 0);
                
                xVisitor.VisitChildren();
                yVisitor.VisitChildren();
                xVisitor.VisitNotExpressionChildren();
                yVisitor.VisitNotExpressionChildren();
            } while (xVisitor.Remains > 0 || yVisitor.Remains > 0);

            return result;
        }


        internal class ControlledVisitor : ExpressionVisitor
        {
            public Queue<Expression> Visited { get; set; } = new Queue<Expression>();
            public Queue<object> VisitedNotExpressionNodes { get; set; } = new Queue<object>();

            public int Remains => Visited.Count + VisitedNotExpressionNodes.Count;

            public void VisitChildren()
            {
                int length = Visited.Count;
                for (int i = 0; i < length; i++)
                {
                    var expr = Visited.Dequeue();
                    BaseVisit(expr);
                }
            }

            public void VisitNotExpressionChildren()
            {
                int length = VisitedNotExpressionNodes.Count;
                for (int i = 0; i < length; i++)
                {
                    var expr = VisitedNotExpressionNodes.Dequeue();
                    BaseVisit(expr);
                }
                
            }

            private void BaseVisit(Expression node)
            {
                base.Visit(node);
            }

            private void BaseVisit(object node)
            {
                switch (node)
                {
                    case LabelTarget x:
                        base.VisitLabelTarget(x);
                        break;
                    case MemberAssignment x:
                        base.VisitMemberAssignment(x);
                        break;
                    case MemberMemberBinding x:
                        base.VisitMemberMemberBinding(x);
                        break;
                    case MemberListBinding x:
                        base.VisitMemberListBinding(x);
                        break;
                    case ElementInit x:
                        base.VisitElementInit(x);
                        break;
                    case SwitchCase x:
                        base.VisitSwitchCase(x);
                        break;
                    case CatchBlock x:
                        base.VisitCatchBlock(x);
                        break;
                    default:
                        throw new NotSupportedException($"{node.GetType().Name} is not supported.");
                }
            }

            public override Expression Visit(Expression node)
            {
                Visited.Enqueue(node);
                return node;
            }

            protected override LabelTarget VisitLabelTarget(LabelTarget node)
            {
                VisitedNotExpressionNodes.Enqueue(node);
                return base.VisitLabelTarget(node);
            }

            protected override ElementInit VisitElementInit(ElementInit node)
            {
                VisitedNotExpressionNodes.Enqueue(node);
                return base.VisitElementInit(node);
            }

            protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
            {
                VisitedNotExpressionNodes.Enqueue(node);
                return base.VisitMemberAssignment(node);
            }

            protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
            {
                VisitedNotExpressionNodes.Enqueue(node);
                return base.VisitMemberListBinding(node);
            }

            protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
            {
                VisitedNotExpressionNodes.Enqueue(node);
                return base.VisitMemberMemberBinding(node);
            }

            protected override SwitchCase VisitSwitchCase(SwitchCase node)
            {
                VisitedNotExpressionNodes.Enqueue(node);
                return base.VisitSwitchCase(node);
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                VisitedNotExpressionNodes.Enqueue(node);
                return base.VisitCatchBlock(node);
            }
        }
    }
}