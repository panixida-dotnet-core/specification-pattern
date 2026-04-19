using PANiXiDA.Core.SpecificationPattern.Abstractions;
namespace PANiXiDA.Core.SpecificationPattern.Extensions;

/// <summary>
/// Provides filtering extensions that apply specifications to queryable and enumerable sources.
/// </summary>
public static class SpecificationQueryableExtensions
{
    /// <summary>
    /// Filters a queryable source by using the expression exposed by the specified specification.
    /// </summary>
    /// <param name="query">The queryable source to filter.</param>
    /// <param name="specification">The specification that defines the filter predicate.</param>
    /// <typeparam name="T">The type of element in the queryable source.</typeparam>
    /// <returns>A query that applies the specified specification.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query" /> or <paramref name="specification" /> is <see langword="null" />.</exception>
    public static IQueryable<T> Where<T>(
        this IQueryable<T> query,
        ISpecification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(specification);

        return query.Where(specification.ToExpression());
    }

    /// <summary>
    /// Filters an enumerable source by evaluating each element with the specified specification.
    /// </summary>
    /// <param name="source">The enumerable source to filter.</param>
    /// <param name="specification">The specification that defines the filter predicate.</param>
    /// <typeparam name="T">The type of element in the enumerable source.</typeparam>
    /// <returns>A sequence that contains only elements satisfying the specified specification.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source" /> or <paramref name="specification" /> is <see langword="null" />.</exception>
    public static IEnumerable<T> Where<T>(
        this IEnumerable<T> source,
        ISpecification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(specification);

        return source.Where(specification.IsSatisfiedBy);
    }
}
