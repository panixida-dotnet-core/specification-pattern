using PANiXiDA.Core.SpecificationPattern.Abstractions;
using PANiXiDA.Core.SpecificationPattern.Core;
using PANiXiDA.Core.SpecificationPattern.Expressions;

using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.Composition;

internal sealed class NotSpecification<T>(ISpecification<T> specification)
    : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return SpecificationExpressionComposer.Not(specification.ToExpression());
    }
}
