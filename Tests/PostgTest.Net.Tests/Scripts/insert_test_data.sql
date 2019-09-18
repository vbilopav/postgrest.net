do 
$$
declare _company_id int;
begin

    insert into companies (name) values ('vb-software');

    select id into _company_id from companies limit 1;

    insert into employees 
    (first_name, last_name, email, company_id) 
    values 
    ('Vedran', 'Bilopavlović', 'vbilopav@gmail.com', _company_id),
    ('Floki', 'The Dog', 'vbilopav+floki@gmail.com', _company_id);

end
$$;
