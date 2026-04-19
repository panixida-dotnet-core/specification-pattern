using PANiXiDA.Core.SpecificationPattern.Abstractions;
using PANiXiDA.Core.SpecificationPattern.Expressions;

using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.Composition;

internal sealed class AndSpecification<T>(ISpecification<T> left, ISpecification<T> right)
    : CompositeSpecification<T>(left, right)
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return SpecificationExpressionComposer.AndAlso(
            Left.ToExpression(),
            Right.ToExpression());
    }
}
