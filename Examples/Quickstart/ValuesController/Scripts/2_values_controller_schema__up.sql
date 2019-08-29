do $$
begin
    if current_database() <> 'quickstart_values_controller' or current_user = 'quickstart_values_controller'
    then
        raise exception 'wrong connection';
    end if;
    
    drop function if exists rest__get_values();
    drop function if exists rest__get_values(int);
    drop function if exists rest__post_values(json);
    drop function if exists rest__put_values(json);
    drop function if exists rest__delete_values(int);
    
    drop table if exists "values";
end
$$;

/*
* get all values
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

/*
* get value by id
*/
create function rest__get_values(_id int) returns text as
$$
begin
    raise info 'rest__get_values(%)', _id;
    return (
        select "value" from "values" where id = _id
    );
end
$$ language plpgsql security definer;
revoke all on function rest__get_values(int) from public;
grant execute on function rest__get_values(int) to quickstart_values_controller;


/*
* post new value
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

/*
* put value to update a record
*/
create function rest__put_values(_body json) returns json as
$$
declare _result json;
begin
    raise info 'rest__put_values(%)', _body;
    with cte as (
        update "values"
        set "value" = _body->>'value'
        where id = (_body->>'id')::int
        returning id, "value"
    )
    select to_json(cte) into _result from cte;
    return _result;
end
$$ language plpgsql security definer;
revoke all on function rest__put_values(json) from public;
grant execute on function rest__put_values(json) to quickstart_values_controller;

/*
* delete record by id
*/
create function rest__delete_values(_id int) returns json as
$$
declare _result json;
begin
    with cte as (
        delete from "values"
        where id = _id
        returning id, "value"
    )
    select to_json(cte) into _result from cte;
    return _result;
end
$$ language plpgsql security definer;
revoke all on function rest__delete_values(int) from public;
grant execute on function rest__delete_values(int) to quickstart_values_controller;
