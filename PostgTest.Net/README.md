# postgtest.net

Facilitate **unit testings** and/or **TDD** in your .NET Core applications of

- your **PostgreSQL** functions and procedures

or

- **`postgrest.net`** RESTful API.

or

- any API backed by **PostgreSQL** database

## Packages

- [PostgTest.Net](https://github.com/vbilopav/postgrest.net/tree/master/PostgTest.Net/PostgTest.Net) - core functionality. Used directly with `XUnit`. It can be used with `MSTest` but it needs more plumbing code.

- [PostgTest.Net.MSTest](https://github.com/vbilopav/postgrest.net/tree/master/PostgTest.Net/PostgTest.Net.MSTest) - wrapper around `PostgTest.Net` that implements plumbing code for `MSTest`

## How it works

- Before any test executes:

	- Create **new database** for testing and optionally **new database user**  (from configuration data)
	- Run migration up scripts (if any configured)
	- Run database seed fixture (data available for every test) from script or data factory (if seeding data configured)
	- Do any other pending initialization like creating .NET Core server and configuring `postgrest.net` (if any initialization configured)

- Before every test executes:

	- Start new transaction

- After every test executes:

## Quick start

1. Add **postgrest.net** NuGet to your project.

## Licence

Copyright (c) Vedran Bilopavlović.
This source code is licensed under the [MIT license](https://github.com/vbilopav/postgrest.net/blob/master/LICENSE).
