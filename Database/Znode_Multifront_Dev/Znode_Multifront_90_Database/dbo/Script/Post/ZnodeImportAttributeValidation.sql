----ZPD-7111 --> ZPD-8887
insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','CostPrice',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Pricing'),0,'Yes/No','AllowNegative',
null,'false','',null,2,getdate(),2,getdate(),11
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'CostPrice' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Pricing')
	  and ValidationName = 'AllowNegative')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','CostPrice',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Pricing'),0,'Yes/No','AllowDecimals',
null,'false','',null,2,getdate(),2,getdate(),11
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'CostPrice' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Pricing')
	  and ValidationName = 'AllowDecimals')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','CostPrice',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Pricing'),0,'Number','MinNumber',
null,'0','',null,2,getdate(),2,getdate(),11
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'CostPrice' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Pricing')
	  and ValidationName = 'MinNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','CostPrice',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Pricing'),0,'Number','MaxNumber',
null,'999999','',null,2,getdate(),2,getdate(),11
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'CostPrice' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Pricing')
	  and ValidationName = 'MaxNumber')

--dt 31-07-2020
update ZnodeImportAttributeValidation set IsRequired = 1 
where AttributeCode = 'IsActive' and ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')

update  ZnodeImportAttributeValidation set IsRequired = 0
where AttributeCode = 'UserName' and ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')

delete from ZnodeImportAttributeValidation where importheadid = (select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'VoucherAmount' and AttributeTypeName = 'Number'

delete from ZnodeImportAttributeValidation where importheadid = (select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'RemainingAmount' and AttributeTypeName = 'Number'

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','VoucherAmount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),null
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'VoucherAmount' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','RemainingAmount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),null
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'RemainingAmount' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'RegularExpression')

------------ZPD-13943
insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select 'Number','EmailOptIn',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Yes/No','AllowNegative',	NULL,	'false',' '	,	NULL,	2,getdate(),2,getdate(),15
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'EmailOptIn' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'AllowNegative')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select'Number','EmailOptIn',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Yes/No','AllowDecimals',	NULL,	'false',' ',		NULL,	2,getdate(),2,getdate(),15
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'EmailOptIn' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'AllowDecimals')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select'Number','EmailOptIn',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Number','MinNumber',	NULL,	0	,'',	NULL,	2,getdate(),2,getdate(),15
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'EmailOptIn' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'MinNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)  
select 'Number','EmailOptIn',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Number','MaxNumber',	NULL,	999999,'',		NULL,	2,getdate(),2,getdate(),15
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'EmailOptIn' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'MaxNumber')


insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select 'Text','PerOrderAnnualLimit',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Text','RegularExpression',	NULL,'',	'',		NULL,	2,getdate(),2,getdate(),16
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'PerOrderAnnualLimit' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)  
select 'Text','BillingAccountNumber',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Text','RegularExpression',	NULL,'',	'',		NULL,	2,getdate(),2,getdate(),17
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'BillingAccountNumber' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select 'Text','EnableUserShippingAddressSuggestion',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Text','RegularExpression',	NULL,'',	'',		NULL,	2,getdate(),2,getdate(),18
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'EnableUserShippingAddressSuggestion' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select 'Number','EnablePowerBIReportOnWebStore',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Yes/No','AllowNegative',	NULL,	'false'	,' '	,	NULL,	2,getdate(),2,getdate(),19
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'EnablePowerBIReportOnWebStore' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'AllowNegative')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)   
select 'Number','EnablePowerBIReportOnWebStore',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Yes/No','AllowDecimals',	NULL,	'false',' '	,		NULL,	2,getdate(),2,getdate(),19
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'EnablePowerBIReportOnWebStore' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'AllowDecimals')
insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select 'Number','EnablePowerBIReportOnWebStore',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Number','MinNumber',	NULL,	0	,' '	,	NULL,	2,getdate(),2,getdate(),19
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'EnablePowerBIReportOnWebStore' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'MinNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)  
select 'Number','EnablePowerBIReportOnWebStore',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Number','MaxNumber',	NULL,	999999,' '	,		NULL,	2,getdate(),2,getdate(),19
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'EnablePowerBIReportOnWebStore' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'MaxNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select 'Text','PerOrderLimit',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),	0,	'Text','RegularExpression',	NULL,'',	'',		NULL,	2,getdate(),2,getdate(),20
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'PerOrderLimit' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'RegularExpression')


