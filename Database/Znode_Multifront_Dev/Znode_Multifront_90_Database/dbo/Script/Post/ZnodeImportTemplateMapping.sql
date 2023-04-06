
----ZPD-7111 --> ZPD-8887
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'PriceTemplate'),
'CostPrice' SourceColumnName,'CostPrice' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=3 AND SourceColumnName ='CostPrice')

--dt 27-02-2020 ZPD-4958 and ZLMC-1154
update a set a.SourceColumnName = 'CategoryCode' , a.TargetColumnName = 'CategoryCode'
from ZnodeImportTemplateMapping a
inner join ZnodeImportTemplate b on a.ImportTemplateId = b.ImportTemplateId
where b.TemplateName = 'CategoryAssociation' and SourceColumnName = 'CategoryName'

update a set a.AttributeCode = 'CategoryCode'
from ZnodeImportAttributeValidation a
inner join ZnodeImportHead b on a.ImportHeadId = b.ImportHeadId
where b.Name = 'CategoryAssociation' and a. AttributeCode = 'CategoryName'

---------------------------------------------------------------------
-- ZPD-11728

insert into ZnodeImportHead(Name,	IsUsedInImport,	IsUsedInDynamicReport,	IsActive,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate,	IsCsvUploader)
select 'Voucher',1,1,1,2,getdate(),2,getdate(),null
where not exists(select * from ZnodeImportHead where Name = 'Voucher')

insert into ZnodeImportTemplate(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 ImportHeadId from ZnodeImportHead where name = 'Voucher'),'VoucherTemplate',null,null,1,2,getdate(),2,getdate()
where not exists(select * from ZnodeImportTemplate where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where name = 'Voucher')
	and TemplateName = 'VoucherTemplate' )


INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'StoreCode' SourceColumnName,'StoreCode' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='StoreCode')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'VoucherName' SourceColumnName,'VoucherName' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='VoucherName')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'CardNumber' SourceColumnName,'CardNumber' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='CardNumber')


INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'UserName' SourceColumnName,'UserName' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='UserName')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'ExpirationDate' SourceColumnName,'ExpirationDate' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='ExpirationDate')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'IsReferralCommission' SourceColumnName,'IsReferralCommission' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='IsReferralCommission')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'IsActive' SourceColumnName,'IsActive' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='IsActive')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'RemainingAmount' SourceColumnName,'RemainingAmount' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='RemainingAmount')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'RestrictToCustomerAccount' SourceColumnName,'RestrictToCustomerAccount' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='RestrictToCustomerAccount')


INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'StartDate' SourceColumnName,'StartDate' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='StartDate')


insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','StoreCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),1
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'StoreCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','VoucherName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),2
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'VoucherName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','CardNumber',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'CardNumber' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','Amount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Yes/No','AllowNegative',
null,'false','',null,2,getdate(),2,getdate(),4
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'Amount' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'AllowNegative')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','Amount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Yes/No','AllowDecimals',
null,'false','',null,2,getdate(),2,getdate(),4
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'Amount' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'AllowDecimals')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','Amount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Number','MinNumber',
null,'0','',null,2,getdate(),2,getdate(),4
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'Amount' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MinNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','Amount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Number','MaxNumber',
null,'999999','',null,2,getdate(),2,getdate(),4
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'Amount' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MaxNumber')


insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','UserName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),5
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'UserName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Date','StartDate',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),0,'Date','MinDate',
null,'','',null,2,getdate(),2,getdate(),6
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Date' and AttributeCode = 'StartDate' 
      and ControlName = 'Date' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MinDate')



insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Date','ExpirationDate',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),0,'Date','MinDate',
null,'','',null,2,getdate(),2,getdate(),7
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Date' and AttributeCode = 'ExpirationDate' 
      and ControlName = 'Date' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MinDate')


insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','IsReferralCommission',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Yes/No','AllowNegative',
null,'false','',null,2,getdate(),2,getdate(),8
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsReferralCommission' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'AllowNegative')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','IsReferralCommission',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Yes/No','AllowDecimals',
null,'false','',null,2,getdate(),2,getdate(),8
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsReferralCommission' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'AllowDecimals')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','IsReferralCommission',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Number','MinNumber',
null,'0','',null,2,getdate(),2,getdate(),8
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsReferralCommission' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MinNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','IsReferralCommission',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Number','MaxNumber',
null,'999999','',null,2,getdate(),2,getdate(),8
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsReferralCommission' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MaxNumber')


insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','IsActive',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),0,'Yes/No','AllowNegative',
null,'false','',null,2,getdate(),2,getdate(),9
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsActive' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'AllowNegative')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','IsActive',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),0,'Yes/No','AllowDecimals',
null,'false','',null,2,getdate(),2,getdate(),9
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsActive' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'AllowDecimals')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','IsActive',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),0,'Number','MinNumber',
null,'0','',null,2,getdate(),2,getdate(),9
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsActive' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MinNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','IsActive',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),0,'Number','MaxNumber',
null,'999999','',null,2,getdate(),2,getdate(),9
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'IsActive' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MaxNumber')


insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','RemainingAmount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Yes/No','AllowNegative',
null,'false','',null,2,getdate(),2,getdate(),10
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'RemainingAmount' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'AllowNegative')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','RemainingAmount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Yes/No','AllowDecimals',
null,'false','',null,2,getdate(),2,getdate(),10
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'RemainingAmount' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'AllowDecimals')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','RemainingAmount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Number','MinNumber',
null,'0','',null,2,getdate(),2,getdate(),10
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'RemainingAmount' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MinNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','RemainingAmount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Number','MaxNumber',
null,'999999','',null,2,getdate(),2,getdate(),10
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'RemainingAmount' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MaxNumber')


insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','RestrictToCustomerAccount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Yes/No','AllowNegative',
null,'false','',null,2,getdate(),2,getdate(),11
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'RestrictToCustomerAccount' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'AllowNegative')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','RestrictToCustomerAccount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Yes/No','AllowDecimals',
null,'false','',null,2,getdate(),2,getdate(),11
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'RestrictToCustomerAccount' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'AllowDecimals')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','RestrictToCustomerAccount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Number','MinNumber',
null,'0','',null,2,getdate(),2,getdate(),11
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'RestrictToCustomerAccount' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MinNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','RestrictToCustomerAccount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Number','MaxNumber',
null,'999999','',null,2,getdate(),2,getdate(),11
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'RestrictToCustomerAccount' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'MaxNumber')

go
update ZnodeImportAttributeValidation set ValidationValue = '0.01'
where AttributeCode = 'Amount' and ValidationName = 'MinNumber'
go

--dt 18-08-2020

delete from ZnodeImportAttributeValidation
where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'CardNumber'

delete from ZnodeImportTemplateMapping 
where ImportTemplateId = (select top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate')
and SourceColumnName  = 'CardNumber'

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'VoucherNumber' SourceColumnName,'VoucherNumber' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='VoucherNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','VoucherNumber',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'VoucherNumber' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'RegularExpression')

update  ZnodeImportAttributeValidation set IsRequired = 0
where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'VoucherNumber' and AttributeTypeName = 'Text'


update ZnodeImportAttributeValidation set AttributeCode = 'VoucherAmount'
where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'Amount' 

update ZnodeImportAttributeValidation set AttributeCode = 'RestrictVoucherToCustomer'
where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'RestrictToCustomerAccount' 

update ZnodeImportTemplateMapping set SourceColumnName = 'VoucherAmount', TargetColumnName = 'VoucherAmount'
where SourceColumnName = 'Amount' AND ImportTemplateId = (select top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate')

update ZnodeImportTemplateMapping set SourceColumnName = 'RestrictVoucherToCustomer', TargetColumnName = 'RestrictVoucherToCustomer'
where SourceColumnName = 'RestrictToCustomerAccount' AND ImportTemplateId = (select top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'VoucherAmount' SourceColumnName,'VoucherAmount' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='VoucherAmount')
--------

delete from ZnodeImportAttributeValidation
where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'IsActive' and AttributeTypeName = 'Number'

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','IsActive',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'IsActive' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'RegularExpression')

delete from ZnodeImportAttributeValidation
where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'RestrictToCustomerAccount' and AttributeTypeName = 'Number'

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','RestrictToCustomerAccount',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'RestrictToCustomerAccount' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'RegularExpression')
--------
delete from ZnodeImportAttributeValidation
where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'RestrictVoucherToCustomer' and AttributeTypeName = 'Number'


delete from ZnodeImportAttributeValidation
where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'RestrictToCustomerAccount' and AttributeTypeName = 'Text'

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','RestrictVoucherToCustomer',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'RestrictVoucherToCustomer' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'RegularExpression')

delete from ZnodeImportAttributeValidation
where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'IsReferralCommission'

delete from ZnodeImportTemplateMapping 
where ImportTemplateId = (select top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate')
and SourceColumnName  = 'IsReferralCommission'


delete from ZnodeImportAttributeValidation
where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
and AttributeCode = 'RestrictVoucherToCustomer' 

delete from ZnodeImportTemplateMapping
where SourceColumnName = 'RestrictVoucherToCustomer' and ImportTemplateId = (select top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate'),
'RestrictVoucherToACustomer' SourceColumnName,'RestrictVoucherToACustomer' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'VoucherTemplate') 
	AND SourceColumnName ='RestrictVoucherToACustomer')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','RestrictVoucherToACustomer',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'RestrictVoucherToACustomer' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Voucher')
	  and ValidationName = 'RegularExpression')



INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate'),
'AccountCode' SourceColumnName,'AccountCode' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='AccountCode')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','AccountCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),12
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'AccountCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'RegularExpression')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate'),
'DepartmentName' SourceColumnName,'DepartmentName' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='DepartmentName')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','DepartmentName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),13
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'DepartmentName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'RegularExpression')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate'),
'RoleName' SourceColumnName,'RoleName' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='RoleName')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','RoleName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),14
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'RoleName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Customer')
	  and ValidationName = 'RegularExpression')

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 73,'Text','The value is not associated with any existing Account.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 73)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 74,'Text','The value should be either User or Administrator or Manager',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 74)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 75,'Text','Account Code is mandatory to add a Role Name.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 75)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 76,'Text','The value is not associated with the Account.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 76)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 77,'Text','Account and Customer should belong to the same Store.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 77)

