using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
            SelectorCache = new DictionaryExpressionCache<Expression, string>();

        private static readonly ConcurrentDictionary<Type, string> TableNameCache =
            new ConcurrentDictionary<Type, string>();

        private static readonly ConcurrentDictionary<Type, string[]> TableColumnsCache =
            new ConcurrentDictionary<Type, string[]>();

        public EfKataContext(IModel model)
        {
            _model = model;
        }

        public EfKataEntityContext<TE> Entity<TE>()
        {
            return new EfKataEntityContext<TE>(_model);
        }

        public string Table(Type type)
        {
            return TableNameCache.GetOrAdd(type, GetTableName);
        }

        public string Table<T>()
        {
            var type = typeof(T);
            return Table(type);
        }

        public string Column<TE, TP>(Expression<Func<TE, TP>> propertySelector)
        {
            return SelectorCache.GetOrAdd(propertySelector, (expr) =>
            {
                if (expr is Expression<Func<TE, TP>> selector)
                {
                    return GetPropertyColumnNameFromSelector(selector);
                }

                throw new NotSupportedException("Only Lambda expression is supported.");
            });
        }

        public string[] Columns(Type type)
        {
            return TableColumnsCache.GetOrAdd(type, GetAllColumns);
        }

        public string[] Columns<T>()
        {
            var type = typeof(T);
            return TableColumnsCache.GetOrAdd(type, GetAllColumns);
        }

        private string[] GetAllColumns(Type type)
        {
            var tableModel = _model.FindEntityType(type);
            if (tableModel is null)
            {
                throw new InvalidOperationException($"Can not find relationship model for {type.FullName}");
            }

            var tableName = Table(type);

            return tableModel.GetProperties()
                .Select(p => $"{tableName}.{p.Relational().ColumnName}")
                .ToArray();
        }

        private string GetTableName(Type type)
        {
            var tableModel = _model.FindEntityType(type);
            if (tableModel is null)
            {
                throw new InvalidOperationException($"Can not find relationship model for {type.FullName}");
            }

            return tableModel.Relational().TableName;
        }

        private string GetPropertyColumnName(MemberInfo memberInfo)
        {
            var entityType = memberInfo.DeclaringType;
            if (entityType is null)
            {
                throw new InvalidOperationException(
                    $"Can not find relationship model for {memberInfo.Name}");
            }

            var columnName = _model.FindEntityType(entityType).FindProperty(memberInfo.Name).Relational()
                .ColumnName;
            var tableName = Table(entityType);
            return $"{tableName}.{columnName}";
        }

        private string GetPropertyColumnNameFromSelector<TE, TP>(Expression<Func<TE, TP>> selector)
        {
            switch (selector.Body)
            {
                case MemberExpression memberExpr:
                    return GetPropertyColumnName(memberExpr.Member);
                default:
                    throw new InvalidOperationException("Only member assess expresion can be parsed.");
            }
        }
    }
}