update ZnodeImportAttributeValidation
set IsRequired = 0
where AttributeCode ='IsActive'
and ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'CustomerAddress')	

DECLARE @SequenceNumber INT
SELECT @SequenceNumber = max(sequenceNumber) FROM ZnodeImportAttributeValidation 
WHERE importheadid = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer')

INSERT INTO ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
SELECT 'Text','Custom1',	(SELECT Top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer'),	0,	'Text','RegularExpression',	NULL,'',	'',		NULL,	2,getdate(),2,getdate(),@SequenceNumber+1
WHERE NOT EXISTS(SELECT * from ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' and AttributeCode = 'Custom1' 
      and ControlName = 'Text' and ImportHeadId=(SELECT Top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
	  and ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
SELECT 'Text','Custom2',	(SELECT Top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer'),	0,	'Text','RegularExpression',	NULL,'',	'',		NULL,	2,getdate(),2,getdate(),@SequenceNumber+2
WHERE NOT EXISTS(SELECT * from ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' and AttributeCode = 'Custom2' 
      and ControlName = 'Text' and ImportHeadId=(SELECT Top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
	  and ValidationName = 'RegularExpression')


INSERT INTO ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
SELECT 'Text','Custom3',	(SELECT Top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer'),	0,	'Text','RegularExpression',	NULL,'',	'',		NULL,	2,getdate(),2,getdate(),@SequenceNumber+3
WHERE NOT EXISTS(SELECT * from ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' and AttributeCode = 'Custom3' 
      and ControlName = 'Text' and ImportHeadId=(SELECT Top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
	  and ValidationName = 'RegularExpression')


INSERT INTO ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
SELECT 'Text','Custom4',	(SELECT Top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer'),	0,	'Text','RegularExpression',	NULL,'',	'',		NULL,	2,getdate(),2,getdate(),@SequenceNumber+4
WHERE NOT EXISTS(SELECT * from ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' and AttributeCode = 'Custom4' 
      and ControlName = 'Text' and ImportHeadId=(SELECT Top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
	  and ValidationName = 'RegularExpression')


INSERT INTO ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
SELECT 'Text','Custom5',	(SELECT Top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer'),	0,	'Text','RegularExpression',	NULL,'',	'',		NULL,	2,getdate(),2,getdate(),@SequenceNumber+5
WHERE NOT EXISTS(SELECT * from ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' and AttributeCode = 'Custom5' 
      and ControlName = 'Text' and ImportHeadId=(SELECT Top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
	  and ValidationName = 'RegularExpression')

---ZPD-14698

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select 'Number','Quantity',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation'),	0,	'Yes/No','AllowNegative',	NULL,	'false'	,' '	,	NULL,	2,getdate(),2,getdate(),NULL
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'Quantity' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation')
	  and ValidationName = 'AllowNegative')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)   
select 'Number','Quantity',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation'),	0,	'Yes/No','AllowDecimals',	NULL,	'false',' '	,		NULL,	2,getdate(),2,getdate(),NULL
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'Quantity' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation')
	  and ValidationName = 'AllowDecimals')
insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select 'Number','Quantity',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation'),	0,	'Number','MinNumber',	NULL,	0	,' '	,	NULL,	2,getdate(),2,getdate(),NULL
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'Quantity' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation')
	  and ValidationName = 'MinNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)  
select 'Number','Quantity',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation'),	0,	'Number','MaxNumber',	NULL,	99999,' '	,		NULL,	2,getdate(),2,getdate(),NULL
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'Quantity' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation')
	  and ValidationName = 'MaxNumber')


delete from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'IsDefault' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation')
	  and ValidationName = 'RegularExpression'


insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select 'Number','IsDefault',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation'),	0,	'Yes/No','AllowNegative',	NULL,	'false'	,' '	,	NULL,	2,getdate(),2,getdate(),NULL
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsDefault' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation')
	  and ValidationName = 'AllowNegative')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)   
select 'Number','IsDefault',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation'),	0,	'Yes/No','AllowDecimals',	NULL,	'false',' '	,		NULL,	2,getdate(),2,getdate(),NULL
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsDefault' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation')
	  and ValidationName = 'AllowDecimals')
insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber) 
select 'Number','IsDefault',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation'),	0,	'Number','MinNumber',	NULL,	0	,' '	,	NULL,	2,getdate(),2,getdate(),NULL
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsDefault' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation')
	  and ValidationName = 'MinNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)  
