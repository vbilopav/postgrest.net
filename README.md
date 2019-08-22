# postgrest.net

Easily add REST http endpoints to your PostgreSQL server using .NET Core.

## Quick start

1. Add postgrest.net Nuget to your project.

2. Add following code your MVC builder configuration:

```csharp
services.AddMvc().AddPostgRest(new PostgRestOptions{Connection = '<postgres connection name or string>'});
```

- or add `PostgRest` configuration section with `Connection` configured
- or inject `NpgsqlConnection` way you like in your services

and:

```csharp
services.AddMvc().AddPostgRest();
```

3. Add PostgreSQL function

```plpgsql
create function rest__get_return_query_string(_query json) returns json as 
$$
begin
    return _query;
end 
$$ language plpgsql;
```

4. Run your app. 

Congratulations! 

Now you have following enpoint `GET /api/return-query-string` that returns your query string in json.
