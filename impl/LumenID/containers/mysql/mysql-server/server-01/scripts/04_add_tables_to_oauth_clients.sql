-- Set database
use `oauth_clients`;

-- Create tables
create table if not exists `clients` (
    `id`           varchar(36)     not null,
    `public_key`   varchar(100)     not null,
    `public_value` varchar(100)    not null,
    `secret_key`   varchar(64)     not null,
    `redirect_url` text            not null,

    primary key (`id`)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_general_ci;

-- Load data
insert into `clients` values (
    'b33442b7-1f5a-4970-b623-93c2588f6001',
    'MmVlYTJmZmMtMzRmMi00NTY0LWEzNWUtZDAxYzVmMTNlNTg5',
    '445c3c148c5c2c34a9e179c890b52ea5d92eee774ee1d6d886c68fe84ed15b77',
    'd1e57541ff5d011f6e5c37920993174e53b586aa929ca52785ae87f21c859283',
    'http://localhost:5099/oauth/callback'
);
