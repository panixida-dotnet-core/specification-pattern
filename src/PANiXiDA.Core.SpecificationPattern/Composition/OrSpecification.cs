using PANiXiDA.Core.SpecificationPattern.Abstractions;
using PANiXiDA.Core.SpecificationPattern.Expressions;

using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.Composition;

internal sealed class OrSpecification<T>(ISpecification<T> left, ISpecification<T> right)
    : CompositeSpecification<T>(left, right)
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return SpecificationExpressionComposer.OrElse(
            Left.ToExpression(),
            Right.ToExpression());
    }
}
