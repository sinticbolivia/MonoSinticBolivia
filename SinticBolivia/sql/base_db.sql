create table parameters(id bigint not null auto_increment, 
					p_name varchar(100), 
					p_value text, 
					creation_date datetime,
					primary key(id)
					);
create table person(person_code varchar(250), 
					person_type_id bigint not null, 
					branch_id bigint default 0,
					first_name varchar(150), 
					last_name varchar(150), 
					birth_date date, 
					identity_document varchar(150),
					address varchar(150), 
					telephone varchar(50), 
					email varchar(120), 
					status varchar(50), 
					creation_date datetime,
					primary key(person_code));
					
create table person_attributes(attribute_id bigint not null auto_increment, person_code varchar(250), attribute_name varchar(100), attribute_value text, creation_date datetime, 
								primary key(attribute_id));
create table person_type(person_type_id bigint not null auto_increment, person_type varchar(100), creation_date datetime, primary key(person_type_id));
create table users(user_id bigint not null auto_increment, 
					username varchar(100), 
					pass varchar(300), 
					person_code varchar(250), 
					rol_id bigint not null,
					status tinyint(1), 
					creation_date datetime, 
					primary key(user_id) );
create table roles(rol_id bigint not null auto_increment, 
					rol_name varchar(120), 
					rol_description varchar(250), 
					creation_date datetime, 
					primary key(rol_id) );			
create table rol_capabilities(capability_id bigint not null auto_increment, rol_id bigint not null, name varchar(120), `key` varchar(100), description varchar(250), 
								attached_object varchar(250), creation_date datetime, 
								primary key(capability_id));		
create table object_attachments(attachment_id bigint not null auto_increment, object_id varchar(150), object_type varchar(100),
								attachment_type varchar(80), attachment_file varchar(250), creation_date datetime, 
								primary key(attachment_id));
create table branches(branch_id bigint not null auto_increment, branch_name varchar(150), 
						branch_address varchar(250),
						branch_lat decimal(20,20),
						branch_lng decimal(20,20),
						company_id bigint not null,
						branch_manage varchar(250),
						creation_date datetime,
						primary key(branch_id)
						);
create table terminals(terminal_id bigint not null auto_increment,
						branch_id bigint not null,
						terminal_name varchar(150),
						terminal_type varchar(100),
						status varchar(50),
						creation_date datetime,
						primary key(terminal_id)
						);