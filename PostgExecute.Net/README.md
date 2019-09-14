# postgexecute.net

Lightweight wrapper around `NpgsqlConnection` object to facilitate simple `PostgreSQL` execution data retrieval in .NET Core.

**This is NOT an ORM**

I repeat:

**This is NOT an ORM**

Read operations will serialize results to `IEnumerable<IDictionary<string, object>>`

* or

it will provide callback function (either `sync` or `async`) so that data serialization can be further customized as needed.

If someone whishes to build micro-ORM - `Dapper` style - on this code base - he or she may freely do so if they give proper credits/mentions and link to this repository. And if I may suggest a name: `NoORM` for example.

Current version works only with `PostgreSQL` and there are no plans for now to expand to other databases.

## Usage

### Install `PostgExecute.Net` or clone this project repo.

### There are two ways to use this API

####  1. `PostgreSQL` connection extensions

For example:

```csharp
using (var connection = new NpgsqlConnection("<connection string>"))
{
    // Execute can be chained
    connection.Execute("pgpsql command 1").Execute("pgpsql command 2");
}
```

####  2. Static methods

For example:

```csharp
Postg.Execute("<connection string>", "pgpsql command 1");
Postg.Execute("<connection string>", "pgpsql command 2");
// ...
```

**Note:**
> When using static methods `PostgreSQL` new connection will be created and disposed.

> `NpgsqlConnection` will recycle connection made from same thread (they'll have same PID), but, any pending transaction will be lost and rollback.

### API's

- `Execute` and `ExecuteAsync` - Execute PGPSQL command on `PostgreSQL` database

- `Single` and `SingleAsync`  - Fetch single row from `PostgreSQL` database

- `Read` and `ReadAsync` - Read multiple rows from `PostgreSQL` database

For full list of all overloads see interface definition in [this file](https://github.com/vbilopav/postgrest.net/blob/master/PostgExecute.Net/IPostg.cs).

### Working with parameters

> By  convention, parameters on query must start with letter `@`

#### Positional parameters

Example:

```csharp

var result = connection.Single(
    @"select * from (
        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
    ) as sub
    where first = @1 and bar = @2 and day = @3
    ",
    1, "foo", new DateTime(1977, 5, 19));
```

**Note:**
> Positional parameters are assigned in order of appearance in query, name is completely irrelevant (but, they must have one).

#### Named parameters

Unlike positional parameters, when using named parameters - position is irrelevant and every parameter must have unique name.

To accept named parameters interface exposes parameters collection (type [`NpgsqlParameterCollection`](https://github.com/npgsql/npgsql/blob/dev/src/Npgsql/NpgsqlParameterCollection.cs)) as lambda parameter.

There is also extension on this type that adds name and value pair that returns same object for chaining:

Example:

```csharp
var result = connection.Single(
    @"select * from (
        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
    ) as sub
    where first = @1 and bar = @2 and day = @3
    ", p => p
        .Add("3", new DateTime(1977, 5, 19)) // adding new parameters are chained
        .Add("2", "foo")
        .Add("1", 1));
```

* or even shorter with `@P` alias:

```csharp
var result = connection.Single(
    @"select * from (
        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
    ) as sub
    where first = @1 and bar = @2 and day = @3
    ", p => p
        .@P("3", new DateTime(1977, 5, 19))
        .@P("2", "foo")
        .@P("1", 1));
```

* Each interface method has `async` overload of this lambda parameter in case there is need for `async` operation (such as getting username from database for example):

```csharp
var result = connection.Single(
    @"select * from (
        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
    ) as sub
    where first = @1 and bar = @2 and day = @3
    ", async p => {
        await Task.Delay(0); // some async op...
        p.@P("3", new DateTime(1977, 5, 19)).@P("2", "foo").@P("1", 1);
    });
```

### Fetching the data

Default result set for this API is `IDictionary<string, object>` for each row:

```csharp
var result = connection.Single(
    var result = connection.Single("select 1, 'foo' as bar, '1977-05-19'::date as day, null as null");
    // result is `IDictionary<string, object>` ...
```

> There is no data conversion whatsoever (this is not ORM), `null` values will have C# `null` but `DBNull.Value`. Database and your favorite programming language are not the same thing.

Same logic applies for reading multiple rows. Read will return `IEnumerable<IDictionary<string, object>>`:

```csharp
var result = connection.Read(
    @"select * from (
    values
        (1, 'foo1', '1977-05-19'::date),
        (2, 'foo2', '1978-05-19'::date),
        (3, 'foo3', '1979-05-19'::date)
    ) t(first, bar, day)");
// result is `IEnumerable<IDictionary<string, object>>`
```

Async version works little bit differently.

Instead of returning entire enumerable, it has extra lambda parameter that is executed on each result step. Example:

```csharp
var result = new List<IDictionary<string, object>>();
await connection.ReadAsync(
    @"select * from (
    values
        (1, 'foo1', '1977-05-19'::date),
        (2, 'foo2', '1978-05-19'::date),
        (3, 'foo3', '1979-05-19'::date)
    ) t(first, bar, day)",
    r => result.Add(r));
```

Each version also has lambda overload that have `bool` return value. If it returns `false` iteration will break and command cleaned up:

```csharp
var result = new List<IDictionary<string, object>>();
await connection.ReadAsync(
    @"select * from (
    values
        (1, 'foo1', '1977-05-19'::date),
        (2, 'foo2', '1978-05-19'::date),
        (3, 'foo3', '1979-05-19'::date)
    ) t(first, bar, day)",
    r =>
    {
        if ((int)r["first"] == 2)
        {
            return false;
        }
        result.Add(r);
        return true;
    });
```

Of course, there is `async` overload for each API version.

> This opens up room for working with more flexible structures...

> For example, you can serialize to a class instance and add it to a list, or even better a dictionary where key is your database key.

> **For full list of all overloads see interface definition in [this file](https://github.com/vbilopav/postgrest.net/blob/master/PostgExecute.Net/IPostg.cs).**


## Tests

Tests can be found [here](https://github.com/vbilopav/postgrest.net/tree/master/Tests/PostgExecute.Net.Tests)

## Future plans / TODO

When `C# 8.0` finally comes out - implement those fancy, ultra fast async streams for read operations supported by C# 8.0 (e.g. `async return yield`)

## Licence

Copyright (c) Vedran Bilopavlović.
This source code is licensed under the [MIT license](https://github.com/vbilopav/postgrest.net/blob/master/LICENSE).