--18-09-2020 ZPD-11531 --> ZPD-12188 
insert into ZnodeImportHead(Name,	IsUsedInImport,	IsUsedInDynamicReport,	IsActive,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate,	IsCsvUploader)
select 'Account',1,1,1,2,getdate(),2,getdate(),null
where not exists(select * from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportTemplate(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 ImportHeadId from ZnodeImportHead where name = 'Account'),'AccountTemplate',null,null,1,2,getdate(),2,getdate()
where not exists(select * from ZnodeImportTemplate where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where name = 'Account')
	and TemplateName = 'AccountTemplate' )


INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'ParentAccountCode' SourceColumnName,'ParentAccountCode' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='ParentAccountCode')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'AccountName' SourceColumnName,'AccountName' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='AccountName')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'AccountCode' SourceColumnName,'AccountCode' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='AccountCode')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'ExternalID' SourceColumnName,'ExternalID' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='ExternalID')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'CatalogCode' SourceColumnName,'CatalogCode' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='CatalogCode')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'AddressName' SourceColumnName,'AddressName' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='AddressName')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'FirstName' SourceColumnName,'FirstName' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='FirstName')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'LastName' SourceColumnName,'LastName' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='LastName')

	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'CompanyName' SourceColumnName,'CompanyName' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='CompanyName')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'Address1' SourceColumnName,'Address1' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='Address1')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'Address2' SourceColumnName,'Address2' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='Address2')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'CountryName' SourceColumnName,'CountryName' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='CountryName')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'StateName' SourceColumnName,'StateName' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='StateName')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'CityName' SourceColumnName,'CityName' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='CityName')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'PostalCode' SourceColumnName,'PostalCode' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='PostalCode')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'PhoneNumber' SourceColumnName,'PhoneNumber' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='PhoneNumber')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'IsDefaultBilling' SourceColumnName,'IsDefaultBilling' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='IsDefaultBilling')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate'),
'IsDefaultShipping' SourceColumnName,'IsDefaultShipping' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'AccountTemplate') 
	AND SourceColumnName ='IsDefaultShipping')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','ParentAccountCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),1
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'ParentAccountCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','AccountName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),2
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'AccountName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','AccountCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'AccountCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','ExternalID',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),4
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'ExternalID' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')	  

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','CatalogCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),5
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'CatalogCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')	

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','AddressName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),6
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'AddressName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')	

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','FirstName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),7
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'FirstName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','LastName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),8
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'LastName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','CompanyName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),9
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'CompanyName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','Address1',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),10
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'Address1' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','Address2',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),11
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'Address2' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','CountryName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),12
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'CountryName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','StateName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),13
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'StateName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','CityName',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),14
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'CityName' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','PostalCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),15
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'PostalCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','PhoneNumber',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),16
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'PhoneNumber' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','IsDefaultBilling',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),17
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'IsDefaultBilling' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','IsDefaultShipping',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),18
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'IsDefaultShipping' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'AddressName'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'AddressName')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'FirstName'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'FirstName')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'LastName'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'LastName')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'CompanyName'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'CompanyName')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'Address1'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'Address1')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'Address2'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'Address2')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'CountryName'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'CountryName')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'StateName'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'StateName')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'CityName'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'CityName')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'PostalCode'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'PostalCode')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'PhoneNumber'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'PhoneNumber')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'IsDefaultBilling'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'IsDefaultBilling')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')

insert into ZnodeImportUpdatableColumns(ImportHeadId,ColumnName)
select (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account'),'IsDefaultShipping'
where not exists(select * from ZnodeImportUpdatableColumns where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')
and ColumnName = 'IsDefaultShipping')
and exists(select top 1 ImportHeadId from ZnodeImportHead where Name = 'Account')


insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 78,'Text','Maximum 100 characters are allowed.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 78)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 79,'Text','Only alphabets and numbers are allowed.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 79)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 80,'Text','The value is not associated with any existing Catalog.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 80)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 81,'Text','Maximum 200 characters are allowed.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 81)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 82,'Text','Maximum 300 characters are allowed.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 82)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 83,'Text','One of the predefined values is required.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 83)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 84,'Text','Mandatory value is missing',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 84)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 85,'Text','The value should be 1 for new accounts.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 85)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 86,'Text','Parent Account should belong to the selected Store.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 86)

insert into ZnodeMessage(MessageCode,MessageType,MessageName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 87,'Text','The value is not associated with any existing Parent Account.',2,getdate(),2,getdate()
where not exists(select * from ZnodeMessage where MessageCode = 87)

--18-09-2020 ZPD-12377 --> ZPD-8314
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'ProductAssociation'),
'IsDefault' SourceColumnName,'IsDefault' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'ProductAssociation') 
	AND SourceColumnName ='IsDefault')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','IsDefault',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),null
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'IsDefault' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'ProductAssociation')
	  and ValidationName = 'RegularExpression')

-- ZPD-12377 
update [dbo].[ZnodeImportAttributeValidation]
set IsRequired =0
where importheadid=(select top 1 importheadid from ZnodeImportHead where Name ='ProductAssociation') and AttributeCode ='DisplayOrder'


