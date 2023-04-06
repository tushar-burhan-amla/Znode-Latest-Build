insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 65,'Date','ExpirationDate should be greater than CurrentDate',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 65)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 66,'Date','Amount should be greater than RemainingAmount',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 66)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 67,'Other','InValid UserName',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 67)

update ZnodeMessage set messagename = 'The value should be a future date post Start Date'
where MessageCode = 65

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 68,'Other','The value should be either True or False',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 68)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 69,'Number','The value should be the same as the Voucher Amount.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 69)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 70,'Number','The value is not associated with any existing Store.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 70)
go
insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 71,'Text','The value should consist of 10 digits in combinations of upper case alphabets and numbers.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 71)

--dt 23-09-2020 ZPD-12366
INSERT INTO [dbo].[ZnodeMessage]
          ([MessageCode]
          ,[MessageType]
          ,[MessageName]
          ,[CreatedBy]
          ,[CreatedDate]
          ,[ModifiedBy]
          ,[ModifiedDate])
select 88,'Text','The Account cannot be changed.',2,getdate(),2,getdate()
where not exists(select * from [ZnodeMessage] where [MessageCode] = 88 )

INSERT INTO [dbo].[ZnodeMessage]
          ([MessageCode]
          ,[MessageType]
          ,[MessageName]
          ,[CreatedBy]
          ,[CreatedDate]
          ,[ModifiedBy]
          ,[ModifiedDate])
select 89,'Text','The added value should be the same as the existing value because this value cannot be updated.',2,getdate(),2,getdate()
where not exists(select * from [ZnodeMessage] where [MessageCode] = 89 )
	
INSERT INTO [dbo].[ZnodeMessage]
          ([MessageCode]
          ,[MessageType]
          ,[MessageName]
          ,[CreatedBy]
          ,[CreatedDate]
          ,[ModifiedBy]
          ,[ModifiedDate])
select 90,'Text','Country should belong to the selected Store.',2,getdate(),2,getdate()
where not exists(select * from [ZnodeMessage] where [MessageCode] = 90 )
	select * from znodeportal

INSERT INTO [dbo].[ZnodeMessage]
          ([MessageCode]
          ,[MessageType]
          ,[MessageName]
          ,[CreatedBy]
          ,[CreatedDate]
          ,[ModifiedBy]
          ,[ModifiedDate])
select 91,'Text','The default address cannot be marked as non-default.',2,getdate(),2,getdate()
where not exists(select * from [ZnodeMessage] where [MessageCode] = 91 )


delete from ZnodeImportUpdatableColumns
where ImportHeadId = (SELECT TOP 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName in ('IsDefaultBilling','IsDefaultShipping')

--dt 30-09-2020 ZPD-12523
INSERT INTO [dbo].[ZnodeMessage]
          ([MessageCode]
          ,[MessageType]
          ,[MessageName]
          ,[CreatedBy]
          ,[CreatedDate]
          ,[ModifiedBy]
          ,[ModifiedDate])
select 92,'Text','Role Name cannot be updated.',2,getdate(),2,getdate()
where not exists(select * from [ZnodeMessage] where [MessageCode] = 92 )

Update ZnodeCMSMessage set Message = replace(Message,'2019','2020')
where Message like '%copyright%' and Message like '%2019%'

Update ZnodeCMSMessage set Message = replace(Message,'2020','2021')
where Message like '%copyright%' and Message like '%2020%'

Update ZnodeCMSMessage set Message = replace(Message,'2019','2021')
where Message like '%copyright%' and Message like '%2019%'

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 93,'Other','Import failed due to database error. Please check Application Logs for more information.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 93)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 94,'Text','Input must include a SKU value that belongs to a product.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 94)

update ZnodeMessage
set MessageName ='Input must include an integer value between 1 and 99999'
where MessageCode = 16

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 95,'Text','Input must include any existing Catalog Code as the value.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 95)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 96,'Text','Input must not include any existing Synonym Code as the value.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 96)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 97,'Text','Input must not exceed the maximum value limit of 20.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 97)



update ZnodeMessage set MessageName = 'Input must be a positive numeric value' where MessageCode = 4

update ZnodeMessage set MessageName = 'Input must be a positive numeric value greater than 0' where MessageCode = 17

update ZnodeMessage set MessageName = 'Input must be in alphanumeric format and must start with an alphabet.' where MessageCode = 52

update ZnodeMessage set MessageName = 'Input value must either be True or False.' where MessageCode = 68

update ZnodeMessage set MessageName = 'Default input cannot be marked as non-default.' where MessageCode = 91

update ZnodeMessage set MessageName = 'Input value must not exceed the maximum character limit of 100.' where MessageCode = 78

