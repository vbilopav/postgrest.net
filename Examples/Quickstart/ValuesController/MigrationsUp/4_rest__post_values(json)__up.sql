/*
* post new value
* `POST api/values/`
*/
create function rest__post_values(_body json) returns json as
$$
declare _result json;
begin
    raise info 'rest__post_values(%)', _body;
    with cte as (
        insert into "values" ("value")
        values (_body->>'value')
        returning id, "value"
    )
    select to_json(cte) into _result from cte;
    return _result;
end
$$ language plpgsql security definer;
revoke all on function rest__post_values(json) from public;
grant execute on function rest__post_values(json) to quickstart_values_controller;
