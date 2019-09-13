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

## Version history

### 0.0.2

- Name and namespace changed `PostgRest.net` -> `PostgRest.Net` to be more in-line with official convention.
- Upgrade `Npgsql` NuGet from `4.0.9` to `4.0.10`

### 0.0.1

- Initial version

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

## Licence

Copyright (c) Vedran Bilopavlović.
This source code is licensed under the [MIT license](https://github.com/vbilopav/postgrest.net/blob/master/LICENSE).
