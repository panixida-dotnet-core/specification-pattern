using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.Core;

internal sealed class TrueSpecification<T> : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return entity => true;
    }
}
