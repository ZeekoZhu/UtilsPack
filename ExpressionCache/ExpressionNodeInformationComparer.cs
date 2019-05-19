using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable JoinDeclarationAndInitializer

namespace ZeekoUtilsPack.ExpressionCache
{
    /// <summary>
    /// Comparer expression node by information on it
    /// </summary>
    public class ExpressionNodeInformationComparer :
        IComparer<Expression>,
        IComparer<BinaryExpression>,
        IComparer<BlockExpression>,
        IComparer<ConditionalExpression>,
        IComparer<ConstantExpression>,
        IComparer<DebugInfoExpression>,
        IComparer<DefaultExpression>,
        IComparer<DynamicExpression>,
        IComparer<GotoExpression>,
        IComparer<IndexExpression>,
        IComparer<InvocationExpression>,
        IComparer<LabelExpression>,
        IComparer<LambdaExpression>,
        IComparer<ListInitExpression>,
        IComparer<LoopExpression>,
        IComparer<MemberExpression>,
        IComparer<MemberInitExpression>,
        IComparer<MethodCallExpression>,
        IComparer<NewArrayExpression>,
        IComparer<NewExpression>,
        IComparer<ParameterExpression>,
        IComparer<RuntimeVariablesExpression>,
        IComparer<SwitchExpression>,
        IComparer<TryExpression>,
        IComparer<TypeBinaryExpression>,
        IComparer<UnaryExpression>,
        IComparer<LabelTarget>,
        IComparer<MemberBinding>,
        IComparer<ElementInit>,
        IComparer<SwitchCase>,
        IComparer<CatchBlock>
    {
        public int Compare(Expression x, Expression y)
        {
            var result = 0;
            if (CompareNull(x, y, ref result))
            {
                return result;
            }

            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            result = CompareType(x.GetType(), y.GetType());
            if (result != 0)
            {
                return result;
            }

            result = x.NodeType.CompareTo(y.NodeType);
            if (result != 0)
            {
                return result;
            }

            result = CompareType(x.Type, y.Type);
            if (result != 0)
            {
                return result;
            }

            switch (x)
            {
                case BinaryExpression binaryExpression:
                    return Compare(binaryExpression, (BinaryExpression) y);
                case BlockExpression blockExpression:
                    return Compare(blockExpression, y as BlockExpression);
                case ConditionalExpression conditionalExpression:
                    return Compare(conditionalExpression, y as ConditionalExpression);
                case ConstantExpression constantExpression:
                    return Compare(constantExpression, y as ConstantExpression);
                case DebugInfoExpression debugInfoExpression:
                    return Compare(debugInfoExpression, y as DebugInfoExpression);
                case DefaultExpression defaultExpression:
                    return Compare(defaultExpression, y as DefaultExpression);
                case DynamicExpression dynamicExpression:
                    return Compare(dynamicExpression, y as DynamicExpression);
                case GotoExpression gotoExpression:
                    return Compare(gotoExpression, y as GotoExpression);
                case IndexExpression indexExpression:
                    return Compare(indexExpression, y as IndexExpression);
                case InvocationExpression invocationExpression:
                    return Compare(invocationExpression, y as InvocationExpression);
                case LabelExpression labelExpression:
                    return Compare(labelExpression, y as LabelExpression);
                case LambdaExpression lambdaExpression:
                    return Compare(lambdaExpression, y as LambdaExpression);
                case ListInitExpression listInitExpression:
                    return Compare(listInitExpression, y as ListInitExpression);
                case LoopExpression loopExpression:
                    return Compare(loopExpression, y as LoopExpression);
                case MemberExpression memberExpression:
                    return Compare(memberExpression, y as MemberExpression);
                case MemberInitExpression memberInitExpression:
                    return Compare(memberInitExpression, y as MemberInitExpression);
                case MethodCallExpression methodCallExpression:
                    return Compare(methodCallExpression, y as MethodCallExpression);
                case NewArrayExpression newArrayExpression:
                    return Compare(newArrayExpression, y as NewArrayExpression);
                case NewExpression newExpression:
                    return Compare(newExpression, y as NewExpression);
                case ParameterExpression parameterExpression:
                    return Compare(parameterExpression, y as ParameterExpression);
                case RuntimeVariablesExpression runtimeVariablesExpression:
                    return Compare(runtimeVariablesExpression, y as RuntimeVariablesExpression);
                case SwitchExpression switchExpression:
                    return Compare(switchExpression, y as SwitchExpression);
                case TryExpression tryExpression:
                    return Compare(tryExpression, y as TryExpression);
                case TypeBinaryExpression typeBinaryExpression:
                    return Compare(typeBinaryExpression, y as TypeBinaryExpression);
                case UnaryExpression unaryExpression:
                    return Compare(unaryExpression, y as UnaryExpression);
                default:
                    throw new NotSupportedException($"{x.GetType().Name} is not supported.");
            }
        }

        public int CompareNotExpressionNode(object x, object y)
        {
            switch (x, y)
            {
                case (LabelTarget a, LabelTarget b):
                    return Compare(a, b);
                case (MemberAssignment a, MemberAssignment b):
                    return Compare(a, b);
                case (MemberMemberBinding a, MemberMemberBinding b):
                    return Compare(a, b);
                case (MemberListBinding a, MemberListBinding b):
                    return Compare(a, b);
                case (ElementInit a, ElementInit b):
                    return Compare(a, b);
                case (SwitchCase a, SwitchCase b):
                    return Compare(a, b);
                case (CatchBlock a, CatchBlock b):
                    return Compare(a, b);
                default:
                    throw new InvalidOperationException($"Can not compare {x.GetType().Name} with {y.GetType().Name}");
            }
        }

        int CompareMemberInfo(MemberInfo x, MemberInfo y)
        {
            if (x == y)
            {
                return 0;
            }

            int result = x.GetHashCode().CompareTo(y.GetHashCode());
            if (result != 0) return result;

            result = String.Compare(x.Module.FullyQualifiedName, y.Module.FullyQualifiedName, StringComparison.Ordinal);
            if (result != 0) return result;

            result = x.MetadataToken.CompareTo(y.MetadataToken);
            return result;
        }

        int CompareSymbolDocumentInfo(SymbolDocumentInfo x, SymbolDocumentInfo y)
        {
            int result;

            result = x.GetHashCode().CompareTo(y.GetHashCode());
            if (result != 0) return result;

            return String.Compare(x.FileName, y.FileName, StringComparison.Ordinal);
        }

        protected int CompareType(Type x, Type y)
        {
            var result = 0;
            if (x == y)
            {
                return result;
            }

            result = x.GetHashCode().CompareTo(y.GetHashCode());
            if (result != 0)
            {
                return result;
            }

            result = String.Compare(x.AssemblyQualifiedName, y.AssemblyQualifiedName, StringComparison.Ordinal);
            return result;
        }

        protected bool CompareNull<T>(T x, T y, ref int result) where T : class
        {
            switch ((x, y))
            {
                case (null, null):
                    result = 0;
                    return true;
                case (null, _):
                    result = -1;
                    return true;
                case (_, null):
                    result = 1;
                    return true;
                default:
                    return false;
            }
        }

        public int Compare(BinaryExpression x, BinaryExpression y)
        {
            int result;
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            result = x.IsLifted.CompareTo(y.IsLifted);
            if (result != 0) return result;

            result = x.IsLiftedToNull.CompareTo(y.IsLiftedToNull);
            if (result != 0) return result;

            result = CompareMemberInfo(x.Method, y.Method);

            return result;
        }

        public int Compare(BlockExpression x, BlockExpression y)
        {
            int result;
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            result = x.Variables.Count.CompareTo(y.Variables.Count);
            if (result != 0) return result;
            return x.Expressions.Count.CompareTo(y.Expressions.Count);
        }

        public int Compare(ConditionalExpression x, ConditionalExpression y)
        {
            return 0;
        }

        public int Compare(ConstantExpression x, ConstantExpression y)
        {
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            return Comparer.DefaultInvariant.Compare(x.Value, y.Value);
        }

        public int Compare(DebugInfoExpression x, DebugInfoExpression y)
        {
            int result;
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            result = x.StartLine.CompareTo(y.StartLine);
            if (result != 0) return result;

            result = x.EndLine.CompareTo(y.EndLine);
            if (result != 0) return result;

            result = x.StartColumn.CompareTo(y.StartColumn);
            if (result != 0) return result;

            result = x.EndColumn.CompareTo(y.EndColumn);
            if (result != 0) return result;

            result = x.IsClear.CompareTo(y.IsClear);
            if (result != 0) return result;

            result = CompareSymbolDocumentInfo(x.Document, y.Document);

            return result;
        }

        public int Compare(DefaultExpression x, DefaultExpression y)
        {
            return 0;
        }

        public int Compare(DynamicExpression x, DynamicExpression y)
        {
            return 0;
        }

        public int Compare(GotoExpression x, GotoExpression y)
        {
            int result;
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            result = x.Kind.CompareTo(y.Kind);
            if (result != 0) return result;

            return 0;
        }

        public int Compare(IndexExpression x, IndexExpression y)
        {
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            return CompareMemberInfo(x.Indexer, y.Indexer);
        }

        public int Compare(InvocationExpression x, InvocationExpression y)
        {
            return 0;
        }

        public int Compare(LabelExpression x, LabelExpression y)
        {
            return 0;
        }

        public int Compare(LambdaExpression x, LambdaExpression y)
        {
            int result;
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            result = x.TailCall.CompareTo(y.TailCall);
            if (result != 0) return result;
            result = CompareType(x.ReturnType, y.ReturnType);
            if (result != 0) return result;
            return String.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }

        public int Compare(ListInitExpression x, ListInitExpression y)
        {
            return 0;
        }

        public int Compare(LoopExpression x, LoopExpression y)
        {
            return 0;
        }

        public int Compare(MemberExpression x, MemberExpression y)
        {
            Debug.Assert(y != null, nameof(y) + " != null");
            Debug.Assert(x != null, nameof(x) + " != null");
            return CompareMemberInfo(x.Member, y.Member);
        }

        public int Compare(MemberInitExpression x, MemberInitExpression y)
        {
            return 0;
        }

        public int Compare(MethodCallExpression x, MethodCallExpression y)
        {
            Debug.Assert(y != null, nameof(y) + " != null");
            Debug.Assert(x != null, nameof(x) + " != null");
            return CompareMemberInfo(x.Method, y.Method);
        }

        public int Compare(NewArrayExpression x, NewArrayExpression y)
        {
            return 0;
        }

        public int Compare(NewExpression x, NewExpression y)
        {
            int result;
            Debug.Assert(y != null, nameof(y) + " != null");
            Debug.Assert(x != null, nameof(x) + " != null");
            result = x.Members.Count.CompareTo(y.Members.Count);
            if (result != 0) return result;
            result = CompareMemberInfo(x.Constructor, y.Constructor);
            if (result != 0) return result;

            for (int i = 0; i < x.Members.Count; i++)
            {
                var xm = x.Members[i];
                var ym = y.Members[i];
                result = CompareMemberInfo(xm, ym);
                if (result != 0) return result;
            }

            return result;
        }

        public int Compare(ParameterExpression x, ParameterExpression y)
        {
            int result;
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            result = x.IsByRef.CompareTo(y.IsByRef);
            if (result != 0) return result;

            return String.CompareOrdinal(x.Name, y.Name);
        }

        public int Compare(RuntimeVariablesExpression x, RuntimeVariablesExpression y)
        {
            return 0;
        }

        public int Compare(SwitchExpression x, SwitchExpression y)
        {
            Debug.Assert(y != null, nameof(y) + " != null");
            Debug.Assert(x != null, nameof(x) + " != null");
            return CompareMemberInfo(x.Comparison, y.Comparison);
        }

        public int Compare(TryExpression x, TryExpression y)
        {
            return 0;
        }

        public int Compare(TypeBinaryExpression x, TypeBinaryExpression y)
        {
            Debug.Assert(y != null, nameof(y) + " != null");
            Debug.Assert(x != null, nameof(x) + " != null");
            return CompareType(x.TypeOperand, y.TypeOperand);
        }

        public int Compare(UnaryExpression x, UnaryExpression y)
        {
            int result;
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            result = x.IsLifted.CompareTo(y.IsLifted);
            if (result != 0) return result;
            result = x.IsLiftedToNull.CompareTo(y.IsLiftedToNull);
            if (result != 0) return result;
            return CompareMemberInfo(x.Method, y.Method);
        }

        public int Compare(LabelTarget x, LabelTarget y)
        {
            int result;
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            result = CompareType(x.Type, y.Type);
            if (result != 0) return result;
            return String.CompareOrdinal(x.Name, y.Name);
        }

        public int Compare(MemberBinding x, MemberBinding y)
        {
            int result;
            Debug.Assert(y != null, nameof(y) + " != null");
            Debug.Assert(x != null, nameof(x) + " != null");
            result = x.BindingType.CompareTo(y.BindingType);
            if (result != 0) return result;
            return CompareMemberInfo(x.Member, y.Member);
        }

        public int Compare(MemberMemberBinding x, MemberMemberBinding y)
        {
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            return x.Bindings.Count.CompareTo(y.Bindings.Count);
        }

        public int Compare(MemberListBinding x, MemberListBinding y)
        {
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            return x.Initializers.Count.CompareTo(y.Initializers.Count);
        }

        public int Compare(ElementInit x, ElementInit y)
        {
            int result;
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            result = x.Arguments.Count.CompareTo(y.Arguments.Count);
            if (result != 0) return result;
            return CompareMemberInfo(x.AddMethod, y.AddMethod);
        }

        public int Compare(SwitchCase x, SwitchCase y)
        {
            return 0;
        }

        public int Compare(CatchBlock x, CatchBlock y)
        {
            Debug.Assert(x != null, nameof(x) + " != null");
            Debug.Assert(y != null, nameof(y) + " != null");
            return CompareType(x.Test, y.Test);
        }
    }
}