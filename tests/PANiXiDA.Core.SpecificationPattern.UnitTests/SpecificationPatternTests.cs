using PANiXiDA.Core.SpecificationPattern.Core;
using PANiXiDA.Core.SpecificationPattern.Extensions;
using PANiXiDA.Core.SpecificationPattern.Factories;

using System.Linq.Expressions;

namespace PANiXiDA.Core.SpecificationPattern.UnitTests;

public sealed class SpecificationPatternTests
{
    [Fact(DisplayName = "All returns a specification satisfied by every candidate")]
    public void All_ReturnsSpecificationSatisfiedByEveryCandidate()
    {
        Specification<TestItem> specification = SpecificationFactory.All<TestItem>();
        TestItem candidate = CreateItem(0);

        bool isSatisfied = specification.IsSatisfiedBy(candidate);
        bool expressionResult = specification.ToExpression().Compile()(candidate);

        isSatisfied.Should().BeTrue();
        expressionResult.Should().BeTrue();
    }

    [Fact(DisplayName = "None returns a specification satisfied by no candidate")]
    public void None_ReturnsSpecificationSatisfiedByNoCandidate()
    {
        Specification<TestItem> specification = SpecificationFactory.None<TestItem>();
        TestItem candidate = CreateItem(0);

        bool isSatisfied = specification.IsSatisfiedBy(candidate);
        bool expressionResult = specification.ToExpression().Compile()(candidate);

        isSatisfied.Should().BeFalse();
        expressionResult.Should().BeFalse();
    }

    [Fact(DisplayName = "Create returns a specification backed by the specified expression")]
    public void Create_ReturnsSpecificationBackedBySpecifiedExpression()
    {
        Expression<Func<TestItem, bool>> expression = item => item.Value >= 10;

        Specification<TestItem> specification = SpecificationFactory.Create(expression);

        specification.IsSatisfiedBy(CreateItem(12)).Should().BeTrue();
        specification.IsSatisfiedBy(CreateItem(9)).Should().BeFalse();
        specification.ToExpression().Should().BeSameAs(expression);
    }

    [Fact(DisplayName = "Create throws when expression is null")]
    public void Create_WhenExpressionIsNull_ThrowsArgumentNullException()
    {
        Action act = () => SpecificationFactory.Create<TestItem>(null!);

        act.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("expression");
    }

    [Fact(DisplayName = "And returns a specification satisfied only when both specifications are satisfied")]
    public void And_ReturnsSpecificationSatisfiedOnlyWhenBothSpecificationsAreSatisfied()
    {
        Specification<TestItem> hasMatchingTag = SpecificationFactory.Create<TestItem>(
            item => item.Tags.Any(tag => tag == "match"));
        Specification<TestItem> hasPositiveValue = SpecificationFactory.Create<TestItem>(
            item => item.Value > 0);

        Specification<TestItem> specification = hasMatchingTag.And(hasPositiveValue);

        specification.IsSatisfiedBy(CreateItem(1, "default", "match")).Should().BeTrue();
        specification.IsSatisfiedBy(CreateItem(-1, "default", "match")).Should().BeFalse();
        specification.IsSatisfiedBy(CreateItem(1, "default", "other")).Should().BeFalse();
    }

    [Fact(DisplayName = "And throws when specification is null")]
    public void And_WhenSpecificationIsNull_ThrowsArgumentNullException()
    {
        Specification<TestItem> specification = SpecificationFactory.All<TestItem>();

        Action act = () => specification.And(null!);

        act.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("specification");
    }

    [Fact(DisplayName = "Or returns a specification satisfied when any specification is satisfied")]
    public void Or_ReturnsSpecificationSatisfiedWhenAnySpecificationIsSatisfied()
    {
        Specification<TestItem> hasPositiveValue = SpecificationFactory.Create<TestItem>(
            item => item.Value > 0);
        Specification<TestItem> hasTargetCategory = SpecificationFactory.Create<TestItem>(
            item => item.Category == "target");

        Specification<TestItem> specification = hasPositiveValue.Or(hasTargetCategory);

        specification.IsSatisfiedBy(CreateItem(1, category: "other")).Should().BeTrue();
        specification.IsSatisfiedBy(CreateItem(-1, category: "target")).Should().BeTrue();
        specification.IsSatisfiedBy(CreateItem(-1, category: "other")).Should().BeFalse();
    }

