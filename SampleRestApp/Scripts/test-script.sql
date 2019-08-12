﻿create or replace function rest__get_values() returns json as
$$
begin
    return json_build_object(
        'timestamp', now() at time zone 'utc',
        'values', array['value1', 'value2', 'value3']
    );
end
$$ language plpgsql security definer;
revoke all on function rest__get_values() from public;

create or replace function rest__get_values_json(_query json) returns json as
$$
begin
    return json_build_object(
        'timestamp', now() at time zone 'utc',
        'values', _query
    );
end
$$ language plpgsql security definer;
revoke all on function rest__get_values_json(_query json) from public;

drop table if exists test;
create table  test ( text text not null );
insert into test values (('value1')), (('value2')), (('value3'));

create or replace function rest__get_values_and_table(_query json) returns json as
$$
begin
    return json_build_object(
        'timestamp', now() at time zone 'utc',
        'values', _query,
        'test', (select json_agg(test) from test)
    );
end
$$ language plpgsql security definer;
revoke all on function rest__get_values_and_table(_query json) from public;

grant execute on function
    rest__get_values(),
    rest__get_values_json(json),
    rest__get_values_and_table(json)
to sample;

--select rest__get_values_and_table('{}');
