using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.Core;

internal sealed class ExpressionSpecification<T>(Expression<Func<T, bool>> expression) : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return expression;
    }
}
