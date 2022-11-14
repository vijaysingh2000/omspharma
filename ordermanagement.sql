use test

drop table orderassay
drop table orderpaymentsinsurance
drop table orderpaymentsclient
drop table batchpayments
drop table ordertasks
drop table orders
drop table patients
drop table drugs
drop table manufacturers
drop table insurances
drop table providers
drop table users
drop table clients
go

create table clients
(
	id int IDENTITY not null primary key nonclustered with fillfactor = 50,
	name  nvarchar(100) not null default(''),
	description nvarchar(max) not null default(''),
	active bit not null default(0),
	perunitfees float not null default(0),
	flatfees float not null default(0)
)

create table users
(
	id int IDENTITY not null primary key nonclustered with fillfactor = 50,
	firstname nvarchar(max) not null default(''),
	lastname nvarchar(max) not null default(''),
	email nvarchar(100) not null default('') unique,
	loginid nvarchar(50) not null default('') unique,
	password nvarchar(max) not null default('')
)
go

create table drugs
(
	id int IDENTITY not null primary key nonclustered with fillfactor = 50,
	name nvarchar(max) not null default(''),
	description nvarchar(max) not null default(''),
	active bit not null default(1),
	perunitfees float not null default(0),
	flatfees float not null default(0)
)
go

create table manufacturers
(
	id int IDENTITY not null primary key nonclustered with fillfactor = 50,
	name nvarchar(max) not null default(''),
	description nvarchar(max) not null default(''),
	active bit not null default(1),
	perunitfees float not null default(0),
	flatfees float not null default(0)
)
go

create table insurances
(
	id int IDENTITY not null primary key nonclustered with fillfactor = 50,
	name nvarchar(max) not null default(''),
	description nvarchar(max) not null default(''),
	active bit not null default(1),
	perunitfees float not null default(0),
	flatfees float not null default(0)
)
go

create table providers
(
	id int IDENTITY not null primary key nonclustered with fillfactor = 50,
	name nvarchar(max) not null default(''),
	description nvarchar(max) not null default(''),
	active bit not null default(1),
	perunitfees float not null default(0),
	flatfees float not null default(0)
)
go

create table patients
(
	pid uniqueidentifier not null default ('00000000-0000-0000-0000-000000000000') primary key nonclustered with fillfactor = 50,
	mrn nvarchar(50)  not null default(''),
	cid int not null foreign key references clients(id),
	dob date not null default('01/01/1800'),
	firstname nvarchar(max) not null default(''),
	lastname nvarchar(max) not null default(''),
	email nvarchar(max) not null default(''),
	phone1 nvarchar(max) not null default(''),
	phone2 nvarchar(max) not null default(''),
	address1  nvarchar(max) not null default(''),
	address2  nvarchar(max) not null default(''),
	address3  nvarchar(max) not null default(''),
	guardiandetails nvarchar(max) not null default(''),
	defaultaddresstype int not null default(1),
	insuranceid int not null foreign key references insurances(id),
	notes nvarchar(max) not null default(''),
	ordercompleted int not null default(0),
	orderinprogress int not null default(0),

	constraint patients_unqiue_mrncid UNIQUE(mrn,cid)
)
create index idx_patients_mrn_cid ON patients (mrn,cid)
create index idx_patients_cid ON patients (cid)
go


create table orders
(
	oid uniqueidentifier not null default ('00000000-0000-0000-0000-000000000000') primary key nonclustered with fillfactor = 50,
	pid uniqueidentifier not null default ('00000000-0000-0000-0000-000000000000') foreign key references patients(pid),
	ordernumber nvarchar(50) not null default (''),
	dos datetime not null default('01/01/1900'),
	confirmeddos datetime not null default('01/01/1900'),
	confirmeddeliverydate datetime not null default('01/01/1900'),
	estimateddeliverydate datetime not null default(''),
	orderstatus int not null default(1),
	deliveryaddress nvarchar(max) not null default(''),
	nextcalldate datetime not null default('01/01/1900'),
	dateordered datetime not null default('01/01/1900'),
	prophyorprn nvarchar(max) not null default(''),
	drugid int not null foreign key references drugs(id),
	manufacturerid int not null foreign key references manufacturers(id),
	insuranceid int not null foreign key references insurances(id),
	confirmationnumber nvarchar(max) not null default (''),
	totalunitprescribed float not null default(0),
	dosecount float not null default(0),
	cogperunit float not null default(0),
	billperunit float not null default(0),
	providerid int not null foreign key references providers(id),
	acceptableoutdatesid int not null default (0),
	id340B int not null default (0),
	createdby int not null foreign key references users(id),
	createddttm datetime not null default('01/01/1900'),
	lastupdatedby int not null foreign key references users(id),
	lastupdatedttm datetime not null default(GETDATE()),	
	otherdetails nvarchar(max) not null default(''),
)
create index idx_orders_ordernumber ON orders (ordernumber)
go