update ZnodeMessage set MessageName = 'Input must include a value in alphanumeric format.' where MessageCode = 79

update ZnodeMessage set MessageName = 'Input must be associated with any existing Catalog.' where MessageCode = 80

update ZnodeMessage set MessageName = 'Input must not exceed the maximum character limit of 200.' where MessageCode = 81

update ZnodeMessage set MessageName = 'Input must not exceed the maximum character limit of 300.' where MessageCode = 82

update ZnodeMessage set MessageName = 'Input is required.' where MessageCode = 84

update ZnodeMessage set MessageName = 'Input value must be 1 for new Account.' where MessageCode = 85

update ZnodeMessage set MessageName = 'Parent Account must be associated with the specified Store.' where MessageCode = 86

update ZnodeMessage set MessageName = 'Input must be available as a Code saved against any existing Parent Account.' where MessageCode = 87

update ZnodeMessage set MessageName = 'Input cannot be updated because of its unavailability in database.' where MessageCode = 19

update ZnodeMessage set MessageName = 'Duplicate records must be excluded from the import/update file.' where MessageCode = 53

update ZnodeMessage set MessageName = 'Input must include be a positive integer value of maximum 3 digits and must be greater than 0.' where MessageCode = 64

update ZnodeMessage set MessageName = 'Input must include SKU of a product that is not a simple product.' where MessageCode = 49

update ZnodeMessage set MessageName = 'Input must include SKU of a simple product as a value.' where MessageCode = 51

update ZnodeMessage set MessageName = 'Input must include a value from the defined values.' where MessageCode = 9

update ZnodeMessage set MessageName = 'Input must include a value in alphanumeric format.' where MessageCode = 50

update ZnodeMessage set MessageName = 'Input must be unique.' where MessageCode = 30

update ZnodeMessage set MessageName = 'Input must be available as a value in the specified store.' where MessageCode = 62

update ZnodeMessage set MessageName = 'Input is required.' where MessageCode = 8

update ZnodeMessage set MessageName = 'Data column must be mapped with the template.' where MessageCode = 42

update ZnodeMessage set MessageName = 'Input must include numeric value greater than 0' where MessageCode = 26

update ZnodeMessage set MessageName = 'Input date added for SKUActivationDate must come prior to the input date added for SKUExpirationDate.' where MessageCode = 39

update ZnodeMessage set MessageName = 'Input must be unique.' where MessageCode = 10

update ZnodeMessage set MessageName = 'Mandatory column must be included in the import/update file.' where MessageCode = 14

update ZnodeMessage set MessageName = 'Input is invalid.' where MessageCode = 45

update ZnodeMessage set MessageName = 'Input must be available as a Username saved against any existing User who belongs to the same Store as that of the Voucher.' where MessageCode = 67

update ZnodeMessage set MessageName = 'Input must include a value same as Voucher Amount.' where MessageCode = 69

update ZnodeMessage set MessageName = 'Input must be associated with any existing Store.' where MessageCode = 70

update ZnodeMessage set MessageName = 'Input value must be of 10 characters and should include a combination of upper case, alphabets and numeric values.' where MessageCode = 71

update ZnodeMessage set MessageName = 'Input must be available as a Code saved against any existing Parent Account.' where MessageCode = 89

update ZnodeMessage set MessageName = 'RowNumber, ProductId and CatalogId are reserved columns and must be excluded from the import file.' where MessageCode = 43

update ZnodeMessage set MessageName = 'Input must be a numeric value.' where MessageCode = 2

update ZnodeMessage set MessageName = 'Input must be an integer value between 1 and 99999' where MessageCode = 16

update ZnodeMessage set MessageName = 'Input must include a valid date.' where MessageCode = 5

update ZnodeMessage set MessageName = 'Input must include a value in Date format and must be a value post Start Date.' where MessageCode = 65

update ZnodeMessage set MessageName = 'The attribute is not associated with the selected Attribute Family' where MessageCode = 1

update ZnodeMessage set MessageName = 'Input must include the same value associated with the User since the associated account cannot be changed.' where MessageCode = 88

update ZnodeMessage set MessageName = 'Input should be available as Account Code with any existing Account.' where MessageCode = 73

update ZnodeMessage set MessageName = 'Input must include either User or Administrator or Manager as a value.' where MessageCode = 74

update ZnodeMessage set MessageName = 'Include Account Code first to add a Role Name.' where MessageCode = 75

update ZnodeMessage set MessageName = 'Input should be available as Department Name with any existing Account.' where MessageCode = 76

