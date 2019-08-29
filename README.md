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
	
4. Add some PostgreSQL functions to your database. 

Now, When you run you application all functiion that have default naming convention (name starts with **`rest_{get|post|put|delete}`**) are turned automatically into RESTful enpoints.

## Examples

- Add following function to you PostgreSQL database:

```sql
create function rest__get_values(_id int) returns text as
$$
begin
	return (
		select "value" from "values" where id = _id
	);
end
$$ language plpgsql;
```

- Run your application. Notice following info log on startup in your application console:

```
info: PostgRest.net.FeatureProvider[0]
	  Mapping PostgreSQL routine "function rest__get_values(IN _id integer) returns text" to REST API endpoint "GET api/values/{_id}/" ...
```

- You can also map **query string** values or **body** values (either json or multipart form) to your PostgreSQL function parameters.
By convention if parameter name contains `body` or `query` (this is configurable) it will be mapped to body or query string respectivly.
For example, if you include `_body` parameter in function, you may see following info log in your application console:

```
info: PostgRest.net.FeatureProvider[0]
      Mapping PostgreSQL routine "function rest__put_values(/*from body*/IN _body json) returns json" to REST API endpoint "PUT api/values/" ...
```


Checkout full, working examples in [examples directory](https://github.com/vbilopav/postgrest.net/tree/master/Examples) of this repository


Also, you may consult [unit tests directory](https://github.com/vbilopav/postgrest.net/tree/master/UnitTests) for different usage examples.

## Licence

Copyright (c) Vedran Bilopavlović.
This source code is licensed under the [MIT license](https://github.com/vbilopav/postgrest.net/blob/master/LICENSE).