------------ZPD-13943
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') ,'EnablePowerBIReportOnWebStore','EnablePowerBIReportOnWebStore',0,1,1,2,getdate(),2,getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='EnablePowerBIReportOnWebStore')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') ,'EnableUserShippingAddressSuggestion','EnableUserShippingAddressSuggestion',0,1,1,2,getdate(),2,getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='EnableUserShippingAddressSuggestion')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') ,'BillingAccountNumber','BillingAccountNumber',0,1,1,2,getdate(),2,getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='BillingAccountNumber')


INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') ,'PerOrderAnnualLimit','PerOrderAnnualLimit',0,1,1,2,getdate(),2,getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='PerOrderAnnualLimit')


INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') ,'PerOrderLimit','PerOrderLimit',0,1,1,2,getdate(),2,getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='PerOrderLimit')


INSERT INTO ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (SELECT Top 1 ImportTemplateId from ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate') ,'Custom1','Custom1',0,1,1,2,getdate(),2,getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(SELECT Top 1 ImportTemplateId from ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='Custom1')

INSERT INTO ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (SELECT Top 1 ImportTemplateId from ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate') ,'Custom2','Custom2',0,1,1,2,getdate(),2,getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(SELECT Top 1 ImportTemplateId from ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='Custom2')

INSERT INTO ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (SELECT Top 1 ImportTemplateId from ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate') ,'Custom3','Custom3',0,1,1,2,getdate(),2,getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(SELECT Top 1 ImportTemplateId from ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='Custom3')


INSERT INTO ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (SELECT Top 1 ImportTemplateId from ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate') ,'Custom4','Custom4',0,1,1,2,getdate(),2,getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(SELECT Top 1 ImportTemplateId from ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='Custom4')


INSERT INTO ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (SELECT Top 1 ImportTemplateId from ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate') ,'Custom5','Custom5',0,1,1,2,getdate(),2,getdate()
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(SELECT Top 1 ImportTemplateId from ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate') 
	AND SourceColumnName ='Custom5')

--ZPD-10135
;WITH CTE AS
(
	SELECT ImportTemplateId, SourceColumnName,TargetColumnName,MIN(ImportTemplateMappingId) AS ImportTemplateMappingId
	FROM ZnodeImportTemplateMapping
	WHERE ImportTemplateId = (SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'CustomerAddressTemplate')
	AND SourceColumnName = 'IsDefaultShipping'
	GROUP BY ImportTemplateId, SourceColumnName,TargetColumnName
)
DELETE A FROM ZnodeImportTemplateMapping A
WHERE EXISTS( SELECT * FROM CTE B WHERE  A.ImportTemplateId = B.ImportTemplateId AND SourceColumnName = B.SourceColumnName AND A.TargetColumnName = B.TargetColumnName)
AND NOT EXISTS( SELECT * FROM CTE B WHERE A.ImportTemplateMappingId = B.ImportTemplateMappingId )

--------ZPD-14606
insert into ZnodeImportHead(Name,	IsUsedInImport,	IsUsedInDynamicReport,	IsActive,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate,	IsCsvUploader)
select 'ProductAssociation',1,1,1,2,getdate(),2,getdate(),null
where not exists(select * from ZnodeImportHead where Name = 'ProductAssociation')

insert into ZnodeImportTemplate(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 ImportHeadId from ZnodeImportHead where name = 'ProductAssociation'),'ProductAssociation',null,null,1,2,getdate(),2,getdate()
where not exists(select * from ZnodeImportTemplate where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where name = 'ProductAssociation')
	and TemplateName = 'ProductAssociation' )


INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'ProductAssociation'),
'Quantity' SourceColumnName,'Quantity' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'ProductAssociation') 
	AND SourceColumnName ='Quantity')
--ZPD-17736 Dt.23-Feb-2022
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'CategoryTemplate'),
'HideCategoryonMenu','HideCategoryonMenu',0,0,0,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
	WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'CategoryTemplate') 
		AND SourceColumnName ='HideCategoryonMenu')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,RegExp,DisplayOrder,
		CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','HideCategoryonMenu',
