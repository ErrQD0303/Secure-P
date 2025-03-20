BEGIN TRANSACTION;

CREATE TABLE country
( id serial primary key
, code varchar(2) unique not null
, name varchar(255) not null
);

CREATE TABLE city
( id serial primary key
, name varchar(255) not null
, country_id integer
, constraint fk_city_country
	foreign key (country_id)
	references country(id)
	on delete set null
);

CREATE TABLE "user"
( id serial primary key
, username varchar(255)
, full_name varchar(255) not null
, mobile_number varchar(50) not null
, email varchar(255) not null
, dob timestamp
, city_id integer
, constraint fk_user_city
	foreign key (city_id)
	references city(id)
	on delete set null
, address_line_1 varchar(255)
, address_line_2 varchar(255)
, post_code varchar(10)
, license_plate_number varchar(50)
);

CREATE TABLE parking_rate
( id serial primary key
, hourly numeric(15, 2)
, daily numeric(15, 2)
, monthly numeric(15, 2)
);

CREATE TABLE parking_location
( id serial primary key
, name varchar(255) not null
, address varchar(255)
, city_id integer
, constraint fk_parking_location_city
	foreign key (city_id)
	references city(id)
	on delete set null
, parking_rate_id integer
, constraint fk_parking_location_parking_rate
	foreign key (parking_rate_id)
	references parking_rate(id)
	on delete set null
, clamping_fee numeric(15, 2)
, change_signage_fee numeric(15, 2)
);

CREATE TABLE product_type
( id serial primary key
, name varchar(255) not null
);

CREATE TABLE parking_zone
( id serial primary key
, name varchar(255) not null
, parking_location_id INTEGER
, CONSTRAINT fk_parking_zone_parking_location
	FOREIGN KEY (parking_location_id)
	REFERENCES parking_location(id)
	ON DELETE SET NULL
, capacity integer
);

CREATE TABLE subscription
( id serial primary key
, user_id integer
, constraint fk_subscription_user
	foreign key (user_id)
	references "user"(id)
	on delete set null
, parking_zone_id integer
, constraint fk_subscription_parking_zone
	foreign key (parking_zone_id)
	references parking_zone(id)
	on delete set null
, product_type_id integer
, constraint fk_subscription_product_type
	foreign key(product_type_id)
	references product_type(id)
	on delete set null
, is_paid boolean default false
, payment_date timestamp
);

set constraints fk_city_country immediate;
set constraints fk_user_city immediate;
set constraints fk_parking_location_city immediate;
set constraints fk_parking_location_parking_rate immediate;
set constraints fk_parking_zone_parking_location immediate;
set constraints fk_subscription_user immediate;
set constraints fk_subscription_parking_zone immediate;
set constraints fk_subscription_product_type immediate;

-- Insert sample data into the users table
-- INSERT INTO user (first_name, last_name)
-- VALUES 
--     ('Alice', 'Smith'),
--     ('Bob', 'Johnson'),
--     ('Carol', 'Williams');

-- -- Insert sample data into the parking_locations table
-- INSERT INTO parking_location (name)
-- VALUES 
--     ('Downtown Parking'),
--     ('Mall Parking'),
--     ('Airport Parking');

-- -- Insert sample data into the parking_zones table
-- -- Ensure parking_location_id references existing parking_locations(id)
-- INSERT INTO parking_zone (name, parking_location_id)
-- VALUES 
--     ('Zone A', 1),  -- Downtown Parking
--     ('Zone B', 1),  -- Downtown Parking
--     ('Zone C', 2),  -- Mall Parking
--     ('Zone D', 3);  -- Airport Parking

create index city_country_ix on city(country_id);
create index users_full_name_ix on "user"(full_name);
create index user_city_ix on "user"(city_id);
create index parking_location_city_ix on parking_location(city_id);
create index parking_location_parking_rate_ix on parking_location(parking_rate_id);
create index parking_zone_parking_location_ix on parking_zone(parking_location_id);
create index subscription_user_ix on subscription(user_id);
create index subscription_parking_zone_ix on subscription(parking_zone_id);
create index subscription_product_type_ix on subscription(product_type_id);

COMMIT;