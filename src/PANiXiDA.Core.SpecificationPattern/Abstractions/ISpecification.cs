using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.Abstractions;

/// <summary>
/// Defines a reusable business rule that can evaluate a candidate object and expose its predicate as an expression.
/// </summary>
/// <typeparam name="T">The type of candidate object evaluated by the specification.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Determines whether the specified candidate satisfies the specification.
    /// </summary>
    /// <param name="candidate">The candidate object to evaluate.</param>
    /// <returns><see langword="true" /> when the candidate satisfies the specification; otherwise, <see langword="false" />.</returns>
    bool IsSatisfiedBy(T candidate);

    /// <summary>
    /// Converts the specification to an expression that can be used by query providers.
    /// </summary>
    /// <returns>The expression representation of the specification.</returns>
    Expression<Func<T, bool>> ToExpression();
}
