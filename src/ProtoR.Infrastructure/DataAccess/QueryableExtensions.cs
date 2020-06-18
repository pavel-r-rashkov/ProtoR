namespace ProtoR.Infrastructure.DataAccess
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public static class QueryableExtensions
    {
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
    }
}
