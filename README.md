# PANiXiDA.Core.SpecificationPattern

`PANiXiDA.Core.SpecificationPattern` is a .NET library for implementing the Specification pattern with reusable business rules, composable predicates, and filtering support for in-memory collections and query providers.

It is designed for domain and application code that needs explicit, testable rules which can be evaluated against objects or converted to expression trees for querying.

## Status

[![CI](https://github.com/panixida-dotnet-core/specification-pattern/actions/workflows/ci.yml/badge.svg)](https://github.com/panixida-dotnet-core/specification-pattern/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/PANiXiDA.Core.SpecificationPattern.svg)](https://www.nuget.org/packages/PANiXiDA.Core.SpecificationPattern)
[![NuGet downloads](https://img.shields.io/nuget/dt/PANiXiDA.Core.SpecificationPattern.svg)](https://www.nuget.org/packages/PANiXiDA.Core.SpecificationPattern)
[![Target Framework](https://img.shields.io/badge/target-net10.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](LICENSE)

## Overview

The package provides a small public API for creating and combining specifications:

- define business rules as `ISpecification<T>` or `Specification<T>`;
- create expression-backed specifications with `SpecificationFactory.Create`;
- create always-true and always-false specifications with `SpecificationFactory.All` and `SpecificationFactory.None`;
- compose specifications with `And`, `Or`, and `Not`;
- filter `IQueryable<T>` sources by expression;
- filter `IEnumerable<T>` sources by compiled predicate.

Composition implementations are internal. Consumers work through the public abstraction, base class, factory, and extension methods instead of depending on concrete composition classes.

## Requirements

- .NET 10 SDK
- A project targeting `net10.0` or a compatible target framework

## Installation

Package Manager:

```powershell
dotnet add package PANiXiDA.Core.SpecificationPattern
```

PackageReference:

```xml
<ItemGroup>
  <PackageReference Include="PANiXiDA.Core.SpecificationPattern" Version="1.0.1" />
</ItemGroup>
```

For projects using central package management, define the version in `Directory.Packages.props`.

```xml
<ItemGroup>
  <PackageVersion Include="PANiXiDA.Core.SpecificationPattern" Version="1.0.1" />
</ItemGroup>
```

## Quick Start

Create a specification from an expression and use it to evaluate a candidate object:

```csharp
using PANiXiDA.Core.SpecificationPattern.Core;
using PANiXiDA.Core.SpecificationPattern.Factories;

public sealed class User
{
    public bool IsActive { get; init; }

    public bool IsBlocked { get; init; }

    public int Age { get; init; }
}

Specification<User> activeAdult = SpecificationFactory.Create<User>(
    user => user.IsActive && user.Age >= 18);

bool isSatisfied = activeAdult.IsSatisfiedBy(new User
{
    IsActive = true,
    Age = 21
});
```

## Usage

### Custom specifications

Use `Specification<T>` when a rule deserves a named type:

```csharp
using System.Linq.Expressions;
using PANiXiDA.Core.SpecificationPattern.Core;

public sealed class ActiveUserSpecification : Specification<User>
{
    public override Expression<Func<User, bool>> ToExpression()
    {
        return user => user.IsActive;
    }
}
```

Named specifications are useful when the same rule is reused across domain logic, application services, and query scenarios.

### Composition

Specifications derived from `Specification<T>` can be combined with `And`, `Or`, and `Not`.

```csharp
using PANiXiDA.Core.SpecificationPattern.Core;
using PANiXiDA.Core.SpecificationPattern.Factories;

Specification<User> active = new ActiveUserSpecification();
Specification<User> adult = SpecificationFactory.Create<User>(user => user.Age >= 18);
Specification<User> blocked = SpecificationFactory.Create<User>(user => user.IsBlocked);

Specification<User> activeAdult = active.And(adult);
Specification<User> activeOrAdult = active.Or(adult);
Specification<User> allowedActiveAdult = active.And(adult).And(blocked.Not());
```

`And` and `Or` accept `ISpecification<T>` arguments, so custom implementations can participate in composition. The composition methods themselves are available on `Specification<T>`.

### Queryable filtering

Use the `IQueryable<T>` extension when the source should receive the expression tree.

```csharp
using PANiXiDA.Core.SpecificationPattern.Core;
using PANiXiDA.Core.SpecificationPattern.Extensions;
using PANiXiDA.Core.SpecificationPattern.Factories;
using System.Linq;

IQueryable<User> users = new[]
{
    new User { IsActive = true, Age = 21 },
    new User { IsActive = true, Age = 16 },
    new User { IsActive = false, Age = 30 }
}.AsQueryable();

Specification<User> activeAdult = SpecificationFactory
    .Create<User>(user => user.IsActive)
    .And(SpecificationFactory.Create<User>(user => user.Age >= 18));

IQueryable<User> query = users.Where(activeAdult);
```

This is intended for query providers such as Entity Framework Core, provided the expression can be translated by the provider.

### Enumerable filtering

Use the `IEnumerable<T>` extension for in-memory collections.

```csharp
using PANiXiDA.Core.SpecificationPattern.Core;
using PANiXiDA.Core.SpecificationPattern.Extensions;
using PANiXiDA.Core.SpecificationPattern.Factories;
using System.Collections.Generic;

IEnumerable<User> users = new[]
{
    new User { IsActive = true, Age = 21 },
    new User { IsActive = false, Age = 30 }
};

Specification<User> active = SpecificationFactory.Create<User>(user => user.IsActive);

IEnumerable<User> result = users.Where(active);
```

For `IEnumerable<T>`, the specification expression is compiled and evaluated as a predicate.

## Public API

### `ISpecification<T>`

Core abstraction for specification implementations.

```csharp
bool IsSatisfiedBy(T candidate);
Expression<Func<T, bool>> ToExpression();
```

### `Specification<T>`

Base class for reusable specifications.

- caches the compiled predicate used by `IsSatisfiedBy`;
- requires derived classes to implement `ToExpression`;
- exposes `And`, `Or`, and `Not` composition methods.

### `SpecificationFactory`

Factory for common specification creation scenarios.

```csharp
Specification<T> All<T>();
Specification<T> None<T>();
Specification<T> Create<T>(Expression<Func<T, bool>> expression);
```

### `SpecificationQueryableExtensions`

Filtering extensions for queryable and enumerable sources.

```csharp
IQueryable<T> Where<T>(this IQueryable<T> query, ISpecification<T> specification);
IEnumerable<T> Where<T>(this IEnumerable<T> source, ISpecification<T> specification);
```

## Behavior Notes

- `SpecificationFactory.Create` throws `ArgumentNullException` when `expression` is `null`.
- `Specification<T>.And` and `Specification<T>.Or` throw `ArgumentNullException` when `specification` is `null`.
- Filtering extensions throw `ArgumentNullException` when the source or specification is `null`.
- `Not` negates the current specification.
- `All<T>` is satisfied by every candidate.
- `None<T>` is not satisfied by any candidate.
- Query provider compatibility depends on the expression used by the specification.

## Configuration

The package does not require runtime configuration, environment variables, external services, or application settings.

## Project Structure

```text
.
|-- src/
|   `-- PANiXiDA.Core.SpecificationPattern/
|-- tests/
|   `-- PANiXiDA.Core.SpecificationPattern.UnitTests/
|-- Directory.Build.props
|-- Directory.Build.targets
|-- Directory.Packages.props
|-- global.json
|-- version.json
|-- LICENSE
`-- README.md
```

## Development

Restore dependencies:

```bash
dotnet restore
```

Format code:

```bash
dotnet format
```

Build:

```bash
dotnet build --configuration Release
```

Run tests:

```bash
dotnet test --configuration Release
```

Pack:

```bash
dotnet pack --configuration Release
```

Full local validation:

```bash
dotnet restore
dotnet format
dotnet build --configuration Release
dotnet test --configuration Release
dotnet pack --configuration Release
```

### Continuous integration

Every pull request and push to `main` runs formatting, tests, and mandatory
SonarQube analysis. Publishing from `main` starts only after the SonarQube
Quality Gate succeeds.

Before enabling this workflow, add the repository to the
[shared SonarQube inventory](https://github.com/panixida-infrastructure/core-platform/blob/main/inventory/sonarqube/repositories.json).
Reconciliation provisions the `SONAR_PROJECT_KEY` repository variable and the
`SONAR_TOKEN` repository secret; `SONAR_HOST_URL` is configured at the
organization level.

## Tooling

This repository uses:

- .NET 10
- Nullable reference types
- Implicit usings
- Central package management
- Microsoft Testing Platform
- xUnit v3
- FluentAssertions
- Nerdbank.GitVersioning

## Contributing

When changing the package:

- keep the public API small and intentional;
- avoid unnecessary dependencies;
- preserve existing naming and architecture;
- update tests for meaningful behavior changes;
- update this README when public behavior, public API, package metadata, or development workflow changes.

## License

This project is licensed under the Apache-2.0 license.

See [LICENSE](LICENSE) for details.
