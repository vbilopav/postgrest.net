# postgrest.net

Turn your PostgreSQL database directly into a RESTful API with .NET Core.

## Quick start

1. Add **postgrest.net** NuGet to your project.

2. Add `AddPostgRest()` to your MVC builder configuration, like this:

```csharp
services.AddMvc().AddPostgRest();
```

3. Specify default database connection any way you like, by either: 
	- a) Injecting `NpgsqlConnection` into your services 
	- b) Specifying `Connection` parameter in options `AddPostgRest(new PostgRestOptions{Connection = '<connection name or connection string value>'});`
	- c) Add new configuration section `"PostgRest": { "Connection": "<connection name or connection string value>" }`
	
4. Add some PostgreSQL functions to your database. 

Now, when you run you application all function that have default naming convention (name starts with **`rest_{get|post|put|delete}`**) are turned automatically into RESTful endpoints.

## Examples

- Add following function to your PostgreSQL database:

```sql
create function rest__get_values(_id int) returns text as
$$
begin
    return (
        select "value"
        from "values"
        where id = _id
    );
end
$$ language plpgsql;
```

- Run your application. Notice following info log on startup in your application console:

```txt
info: PostgRest.net.FeatureProvider[0]
Mapping PostgreSQL routine "function rest__get_values(IN _id integer) returns text" to REST API endpoint "GET api/values/{_id}/" ...
```

- You can also map **query string** values or **body** values (either json or multipart form) to your PostgreSQL function parameters.
By convention if parameter name contains `body` or `query` (this is configurable) it will be mapped to body or query string respectively.
For example, if you include `_body` parameter in function, you may see following info log in your application console:

```txt
info: PostgRest.net.FeatureProvider[0]
Mapping PostgreSQL routine "function rest__put_values(/*from body*/IN _body json) returns json" to REST API endpoint "PUT api/values/" ...
```

- Also, in spirit of full integration, any log executed inside your PostgreSQL routine (`raise info`, etc) - will be treated as normal part of .NET core logging.

