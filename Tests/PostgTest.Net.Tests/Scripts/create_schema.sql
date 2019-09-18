do $$
begin

    create table companies (
        id int not null generated always as identity primary key,
        name varchar(256) not null
    );

    create table employees (
        id int not null generated always as identity primary key,
        first_name varchar(256) not null,
        last_name varchar(256) not null,
        email varchar(1024) not null,
        company_id int not null,
        constraint fk_employees_company_id__companies_id foreign key (company_id)
        references companies (id)
        on update no action
        on delete cascade,
        unique (email)
    );

end
$$;