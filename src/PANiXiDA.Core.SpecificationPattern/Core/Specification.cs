using PANiXiDA.Core.SpecificationPattern.Abstractions;
using PANiXiDA.Core.SpecificationPattern.Composition;

using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.Core;

/// <summary>
/// Provides a base implementation for specifications with compiled predicate evaluation and composition helpers.
/// </summary>
/// <typeparam name="T">The type of candidate object evaluated by the specification.</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    private readonly Lazy<Func<T, bool>> compiledPredicate;

    /// <summary>
    /// Initializes a new instance of the <see cref="Specification{T}" /> class.
    /// </summary>
    protected Specification()
    {
        compiledPredicate = new Lazy<Func<T, bool>>(
            BuildCompiledPredicate,
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <summary>
    /// Converts the specification to an expression that can be used by query providers.
    /// </summary>
    /// <returns>The expression representation of the specification.</returns>
    public abstract Expression<Func<T, bool>> ToExpression();

    /// <summary>
    /// Determines whether the specified candidate satisfies the specification.
    /// </summary>
    /// <param name="candidate">The candidate object to evaluate.</param>
    /// <returns><see langword="true" /> when the candidate satisfies the specification; otherwise, <see langword="false" />.</returns>
    public bool IsSatisfiedBy(T candidate)
    {
        return compiledPredicate.Value(candidate);
    }

    /// <summary>
    /// Creates a specification that is satisfied only when both the current specification and the specified specification are satisfied.
    /// </summary>
    /// <param name="specification">The specification to combine with the current specification.</param>
    /// <returns>A specification that applies logical AND composition.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="specification" /> is <see langword="null" />.</exception>
    public Specification<T> And(ISpecification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        return new AndSpecification<T>(this, specification);
    }

    /// <summary>
    /// Creates a specification that is satisfied when either the current specification or the specified specification is satisfied.
    /// </summary>
    /// <param name="specification">The specification to combine with the current specification.</param>
    /// <returns>A specification that applies logical OR composition.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="specification" /> is <see langword="null" />.</exception>
    public Specification<T> Or(ISpecification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        return new OrSpecification<T>(this, specification);
    }

    /// <summary>
    /// Creates a specification that is satisfied when the current specification is not satisfied.
    /// </summary>
    /// <returns>A specification that applies logical NOT composition.</returns>
    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }

    private Func<T, bool> BuildCompiledPredicate()
    {
        return ToExpression().Compile();
    }
}
