using PANiXiDA.Core.SpecificationPattern.Core;

using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.Factories;

/// <summary>
/// Creates common specification instances.
/// </summary>
public static class SpecificationFactory
{
    /// <summary>
    /// Creates a specification that is satisfied by every candidate.
    /// </summary>
    /// <typeparam name="T">The type of candidate object evaluated by the specification.</typeparam>
    /// <returns>A specification that always returns <see langword="true" />.</returns>
    public static Specification<T> All<T>()
    {
        return new TrueSpecification<T>();
    }

    /// <summary>
    /// Creates a specification that is not satisfied by any candidate.
    /// </summary>
    /// <typeparam name="T">The type of candidate object evaluated by the specification.</typeparam>
    /// <returns>A specification that always returns <see langword="false" />.</returns>
    public static Specification<T> None<T>()
    {
        return new FalseSpecification<T>();
    }

    /// <summary>
    /// Creates a specification from the specified predicate expression.
    /// </summary>
    /// <param name="expression">The predicate expression that defines the specification.</param>
    /// <typeparam name="T">The type of candidate object evaluated by the specification.</typeparam>
    /// <returns>A specification backed by the specified predicate expression.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expression" /> is <see langword="null" />.</exception>
    public static Specification<T> Create<T>(Expression<Func<T, bool>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        return new ExpressionSpecification<T>(expression);
    }
}