(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Category'),
0,'Number','MaxNumber',NULL,'2','',NULL,2,GETDATE(),2,GETDATE(),NULL
WHERE NOT EXISTS(SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'HideCategoryonMenu' 
      AND ControlName = 'Number' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Category')
	  AND ValidationName = 'MaxNumber')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,RegExp,DisplayOrder,
		CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','HideCategoryonMenu',
(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Category'),
0,'Number','MinNumber',NULL,'0','',NULL,2,GETDATE(),2,GETDATE(),NULL
WHERE NOT EXISTS(SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'HideCategoryonMenu' 
      AND ControlName = 'Number' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Category')
	  AND ValidationName = 'MinNumber')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,RegExp,DisplayOrder,
		CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','HideCategoryonMenu',
(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Category'),
0,'Yes/No','AllowDecimals',NULL,'false','',NULL,2,GETDATE(),2,GETDATE(),NULL
WHERE NOT EXISTS(SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'HideCategoryonMenu' 
      AND ControlName = 'Yes/No' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Category')
	  AND ValidationName = 'AllowDecimals')

INSERT INTO ZnodeImportAttributeValidation
	(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName,ValidationValue,RegExp,DisplayOrder,
		CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
SELECT 'Number','HideCategoryonMenu',
(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Category'),
0,'Yes/No','AllowNegative',NULL,'false','',NULL,2,GETDATE(),2,GETDATE(),NULL
WHERE NOT EXISTS(SELECT TOP 1 1 FROM ZnodeImportAttributeValidation WHERE AttributeTypeName ='Number' AND AttributeCode = 'HideCategoryonMenu' 
      AND ControlName = 'Yes/No' AND ImportHeadId=(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name = 'Category')
	  AND ValidationName = 'AllowNegative')

insert into ZnodeImportHead(Name,	IsUsedInImport,	IsUsedInDynamicReport,	IsActive,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate,	IsCsvUploader)
select 'Synonyms',1,1,1,2,getdate(),2,getdate(),null
where not exists(select * from ZnodeImportHead where Name = 'Synonyms')

insert into ZnodeImportTemplate(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 ImportHeadId from ZnodeImportHead where name = 'Synonyms'),'Synonym Template',null,null,1,2,getdate(),2,getdate()
where not exists(select * from ZnodeImportTemplate where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where name = 'Synonyms')
	and TemplateName = 'Synonym Template' )

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template'),
'CatalogCode' SourceColumnName,'CatalogCode' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template') 
	AND SourceColumnName ='CatalogCode')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template'),
'SynonymCode' SourceColumnName,'SynonymCode' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template') 
	AND SourceColumnName ='SynonymCode')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template'),
'RenameSynonymCode' SourceColumnName,'RenameSynonymCode' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template') 
	AND SourceColumnName ='RenameSynonymCode')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template'),
'OriginalTerm' SourceColumnName,'OriginalTerm' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template') 
	AND SourceColumnName ='OriginalTerm')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template'),
'ReplacedBy' SourceColumnName,'ReplacedBy' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template') 
	AND SourceColumnName ='ReplacedBy')
	
INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template'),
'IsBidirectional' SourceColumnName,'IsBidirectional' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Synonym Template') 
	AND SourceColumnName ='IsBidirectional')


insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','CatalogCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),1
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'CatalogCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','SynonymCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),2
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'SynonymCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','RenameSynonymCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'RenameSynonymCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','OriginalTerm',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms'),1,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),4
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'OriginalTerm' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','ReplacedBy',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),5
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'ReplacedBy' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms')
	  and ValidationName = 'RegularExpression')


insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','IsBidirectional',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),6
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'IsBidirectional' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'Synonyms')
	  and ValidationName = 'RegularExpression')

update ZnodeImportAttributeValidation set IsRequired = 0 where ImportHeadId = (select ImportHeadId from ZnodeImportHead where Name = 'Synonyms') 
and AttributeCode = 'OriginalTerm'

insert into ZnodeImportHead(Name,	IsUsedInImport,	IsUsedInDynamicReport,	IsActive,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate,	IsCsvUploader)
select 'CatalogCategoryAssociation',1,1,1,2,getdate(),2,getdate(),null
where not exists(select * from ZnodeImportHead where Name = 'CatalogCategoryAssociation')

insert into ZnodeImportTemplate(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 ImportHeadId from ZnodeImportHead where name = 'CatalogCategoryAssociation'),'CatalogCategoryAssociationTemplate',null,null,1,2,getdate(),2,getdate()
where not exists(select * from ZnodeImportTemplate where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where name = 'CatalogCategoryAssociation')
	and TemplateName = 'CatalogCategoryAssociationTemplate' )


INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CatalogCategoryAssociationTemplate'),
'ParentCode' SourceColumnName,'ParentCode' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CatalogCategoryAssociationTemplate') 
	AND SourceColumnName ='ParentCode')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CatalogCategoryAssociationTemplate'),
'CategoryCode' SourceColumnName,'CategoryCode' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CatalogCategoryAssociationTemplate') 
	AND SourceColumnName ='CategoryCode')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CatalogCategoryAssociationTemplate'),
'DisplayOrder' SourceColumnName,'DisplayOrder' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CatalogCategoryAssociationTemplate') 
	AND SourceColumnName ='DisplayOrder')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CatalogCategoryAssociationTemplate'),
'Action' SourceColumnName,'Action' TargetColumnName,0 DisplayOrder,1 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'CatalogCategoryAssociationTemplate') 
	AND SourceColumnName ='Action')



insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','ParentCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),1
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'ParentCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','CategoryCode',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),2
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'CategoryCode' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation')
	  and ValidationName = 'RegularExpression')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','DisplayOrder',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation'),0,'Yes/No','AllowNegative',
null,'false','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'DisplayOrder' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation')
	  and ValidationName = 'AllowNegative')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','DisplayOrder',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation'),0,'Yes/No','AllowDecimals',
null,'false','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'DisplayOrder' 
      and ControlName = 'Yes/No' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation')
	  and ValidationName = 'AllowDecimals')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','DisplayOrder',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation'),0,'Number','MinNumber',
null,'0','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'DisplayOrder' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation')
	  and ValidationName = 'MinNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Number','DisplayOrder',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation'),0,'Number','MaxNumber',
null,'999999','',null,2,getdate(),2,getdate(),3
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Number' and AttributeCode = 'DisplayOrder' 
      and ControlName = 'Number' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation')
	  and ValidationName = 'MaxNumber')

