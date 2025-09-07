-- Set database
use `accounts`;

-- Infos
create table if not exists `infos` (
    `id`        varchar(36)     not null,
    `username`  varchar(255) not null,
    `email`     varchar(255) not null,
    `avatar_id` varchar(36)     null,
    `header_id` varchar(36)     null,
    `key_color` varchar(7)   not null default '#000000',

    primary key (`id`),
    unique key `uk_username` (`username`),
    unique key `uk_email` (`email`),

    index `idx_username` (`username`),
    index `idx_email` (`email`)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_general_ci;
-- Configs
create table if not exists `configs` (
    `id`         varchar(36)     not null,
    `notify`     json         not null,
    `theme`      json         not null,
    primary key (`id`)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_general_ci;
-- Secrets
create table if not exists `secrets` (
    `id`           varchar(36)     not null,
    `password`     varchar(60)     not null,
    `secret_key`   varchar(64)     not null,

    primary key (`id`)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_general_ci;
-- Sessions
create table if not exists `sessions` (
    `id`          varchar(36)     not null,
    `meta_id`     varchar(36)     not null,
    `token`       text         not null,
    `created_at`  datetime(6)  not null default current_timestamp(6),
    `updated_at`  datetime(6)  not null default current_timestamp(6) on update current_timestamp(6),
    `expired_at`  datetime(6)  not null,

    primary key (`id`),

    index `idx_meta_id` (`meta_id`),
    index `idx_expired_at` (`expired_at`)
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_general_ci;

-- Metadata
create table if not exists `metadata` (
    `id`             varchar(36)     not null,
    `info_id`        varchar(36)     not null,
    `config_id`      varchar(36)     not null,
    `secret_id`      varchar(36)     not null,
    `created_at`     datetime(6)  not null default current_timestamp(6),
    `updated_at`     datetime(6)  not null default current_timestamp(6) on update current_timestamp(6),
    `deactivated_at` datetime(6)  null,

    primary key (`id`),

    index `idx_info_id` (`info_id`),
    index `idx_config_id` (`config_id`),
    index `idx_secret_id` (`secret_id`),

    foreign key (`info_id`) references `infos` (`id`) on delete cascade on update cascade,
    foreign key (`config_id`) references `configs` (`id`) on delete cascade on update cascade,
    foreign key (`secret_id`) references `secrets` (`id`) on delete cascade on update cascade
) engine=InnoDB default charset=utf8mb4 collate=utf8mb4_general_ci;
