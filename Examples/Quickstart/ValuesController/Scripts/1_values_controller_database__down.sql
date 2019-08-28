revoke connect on database quickstart_values_controller from public;

select pid, pg_terminate_backend(pid)
from pg_stat_activity
where datname = 'quickstart_values_controller' AND pid <> pg_backend_pid();

drop database quickstart_values_controller;

drop role quickstart_values_controller;
