create table companies (
    id int not null generated always as identity primary key,
    name varchar(256) not null
)
