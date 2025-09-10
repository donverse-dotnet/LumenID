-- Set database
use `oauth_clients`;

-- Create tables
create table if not exists `secrets` (
    `id`           varchar(36)     not null,
    `public_key`   varchar(100)     not null,
    `public_value` varchar(100)    not null,
    `secret_key`   varchar(64)     not null,
    `redirect_url` text            not null,

    primary key (`id`)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_general_ci;

create table if not exists `profiles` (
     `id`           varchar(36)     not null,
     `name`         varchar(100)    not null,
     `description`  text            not null,
     `icon_id`      varchar(64)     not null,
     `terms`        text            not null,
     `privacy`      text            not null,

     primary key (`id`)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_general_ci;

create table if not exists `metadata` (
    `id`         varchar(36) primary key,
    `secret_id`  varchar(36) not null,
    `profile_id` varchar(36) not null,
    `created_at` datetime(6) not null default current_timestamp(6),
    `updated_at` datetime(6) not null default current_timestamp(6) on update current_timestamp(6),
    
    foreign key (`secret_id`) references secrets (`id`) on delete cascade on update cascade,
    foreign key (`profile_id`) references profiles (`id`) on delete cascade on update cascade
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_general_ci;

create table if not exists `codes` (
    `id`      varchar(36)   primary key,
    `code`    varchar(255)  not null,
    `user_id` varchar(36)   not null,
    `app_id`  varchar(36)   not null,
    `used`    bool          default false,
    
    foreign key (`app_id`) references `metadata` (`id`) on delete cascade on update cascade
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_general_ci;

-- Load data
insert into `secrets` values (
    'b33442b7-1f5a-4970-b623-93c2588f6001',
    'MmVlYTJmZmMtMzRmMi00NTY0LWEzNWUtZDAxYzVmMTNlNTg5',
    '445c3c148c5c2c34a9e179c890b52ea5d92eee774ee1d6d886c68fe84ed15b77',
    'd1e57541ff5d011f6e5c37920993174e53b586aa929ca52785ae87f21c859283',
    'http://localhost:5099/oauth/callback'
);
insert into `profiles` values (
    'b9e7c0ab-0bd7-4d07-a5df-a4400cebdddc',
    'pocco__local-test',
    'This is a local test client for pocco.',
    '1429ee52-7518-4df7-82f9-ad77ea7742a7',
    'https://localhost:5099/terms',
    'https://localhost:5099/privacy'
);
insert into `metadata` values (
    'b1397f99-e1b2-4ab6-9b63-3dcfa2031e37',
    'b33442b7-1f5a-4970-b623-93c2588f6001',
    'b9e7c0ab-0bd7-4d07-a5df-a4400cebdddc',
    '2023-01-01 00:00:00.000001',
    '2025-09-10 14:23:45.123456'
);
