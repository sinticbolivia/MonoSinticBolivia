create table product_types(product_type_id bigint not null auto_increment, product_type varchar(100), description varchar(250), creation_date datetime, 
							primary key(product_type_id) );
create table product_categories(category_id bigint not null auto_increment, category_name varchar(100), category_description varchar(250), 
								parent bigint not null, creation_date datetime, 
								primary key(category_id));
create table product2category(id bigint not null auto_increment, product_code varchar(250), category_id bigint not null, creation_date datetime, primary key(id));

CREATE TABLE products(product_code varchar(250), 
					product_type_id bigint not null, 
					product_name varchar(150), 
					product_description varchar(500), 
					product_barcode varchar(250),
					product_cost decimal(10,2), 
					product_price decimal(10,2), 
					product_qty bigint, 
					product_unit_x_paq integer,
					product_units integer,
					stock_type varchar(20),
					status varchar(50),
					creation_date datetime,
					primary key(product_code));
					
CREATE TABLE product_attributes(product_attribute_id bigint not null auto_increment, product_code varchar(250), 
								product_attribute_name varchar(150), product_attribute_value varchar(150),
								product_attribute_price decimal(10,4), product_attribute_prefix char(2),
								creation_date datetime, 
								primary key(product_attribute_id));
CREATE TABLE suppliers(supplier_id bigint not null auto_increment, 
						person_code varchar(250), company_name varchar(100), account_number varchar(100), creation_date datetime,
						primary key(supplier_id));
CREATE TABLE transactionss(
							transaction_code varchar(250),
							transaction_type varchar(100),
							customer_code varchar(250),
							customer_first_name varchar(100),
							customer_last_name varchar(100),
							customer_address varchar(200),
							priority int, 
							transaction_amount decimal(20,4),
							payment_method varchar(100),
							payment_type varchar(100),
							status varchar(100),
							notes varchar(500),
							creation_date datetime, 
							primary key(transaction_code));
CREATE TABLE transaction_attributes(
									attribute_id bigint not null auto_increment, 
									transaction_code varchar(250) not null,
									attribute_name varchar(150), 
									attribute_value varchar(150),
									creation_date datetime,
									primary key(attribute_id));
CREATE TABLE transaction_details(detail_id bigint not null auto_increment, 
											transaction_code varchar(250),
											product_code varchar(250),
											quantity int,
											primary key(detail_id)
											);
/*
CREATE TABLE ingresos_details(ingreso_id bigint not null auto_increment, 
								product_code varchar(250),
								tipo_ingreso varchar(250),
								uni_x_paq integer,
								precio_costo decimal(10,2),
								precio_venta decimal(10,2),
								sub_total decimal(10,2),
								cantidad_productos_real integer,
								cantidad_paquetes integer,
								cantidad_unidades integer,
								creation_date datetime,
								primary key(ingreso_id)
								);
*/
create table if not exists cash_register(id bigint not null auto_increment, 
							opening_balance decimal(20,2),
							ending_balance decimal(20.2),
							currency_id bigint not null,
							user_id bigint not null,
							terminal_id bigint not null,
							branch_id bigint not null,
							status varchar(50),
							creation_date datetime,
							primary key(id)
							);
create table if not exists warehouse_orders(order_id bigint not null,
							order_ref varchar(100),
							observations varchar(250),
							order_type varchar(100),
							shipping_date date,
							status varchar(100),
							creation_date datetime,
							primary key(order_id)
							);
create table if not exists warehouse_order_details(
							detail_id bigint not null auto_increment,
							product_code varchar(250),
							quantity integer,
							creation_date datetime,
							primary key(detail_id)
);