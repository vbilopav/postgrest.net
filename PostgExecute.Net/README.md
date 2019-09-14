# postgexecute.net

Lightweight wrapper around `NpgsqlConnection` object to facilitate simple `PostgreSQL` execution and data retrieval in .NET Core.

**This is NOT an ORM**

I repeat:

**This is NOT an ORM**

> There is not data conversion whatsoever, and so there is no impedance mismatch issue.

>All read operations will serialize rows directly and only to **`IDictionary<string, object>>`**

* or, even much, much better:

> All read operations ....

it will provide callback function (either `sync` or `async`) so that data serialization can be further customized as needed.









## Developer notes

If someone whishes to build micro-ORM `Dapper` style based on this code base - he or she may freely do so as long as they give proper credits/mentions and link to this repository. And if I may suggest a name - `NoORM` would be just perfect.

Current version works only with `PostgreSQL` and there are no plans for now to expand to other databases.

## Usage

- Install `PostgExecute.Net` or clone this project repo.

- Two ways to use this API:

### 1. `PostgreSQL` connection extensions

Extensions on `NpgsqlConnection` object (like `Dapper`).

For example:

```csharp
using (var connection = new NpgsqlConnection("<connection string>"))
{
    // Execute can be chained. Two chained executes under same connection and/or transaction
    connection.Execute("<pgpsql command 1>").Execute("<pgpsql command 2>");

    // ALL methods not returning any results can be chained

    // Execute another command and read some data and return results
    var myResults1 = connection.Execute("<pgpsql command 3>").Read("<pgpsql read 1>");
    // myResults1 is enumerable of row dictionaries

    // Read into dictionary
    var myResults2 = new List<IDictionary<string, object>>();
    connection.Read("<pgpsql read 2>", result => myResults2.Add(result));

    // Read into custom class
    var myResults3 = List<MyResults>();
    connection.Read("<pgpsql read 3>", r => myResults3.Add(new MyResults{ Field1 = result["field1"] }));

    // etc..
}
```

### 2. Static methods

Each extension have same exact version as static method which takes first parameter connection string.

For example:

```csharp
Postg.Execute("<connection string>", "pgpsql command 1");
Postg.Execute("<connection string>", "pgpsql command 2");
// ...
```

**Notes:**
> No chaining available because connection is used once and disposed immediatel.

> **When using static methods `PostgreSQL` new connection will be created and disposed.**

> `NpgsqlConnection` will recycle connection made from same thread (they'll have same PID), but, any pending transaction will be lost and rolled back.

### API

Full list of entire available API's and their overloads can be found on [this interface definition](https://github.com/vbilopav/postgrest.net/blob/master/PostgExecute.Net/IPostg.cs).

They fall in following three categories:

1. `Execute` and `ExecuteAsync` - Execute PGPSQL command on `PostgreSQL` database

2. `Single` and `SingleAsync`  - Fetch single row from `PostgreSQL` database

3. `Read` and `ReadAsync` - Read multiple rows from `PostgreSQL` database

Each version have it `async` version that ends with `Async` suffix and they process parameters in same way:

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
> Positional parameters are assigned **in order of appearance** in query, **name is completely irrelevant** (but, they must have one).

#### Named parameters

Unlike positional parameters, when using named parameters - position is irrelevant and every parameter **must have unique name.**

To accept named parameters interface exposes parameters collection (type [`NpgsqlParameterCollection`](https://github.com/npgsql/npgsql/blob/dev/src/Npgsql/NpgsqlParameterCollection.cs)) - as lambda parameter:

```csharp
var result = connection.Single(
    @"select * from (
        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
    ) as sub
    where first = @1 and bar = @2 and day = @3
    ", p => {
        p.AddWithValue("3", new DateTime(1977, 5, 19));
        p.AddWithValue("2", "foo");
        p.AddWithValue("1", 1);
    });
```

This API also defines extensions for parameter collection type - that allows chaining. This allows muc mroe elegant syntax:

```csharp
var result = connection.Single(
    @"select * from (
        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
    ) as sub
    where first = @1 and bar = @2 and day = @3

    ", p => p.Add("3", new DateTime(1977, 5, 19)).Add("2", "foo").Add("1", 1));
```

There is also shorter alias named `@P`:

```csharp
var result = connection.Single(
    @"select * from (
        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
    ) as sub
    where first = @1 and bar = @2 and day = @3

    ", p => p.@P("3", new DateTime(1977, 5, 19)).@P("2", "foo").@P("1", 1));
```

API can also take `async` version of this lambda:

```csharp
var result = connection.Single(
    @"select * from (
        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
    ) as sub
    where first = @1 and bar = @2 and day = @3

    ", async p => {
        await Task.Delay(0); // some async operation...
        p.@P("3", new DateTime(1977, 5, 19)).@P("2", "foo").@P("1", 1);
    });
```

### Fetching the data

Default result set for this API is `IDictionary<string, object>` for each row:

```csharp
var result = connection.Single("select 1, 'foo' as bar, '1977-05-19'::date as day, null as null");
// result is `IDictionary<string, object>` that represent result row.
```

Empty result set will yield empty dictionary.

**Note:**

> There is no data conversion whatsoever (this is not ORM).

> `null` values will **NOT** have `C#` `null` - but `DBNull.Value` instead.

Reason for this is because your `null` and database `null` **are not same thing.**

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

### Read callback lambda

Each read method has version that accepts lambda callback that is executed for each row:

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

Each version also has lambda overload that have `bool` as return value.

In that case you can return `false` to break from iteration immediately and safely:

```csharp
var result = new List<IDictionary<string, object>>();
connection.Read(
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
// breaks on second row, third is never executed, result will have only one entry
```

Of course, there is `async` overload for each API version:

```csharp
await connection.ReadAsync(@"
        select * from (
        values 
        (1, 'foo1', '1977-05-19'::date),
        (2, 'foo2', '1978-05-19'::date),
        (3, 'foo3', '1979-05-19'::date)
        ) t(first, bar, day)",
async r =>
{
    await Task.Delay(0); // some async operation...
    result.Add(r);
});
```

## Tests

Test coverage is 100% and it can be found [here](https://github.com/vbilopav/postgrest.net/tree/master/Tests/PostgExecute.Net.Tests)

## Future plans / TODO

When `C# 8.0` finally comes out - implement those fancy, ultra fast async streams for read operations supported by C# 8.0 (e.g. `async return yield`)

## Licence

Copyright (c) Vedran Bilopavlović.
This source code is licensed under the [MIT license](https://github.com/vbilopav/postgrest.net/blob/master/LICENSE).