select 'Number','IsDefault',	(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation'),	0,	'Number','MaxNumber',	NULL,	2,' '	,		NULL,	2,getdate(),2,getdate(),NULL
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsDefault' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation')
	  and ValidationName = 'MaxNumber')


update ZnodeImportAttributeValidation
set ValidationValue = 99999
where AttributeTypeName ='Number' and AttributeCode = 'Quantity' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation')
	  and ValidationName = 'MaxNumber'


DECLARE @SequenceNumber1 INT=(SELECT MAX(SequenceNumber) FROM ZnodeImportAttributeValidation 
Where  ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer'))
Update ZnodeImportAttributeValidation SET SequenceNumber = @SequenceNumber1+1   
Where  AttributeCode ='PerOrderAnnualLimit' and ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
and SequenceNumber is null

set @SequenceNumber1 =(SELECT MAX(SequenceNumber) FROM ZnodeImportAttributeValidation 
Where  ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer'))
Update ZnodeImportAttributeValidation SET SequenceNumber =@SequenceNumber1+1  
Where  AttributeCode ='BillingAccountNumber' and ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
and SequenceNumber is null

set @SequenceNumber1 =(SELECT MAX(SequenceNumber) FROM ZnodeImportAttributeValidation 
Where  ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer'))
Update ZnodeImportAttributeValidation SET SequenceNumber = @SequenceNumber1+1
Where  AttributeCode ='EnableUserShippingAddressSuggestion' and ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
and SequenceNumber is null 

Update ZnodeImportAttributeValidation SET SequenceNumber = @SequenceNumber1+1 
Where  AttributeCode ='EnablePowerBIReportOnWebStore' and ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
and SequenceNumber is null and ControlName = 'Yes/No'  and ValidationName = 'AllowNegative'

Update ZnodeImportAttributeValidation SET SequenceNumber = @SequenceNumber1+1 
Where  AttributeCode ='EnablePowerBIReportOnWebStore' and ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
and SequenceNumber is null and ControlName = 'Yes/No'  and ValidationName = 'AllowDecimals'

Update ZnodeImportAttributeValidation SET SequenceNumber = @SequenceNumber1+1 
Where  AttributeCode ='EnablePowerBIReportOnWebStore' and ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
and SequenceNumber is null and ControlName = 'Number'  and ValidationName = 'MinNumber'

Update ZnodeImportAttributeValidation SET SequenceNumber = @SequenceNumber1+1 
Where  AttributeCode ='EnablePowerBIReportOnWebStore' and ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
and SequenceNumber is null and ControlName = 'Number'  and ValidationName = 'MaxNumber'

set @SequenceNumber1 =(SELECT MAX(SequenceNumber) FROM ZnodeImportAttributeValidation 
Where  ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer'))
Update ZnodeImportAttributeValidation SET SequenceNumber = @SequenceNumber1+1 
Where  AttributeCode ='PerOrderLimit' and ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead WHERE Name = 'Customer')
and SequenceNumber is null

--ZPD-ZPD-16959
INSERT [dbo].[ZnodeImportAttributeDefaultValue] ([ImportAttributeType], [TargetAttributeCode], [AllowAttributeValue],
[ReplacedAttributeValue], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate])
SELECT N'Category', N'HideCategoryonMenu', N'1,true,yes', N'true', 1, 2, GETDATE(), 
	2, GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeDefaultValue WHERE [ImportAttributeType] ='Category' and [TargetAttributeCode] = 'HideCategoryonMenu' and [AllowAttributeValue] = '1,true,yes')
GO
INSERT [dbo].[ZnodeImportAttributeDefaultValue] ([ImportAttributeType], [TargetAttributeCode], [AllowAttributeValue], [ReplacedAttributeValue], [IsActive], [CreatedBy], 
[CreatedDate], [ModifiedBy], [ModifiedDate]) 
select  N'Category', N'HideCategoryonMenu', N'0,false,no', N'false', 1, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeDefaultValue WHERE [ImportAttributeType] ='Category' and [TargetAttributeCode] = 'HideCategoryonMenu' and [AllowAttributeValue] = '0,false,no')