insert into ZnodeImportAttributeValidation(AttributeTypeName,AttributeCode,ImportHeadId,IsRequired,ControlName,ValidationName,SubValidationName
,ValidationValue,RegExp,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SequenceNumber)
select 'Text','Action',(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation'),0,'Text','RegularExpression',
null,'','',null,2,getdate(),2,getdate(),4
where not exists(select * from ZnodeImportAttributeValidation where AttributeTypeName ='Text' and AttributeCode = 'Action' 
      and ControlName = 'Text' and ImportHeadId=(select Top 1 ImportHeadId from ZnodeImportHead where Name = 'CatalogCategoryAssociation')
	  and ValidationName = 'RegularExpression')
GO

--ZPD-8428 Dt.26-May-2022
insert into ZnodeImportHead(Name,IsUsedInImport,IsUsedInDynamicReport,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsCsvUploader)
select 'Brands',1,1,1,2,getdate(),2,getdate(),null
where not exists(select * from ZnodeImportHead where Name = 'Brands')

insert into ZnodeImportTemplate(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 ImportHeadId from ZnodeImportHead where name = 'Brands'),'Brands Template',null,null,1,2,getdate(),2,getdate()
where not exists(select * from ZnodeImportTemplate where ImportHeadId = (select top 1 ImportHeadId from ZnodeImportHead where name = 'Brands')
    and TemplateName = 'Brands Template' )

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'BrandCode' SourceColumnName,'BrandCode' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='BrandCode')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'IsActive' SourceColumnName,'IsActive' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='IsActive')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'BrandDescription' SourceColumnName,'BrandDescription' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='BrandDescription')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'SEOKeyword' SourceColumnName,'SEOKeyword' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='SEOKeyword')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'SEODescription' SourceColumnName,'SEODescription' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='SEODescription')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'BrandLogo' SourceColumnName,'BrandLogo' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='BrandLogo')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'SEOTitle' SourceColumnName,'SEOTitle' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='SEOTitle')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'SEOFriendlyPageName' SourceColumnName,'SEOFriendlyPageName' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='SEOFriendlyPageName')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'URLKey' SourceColumnName,'URLKey' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='URLKey')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'Custom1' SourceColumnName,'Custom1' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='Custom1')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'Custom2' SourceColumnName,'Custom2' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='Custom2')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'Custom3' SourceColumnName,'Custom3' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='Custom3')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'Custom4' SourceColumnName,'Custom4' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='Custom4')

INSERT ZnodeImportTemplateMapping(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 ImportTemplateId from ZnodeImportTemplate where ImportHeadId=(select ImportHeadId from ZnodeImportHead where name='Brands')),
'Custom5' SourceColumnName,'Custom5' TargetColumnName,0 DisplayOrder,0 IsActive, 0 IsAllowNull,2 CreatedBy,GETDATE() CreatedDate,2 ModifiedBy,GETDATE() ModifiedDate
WHERE NOT EXISTS(SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId=(select Top 1 ImportTemplateId from ZnodeImportTemplate where TemplateName = 'Brands Template') 
    AND SourceColumnName ='Custom5')

--ZPD-20660 Dt.06/07-July-2022
DELETE 
FROM ZnodeImportTemplateMapping
WHERE NOT EXISTS (SELECT * FROM ZnodePimAttribute WHERE AttributeCode=ZnodeImportTemplateMapping.TargetColumnName AND IsCategory = 0)
		AND ImportTemplateId IN (SELECT ImportTemplateId FROM ZnodeImportTemplate 
								WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE Name='Product'))

--ZPD-21754 Dt.24-Aug-2022
-- ZnodeImportHead
INSERT INTO ZnodeImportHead
	([Name],IsUsedInImport,IsUsedInDynamicReport,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsCsvUploader)
SELECT 'Promotions', 1, 0, 1, 2, GETDATE(), 2, GETDATE(), NULL
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportHead WHERE [Name] = 'Promotions')

-- ZnodeImportTemplate
INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Amount Off Brand', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Brand')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Amount Off Brand'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Brand'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Percent Off Brand', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Brand')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Percent Off Brand'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Brand'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Amount Off Catalog', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Catalog')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Amount Off Catalog'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Catalog'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Percent Off Catalog', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Catalog')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Percent Off Catalog'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Catalog'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Amount Off Category', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Category')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Amount Off Category'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Category'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Percent Off Category', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Category')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Percent Off Category'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Category'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Amount Off Displayed Product Price', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Displayed Product Price')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Amount Off Displayed Product Price'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Displayed Product Price'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Percent Off Displayed Product Price', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Displayed Product Price')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Percent Off Displayed Product Price'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Displayed Product Price'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Amount Off Order', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Order')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Amount Off Order'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Order'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Percent Off Order', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Order')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Percent Off Order'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Order'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Amount Off Product', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Product')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Amount Off Product'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Product'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Percent Off Product', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Product')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Percent Off Product'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Product'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Amount Off Shipping', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Shipping')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Amount Off Shipping'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Shipping'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Percent Off Shipping', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Shipping')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Percent Off Shipping'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Shipping'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Amount Off Shipping with carrier', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Shipping with carrier')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Amount Off Shipping with carrier'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off Shipping with carrier'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Percent Off Shipping with carrier', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Shipping with carrier')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Percent Off Shipping with carrier'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off Shipping with carrier'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Amount Off X If Y Purchased', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off X If Y Purchased')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Amount Off X If Y Purchased'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Amount Off X If Y Purchased'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Percent Off X If Y Purchased', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off X If Y Purchased')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Percent Off X If Y Purchased'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Percent Off X If Y Purchased'))

