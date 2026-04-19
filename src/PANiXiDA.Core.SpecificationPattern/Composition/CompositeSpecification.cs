using PANiXiDA.Core.SpecificationPattern.Abstractions;
using PANiXiDA.Core.SpecificationPattern.Core;

namespace PANiXiDA.Core.SpecificationPattern.Composition;

internal abstract class CompositeSpecification<T>(ISpecification<T> left, ISpecification<T> right)
    : Specification<T>
{
    protected ISpecification<T> Left { get; } = left;
    protected ISpecification<T> Right { get; } = right;
}
