-- set value defaults, do not overwrite existing
insert into kv (key, value) values
  ('example', 'text')
on conflict do nothing;

