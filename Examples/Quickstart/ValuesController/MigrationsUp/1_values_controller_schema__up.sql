do $$
begin
    if current_database() <> 'quickstart_values_controller' or current_user = 'quickstart_values_controller'
    then
        raise exception 'wrong connection';
    end if;
    
    drop table if exists "values";
    create table "values" ("value" text);
end
$$;
