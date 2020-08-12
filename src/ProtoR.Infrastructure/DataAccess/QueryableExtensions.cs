namespace ProtoR.Infrastructure.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using ProtoR.Application.Common;

    public static class QueryableExtensions
    {
        private static readonly MethodInfo ChangeTypeMethod = typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) });

        public static IQueryable<TDestination> WhereWithTypeConversion<TSource, TDestination>(
            this IQueryable<TDestination> query,
            Expression<Func<TSource, bool>> expression)
        {
            var visitor = new ChangeTypeVisitor<TSource, TDestination>();
            var converted = (Expression<Func<TDestination, bool>>)visitor.Visit(expression);

            return query.Where(converted);
        }

        public static IQueryable<TDestination> WhereProjection<TSource, TDestination>(
            this IQueryable<TSource> query,
            Expression<Func<TDestination, bool>> filter)
        {
            var source = Expression.Parameter(typeof(TSource), "source");

            var propertyExpressions = typeof(TDestination)
                .GetProperties()
                .Select(p => Expression.Bind(p, Expression.Property(source, p.Name)));

            var selector = Expression.Lambda<Func<TSource, TDestination>>(
                Expression.MemberInit(
                    Expression.New(typeof(TDestination)),
                    propertyExpressions),
                source);

            return query.Select(selector).Where(filter);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> queryable, Pagination pagination)
        {
            if (pagination == null)
            {
                return queryable;
            }

            return queryable
                .Skip((pagination.Page - 1) * pagination.Size)
                .Take(pagination.Size);
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> queryable, IEnumerable<Filter> filters)
        {
            if (filters == null || !filters.Any())
            {
                return queryable;
            }

            BinaryExpression combinedFilterExpression = default;
            var parameter = Expression.Parameter(typeof(T));

            foreach (var filter in filters)
            {
                var property = Expression.Property(parameter, filter.PropertyName);
                var expressionType = ExpressionType.Equal;

                switch (filter.Type)
                {
                    case FilterType.Equal:
                        expressionType = ExpressionType.Equal;
                        break;
                    case FilterType.NotEqual:
                        expressionType = ExpressionType.NotEqual;
                        break;
                    case FilterType.LessThan:
                        expressionType = ExpressionType.LessThan;
                        break;
                    case FilterType.LessThanOrEqual:
                        expressionType = ExpressionType.LessThanOrEqual;
                        break;
                    case FilterType.GreaterThan:
                        expressionType = ExpressionType.GreaterThan;
                        break;
                    case FilterType.GreaterThanOrEqual:
                        expressionType = ExpressionType.GreaterThanOrEqual;
                        break;
                }

                var propertyType = ((PropertyInfo)property.Member).PropertyType;
                var filterValue = Expression.Call(ChangeTypeMethod, Expression.Constant(filter.Value), Expression.Constant(propertyType));

                var binaryExpression = Expression.MakeBinary(
                    expressionType,
                    property,
                    Expression.Convert(filterValue, propertyType));

                if (combinedFilterExpression == default)
                {
                    combinedFilterExpression = binaryExpression;
                }
                else
                {
                    combinedFilterExpression = Expression.AndAlso(combinedFilterExpression, binaryExpression);
                }
            }

            var filterLambda = Expression.Lambda<Func<T, bool>>(combinedFilterExpression, parameter);

            return queryable.Where(filterLambda);
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> queryable, IEnumerable<SortOrder> sortOrders)
        {
            if (sortOrders == null)
            {
                return queryable;
            }

            IOrderedQueryable<T> ordered = default;

            foreach (var sortOrder in sortOrders)
            {
                var parameter = Expression.Parameter(typeof(T));
                var property = Expression.Property(parameter, sortOrder.PropertyName);
                var propertyAsObject = Expression.Convert(property, typeof(object));

                var orderLambda = Expression.Lambda<Func<T, object>>(propertyAsObject, parameter);

                if (ordered == default)
                {
                    ordered = sortOrder.Direction == SortDirection.Ascending
                        ? queryable.OrderBy(orderLambda)
                        : queryable.OrderByDescending(orderLambda);
                }
                else
                {
                    ordered = sortOrder.Direction == SortDirection.Ascending
                        ? ordered.ThenBy(orderLambda)
                        : ordered.ThenByDescending(orderLambda);
                }
            }

            return ordered;
        }
    }
}
