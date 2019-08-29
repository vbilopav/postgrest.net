# postgrest.net

Turn your PostgreSQL database directly into a RESTful API with .NET Core.

## Quick start

1. Add **postgrest.net** Nuget to your project.

2. Add `AddPostgRest()` to your MVC builder configuration, like this:

```csharp
services.AddMvc().AddPostgRest();
```

3. Specify default database connection any way you like, by either: 
	- a) Injecting `NpgsqlConnection` into your services 
	- b) Specifying `Connection` parameter in options `AddPostgRest(new PostgRestOptions{Connection = '<connection name or connection string value>'});`
	- c) Add new configuration section `"PostgRest": { "Connection": "<connection name or connection string value>" }`
	
4. Add some PostgreSQL functions to your database. When you run you application all functiion that have default naming convention (name starts with `rest_{get|post|put|delete}`) are turned automatically into RESTful enpoints.
