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

