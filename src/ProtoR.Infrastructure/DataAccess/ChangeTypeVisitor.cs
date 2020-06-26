namespace ProtoR.Infrastructure.DataAccess
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;

    public class ChangeTypeVisitor<TSource, TTarget> : ExpressionVisitor
    {
        private ReadOnlyCollection<ParameterExpression> parameters;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var parm = this.parameters?.FirstOrDefault(p => p.Name == node.Name);

            if (parm != null)
            {
                return parm;
            }

            if (node.Type == typeof(TSource))
            {
                return Expression.Parameter(typeof(TTarget), node.Name);
            }

            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            this.parameters = this.VisitAndConvert(node.Parameters, "VisitLambda");
            return Expression.Lambda(this.Visit(node.Body), this.parameters);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.DeclaringType == typeof(TSource))
            {
                return Expression.MakeMemberAccess(this.Visit(node.Expression), typeof(TTarget).GetProperty(node.Member.Name));
            }

            return base.VisitMember(node);
        }
    }
}
