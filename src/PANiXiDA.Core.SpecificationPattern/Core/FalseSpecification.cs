using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.Core;

internal sealed class FalseSpecification<T> : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return entity => false;
    }
}
