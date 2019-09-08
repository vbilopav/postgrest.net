/*
* get all values
* `GET api/values/`
*/
create function rest__get_values() returns json as
$$
begin
    raise info 'rest__get_values()';
    return (
        select json_agg("value") from "values"
    );
end
$$ language plpgsql security definer;
revoke all on function rest__get_values() from public;
grant execute on function rest__get_values() to quickstart_values_controller;