- You may want to checkout full, working examples in [examples directory](https://github.com/vbilopav/postgrest.net/tree/master/Examples) of this repository

There are also [unit tests directory](https://github.com/vbilopav/postgrest.net/tree/master/UnitTests) with full tests coverage (who says SQL can't be tested) for different usage examples.

- Also, Infrastructure from library can be used without direct mapping in order to facilitate PostgreSQL JSON capabilities directly to your response. For example function that returns JSON

```sql
create function select_company_and_sectors_by_user_id(_user_id integer) returns json as
$$
declare _company json;
declare _company_id integer;
begin
    select to_json(c), c.id into _company, _company_id
    from (
        select id, name from companies c where user_id = _user_id limit 1
    ) c;
    return json_build_object(
        'company', _company,
        'sectors', (
            select coalesce(json_agg(s), '[]') from (
                select id, name from sectors where company_id = _company_id order by company_id
            ) s
        )
    );
end
$$ language plpgsql;
```

That function returning JSON can be simply wired up with `postgrest.net.StringContentService` service:

```csharp
    [HttpGet]
    [Route("api/company-and-sectors")]
    public async Task<ContentResult> GetCompanyAndSectorsAsync() =>
        await _content.GetContentAsync("select select_company_and_sectors_by_user_id(@userId)",
            parameters => parameters.AddWithValue("userId", this.User.GetId()));
```

## Yes, but why? / Benefits

A lot has changed since SQL-92.

Modern SQL language is [turing complete](https://en.wikipedia.org/wiki/Turing_completeness) calculus engine. It is only ever successful programming language of forth generation with elements of fifth generation.

As a contrast C# (as well as Java, JavaScript, Python, etc) - are advanced third generation languages.

So it makes very much sense to use Modern SQL, such as PostgreSQL implementation for your entire application backend logic.

Benefits are following:

### Productivity

SQL is also a declarative langanugue that abstracts (or hides) hardware and as well algorithms from the user (or programmer in this case).

That means that we program with SQL by telling the machine **WHAT** to do - not **HOW** to do what we want. Machine will figure out **HOW** for us (in most cases). All we have to do, ideally, as programmers developers - **is to declare the results we want.**

It could be argued that it is final form of functional programming - instead of declaring function to return the results we want - we just simply declare the results we want, let the machines sort it out how to do it. Simple as that. But it requires practice.

To be able to achieve such advanced and high concept - SQL needs to abstract or hide unimportant details from the programmer, such as: entire hardware, operating system and algorithms. So there is no processors, there is no memory, there are no file systems and files whatsoever, there are no processes, no threads and of course locks. Also, there no data structures dictionaries, hash tables, linked lists, no nothing. Only thing what is actually left is - your data and your business logic.

SQL lets you focus on what your really need, on your data and your business logic.

So yes, SQL, given that programmers have the right amount of training - SQL can be make entire development team and individual programmers - **extremely productive.**
I suggest stronlgy watching tech talk [How Modern SQL Databases Come up with Algorithms that You Would Have Never Dreamed Of by Lukas Eder](https://www.youtube.com/watch?v=wTPGW1PNy_Y)

Productivity can give you serious competitive advantage on market.

### Maintainability and Changeability

Maintainability and changeability are two attributes that are somewhat interchangeable.
It is also true that if productivity is at high level that maintenance and change rate is also higher.

However, when using PostgreSQL JSON fields we can change the result without stopping or re-deploying anything. 
For example following endpoint:

```sql
create function rest__get_my_data_set(_query json) returns json as
$$
declare _company json;
declare _company_id integer;
begin
    select to_json(c), c.id into _company, _company_id
    from (
        select id, name
        from companies c
        where user_id = (_query->>'user_id')::int --_query parameter is query string serialized to json
    ) c;
    return json_build_object(
        'company', _company,
        'sectors', (
            select coalesce(json_agg(s), '[]') from (
                select id, name from sectors where company_id = _company_id order by company_id
            ) s
        )
    );
end
$$ language plpgsql;
```

We can replace it safely with new version that inserts log statement and adds new fields to result (address):

```sql
create function rest__get_my_data_set(_query json) returns json as
$$
declare _company json;
declare _company_id integer;
begin
    select to_json(c), c.id into _company, _company_id
    from (
        select id, name, address
        from companies c
        where user_id = (_query->>'user_id')::int --_query parameter is query string serialized to json
    ) c;

    -- this will create log as normal .net core log.information would
    raise info 'selected company: %', _company;

    return json_build_object(
        'company', _company,
        'sectors', (
            select coalesce(json_agg(s), '[]') from (
                select id, name, address from sectors where company_id = _company_id order by company_id
            ) s
        )
    );
end
$$ language plpgsql;
```

And that is it.

Now we have log statement that will help us to see what is going on and we are returning some new fields. We can add aggregates, or anything that SQL will allow and that is literally anything.

Reaction time for maintenance issue is reduced to a minimum. You can always do the software change bureaucracy later - **customers can't wait.**

### Performances

Just using PostgreSQL functions doesn't  mean that the execution plans will be automatically cached.

Execution plans are algorithms that SQL engine has preselected for your declaration of your results when you told it what you want. That is element of 5th generation langanugue - it abstracts (hides) algorithms, so programmer can focus on business logic.

Executions plans are determined on fly, before execution. For same statements that are repeated they can be cached. Currently, the execution plan of a PostgreSQL function may be cached, only if:

- The function is written in `plpgsql`.

- The function is run more than 5 times in a single session,

- The generic plan is not significantly worse than the non-generic plan,

- Dynamic sql is not used,

- The plans are not shared between sessions.

However, besides caching of your algorithms or execution plans - there is great deal of performance gains just by saving of network operations (bandwidth + latency). Because most of the operations are happening on server and there is no need for network operations as much. In example above `rest__get_my_data_set`, normally there would be two queries issued to server and takes toll on network latency.

Certainly, some anti-patterns, such as N+1 query are guaranteed to be avoided.

Furthermore, having high cohesion of SQL code (close to each other and easier to find) - it is easier to determent performance bottlenecks and easier to tune indexing.

### Security

Virtually all database systems have their own security. By using PostgreSQL function you can take full advantage of PostgreSQL role-based security and add another security layer around your application and take your application security to another level.

Here are full instruction on how to leverage PostgreSQL to protect your system from unauthorized access and SQL injections:
[How to write super-uber-mega secure, sql-injection bullet-proof PostgreSQL queries](https://github.com/vbilopav/articles_repo/blob/master/How%20to%20write%20super-uber-mega%20secure%2C%20sql-injection%20bullet-proof%20PostgreSQL%20queries.md)

## Future plans

### Testing NuGet Library

### Migration tools

### Integration of PostgreSQL role based security with .NET Core security system

### Schema compare tool

## Licence

Copyright (c) Vedran Bilopavlović.
This source code is licensed under the [MIT license](https://github.com/vbilopav/postgrest.net/blob/master/LICENSE).