INSERT INTO ZnodeImportTemplate
	(ImportHeadId,TemplateName,TemplateVersion,PimAttributeFamilyId,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, PromotionTypeId)
SELECT
	(SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions'),
	'Call For Pricing', NULL, NULL, 1, 2, GETDATE(), 2, GETDATE(),
	(SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Call For Pricing')
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplate 
				WHERE ImportHeadId = (SELECT TOP 1 ImportHeadId FROM ZnodeImportHead WHERE [Name] = 'Promotions') AND TemplateName = 'Call For Pricing'
					AND PromotionTypeId = (SELECT TOP 1 PromotionTypeId FROM ZnodePromotionType WHERE [Name] = 'Call For Pricing'))

-- ZnodeImportTemplateMapping, TemplateName = 'Amount Off Brand'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'Brand' As SourceColumnName, 'Brand' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'Brand' AND TargetColumnName = 'Brand')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand'),
	'MinimumQuantity' As SourceColumnName, 'MinimumQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Brand')
					AND SourceColumnName = 'MinimumQuantity' AND TargetColumnName = 'MinimumQuantity')

-- ZnodeImportTemplateMapping, TemplateName = 'Percent Off Brand'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'Brand' As SourceColumnName, 'Brand' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'Brand' AND TargetColumnName = 'Brand')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand'),
	'MinimumQuantity' As SourceColumnName, 'MinimumQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Brand')
					AND SourceColumnName = 'MinimumQuantity' AND TargetColumnName = 'MinimumQuantity')

-- ZnodeImportTemplateMapping, TemplateName = 'Amount Off Catalog'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'Catalog' As SourceColumnName, 'Catalog' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'Catalog' AND TargetColumnName = 'Catalog')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog'),
	'MinimumQuantity' As SourceColumnName, 'MinimumQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Catalog')
					AND SourceColumnName = 'MinimumQuantity' AND TargetColumnName = 'MinimumQuantity')

-- ZnodeImportTemplateMapping, TemplateName = 'Percent Off Catalog'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'Catalog' As SourceColumnName, 'Catalog' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'Catalog' AND TargetColumnName = 'Catalog')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog'),
	'MinimumQuantity' As SourceColumnName, 'MinimumQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Catalog')
					AND SourceColumnName = 'MinimumQuantity' AND TargetColumnName = 'MinimumQuantity')

-- ZnodeImportTemplateMapping, TemplateName = 'Amount Off Category'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'Category' As SourceColumnName, 'Category' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'Category' AND TargetColumnName = 'Category')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category'),
	'MinimumQuantity' As SourceColumnName, 'MinimumQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Category')
					AND SourceColumnName = 'MinimumQuantity' AND TargetColumnName = 'MinimumQuantity')

-- ZnodeImportTemplateMapping, TemplateName = 'Percent Off Category'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'Category' As SourceColumnName, 'Category' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'Category' AND TargetColumnName = 'Category')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category'),
	'MinimumQuantity' As SourceColumnName, 'MinimumQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Category')
					AND SourceColumnName = 'MinimumQuantity' AND TargetColumnName = 'MinimumQuantity')

-- ZnodeImportTemplateMapping, TemplateName = 'Amount Off Displayed Product Price'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price'),
	'ProductToDiscount' As SourceColumnName, 'ProductToDiscount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Displayed Product Price')
					AND SourceColumnName = 'ProductToDiscount' AND TargetColumnName = 'ProductToDiscount')

-- ZnodeImportTemplateMapping, TemplateName = 'Percent Off Displayed Product Price'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price'),
	'ProductToDiscount' As SourceColumnName, 'ProductToDiscount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Displayed Product Price')
					AND SourceColumnName = 'ProductToDiscount' AND TargetColumnName = 'ProductToDiscount')

-- ZnodeImportTemplateMapping, TemplateName = 'Amount Off Order'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Order')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

-- ZnodeImportTemplateMapping, TemplateName = 'Percent Off Order'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Order')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

-- ZnodeImportTemplateMapping, TemplateName = 'Amount Off Product'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'MinimumQuantity' As SourceColumnName, 'MinimumQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'MinimumQuantity' AND TargetColumnName = 'MinimumQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product'),
	'ProductToDiscount' As SourceColumnName, 'ProductToDiscount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Product')
					AND SourceColumnName = 'ProductToDiscount' AND TargetColumnName = 'ProductToDiscount')

-- ZnodeImportTemplateMapping, TemplateName = 'Percent Off Product'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'MinimumQuantity' As SourceColumnName, 'MinimumQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'MinimumQuantity' AND TargetColumnName = 'MinimumQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product'),
	'ProductToDiscount' As SourceColumnName, 'ProductToDiscount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Product')
					AND SourceColumnName = 'ProductToDiscount' AND TargetColumnName = 'ProductToDiscount')

-- ZnodeImportTemplateMapping, TemplateName = 'Amount Off Shipping'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