    [Fact(DisplayName = "Or throws when specification is null")]
    public void Or_WhenSpecificationIsNull_ThrowsArgumentNullException()
    {
        Specification<TestItem> specification = SpecificationFactory.All<TestItem>();

        Action act = () => specification.Or(null!);

        act.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("specification");
    }

    [Fact(DisplayName = "Not returns a specification satisfied when the inner specification is not satisfied")]
    public void Not_ReturnsSpecificationSatisfiedWhenInnerSpecificationIsNotSatisfied()
    {
        Specification<TestItem> hasPositiveValue = SpecificationFactory.Create<TestItem>(
            item => item.Value > 0);

        Specification<TestItem> specification = hasPositiveValue.Not();

        specification.IsSatisfiedBy(CreateItem(-1)).Should().BeTrue();
        specification.IsSatisfiedBy(CreateItem(1)).Should().BeFalse();
    }

    [Fact(DisplayName = "Queryable Where filters by specification expression")]
    public void QueryableWhere_FiltersBySpecificationExpression()
    {
        IQueryable<TestItem> query = new[]
        {
            CreateItem(1, category: "target"),
            CreateItem(2, category: "other"),
            CreateItem(3, category: "target")
        }.AsQueryable();
        Specification<TestItem> specification = SpecificationFactory.Create<TestItem>(
            item => item.Category == "target");

        TestItem[] result = [.. query.Where(specification)];

        result.Should().HaveCount(2);
        result.Select(item => item.Value).Should().Equal(1, 3);
    }

    [Fact(DisplayName = "Queryable Where throws when arguments are null")]
    public void QueryableWhere_WhenArgumentsAreNull_ThrowsArgumentNullException()
    {
        IQueryable<TestItem> query = Array.Empty<TestItem>().AsQueryable();
        Specification<TestItem> specification = SpecificationFactory.All<TestItem>();

        Action nullQueryAct = () => SpecificationQueryableExtensions.Where(
            (IQueryable<TestItem>)null!,
            specification);
        Action nullSpecificationAct = () => SpecificationQueryableExtensions.Where(
            query,
            null!);

        nullQueryAct.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("query");
        nullSpecificationAct.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("specification");
    }

    [Fact(DisplayName = "Enumerable Where filters by specification predicate")]
    public void EnumerableWhere_FiltersBySpecificationPredicate()
    {
        IEnumerable<TestItem> source =
        [
            CreateItem(1, category: "target"),
            CreateItem(2, category: "other"),
            CreateItem(3, category: "target")
        ];
        Specification<TestItem> specification = SpecificationFactory.Create<TestItem>(
            item => item.Category == "target");

        TestItem[] result = [.. source.Where(specification)];

        result.Should().HaveCount(2);
        result.Select(item => item.Value).Should().Equal(1, 3);
    }

    [Fact(DisplayName = "Enumerable Where throws when arguments are null")]
    public void EnumerableWhere_WhenArgumentsAreNull_ThrowsArgumentNullException()
    {
        IEnumerable<TestItem> source = [];
        Specification<TestItem> specification = SpecificationFactory.All<TestItem>();

        Action nullSourceAct = () => SpecificationQueryableExtensions.Where(
            (IEnumerable<TestItem>)null!,
            specification);
        Action nullSpecificationAct = () => SpecificationQueryableExtensions.Where(
            source,
            null!);

        nullSourceAct.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("source");
        nullSpecificationAct.Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("specification");
    }

    private static TestItem CreateItem(
        int value,
        string category = "default",
        params string[] tags)
    {
        return new TestItem(value, category, tags);
    }

    private sealed class TestItem(
        int value,
        string category,
        IReadOnlyCollection<string> tags)
    {
        public int Value { get; } = value;

        public string Category { get; } = category;

        public IReadOnlyCollection<string> Tags { get; } = tags;
    }
}