--ZPD-19062 & ZPD-19098 Dt.22-April-2022
DELETE FROM ZnodeImportAttributeValidation
WHERE ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer')
	AND AttributeCode IN ('EmailOptIn','IsActive')
	AND (SELECT COUNT(1) FROM ZnodeImportAttributeValidation
		WHERE AttributeCode IN ('EmailOptIn','IsActive')
			AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer'))>2
		
INSERT INTO ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
	,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','EmailOptIn',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer'),0,'Text','RegularExpression',
	NULL,'','',NULL,2,GETDATE(),2,GETDATE(),7
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'EmailOptIn' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer')
	  AND ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
	,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','IsActive',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer'),1,'Text','RegularExpression',
	NULL,'','',NULL,2,GETDATE(),2,GETDATE(),8
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'IsActive' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer')
	  AND ValidationName = 'RegularExpression')

UPDATE ZnodeImportAttributeValidation
SET SequenceNumber=SequenceNumber+1
WHERE SequenceNumber>7 AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer')
	AND NOT EXISTS (SELECT * FROM ZnodeImportAttributeValidation WHERE AttributeCode='SMSOptIn'
		AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer'))

INSERT INTO ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
	,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','SMSOptIn',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer'),0,'Text','RegularExpression',
	NULL,'','',NULL,2,GETDATE(),2,GETDATE(),8
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'SMSOptIn' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer')
	  AND ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate'),
'SMSOptIn','SMSOptIn',0,0,0,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
	WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate') 
		AND SourceColumnName ='SMSOptIn')

--ZPD-19644 Dt.20-May-2022
UPDATE ZnodeImportAttributeValidation
SET SequenceNumber=9
WHERE ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer')
	AND AttributeCode='IsActive' AND SequenceNumber=8

--ZPD-8428 Dt.26-May-2022
insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','BrandCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),1
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'BrandCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','IsActive',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),2
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'IsActive' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','BrandDescription',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'BrandDescription' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','SEOKeyword',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),4
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'SEOKeyword' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','SEODescription',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),5
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'SEODescription' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','BrandLogo',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),6
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'BrandLogo' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','SEOTitle',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),7
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'SEOTitle' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','SEOFriendlyPageName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),8
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'SEOFriendlyPageName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','URLKey',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),9
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'URLKey' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','Custom1',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),10
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'Custom1' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','Custom2',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),11
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'Custom2' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','Custom3',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),12
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'Custom3' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','Custom4',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),13
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'Custom4' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','Custom5',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),14
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'Custom5' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'RegularExpression')

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=100, ValidationName = 'MaxCharacters' , ControlName = 'Number'
WHERE AttributeCode='AttributeName' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='ProductAttribute' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=100, ValidationName = 'MaxCharacters' , ControlName = 'Number'
WHERE AttributeCode='Attributecode' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='ProductAttribute' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=100, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode='VoucherName' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Voucher' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=200, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('ProfileName','DepartmentName','AccountCode') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Customer' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=300, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('Address1','Address2') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='CustomerAddress' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=600, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('DisplayName','CompanyName') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='CustomerAddress' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=100, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('FirstName','LastName') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Customer' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=30, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='PhoneNumber' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Customer' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=300, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('FirstName','LastName') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='CustomerAddress' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=200, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('Address1','Address2') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='CustomerAddress' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=100, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('CityName','PostalCode') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='CustomerAddress' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=30, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='PhoneNumber' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='CustomerAddress' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=200, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='CanonicalURL' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='SEODetails' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=50, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='RobotTag' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='SEODetails' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=300, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('HighlightCode','ImageAltTag','HighlightName') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Highlight' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=300, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='AddonGroupName' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='AddonAssociation' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=300, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('AttributeCode','AttributeDefaultValueCode') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='AttributeDefaultValue' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=200, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('AccountCode','AccountName','AddressName','CompanyName','Address1','Address2') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Account' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=300, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='IsBidirectional' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Synonyms' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=200, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='HighlightType' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Highlight' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=200, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('ChildSKU','ParentSKU') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='ProductAssociation' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=200, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('CityName','CountyCode','StateCode') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='ZipCode' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=50, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode IN ('CityType','ZIPType','CountyFIPS','StateFIPS','MSACode','TimeZone') AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='ZipCode' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=1, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='DST' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='ZipCode' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)


UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=300, ValidationName = 'MaxCharacters',ControlName = 'Number'  
WHERE AttributeCode ='ParentCode' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='CatalogCategoryAssociation' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)


UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=100, ValidationName = 'MaxCharacters',ControlName = 'Number'  
WHERE AttributeCode ='CategoryCode' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='CatalogCategoryAssociation' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=100, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='ImportType' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='SEODetails' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=2000, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='Code' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='SEODetails' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=50, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='IsActive' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='SEODetails' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=200, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='DisplayName' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='CustomerAddress' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)


UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=300, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode = 'CompanyName' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='CustomerAddress' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

--ZPD-20693 Dt.23-June-2022
DELETE A 
FROM 
(
SELECT *, ROW_NUMBER() OVER (PARTITION BY AttributeCode,ImportHeadId,AttributeTypeName,ControlName,ValidationName ORDER BY CreatedDate ASC) As Rn
FROM ZnodeImportAttributeValidation 
) A WHERE A.Rn<>1


INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','ProfileName',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer'),0,'Number','MaxCharacters',
	NULL,'200','',NULL,2,GETDATE(),2,GETDATE(),12
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'ProfileName'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','DepartmentName',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer'),0,'Number','MaxCharacters',
	NULL,'200','',NULL,2,GETDATE(),2,GETDATE(),14
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'DepartmentName'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','AccountCode',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer'),0,'Number','MaxCharacters',
	NULL,'200','',NULL,2,GETDATE(),2,GETDATE(),13
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'AccountCode'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Customer')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)



INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','AccountCode',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account'),0,'Number','MaxCharacters',
	NULL,'300','',NULL,2,GETDATE(),2,GETDATE(),3
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'AccountCode'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','AccountName',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account'),0,'Number','MaxCharacters',
	NULL,'300','',NULL,2,GETDATE(),2,GETDATE(),2
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'AccountName'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','AddressName',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account'),0,'Number','MaxCharacters',
	NULL,'300','',NULL,2,GETDATE(),2,GETDATE(),6
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'AddressName'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','CompanyName',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account'),0,'Number','MaxCharacters',
	NULL,'300','',NULL,2,GETDATE(),2,GETDATE(),9
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'CompanyName'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','Address1',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account'),0,'Number','MaxCharacters',
	NULL,'300','',NULL,2,GETDATE(),2,GETDATE(),10
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'Address1'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','Address2',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account'),0,'Number','MaxCharacters',
	NULL,'300','',NULL,2,GETDATE(),2,GETDATE(),11
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'Address2'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','BrandCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),1,'Number','MaxCharacters',
null,300,'',null,2,getdate(),2,getdate(),1
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'BrandCode' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'MaxCharacters')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','SEOKeyword',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Number','MaxCharacters',
null,300,'',null,2,getdate(),2,getdate(),4
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'SEOKeyword' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'MaxCharacters')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','SEODescription',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Number','MaxCharacters',
null,300,'',null,2,getdate(),2,getdate(),5
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'SEODescription' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'MaxCharacters')

	  
insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','SEOTitle',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Number','MaxCharacters',
null,200,'',null,2,getdate(),2,getdate(),7
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'SEOTitle' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'MaxCharacters')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','URLKey',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Number','MaxCharacters',
null,200,'',null,2,getdate(),2,getdate(),9
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'URLKey' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'MaxCharacters')


--ZPD-20811

INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','FirstName',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account'),0,'Number','MaxCharacters',
	NULL,'300','',NULL,2,GETDATE(),2,GETDATE(),10
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'FirstName'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','LastName',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account'),0,'Number','MaxCharacters',
	NULL,'300','',NULL,2,GETDATE(),2,GETDATE(),11
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'LastName'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
                )


--Ticket number:-ZPD-19531
UPDATE ZnodeImportAttributeValidation 
SET ControlName='Yes/No' 
WHERE AttributeCode='Displayorder' AND ValidationName IN ('MinNumber','MaxNumber') 
AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='CategoryAssociation' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','SEOFriendlyPageName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands'),0,'Number','MaxCharacters',
null,'100','',null,2,getdate(),2,getdate(),8
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'SEOFriendlyPageName' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Brands')
      and ValidationName = 'MaxCharacters')

 INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','Phonenumber',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account'),0,'Number','MaxCharacters',
	NULL,'30','',NULL,2,GETDATE(),2,GETDATE(),16
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'Phonenumber'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Account')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','SynonymCode',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Synonyms'),0,'Number','MaxCharacters',
	NULL,'100','',NULL,2,GETDATE(),2,GETDATE(),2
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'SynonymCode'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Synonyms')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

				

UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=100, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='AccountCode'
AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Customer' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)


UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=100, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='AccountCode'
AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Account' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)


UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=100, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode ='AccountName' 
AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Account' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

INSERT INTO ZnodeImportAttributeValidation (AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,
	ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','Categorycode',
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'CategoryAssociation'),0,'Number','MaxCharacters',
	NULL,'100','',NULL,2,GETDATE(),2,GETDATE(),3
WHERE NOT EXISTS(SELECT * FROM ZnodeImportAttributeValidation 
				WHERE AttributeTypeName ='Text' AND AttributeCode = 'Categorycode'
					AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'CategoryAssociation')
					AND ControlName = 'Number' AND ValidationName = 'MaxCharacters'
				)

---Ticket No:-ZPD-21130
UPDATE ZnodeImportAttributeValidation 
SET ValidationValue=100, ValidationName = 'MaxCharacters',ControlName = 'Number'
WHERE AttributeCode='CatalogCode' AND EXISTS( SELECT TOP 1 1 FROM ZnodeImportHead B WHERE B.Name='Synonyms' AND B.ImportHeadId=ZnodeImportAttributeValidation.ImportHeadId)

--ZPD-21754 Dt.24-Aug-2022
--ZnodeImportAttributeValidation
INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','PromoCode',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),1,'Text','RegularExpression',
NULL,300,'',NULL,2,GETDATE(),2,GETDATE(),1
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'PromoCode' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','Name',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),1,'Text','RegularExpression',
NULL,100,'',NULL,2,GETDATE(),2,GETDATE(),2
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'Name' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','Description',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),0,'Text','RegularExpression',
NULL,100,'',NULL,2,GETDATE(),2,GETDATE(),3
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'Description' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Date','StartDate',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),1,'Date','MinDate',
NULL,'','',NULL,2,GETDATE(),2,GETDATE(),4
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Date' AND AttributeCode = 'StartDate' 
      AND ControlName = 'Date' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'MinDate')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Date','EndDate',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),1,'Date','MinDate',
NULL,'','',NULL,2,GETDATE(),2,GETDATE(),5
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Date' AND AttributeCode = 'EndDate' 
      AND ControlName = 'Date' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'MinDate')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','DisplayOrder',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),1,'Yes/No','AllowNegative',
NULL,'false','',NULL,2,GETDATE(),2,GETDATE(),6
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'DisplayOrder' 
      AND ControlName = 'Yes/No' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'AllowNegative')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','DisplayOrder',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),1,'Yes/No','AllowDecimals',
NULL,'false','',NULL,2,GETDATE(),2,GETDATE(),6
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'DisplayOrder' 
      AND ControlName = 'Yes/No' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'AllowDecimals')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','DisplayOrder',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),1,'Number','MinNumber',
NULL,'0','',NULL,2,GETDATE(),2,GETDATE(),6
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'DisplayOrder' 
      AND ControlName = 'Number' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'MinNumber')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','DisplayOrder',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),1,'Number','MaxNumber',