create table ordertasks
(
	oid uniqueidentifier not null default ('00000000-0000-0000-0000-000000000000') foreign key references orders(oid),
	taskcode nvarchar(50) not null default(''), 
	idx int not null default(0),
	taskstatus int not null,
	notes nvarchar(max) not null default(''),
	createddate datetime not null,
	lastupdatedby int not null foreign key references users(id),
	lastupdatedttm datetime not null

	CONSTRAINT pk_ordertasks primary key NONCLUSTERED (oid,taskcode) with fillfactor = 50
)
go

create table orderassay
(
	oid uniqueidentifier not null default ('00000000-0000-0000-0000-000000000000') foreign key references orders(oid),
	assayid nvarchar(10) not null default (''),
	assay float not null default(0),
	ndc nvarchar(max) not null default(''),
	qty float not null default(0),
	expdate datetime not null default('01/01/1900'),
	lot nvarchar(max) not null default(''),
	rxnumber nvarchar(max) not null default(''),

	CONSTRAINT pk_orderplacements primary key NONCLUSTERED (oid,assayid) with fillfactor = 50
)

create table batchpayments
(
	bid uniqueidentifier not null default ('00000000-0000-0000-0000-000000000000') primary key nonclustered with fillfactor = 50,
	cid int not null foreign key references clients(id),
	name nvarchar(50) not null default(''),
	notes nvarchar(max) not null default(''),
	emaildate datetime not null default('01/01/1900'),
	reportdate datetime not null default('01/01/1900'),
	createddate datetime not null default('01/01/1900'),
	createdby int not null foreign key references users(id),
	lastupdatedby int not null foreign key references users(id),
	lastupdateddate datetime not null default('01/01/1900'),

	constraint batchpayments_unqiue_namecid UNIQUE(name,cid)
)
create index idx_batchpayments_refernce ON batchpayments (name,cid)
create index idx_batchpayments_cid ON batchpayments (cid)
go


create table orderpaymentsinsurance
(
	oid uniqueidentifier not null default ('00000000-0000-0000-0000-000000000000') foreign key references orders(oid),
	bid uniqueidentifier not null default ('00000000-0000-0000-0000-000000000000') foreign key references batchpayments(bid),
	rxnumber nvarchar(100) not null default(''),
	chequenumber nvarchar(100) not null default(''), 
	chequedate datetime not null default('01//01/1900'),
	amount float not null default(0),
	notes nvarchar(max) not null default(''),
	paymenttype int not null default(0),
	pap bit not null default(0),
	path nvarchar(max) not null default(''),
	filename nvarchar(max) not null default(''),

	CONSTRAINT pk_orderpaymentsinsurance primary key NONCLUSTERED (oid,bid,chequenumber) with fillfactor = 50
)
go

create table orderpaymentsclient
(
	oid uniqueidentifier not null default ('00000000-0000-0000-0000-000000000000') foreign key references orders(oid),
	chequenumber nvarchar(100) not null default(''), 
	chequedate datetime not null default('01//01/1900'),
	amount float not null default(0),
	notes nvarchar(max) not null default(''),
	path nvarchar(max) not null default(''),
	filename nvarchar(max) not null default(''),

	CONSTRAINT pk_orderpaymentsclient primary key NONCLUSTERED (oid,chequenumber) with fillfactor = 50
)
go


insert into clients(name, description, active) values('UC Davis','',1);
insert into clients(name, description, active) values('UC LA','',0);
insert into users values('admin','admin','admin@admin.com','admin','');
insert into manufacturers(name, description, active) values('---','---',1);
insert into providers(name, description, active) values('---','---',1);

go

