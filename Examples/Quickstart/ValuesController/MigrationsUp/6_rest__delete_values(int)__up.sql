/*
* delete record by id
* `DELETE api/values/{_id}/`
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
