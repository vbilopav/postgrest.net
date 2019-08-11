create or replace function rest__get_values() returns json as
$$
begin
    return json_build_object(
        'timestamp', now() at time zone 'utc',
        'values', array['value1', 'value2', 'value3']
    );
end
$$ language plpgsql security definer;


create or replace function rest__get_values_json(_query json) returns json as
$$
begin
    return json_build_object(
        'timestamp', now() at time zone 'utc',
        'values', _query
    );
end
$$ language plpgsql security definer;