NULL,'999999','',NULL,2,GETDATE(),2,GETDATE(),6
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'DisplayOrder' 
      AND ControlName = 'Number' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'MaxNumber')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','Store',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),1,'Text','RegularExpression',
NULL,100,'',NULL,2,GETDATE(),2,GETDATE(),7
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'Store' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','Profile',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),1,'Text','RegularExpression',
NULL,100,'',NULL,2,GETDATE(),2,GETDATE(),8
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'Profile' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','IsCouponRequired',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),0,'Text','RegularExpression',
NULL,'','',NULL,2,GETDATE(),2,GETDATE(),9
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'IsCouponRequired' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','IsAllowedWithOtherCoupons',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),0,'Text','RegularExpression',
NULL,'','',NULL,2,GETDATE(),2,GETDATE(),10
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'IsAllowedWithOtherCoupons' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','PromotionMessage',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),0,'Text','RegularExpression',
NULL,'','',NULL,2,GETDATE(),2,GETDATE(),11
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'PromotionMessage' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Text','Code',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),0,'Text','RegularExpression',
NULL,'','',NULL,2,GETDATE(),2,GETDATE(),12
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'Code' 
      AND ControlName = 'Text' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'RegularExpression')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','AvailableQuantity',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),0,'Yes/No','AllowNegative',
NULL,'false','',NULL,2,GETDATE(),2,GETDATE(),13
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'AvailableQuantity' 
      AND ControlName = 'Yes/No' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'AllowNegative')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','AvailableQuantity',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),0,'Yes/No','AllowDecimals',
NULL,'false','',NULL,2,GETDATE(),2,GETDATE(),13
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'AvailableQuantity' 
      AND ControlName = 'Yes/No' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'AllowDecimals')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','AvailableQuantity',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),0,'Number','MinNumber',
NULL,'0','',NULL,2,GETDATE(),2,GETDATE(),13
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'AvailableQuantity' 
      AND ControlName = 'Number' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'MinNumber')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,
		RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','AvailableQuantity',(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions'),0,'Number','MaxNumber',
NULL,'999999','',NULL,2,GETDATE(),2,GETDATE(),13
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'AvailableQuantity' 
      AND ControlName = 'Number' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Promotions')
      AND ValidationName = 'MaxNumber')

UPDATE ZnodeImportAttributeValidation
SET IsRequired=0
WHERE ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name]='Promotions')
	AND AttributeCode IN ('Name','StartDate','EndDate','DisplayOrder','Store','Profile')
	AND IsRequired=1

--Ticket Number:-ZPD-21798
UPDATE ZnodeImportAttributeValidation 
SET IsRequired=1 WHERE AttributeCode IN ('StartDate','ExpirationDate','VoucherNumber','VoucherAmount','RemainingAmount') 
AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE NAME = 'Voucher')

--Ticket No:-ZPD-21516
DELETE FROM  ZnodeImportAttributeValidation WHERE AttributeTypeName ='Text' AND AttributeCode = 'CategoryCode' 
AND ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CategoryAssociation')
AND ValidationName = 'RegularExpression'

--Ticket No:-ZPD-19899
UPDATE ZnodeAttributeInputValidationRule 
SET ValidationName=UPPER(ValidationName) WHERE ValidationName='Url'
AND ValidationName != UPPER(ValidationName) COLLATE Latin1_General_CS_AS

--Ticket No:-ZPD-23120, ZPD-23121 & ZPD-23122 Dt.10-Jan-2023
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ErrorDescription' AND TABLE_NAME = 'ZnodeImportLog')
	AND (SELECT MAX(LEN(ErrorDescription)) FROM ZnodeImportLog)<=100
BEGIN
	ALTER TABLE ZnodeImportLog ALTER COLUMN ErrorDescription VARCHAR(100);
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'NC_Idx_ZnodeImportLog_ImportProcessLogId' AND object_id = OBJECT_ID('ZnodeImportLog'))
	AND (SELECT MAX(LEN(ErrorDescription)) FROM ZnodeImportLog)<=100
BEGIN
	CREATE NONCLUSTERED INDEX NC_Idx_ZnodeImportLog_ImportProcessLogId
    ON dbo.ZnodeImportLog (ImportProcessLogId,ErrorDescription,ColumnName);
END


