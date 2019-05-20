using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ZeekoUtilsPack.ExpressionCache;

namespace ZeekoUtilsPack.EFKata
{
    public class EfKataEntityContext<TE> : EfKataContext
    {
        public string Table => base.Table<TE>();

        public string Column<TP>(Expression<Func<TE, TP>> propertySelector) => base.Column(propertySelector);

        public string[] Columns => base.Columns<TE>();

        public EfKataEntityContext(IModel model) : base(model)
        {
        }
    }

    public class EfKataContext
    {
        private readonly IModel _model;

        private static readonly IExpressionCache<Expression, string>
            Cache = new DictionaryExpressionCache<Expression, string>();

        public EfKataContext(IModel model)
        {
            _model = model;
        }

        public EfKataEntityContext<TE> Entity<TE>()
        {
            return new EfKataEntityContext<TE>(_model);
        }

        public string Table<T>()
        {
            var tableModel = _model.FindEntityType(typeof(T));
            if (tableModel is null)
            {
                throw new InvalidOperationException($"Can not find relationship model for {typeof(T).FullName}");
            }

            return tableModel.Relational().TableName;
        }

        public string Column<TE, TP>(Expression<Func<TE, TP>> propertySelector)
        {
            return Cache.GetOrAdd(propertySelector, (expr) =>
            {
                if (expr is Expression<Func<TE, TP>> selector)
                {
                    return GetPropertyColumnName(selector);
                }

                throw new NotSupportedException("Only Lambda expression is supported.");
            });
        }

        public string[] Columns<T>()
        {
            var tableModel = _model.FindEntityType(typeof(T));
            if (tableModel is null)
            {
                throw new InvalidOperationException($"Can not find relationship model for {typeof(T).FullName}");
            }

            return tableModel.GetProperties()
                .Select(p => p.Relational().ColumnName)
                .ToArray();
        }

        private string GetPropertyColumnName<TE, TP>(Expression<Func<TE, TP>> selector)
        {
            switch (selector.Body)
            {
                case MemberExpression memberExpr:
                    var memberInfo = memberExpr.Member;
                    var entityType = memberInfo.DeclaringType;
                    if (entityType is null)
                    {
                        throw new InvalidOperationException(
                            $"Can not find relationship model for {selector}");
                    }

                    return _model.FindEntityType(entityType).FindProperty(memberInfo.Name).Relational().ColumnName;
                default:
                    throw new InvalidOperationException("Only member assess expresion can be parsed.");
            }
        }
    }
}