using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.Expressions;

internal sealed class ExpressionParameterReplacer(
    ParameterExpression source,
    ParameterExpression target) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (node == source)
        {
            return target;
        }

        return base.VisitParameter(node);
    }
}
