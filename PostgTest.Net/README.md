# postgtest.net

Facilitate **unit testings** and/or **TDD** in your .NET Core applications of

- your **PostgreSQL** functions and procedures

or

- **`postgrest.net`** RESTful API.

or

- any API backed by **PostgreSQL** database

## Packages

- [PostgTest.Net](https://github.com/vbilopav/postgrest.net/tree/master/PostgTest.Net/PostgTest.Net) - core functionality. It can be used directly with `XUnit` test runner.

- [PostgTest.Net.MSTest](https://github.com/vbilopav/postgrest.net/tree/master/PostgTest.Net/PostgTest.Net.MSTest) - wrapper around `PostgTest.Net` that implements plumbing code for `MSTest` test runner.

## How it works

- Before any test executes:

	- Create **new database** for testing and optionally **new database user**  (from configuration data)

	- Run migration up scripts (if any configured)

	- Run database seed fixture (data available for every test) from insert script or data factory (if seeding data configured)

	- Do any other pending initialization like creating .NET Core Server and/or configuring `postgrest.net` (if any initialization is configured)

- Before every test executes:

	- Starts new transaction

	- Insert test data from script or data factory (if test data is specified) with deffer referential constraints options (so that you won't have to insert half of database) - if configured.

- Test your logic / function / REST endpoint

- After every test executes:

	- Rollback transaction started before test. Tests are autonomous.

- After all test have been executed:

	- Drop test database and test user (if test user was created)

## Quick start

1. Add NuGet package:

- For `XUnit` add **postgtest.net** NuGet to your project.
- For `MSTest` add **postgtest.net.mstest** NuGet to your project.

2. Add minimal plumbing code to your test project:

- For `XUnit`:

```csharp
    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlFixture> { }
```

(collection name can be different)

- For `MSTest`:

```csharp
    [TestClass]
    public class DummyUnitTest
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext ctx) => PostgreSqlFixture.AssemblyInitialize(ctx);
        [AssemblyCleanup]
        public static void AssemblyCleanup() => PostgreSqlFixture.AssemblyCleanup();
    }
```

(test name name can be different)

3. Test class that will use this infrastructure must inherit class `PostgreSqlUnitTest`:

- For `XUnit` test classes must inherit from `PostgTest.Net` and also have `Collection attribute`:

```csharp
    [Collection("PostgreSqlTestDatabase")]
    public class MyPostgreSqlTests : PostgreSqlUnitTest
    {
		// your tests here
    }
```
(collection name is same name from `CollectionDefinition` attribute)

- For `MSUnit` test classes must inherit from `PostgTest.Net.MSTest`:
```csharp
    [TestClass]
    public class MyPostgreSqlTests : PostgreSqlUnitTest
    {
		// your tests here
    }
```

## Configuration

## Testing API

## Licence

Copyright (c) Vedran Bilopavlović.
This source code is licensed under the [MIT license](https://github.com/vbilopav/postgrest.net/blob/master/LICENSE).