-- ZnodeImportTemplateMapping, TemplateName = 'Percent Off Shipping'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

-- ZnodeImportTemplateMapping, TemplateName = 'Amount Off Shipping with carrier'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier'),
	'Shipping' As SourceColumnName, 'Shipping' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off Shipping with carrier')
					AND SourceColumnName = 'Shipping' AND TargetColumnName = 'Shipping')

-- ZnodeImportTemplateMapping, TemplateName = 'Percent Off Shipping with carrier'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'MinimumOrderAmount' As SourceColumnName, 'MinimumOrderAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'MinimumOrderAmount' AND TargetColumnName = 'MinimumOrderAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier'),
	'Shipping' As SourceColumnName, 'Shipping' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off Shipping with carrier')
					AND SourceColumnName = 'Shipping' AND TargetColumnName = 'Shipping')

-- ZnodeImportTemplateMapping, TemplateName = 'Amount Off X If Y Purchased'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'MinimumQuantity' As SourceColumnName, 'MinimumQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'MinimumQuantity' AND TargetColumnName = 'MinimumQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'ProductQuantity' As SourceColumnName, 'ProductQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'ProductQuantity' AND TargetColumnName = 'ProductQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'ProductToDiscount' As SourceColumnName, 'ProductToDiscount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'ProductToDiscount' AND TargetColumnName = 'ProductToDiscount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased'),
	'RequiredProduct' As SourceColumnName, 'RequiredProduct' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Amount Off X If Y Purchased')
					AND SourceColumnName = 'RequiredProduct' AND TargetColumnName = 'RequiredProduct')

-- ZnodeImportTemplateMapping, TemplateName = 'Percent Off X If Y Purchased'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'DiscountAmount' As SourceColumnName, 'DiscountAmount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'DiscountAmount' AND TargetColumnName = 'DiscountAmount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'MinimumQuantity' As SourceColumnName, 'MinimumQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'MinimumQuantity' AND TargetColumnName = 'MinimumQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'ProductQuantity' As SourceColumnName, 'ProductQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'ProductQuantity' AND TargetColumnName = 'ProductQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'ProductToDiscount' As SourceColumnName, 'ProductToDiscount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'ProductToDiscount' AND TargetColumnName = 'ProductToDiscount')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased'),
	'RequiredProduct' As SourceColumnName, 'RequiredProduct' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Percent Off X If Y Purchased')
					AND SourceColumnName = 'RequiredProduct' AND TargetColumnName = 'RequiredProduct')

-- ZnodeImportTemplateMapping, TemplateName = 'Call For Pricing'
INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'PromoCode' As SourceColumnName, 'PromoCode' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'PromoCode' AND TargetColumnName = 'PromoCode')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'Name' As SourceColumnName, 'Name' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'Name' AND TargetColumnName = 'Name')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'Description' As SourceColumnName, 'Description' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'Description' AND TargetColumnName = 'Description')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'StartDate' As SourceColumnName, 'StartDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'StartDate' AND TargetColumnName = 'StartDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'EndDate' As SourceColumnName, 'EndDate' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'EndDate' AND TargetColumnName = 'EndDate')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'DisplayOrder' As SourceColumnName, 'DisplayOrder' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'DisplayOrder' AND TargetColumnName = 'DisplayOrder')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'Store' As SourceColumnName, 'Store' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'Store' AND TargetColumnName = 'Store')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'Profile' As SourceColumnName, 'Profile' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'Profile' AND TargetColumnName = 'Profile')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'IsCouponRequired' As SourceColumnName, 'IsCouponRequired' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'IsCouponRequired' AND TargetColumnName = 'IsCouponRequired')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'IsAllowedWithOtherCoupons' As SourceColumnName, 'IsAllowedWithOtherCoupons' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'IsAllowedWithOtherCoupons' AND TargetColumnName = 'IsAllowedWithOtherCoupons')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'PromotionMessage' As SourceColumnName, 'PromotionMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'PromotionMessage' AND TargetColumnName = 'PromotionMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'Code' As SourceColumnName, 'Code' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'Code' AND TargetColumnName = 'Code')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'AvailableQuantity' As SourceColumnName, 'AvailableQuantity' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'AvailableQuantity' AND TargetColumnName = 'AvailableQuantity')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'CallForPriceMessage' As SourceColumnName, 'CallForPriceMessage' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'CallForPriceMessage' AND TargetColumnName = 'CallForPriceMessage')

INSERT ZnodeImportTemplateMapping
	(ImportTemplateId,SourceColumnName,TargetColumnName,DisplayOrder,IsActive,IsAllowNull,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing'),
	'ProductToDiscount' As SourceColumnName, 'ProductToDiscount' As TargetColumnName, 0 As DisplayOrder, 1 As IsActive, 0 As IsAllowNull,
		2 As CreatedBy, GETDATE() As CreatedDate, 2 As ModifiedBy, GETDATE() As ModifiedDate
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeImportTemplateMapping 
				WHERE ImportTemplateId=(SELECT TOP 1 ImportTemplateId FROM ZnodeImportTemplate WHERE TemplateName = 'Call For Pricing')
					AND SourceColumnName = 'ProductToDiscount' AND TargetColumnName = 'ProductToDiscount')
