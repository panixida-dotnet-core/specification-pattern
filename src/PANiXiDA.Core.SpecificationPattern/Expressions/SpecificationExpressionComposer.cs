using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.Expressions;

internal static class SpecificationExpressionComposer
{
    internal static Expression<Func<T, bool>> AndAlso<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        return Compose(left, right, Expression.AndAlso);
    }

    internal static Expression<Func<T, bool>> OrElse<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        return Compose(left, right, Expression.OrElse);
    }

    internal static Expression<Func<T, bool>> Not<T>(Expression<Func<T, bool>> expression)
    {
        var parameter = Expression.Parameter(typeof(T), "entity");
        var body = ReplaceParameter(expression.Body, expression.Parameters[0], parameter);

        return Expression.Lambda<Func<T, bool>>(Expression.Not(body), parameter);
    }

    private static Expression<Func<T, bool>> Compose<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        Func<Expression, Expression, BinaryExpression> merge)
    {
        var parameter = Expression.Parameter(typeof(T), "entity");

        var leftBody = ReplaceParameter(left.Body, left.Parameters[0], parameter);
        var rightBody = ReplaceParameter(right.Body, right.Parameters[0], parameter);

        var body = merge(leftBody, rightBody);

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression ReplaceParameter(
        Expression body,
        ParameterExpression source,
        ParameterExpression target)
    {
        var visitor = new ExpressionParameterReplacer(source, target);

        return visitor.Visit(body)!;
    }
}
