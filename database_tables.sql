create table addresses
(
    id            char(36) not null
        primary key,
    name          text     not null,
    street        text     not null,
    street_number text     not null,
    country       text     not null
);

create table product_categories
(
    id    char(36) not null
        primary key,
    title text     not null
);

create table product_images
(
    id             char(36) not null
        primary key,
    description    text     not null,
    base_64_string text     not null,
    image_type     text     not null
);

create table users
(
    id         char(36)   not null
        primary key,
    first_name text       not null,
    last_name  text       not null,
    email      text       not null,
    password   text       not null,
    is_admin   tinyint(1) not null,
    constraint email
        unique (email) using hash
);

create table orders
(
    id          char(36) not null
        primary key,
    total_price decimal  not null,
    user_id     char(36) not null,
    constraint orders_users_id_fk
        foreign key (user_id) references users (id)
);

create table products
(
    id                char(36)      not null
        primary key,
    user_id           char(36)      not null,
    label             text          not null,
    release_date      date          not null,
    image_id          char(36)      null,
    category_id       char(36)      not null,
    price             decimal(5, 2) not null,
    title             text          not null,
    description       text          not null,
    description_short text          not null,
    constraint products_product_images_id_fk
        foreign key (image_id) references product_images (id),
    constraint products_users_id_fk
        foreign key (user_id) references users (id)
);

create table orders_products
(
    order_id   char(36) not null,
    product_id char(36) not null,
    amount     int      not null,
    primary key (order_id, product_id),
    constraint orders_products_orders_id_fk
        foreign key (order_id) references orders (id),
    constraint orders_products_products_id_fk
        foreign key (product_id) references products (id)
);

create table wishlist_products
(
    product_id char(36) not null,
    user_id    char(36) not null,
    primary key (product_id, user_id)
);


