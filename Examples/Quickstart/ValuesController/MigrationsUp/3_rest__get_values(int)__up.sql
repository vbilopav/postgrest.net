
/*
* get value by id
* `GET api/values/{_id}/`
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
