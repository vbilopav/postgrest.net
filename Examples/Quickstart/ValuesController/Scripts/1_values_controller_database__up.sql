create database quickstart_values_controller;

create role quickstart_values_controller with
    login
    nosuperuser
    nocreatedb
    nocreaterole
    noinherit
    noreplication
    connection limit -1
    password 'quickstart_values_controller';