update ZnodeMessage set MessageName = 'Account and Customer should belong to the same Store therefore input should be available as Account Code with any existing Account of the respective Store.' where MessageCode = 77
go
insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 98,'Text','Input must be available as a SKU saved against any existing product.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 98)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 99,'Text','Input must be available as a Group Name saved against any existing Add-on Group.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 99)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 101,'Text','Input must match one of the values of the Configurable Attribute which is used to create a Configurable product.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 101)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 102,'Text','Input value must be in alphanumeric format without spaces.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 102)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 103,'Text','Input must be available as any predefined value.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 103)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 104,'Text','Input value must be in email format i.e. _@__.__',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 104)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 105,'Text','Input must be available as a Code saved against any existing Catalog.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 105)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 106,'Text','Input must be available as a Username saved against any existing User.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 106)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 107,'Text','Input must be available as a Code saved against any existing Warehouse.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 107)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 108,'Text','Input must include a positive integer value greater than 0.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 108)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 109,'Text','Input must include a positive numeric value greater than 0.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 109)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 110,'Text','Input must be available as a Code saved against any existing Store.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 110)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 111,'Text','Input must include a positive numeric value between 0 and 6 which can further be used to identify the rounding digit.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 111)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 112,'Text','Input must be available as a State Code value in the database.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 112)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 113,'Text','Tier Price must be included to import a Tier Quantity.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 113)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 114,'Text','Tier Quantity must be included to import a Tier Price.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 114)



--Ticket No:- ZPD-18738
Update znodemessage set MessageName='Input must be available as Department Name saved against the Account.'
where MessageName='Input should be available as Department Name with any existing Account.'

--Ticket No:- ZPD-18734
Insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 118,'Text','Input must be available as a Value Code saved against Highlights Attribute that has not been used to create any other Highlight.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 118) 

--Ticket No:- ZPD-18729
Insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 117,'Text','Input must be available as a Code saved against any existing Attribute.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 117) 

--Ticket No:- ZPD-18719
Update znodemessage set MessageName='Account Code must be included to import/update a Role Name.'
where MessageName= 'Include Account Code first to add a Role Name.'
--Ticket No:-ZPD-18691
Update znodemessage set MessageName='Input value must not exceed the maximum character limit of 300.'
where MessageName= 'Input must not exceed the maximum character limit of 300.'
--Ticket No:-ZPD-18690
Update znodemessage set MessageName='Input value must not exceed the maximum character limit of 200.'
where MessageName= 'Input must not exceed the maximum character limit of 200.'
--Ticket No:-ZPD-18694
Insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 115,'Text','Input must be an integer value between 1 and 999.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 115) 
--Ticket No:-ZPD-18689
Update znodemessage set MessageName='Input must be available as a Code saved against any existing Catalog.'
where MessageName= 'Input must be associated with any existing Catalog.'
--Ticket No:-ZPD-18688
Update znodemessage set MessageName='Input value must be in alphanumeric format without spaces.'
where MessageName= 'Input must include a value in alphanumeric format.'

-- TICKET NUMBER:-ZPD-18285
--START
INSERT INTO ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 116,'Text','Input must include any State that is associated with the selected Country.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 116)  


UPDATE ZnodeMessage set MessageName = 'Input must include any Country that is associated with the Store.'
WHERE MessageName = 'Country should belong to the selected Store.'

UPDATE znodemessage set MessageName='Input value must be in alphanumeric format without spaces.'
WHERE MessageName='Input must include a value in alphanumeric format.'


INSERT INTO ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 119,'Text','Input value is required in alphanumeric format without spaces and must start with an alphabet.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 119)
--END
--Ticket no:- ZPD-19556
Update znodemessage set MessageName='Input must include positive numeric value greater than 0'
where MessageName='Input must be a positive numeric value'

update ZnodeMessage
set MessageName ='Input must be available as Code saved against any existing Account.'
where MessageName = 'Input should be available as Account Code with any existing Account.'

update ZnodeMessage
set MessageName ='Account and Customer should belong to the same Store therefore input must be available as Code saved against any existing Account of the respective Store.'
where MessageName = 'Account and Customer should belong to the same Store therefore input should be available as Account Code with any existing Account of the respective Store.'

update ZnodeMessage
set MessageName ='Input value is required in alphanumeric format without spaces'
where MessageName = 'Input value must be in alphanumeric format without spaces.'
--Ticket no:- ZPD-19519
INSERT INTO ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 120,'Text','Input must be available as a value in any existing Store.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 120)

--ZPD-8428 Dt.26-May-2022
insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 121,'Text','Input must include any Brand Code that is associated with the system defined Brand product attribute as an attribute value and is not used to create any other Brand.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 121)
insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 122,'Text','Invalid input, to associate/disassociate parent category, the input must only be added to the Category Code and Parent Code should be empty.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 122)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 123,'Text','Invalid input, the same category cannot be added as the parent as well as the child category.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 123)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 124,'Text','Invalid input, multiple separators without category code is not allowed.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 124)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 125,'Text','Input value must either be Add or Delete.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 125)
GO
insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 126,'Text','Invalid input, parent category association is required.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 126)
GO
insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 127,'Text','Input must be available as a Category Code saved against any existing Category.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 127)

go
insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 128,'Text','Invalid input, multiple category codes are not allowed in Category Code column.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 128)
--Ticket No-ZPD-20207
Update znodemessage set MessageName='Input value is required in alphanumeric format without spaces.'
where MessageName= 'Input value is required in alphanumeric format without spaces'

Update znodemessage set MessageName='Include Account Code first to add a Role Name.'
where MessageName= 'Account Code must be included to import/update a Role Name.'

UPDATE znodemessage
SET MessageName='Input value is required in alphanumeric format without spaces and must start with an alphabet.'
WHERE MessageName= 'Input value is required in alphanumeric format without spaces.'

UPDATE znodemessage set MessageName='Input value is required in alphanumeric format without spaces.'
WHERE MessageCode=79
--Ticket ZPD:-20383
UPDATE ZnodeMessage
SET MessageName='Input must not exceed the maximum character limit of 100.'
WHERE  MessageName='Input value must not exceed the maximum character limit of 100.'

INSERT INTO ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 129,'Text','Input must not exceed the maximum character limit of',2,getdate(),2,getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 129)
go
insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 130,'Text','Invalid input, parent category to child category association is required.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 130)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 131,'Text','Invalid input, to associate/disassociate parent category, the input must only be added to the Category Code and Parent Code should be empty.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 131)
GO
insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 132,'Text','Catalog association already exists hence no new associations are saved in the Database.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 132)
GO


INSERT INTO ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 133,'Text','Input value is required in alphanumeric format without spaces.Hyphen or Underscore can be used.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 133)

GO
--ZPD-19485 Dt. 22-June-2022
INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 134,'Other','Input must be available as a Username saved against any existing User who belongs to the same Store as that of the Country.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 134);

--Ticket -ZPD-20312
Insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 135,'Text','Input must not exceed the maximum character limit of 600.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 135)

--ZPD-21754 Dt.24-Aug-2022
-- ZnodeMessage
INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 136,'Text','Input must not exceed the maximum value limit of 100.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 136)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 137,'Text','Only 2 digits are allowed after the decimal point.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 137)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 138,'Text','Input must not be available as a Code saved against any existing Coupon.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 138)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 139,'Text','Input value length must be between 1-20.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 139)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 140,'Text','Input must be an integer value between 1 and 9999.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 140)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 141,'Text','No decimal point is allowed.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 141)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 142,'Text','Input value must either be True/1/Yes or False/0/No.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 142)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 143,'Text','Input must be available as a Code saved against any existing Brand associated to the selected Store.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 143)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 144,'Text','Input must be available as a Code saved against any existing catalog associated to the selected Store.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 144)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 145,'Text','Input must be available as a Code saved against any existing Category associated to the selected Store.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 145)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 146,'Text','Input must be available as a Code saved against any existing Product associated to the selected Store.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 146)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 147,'Text','Multiple SKU’s cannot be entered.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 147)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 148,'Text','Input must be any existing Shipping method associated to the selected Store.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 148)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 149,'Text','The field Display Order must be a number.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 149)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 150,'Text','Invalid Date: The entered date precedes the current date.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 150)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 151,'Text','Invaid Date: The entered date exceeds the end date.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 151)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 152,'Text','Invalid Date: The entered date precedes the start date.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 152)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 153,'Text','Input must be available as a Code saved against any existing Profile.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 153)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 154,'Text','Input value cannot be set for this column as the value is not set to TRUE/1/Yes in the Requires a coupon field.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 154)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 155,'Text','Only 1 decimal point is allowed.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 155)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 156,'Text','Invalid Product Code (SKU).',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 156)

--ZPD-22242
INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 157,'Text','Input must be available as a Store Code saved against the specified promotional code.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 157)

--ZPD-22249
INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 158,'Text','Only positive decimal numbers are allowed in the discount amount field.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 158)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 159,'Text','Only positive numbers are allowed in the minimum quantity field.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 159)

INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 160,'Text','Only positive decimal numbers are allowed in the minimum order amount field',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 160)

--ZPD-22337
INSERT INTO ZnodeMessage (MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 161,'Text','Discount Amount must be within a range of 0.01 - 9999999.',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeMessage WHERE MessageCode = 161)
