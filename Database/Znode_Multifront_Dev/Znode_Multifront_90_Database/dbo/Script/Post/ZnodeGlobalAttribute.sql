
--dt 20-01-2020 ZPD-8291 --> ZPD-8921
insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Yes/No'),'IsCloudflareEnabled',1,0,0,1,
'Decide is the Clouflare enabled on webstore?',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'IsCloudflareEnabled')

insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Text'),'CloudflareZoneId',1,1,0,2,
'ZoneId for Cloudflare',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'CloudflareZoneId')

insert into ZnodePortalGlobalAttributeValue(PortalId,GlobalAttributeId,GlobalAttributeDefaultValueId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select PortalId,(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'IsCloudflareEnabled'),null,null,2,getdate(),2,getdate()
from ZnodePortal ZP
where not exists(select * from ZnodePortalGlobalAttributeValue ZPGA where ZP.PortalId = ZPGA.PortalId
      and ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'IsCloudflareEnabled'))


insert into ZnodePortalGlobalAttributeValueLocale(PortalGlobalAttributeValueId,	LocaleId,	AttributeValue	,CreatedBy,	CreatedDate,	ModifiedBy	,ModifiedDate)
select PortalGlobalAttributeValueId,1,'false',2,getdate(),2,getdate()
from ZnodePortalGlobalAttributeValue ZPGA
where ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'IsCloudflareEnabled')
and not exists(select * from ZnodePortalGlobalAttributeValueLocale ZPGAVL where ZPGAVL.PortalGlobalAttributeValueId = ZPGA.PortalGlobalAttributeValueId)

insert into ZnodePortalGlobalAttributeValue(PortalId,GlobalAttributeId,GlobalAttributeDefaultValueId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select PortalId,(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'CloudflareZoneId'),null,null,2,getdate(),2,getdate()
from ZnodePortal ZP
where not exists(select * from ZnodePortalGlobalAttributeValue ZPGA where ZP.PortalId = ZPGA.PortalId
      and ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'CloudflareZoneId'))

insert into ZnodePortalGlobalAttributeValueLocale(PortalGlobalAttributeValueId,	LocaleId,	AttributeValue	,CreatedBy,	CreatedDate,	ModifiedBy	,ModifiedDate)
select PortalGlobalAttributeValueId,1,'',2,getdate(),2,getdate()
from ZnodePortalGlobalAttributeValue ZPGA
where ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'CloudflareZoneId')
and not exists(select * from ZnodePortalGlobalAttributeValueLocale ZPGAVL where ZPGAVL.PortalGlobalAttributeValueId = ZPGA.PortalGlobalAttributeValueId)

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Enable Cloudflare',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'IsCloudflareEnabled' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'ZoneId',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'CloudflareZoneId' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)

----
insert into ZnodeGlobalAttributeGroup(GroupCode,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select 'Cloudflaresetting',null,2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttributeGroup where GroupCode = 'Cloudflaresetting')

insert into ZnodeGlobalAttributeGroupLocale(LocaleId,GlobalAttributeGroupId,AttributeGroupName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeGroupId,'Cloudflare Setting',null,2,getdate(),2,getdate()
from ZnodeGlobalAttributeGroup a
where GroupCode = 'Cloudflaresetting'
and not exists(select * from ZnodeGlobalAttributeGroupLocale b where b.GlobalAttributeGroupId= a.GlobalAttributeGroupId and b.LocaleId = 1)


insert into ZnodeGlobalAttributeGroupMapper (GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Cloudflaresetting'),
(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'IsCloudflareEnabled'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeGroupMapper 
     where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Cloudflaresetting')
	 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'IsCloudflareEnabled'))

insert into ZnodeGlobalAttributeGroupMapper (GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Cloudflaresetting'),
(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'CloudflareZoneId'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeGroupMapper 
     where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Cloudflaresetting')
	 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'CloudflareZoneId'))

	 
insert into ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Cloudflaresetting'),
(select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'),1, 2,getdate(),2,GETDATE()
where not exists(select * from ZnodeGlobalGroupEntityMapper where GlobalAttributeGroupId=(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Cloudflaresetting')
      and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'))

--dt 06-02-2020 ZPD-8956 
update ZnodeGlobalAttribute set IsRequired = 0 where AttributeCode = 'CloudflareZoneId'

--dt 25-02-2020 ZPD-9275
update ZnodeGlobalAttribute set HelpDescription = 'When enabled, the Cloudflare cache will get purged when this store will be published and also users will have the ability to manually purge the Cloudflare cache for this store from the Global Settings.'
where AttributeCode = 'IsCloudflareEnabled'

update ZnodeGlobalAttribute set HelpDescription = 'Every Zone ID is associated with a domains. When Cloudflare cache purge activity is initiated the input in this field is used by Znode to identify the cache of which domain(and its subdomains) to clear.'
where AttributeCode = 'CloudflareZoneId'

update ZGAL set ZGAL.AttributeName = 'Zone ID'
from ZnodeGlobalAttribute ZGA
inner join ZnodeGlobalAttributeLocale ZGAL on ZGA.GlobalAttributeId = ZGAL.GlobalAttributeId
where ZGA.AttributeCode='CloudflareZoneId'

----dt 15-05-2020 ZPD-10318 --> ZPD-10408

insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Yes/No'),'EnableQuoteRequest',1,0,0,500,
'When enabled, customers will be able to create Quote Requests from web store.',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'EnableQuoteRequest')

insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Number'),'QuoteExpireInDays',1,0,0,500,
'Set a number of days after which you want the Quote to expire.',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'QuoteExpireInDays')

insert into ZnodeGlobalAttributeGroup(GroupCode,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select 'QuotesSettings',null,2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttributeGroup where GroupCode = 'QuotesSettings')

insert into ZnodeGlobalAttributeDefaultValue(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnableQuoteRequest'),'false',null,null,null,null,null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeDefaultValue where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnableQuoteRequest') and AttributeDefaultValueCode = 'false')

insert into ZnodeGlobalAttributeDefaultValue(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'QuoteExpireInDays'),'30',null,null,null,null,null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeDefaultValue where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'QuoteExpireInDays') and AttributeDefaultValueCode = '30')


insert into ZnodeGlobalAttributeGroupMapper (GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'QuotesSettings'),
(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableQuoteRequest'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeGroupMapper 
     where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'QuotesSettings')
	 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableQuoteRequest'))

insert into ZnodeGlobalAttributeGroupMapper (GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'QuotesSettings'),
(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeGroupMapper 
     where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'QuotesSettings')
	 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays'))

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Enable Quote Requests From Web Store',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'EnableQuoteRequest' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)


insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Quote Expire In (days)',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'QuoteExpireInDays' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Enable Cloudflare',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'IsCloudflareEnabled' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)
  
insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays'),(select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'AllowNegative')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays')
and InputValidationId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays'),(select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'AllowDecimals')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays')
and InputValidationId = (select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'AllowDecimals'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays'),(select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'MinNumber')
,null,'1',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays')
and InputValidationId = (select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'MinNumber'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays'),(select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'MaxNumber')
,null,'999',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays')
and InputValidationId = (select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'MaxNumber'))

insert into ZnodePortalGlobalAttributeValue(PortalId,GlobalAttributeId,GlobalAttributeDefaultValueId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select PortalId,(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableQuoteRequest'),null,null,2,getdate(),2,getdate()
from ZnodePortal ZP
where not exists(select * from ZnodePortalGlobalAttributeValue z where zp.PortalId = z.PortalId and z.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableQuoteRequest'))

insert into ZnodePortalGlobalAttributeValue(PortalId,GlobalAttributeId,GlobalAttributeDefaultValueId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select PortalId,(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays'),null,null,2,getdate(),2,getdate()
from ZnodePortal ZP
where not exists(select * from ZnodePortalGlobalAttributeValue z where zp.PortalId = z.PortalId and z.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays'))

insert into ZnodeGlobalAttributeGroup(GroupCode,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select 'QuotesSettings',null,2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttributeGroup where GroupCode = 'QuotesSettings')

update ZnodeGlobalAttributeGroupMapper set  GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'QuotesSettings')
where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableQuoteRequest')
and GlobalAttributeGroupId is null

update ZnodeGlobalAttributeGroupMapper set  GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'QuotesSettings')
where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays')
and GlobalAttributeGroupId is null


insert into ZnodeGlobalAttributeGroupLocale(LocaleId,GlobalAttributeGroupId,AttributeGroupName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeGroupId,'Quote Settings',null,2,getdate(),2,getdate()
from ZnodeGlobalAttributeGroup ag
where GroupCode = 'QuotesSettings' 
and not exists(select * from ZnodeGlobalAttributeGroupLocale agl where ag.GlobalAttributeGroupId = agl.GlobalAttributeGroupId and agl.LocaleId = 1)

insert into ZnodePortalGlobalAttributeValueLocale(PortalGlobalAttributeValueId,LocaleId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,GlobalAttributeDefaultValueId,MediaId,MediaPath)
select ZPGAV.PortalGlobalAttributeValueId, 1, 'true',2,getdate(),2,getdate(),null,null,null
from ZnodePortalGlobalAttributeValue ZPGAV
inner join ZnodePortal ZP ON ZP.PortalId = ZPGAV.PortalId
where ZPGAV.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableQuoteRequest')
and not exists(select * from ZnodePortalGlobalAttributeValueLocale ZPGAVL where ZPGAV.PortalGlobalAttributeValueId = ZPGAVL.PortalGlobalAttributeValueId and ZPGAVL.LocaleId = 1)

insert into ZnodePortalGlobalAttributeValueLocale(PortalGlobalAttributeValueId,LocaleId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,GlobalAttributeDefaultValueId,MediaId,MediaPath)
select ZPGAV.PortalGlobalAttributeValueId, 1, '30',2,getdate(),2,getdate(),null,null,null
from ZnodePortalGlobalAttributeValue ZPGAV
inner join ZnodePortal ZP ON ZP.PortalId = ZPGAV.PortalId
where ZPGAV.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays')
and not exists(select * from ZnodePortalGlobalAttributeValueLocale ZPGAVL where ZPGAV.PortalGlobalAttributeValueId = ZPGAVL.PortalGlobalAttributeValueId and ZPGAVL.LocaleId = 1)

insert into ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select GlobalAttributeGroupId,(select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'),9,2,getdate(),2,getdate()
from ZnodeGlobalAttributeGroup ZGAG
where GroupCode = 'QuotesSettings'
and not exists(Select * from ZnodeGlobalGroupEntityMapper ZGGEM where ZGAG.GlobalAttributeGroupId = ZGGEM.GlobalAttributeGroupId 
    and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'))

insert into ZnodePortalGlobalAttributeValueLocale(PortalGlobalAttributeValueId,LocaleId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,GlobalAttributeDefaultValueId,MediaId,MediaPath)
select ZPGAV.PortalGlobalAttributeValueId, 1, 'true',2,getdate(),2,getdate(),null,null,null
from ZnodePortalGlobalAttributeValue ZPGAV
inner join ZnodePortal ZP ON ZP.PortalId = ZPGAV.PortalId
where ZPGAV.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableQuoteRequest')
and not exists(select * from ZnodePortalGlobalAttributeValueLocale ZPGAVL where ZPGAV.PortalGlobalAttributeValueId = ZPGAVL.PortalGlobalAttributeValueId and ZPGAVL.LocaleId = 1)

insert into ZnodePortalGlobalAttributeValueLocale(PortalGlobalAttributeValueId,LocaleId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,GlobalAttributeDefaultValueId,MediaId,MediaPath)
select ZPGAV.PortalGlobalAttributeValueId, 1, '30',2,getdate(),2,getdate(),null,null,null
from ZnodePortalGlobalAttributeValue ZPGAV
inner join ZnodePortal ZP ON ZP.PortalId = ZPGAV.PortalId
where ZPGAV.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'QuoteExpireInDays')
and not exists(select * from ZnodePortalGlobalAttributeValueLocale ZPGAVL where ZPGAV.PortalGlobalAttributeValueId = ZPGAVL.PortalGlobalAttributeValueId and ZPGAVL.LocaleId = 1)

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder =2
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'WebstoreAuthentication') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store') 

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder =3
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Redirections') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store') 

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder =999
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Budgets') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'User') 

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder =4
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'EnableBudgetManagement') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store') 

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder = 999
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'OpenAccountBillingDetails') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'User') 

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder = 999
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'UserAddressSettings') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'User') 

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder = 9
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'StoreAddressSettings') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store') 


update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder = 10
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'QuotesSettings') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store') 

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder = 6
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Captcha') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store') 

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder = 7
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'ContentSecurityPolicy') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store') 

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder = 8
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'WarehouseStockLevels') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store') 

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder = 1
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Cloudflaresetting') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store') 

update ZnodeGlobalGroupEntityMapper set AttributeGroupDisplayOrder = 10
where GlobalAttributeGroupId = (Select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'QuotesSettings') 
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store') 

--dt 06-07-2020 ZPD-11276
insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Yes/No'),'EnablePowerBIReportOnWebStore',0,1,0,500,
null,2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'EnablePowerBIReportOnWebStore')

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Enable Power BI Report On WebStore',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'EnablePowerBIReportOnWebStore' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeDefaultValue(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnablePowerBIReportOnWebStore'),'false',null,null,null,null,null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeDefaultValue where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnablePowerBIReportOnWebStore') and AttributeDefaultValueCode = 'false')

insert into ZnodeGlobalAttributeGroup(GroupCode,	DisplayOrder,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate,	IsSystemDefined)
select 'PowerBISettings',	NULL,	2,	getdate(),	2,	getdate(),	0
where not exists(select * from ZnodeGlobalAttributeGroup where GroupCode = 'PowerBISettings')

insert into ZnodeGlobalAttributeGroupMapper(GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'PowerBISettings'),
      (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnablePowerBIReportOnWebStore'),
	  NULL,	2,	getdate(),	2,	getdate()
where not exists(select * from  ZnodeGlobalAttributeGroupMapper where GlobalAttributeGroupId =  (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'PowerBISettings')
      and  GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnablePowerBIReportOnWebStore'))

insert into ZnodeGlobalAttributeGroupLocale(LocaleId,GlobalAttributeGroupId,AttributeGroupName,Description
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'PowerBISettings'),'Power BI Settings',
       null, 2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeGroupLocale where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'PowerBISettings'))

insert into ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'PowerBISettings'),
       (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'User'), 999,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalGroupEntityMapper where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'PowerBISettings')
      and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'User'))

--dt 15-07-2020 ZPD-11382
update  ZnodeGlobalAttribute set DisplayOrder = 500 where AttributeCode = 'EnableQuoteRequest'

update  ZnodeGlobalAttribute set DisplayOrder = 499 where AttributeCode = 'QuoteExpireInDays'

---dt 30-07-2020 ZPD-11730
delete from ZnodeGlobalAttributeDefaultValueLocale where GlobalAttributeDefaultValueId in (
select GlobalAttributeDefaultValueId  from ZnodeGlobalAttributeDefaultValue where GlobalAttributeId in (
select GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode in ('firstname','lastname','workemailaddress','workphonenumber','businessname','city','zipcode')
))

delete from ZnodeGlobalAttributeDefaultValue where GlobalAttributeId in (
select GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode in ('firstname','lastname','workemailaddress','workphonenumber','businessname','city','zipcode')
)

--ZPD-1197
Declare @GlobalAttribute_EnableECertificate int,@GlobalAttribute_HideeProOrdersFromShopper int

select @GlobalAttribute_EnableECertificate = GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableECertificate'

select @GlobalAttribute_HideeProOrdersFromShopper = GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'HideeProOrdersFromShopper'

delete from ZnodeGlobalAttributeGroupMapper
where GlobalAttributeId = @GlobalAttribute_EnableECertificate

delete from ZnodeGlobalAttributeGroupMapper
where GlobalAttributeId = @GlobalAttribute_HideeProOrdersFromShopper

execute [Znode_DeleteGlobalAttribute] @GlobalAttributeId = @GlobalAttribute_EnableECertificate,@Status = 0
execute [Znode_DeleteGlobalAttribute] @GlobalAttributeId = @GlobalAttribute_HideeProOrdersFromShopper, @Status = 0

--ZPD-11804 : dt- 16-09-2020
insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder, HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined,GlobalEntityId)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Number'),'AccountVoucherExpirationReminderEmailInDays',0,0,1,500,
'The value indicates that the set number of days prior to the expiration date a reminder email notification will be sent to the customers to use the voucher',2,getdate(),2,getdate(),0,(select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account')
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'AccountVoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account'))

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Voucher Expiration Reminder Email (In Days)',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'AccountVoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account') and
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)


insert into ZnodeGlobalAttributeGroup(GroupCode, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsSystemDefined, GlobalEntityId)
select 'AccountVoucherSettings', NULL, 2, getdate(), 2, getdate(), 0, (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account')
where not exists(select * from ZnodeGlobalAttributeGroup where GroupCode = 'AccountVoucherSettings' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account'))


insert into ZnodeGlobalAttributeGroupLocale(LocaleId,GlobalAttributeGroupId,AttributeGroupName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeGroupId,'Voucher Settings',null,2,getdate(),2,getdate()
from ZnodeGlobalAttributeGroup a
where a.GroupCode = 'AccountVoucherSettings' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account') and
not exists(select * from ZnodeGlobalAttributeGroupLocale b where a.GlobalAttributeGroupId = b.GlobalAttributeGroupId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeGroupMapper(GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'AccountVoucherSettings'  AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account')),
      (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountVoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account')),
	  NULL,	2,	getdate(),	2,	getdate()
where not exists(select * from  ZnodeGlobalAttributeGroupMapper where GlobalAttributeGroupId =  (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'AccountVoucherSettings' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account'))
      and  GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountVoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account')))

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Account'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'AccountVoucherSettings' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account')), 999,
2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Account') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'AccountVoucherSettings' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account'))
)


insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'AccountVoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account')),(select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'AllowDecimals')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'AccountVoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account'))
and InputValidationId = (select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'AllowDecimals'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'AccountVoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account')),(select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'AllowNegative')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'AccountVoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account'))
and InputValidationId = (select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'AllowNegative'))


insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'VoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store')),(select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'AllowDecimals')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'VoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'))
and InputValidationId = (select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'AllowDecimals'))


insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'VoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store')),(select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'AllowNegative')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'VoucherExpirationReminderEmailInDays' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'))
and InputValidationId = (select top 1 InputValidationId from znodeattributeinputvalidation  where Name = 'AllowNegative'))

------ZPD-14822
update ZnodeGlobalAttribute
set AttributeTypeId = (select top 1  AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Text Area')
where AttributeCode = 'ContentSecurityPolicy'

--ZPD-15172
INSERT INTO ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder, HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined,GlobalEntityId)
SELECT (SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName = 'Text'),'BusinessIdentificationNumber',0,1,0,500,
NULL,2,GETDATE(),2,GETDATE(),0,(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttribute WHERE  AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account'))

INSERT INTO ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1,GlobalAttributeId,'Business Identification Number',NULL,2,GETDATE(),2,GETDATE()
FROM ZnodeGlobalAttribute a
WHERE AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account') and
NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeLocale b WHERE a.GlobalAttributeId = b.GlobalAttributeId AND b.LocaleId = 1)

INSERT INTO ZnodeGlobalAttributeGroup(GroupCode, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsSystemDefined, GlobalEntityId)
SELECT 'BusinessIdentificationNumber', NULL, 2, GETDATE(), 2, GETDATE(), 0, (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account'))

INSERT INTO ZnodeGlobalAttributeGroupLocale(LocaleId,GlobalAttributeGroupId,AttributeGroupName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1,GlobalAttributeGroupId,'Business Details',NULL,2,GETDATE(),2,GETDATE()
FROM ZnodeGlobalAttributeGroup a
WHERE a.GroupCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account') and
NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeGroupLocale b WHERE a.GlobalAttributeGroupId = b.GlobalAttributeGroupId AND b.LocaleId = 1)

INSERT INTO ZnodeGlobalAttributeGroupMapper(GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'BusinessIdentificationNumber'  AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')),
      (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE  AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')),
	  NULL,	2,	GETDATE(),	2,	GETDATE()
WHERE NOT EXISTS(SELECT * FROM  ZnodeGlobalAttributeGroupMapper WHERE GlobalAttributeGroupId =  (SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account'))
      AND  GlobalAttributeId = (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE  AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')))

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Account'),
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')), 999,
2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Account') and
GlobalAttributeGroupId = (SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account'))
)


INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')),(SELECT TOP 1 InputValidationId FROM znodeattributeinputvalidation  WHERE Name = 'ValidationRule')
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account'))
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM znodeattributeinputvalidation  WHERE Name = 'ValidationRule'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')),(SELECT TOP 1 InputValidationId FROM znodeattributeinputvalidation  WHERE Name = 'RegularExpression')
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account'))
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM znodeattributeinputvalidation  WHERE Name = 'RegularExpression'))


INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')),(SELECT TOP 1 InputValidationId FROM znodeattributeinputvalidation  WHERE Name = 'MaxCharacters' AND AttributeTypeId = 5)
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account'))
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM znodeattributeinputvalidation  WHERE Name = 'MaxCharacters' AND AttributeTypeId = 5) )


INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')),(SELECT TOP 1 InputValidationId FROM znodeattributeinputvalidation  WHERE Name = 'UniqueValue')
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account'))
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM znodeattributeinputvalidation  WHERE Name = 'UniqueValue'))

DECLARE @AccountGlobalAttributeValue TABLE (AccountId INT,AccountGlobalAttributeValueId  INT)
INSERT INTO ZnodeAccountGlobalAttributeValue(AccountId,GlobalAttributeId,GlobalAttributeDefaultValueId,AttributeValue
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
OUTPUT INSERTED.AccountGlobalAttributeValueId,INSERTED.AccountId INTO @AccountGlobalAttributeValue(AccountGlobalAttributeValueId,AccountId)
SELECT ZA.AccountId, (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE  AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account')),
	NULL,NULL,2,GETDATE(),2,GETDATE()
FROM ZnodeAccount ZA
WHERE NOT EXISTS(SELECT * FROM ZnodeAccountGlobalAttributeValue Z WHERE ZA.AccountId = Z.AccountId
	AND GlobalAttributeId = (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE  AttributeCode = 'BusinessIdentificationNumber' AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Account'))
	)

INSERT INTO ZnodeAccountGlobalAttributeValueLocale(AccountGlobalAttributeValueId,LocaleId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,GlobalAttributeDefaultValueId,MediaId,MediaPath)
SELECT AccountGlobalAttributeValueId,1,NULL,2,GETDATE(),2,GETDATE(),NULL,NULL,NULL
FROM @AccountGlobalAttributeValue

Update ZnodeGlobalAttributeGroupLocale set AttributeGroupName = 'Business Details'
where AttributeGroupName = 'Buisness Identification Number'

UPDATE ZnodeGlobalAttribute SET HelpDescription = 'The input saved against this field will be used by all the customer accounts associated with this Account.'
WHERE AttributeCode = 'BusinessIdentificationNumber'

UPDATE ZnodeGlobalAttributeGrouplocale 
SET AttributeGroupName = 'Business Details'
WHERE GlobalAttributeGroupId = (SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'BusinessIdentificationNumber')
AND LocaleId = 1


insert into ZnodeGlobalAttributeGroup(GroupCode,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select 'Captcha',null,2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttributeGroup where GroupCode = 'Captcha')

insert into ZnodeGlobalAttributeGroupLocale(LocaleId,GlobalAttributeGroupId,AttributeGroupName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeGroupId,'Captcha',null,2,getdate(),2,getdate()
from ZnodeGlobalAttributeGroup a
where GroupCode = 'Captcha'
and not exists(select * from ZnodeGlobalAttributeGroupLocale b where b.GlobalAttributeGroupId= a.GlobalAttributeGroupId and b.LocaleId = 1)

insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined,GlobalEntityId)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Yes/No'),'IsCaptchaRequired',0,1,0,2,
'When enabled, the Captcha will be visible on the “Contact Us”, “Feedback Form” pages.',2,getdate(),2,getdate(),0,1
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'IsCaptchaRequired')

insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined,GlobalEntityId)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Yes/No'),'IsCaptchaRequiredForLogin',0,1,0,2,
'IsCaptchaRequiredForLogin',2,getdate(),2,getdate(),0,1
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'IsCaptchaRequiredForLogin')

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Is Captcha Required For Forms',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'IsCaptchaRequired' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Is Captcha Required For Login',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'IsCaptchaRequiredForLogin' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)


insert into ZnodeGlobalAttributeGroupMapper (GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Captcha'),
(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'IsCaptchaRequired'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeGroupMapper 
     where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Captcha')
	 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'IsCaptchaRequired'))

insert into ZnodeGlobalAttributeGroupMapper (GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Captcha'),
(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'IsCaptchaRequiredForLogin'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeGroupMapper 
     where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Captcha')
	 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'IsCaptchaRequiredForLogin'))

insert into ZnodePortalGlobalAttributeValue(PortalId,GlobalAttributeId,GlobalAttributeDefaultValueId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select PortalId,(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'IsCaptchaRequired'),null,null,2,getdate(),2,getdate()
from ZnodePortal ZP
where not exists(select * from ZnodePortalGlobalAttributeValue ZPGA where ZP.PortalId = ZPGA.PortalId
      and ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'IsCaptchaRequired'))

insert into ZnodePortalGlobalAttributeValueLocale(PortalGlobalAttributeValueId,	LocaleId,	AttributeValue	,CreatedBy,	CreatedDate,	ModifiedBy	,ModifiedDate)
select PortalGlobalAttributeValueId,1,'true',2,getdate(),2,getdate()
from ZnodePortalGlobalAttributeValue ZPGA
where ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'IsCaptchaRequired')
and not exists(select * from ZnodePortalGlobalAttributeValueLocale ZPGAVL where ZPGAVL.PortalGlobalAttributeValueId = ZPGA.PortalGlobalAttributeValueId)

insert into ZnodePortalGlobalAttributeValue(PortalId,GlobalAttributeId,GlobalAttributeDefaultValueId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select PortalId,(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'IsCaptchaRequiredForLogin'),null,null,2,getdate(),2,getdate()
from ZnodePortal ZP
where not exists(select * from ZnodePortalGlobalAttributeValue ZPGA where ZP.PortalId = ZPGA.PortalId
      and ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'IsCaptchaRequiredForLogin'))

insert into ZnodePortalGlobalAttributeValueLocale(PortalGlobalAttributeValueId,	LocaleId,	AttributeValue	,CreatedBy,	CreatedDate,	ModifiedBy	,ModifiedDate)
select PortalGlobalAttributeValueId,1,'false',2,getdate(),2,getdate()
from ZnodePortalGlobalAttributeValue ZPGA
where ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'IsCaptchaRequiredForLogin')
and not exists(select * from ZnodePortalGlobalAttributeValueLocale ZPGAVL where ZPGAVL.PortalGlobalAttributeValueId = ZPGA.PortalGlobalAttributeValueId)

update ZnodeGlobalAttribute set HelpDescription = 'When enabled, the Captcha will be visible on the “Contact Us”, “Feedback Form” pages.'
where AttributeCode = 'IsCaptchaRequired'
update ZnodeGlobalAttribute set HelpDescription = 'When enabled, the Captcha will be visible on the “Login” page'
where AttributeCode = 'IsCaptchaRequiredForLogin'
----------ZPD-14025

insert into ZnodeGlobalAttribute
(AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsActive, DisplayOrder,HelpDescription,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate, IsSystemDefined, GlobalEntityId)
select 
(select top 1  AttributeTypeId from ZnodeAttributeType where AttributeTypeName ='Number' ),N'SuccessBufferTime',1,1,0,14,'Success Buffer Time?',2,getdate(),2,getdate(),0 , (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where not exists (select AttributeCode from  ZnodeGlobalAttribute where AttributeCode ='SuccessBufferTime' )

insert into ZnodeGlobalAttribute
(AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsActive, DisplayOrder,HelpDescription,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate, IsSystemDefined, GlobalEntityId)
select 
(select top 1  AttributeTypeId from ZnodeAttributeType where AttributeTypeName ='Number' ),N'SuccessRandomTime',1,1,0,14,'Success Random Time?',2,getdate(),2,getdate(),0, (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where not exists  (select AttributeCode from  ZnodeGlobalAttribute where AttributeCode ='SuccessRandomTime')

insert into ZnodeGlobalAttribute
(AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsActive, DisplayOrder,HelpDescription,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate, IsSystemDefined, GlobalEntityId)
select 
(select top 1  AttributeTypeId from ZnodeAttributeType where AttributeTypeName ='Number' ),N'ErrorBufferTime',1,1,0,14,'Error Buffer Time?',2,getdate(),2,getdate(),0, (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where not exists  (select AttributeCode from  ZnodeGlobalAttribute where AttributeCode ='ErrorBufferTime')

insert into ZnodeGlobalAttribute
(AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsActive, DisplayOrder,HelpDescription,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate, IsSystemDefined, GlobalEntityId)
select 
(select top 1  AttributeTypeId from ZnodeAttributeType where AttributeTypeName ='Number' ),N'ErrorRandomTime',1,1,0,14,'Error Random Time?',2,getdate(),2,getdate(),0, (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where not exists  (select AttributeCode from  ZnodeGlobalAttribute where AttributeCode ='ErrorRandomTime')

insert into ZnodeGlobalAttribute
(AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsActive, DisplayOrder,HelpDescription,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate, IsSystemDefined, GlobalEntityId)
select 
(select top 1  AttributeTypeId from ZnodeAttributeType where AttributeTypeName ='Number' ),N'SuccessSignUpBufferTime',1,1,0,14,'Success SignUp Buffer Time?',2,getdate(),2,getdate(),0, (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where not exists  (select AttributeCode from  ZnodeGlobalAttribute where AttributeCode ='ErrorRandomTime')

insert into ZnodeGlobalAttribute
(AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsActive, DisplayOrder,HelpDescription,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate, IsSystemDefined, GlobalEntityId)
select 
(select top 1  AttributeTypeId from ZnodeAttributeType where AttributeTypeName ='Number' ),N'SuccessSignUpRandomTime',1,1,0,14,'Success SignUp Random Time?',2,getdate(),2,getdate(),0, (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where not exists  (select AttributeCode from  ZnodeGlobalAttribute where AttributeCode ='ErrorRandomTime')

insert into ZnodeGlobalAttribute
(AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsActive, DisplayOrder,HelpDescription,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate, IsSystemDefined, GlobalEntityId)
select 
(select top 1  AttributeTypeId from ZnodeAttributeType where AttributeTypeName ='Number' ),N'ErrorSignUpBufferTime',1,1,0,14,'Error SignUp Buffer Time?',2,getdate(),2,getdate(),0, (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where not exists  (select AttributeCode from  ZnodeGlobalAttribute where AttributeCode ='ErrorRandomTime')

insert into ZnodeGlobalAttribute
(AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsActive, DisplayOrder,HelpDescription,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate, IsSystemDefined, GlobalEntityId)
select 
(select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName ='Number' ),N'ErrorSignUpRandomTime',1,1,0,14,'Error SignUp Random Time?',2,getdate(),2,getdate(),0, (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where not exists  (select AttributeCode from  ZnodeGlobalAttribute where AttributeCode ='ErrorRandomTime')

INSERT INTO ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,
CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1,(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='SuccessBufferTime'),'Success Buffer Time',
NULL,2,GETDATE(),2,GETDATE()
where not exists (select * from ZnodeGlobalAttributeLocale 
where GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='SuccessBufferTime'))

INSERT INTO ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,
CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1,(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='SuccessRandomTime'),'Success Random Time',
NULL,2,GETDATE(),2,GETDATE()
where not exists (select * from ZnodeGlobalAttributeLocale 
where GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='SuccessRandomTime'))

INSERT INTO ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,
CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1,(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ErrorBufferTime'),'Error Buffer Time',
NULL,2,GETDATE(),2,GETDATE()
where not exists (select * from ZnodeGlobalAttributeLocale 
where GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ErrorBufferTime'))


INSERT INTO ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,
CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1,(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ErrorRandomTime'),'Error Random Time',
NULL,2,GETDATE(),2,GETDATE()
where not exists (select * from ZnodeGlobalAttributeLocale 
where GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ErrorRandomTime'))

insert into ZnodeGlobalAttributeGroup
(GroupCode,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined,GlobalEntityId)
select N'DelayTimeConfiguration', NULL, 2, GETDATE(), 2, GETDATE(), 0, (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where  not exists   (select top 1 GroupCode from ZnodeGlobalAttributeGroup where GroupCode =N'DelayTimeConfiguration') 

INSERT INTO ZnodeGlobalAttributeGroupLocale(LocaleId,GlobalAttributeGroupId,AttributeGroupName,Description,CreatedBy,CreatedDate,
ModifiedBy,ModifiedDate)
SELECT 1,(SELECT TOP 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode=N'DelayTimeConfiguration'),'Delay Time Configuration',NULL,2,
GETDATE(),2,GETDATE()
where not exists (select * from ZnodeGlobalAttributeGroupLocale 
where GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode=N'DelayTimeConfiguration'))

Insert into ZnodeGlobalAttributeDefaultValue
(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select top 1  GlobalAttributeId, N'0',NULL,NULL,NULL,NULL,NULL,2,Getdate(),2,Getdate()
from ZnodeGlobalAttribute where AttributeCode ='SuccessBufferTime' and  GlobalAttributeId not in (select GlobalAttributeId from ZnodeGlobalAttributeDefaultValue)

Insert into ZnodeGlobalAttributeDefaultValue
(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select top 1  GlobalAttributeId, N'0',NULL,NULL,NULL,NULL,NULL,2,Getdate(),2,Getdate()
from ZnodeGlobalAttribute where AttributeCode ='SuccessRandomTime' and  GlobalAttributeId not in (select GlobalAttributeId from ZnodeGlobalAttributeDefaultValue)

Insert into ZnodeGlobalAttributeDefaultValue
(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select top 1  GlobalAttributeId, N'0',NULL,NULL,NULL,NULL,NULL,2,Getdate(),2,Getdate()
from ZnodeGlobalAttribute where AttributeCode ='ErrorBufferTime' and  GlobalAttributeId not in (select GlobalAttributeId from ZnodeGlobalAttributeDefaultValue)

Insert into ZnodeGlobalAttributeDefaultValue
(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select top 1  GlobalAttributeId, N'0',NULL,NULL,NULL,NULL,NULL,2,Getdate(),2,Getdate()
from ZnodeGlobalAttribute where AttributeCode ='ErrorRandomTime' and  GlobalAttributeId not in (select GlobalAttributeId from ZnodeGlobalAttributeDefaultValue)
--
Insert into ZnodeGlobalAttributeDefaultValue
(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select top 1  GlobalAttributeId, N'0',NULL,NULL,NULL,NULL,NULL,2,Getdate(),2,Getdate()
from ZnodeGlobalAttribute where AttributeCode ='SuccessSignUpBufferTime' and  GlobalAttributeId not in (select GlobalAttributeId from ZnodeGlobalAttributeDefaultValue)

Insert into ZnodeGlobalAttributeDefaultValue
(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select top 1  GlobalAttributeId, N'0',NULL,NULL,NULL,NULL,NULL,2,Getdate(),2,Getdate()
from ZnodeGlobalAttribute where AttributeCode ='SuccessSignUpRandomTime' and  GlobalAttributeId not in (select GlobalAttributeId from ZnodeGlobalAttributeDefaultValue)

Insert into ZnodeGlobalAttributeDefaultValue
(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select top 1  GlobalAttributeId, N'0',NULL,NULL,NULL,NULL,NULL,2,Getdate(),2,Getdate()
from ZnodeGlobalAttribute where AttributeCode ='ErrorSignUpBufferTime' and  GlobalAttributeId not in (select GlobalAttributeId from ZnodeGlobalAttributeDefaultValue)

Insert into ZnodeGlobalAttributeDefaultValue
(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select top 1  GlobalAttributeId, N'0',NULL,NULL,NULL,NULL,NULL,2,Getdate(),2,Getdate()
from ZnodeGlobalAttribute where AttributeCode ='ErrorSignUpRandomTime' and  GlobalAttributeId not in (select GlobalAttributeId from ZnodeGlobalAttributeDefaultValue)

INSERT INTO ZnodeGlobalAttributeDefaultValueLocale(LocaleId,GlobalAttributeDefaultValueId,AttributeDefaultValue,Description,CreatedBy,CreatedDate,
ModifiedBy,ModifiedDate)
SELECT 1,(SELECT TOP 1 GlobalAttributeDefaultValueId from ZnodeGlobalAttributeDefaultValue where AttributeDefaultValueCode='0'),'0',NULL,2,
GETDATE(),2,GETDATE()
where not exists (select * from ZnodeGlobalAttributeDefaultValueLocale
where GlobalAttributeDefaultValueId=(SELECT TOP 1 GlobalAttributeDefaultValueId from ZnodeGlobalAttributeDefaultValue where AttributeDefaultValueCode='0'))

insert into ZnodeGlobalAttributeGroup
(GroupCode,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined,GlobalEntityId)
select N'DelayTimeConfiguration', NULL, 2, GETDATE(), 2, GETDATE(), 0, (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where  not exists   (select top 1 GroupCode from ZnodeGlobalAttributeGroup where GroupCode =N'DelayTimeConfiguration') 

--ZPD-17466

insert into ZnodeGlobalAttribute
(AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsActive, DisplayOrder,HelpDescription,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate, IsSystemDefined, GlobalEntityId)
select 
(select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName ='Yes/No' ),N'EnableReturnRequest',0,1,1,2,'When enabled, customers will be able to create return requests for the eligible orders from the web store',2,getdate(),2,getdate(),0, (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where not exists  (select AttributeCode from  ZnodeGlobalAttribute where AttributeCode ='EnableReturnRequest')

INSERT INTO ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,
CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1,(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='EnableReturnRequest'),'Enable Return Requests From Webstore',
NULL,2,GETDATE(),2,GETDATE()
where not exists (select * from ZnodeGlobalAttributeLocale 
	where GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='EnableReturnRequest')
	and LocaleId = 1)
	
insert into ZnodeGlobalAttributeGroup
(GroupCode,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined,GlobalEntityId)
select N'ReturnSetting', NULL, 2, GETDATE(), 2, GETDATE(), 0, (select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where  not exists   (select top 1 GroupCode from ZnodeGlobalAttributeGroup where GroupCode =N'ReturnSetting') 

INSERT INTO ZnodeGlobalAttributeGroupLocale(LocaleId,GlobalAttributeGroupId,AttributeGroupName,Description,CreatedBy,CreatedDate,
ModifiedBy,ModifiedDate)
SELECT 1,(SELECT TOP 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode=N'ReturnSetting'),'Return Setting',NULL,2,
GETDATE(),2,GETDATE()
where not exists (select * from ZnodeGlobalAttributeGroupLocale 
where GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode=N'ReturnSetting') 
and LocaleId = 1)


insert into ZnodeGlobalAttributeGroupMapper (GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'ReturnSetting'),
(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableReturnRequest'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeGroupMapper 
     where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'ReturnSetting')
	 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableReturnRequest'))


INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'ReturnSetting' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store')), 9r,
2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'ReturnSetting' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'))
)

INSERT INTO ZnodeGlobalGroupEntityMapper (GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'ReturnSetting'),(select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store') ,
	9,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalGroupEntityMapper WHERE  GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'ReturnSetting') AND 
		GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'))

insert into ZnodePortalGlobalAttributeValue(PortalId,GlobalAttributeId,GlobalAttributeDefaultValueId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select PortalId,(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnableReturnRequest'),null,null,2,getdate(),2,getdate()
from ZnodePortal ZP
where not exists(select * from ZnodePortalGlobalAttributeValue ZPGA where ZP.PortalId = ZPGA.PortalId
      and ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnableReturnRequest'))

insert into ZnodePortalGlobalAttributeValueLocale(PortalGlobalAttributeValueId,	LocaleId,	AttributeValue	,CreatedBy,	CreatedDate,	ModifiedBy	,ModifiedDate)
select PortalGlobalAttributeValueId,1,'true',2,getdate(),2,getdate()
from ZnodePortalGlobalAttributeValue ZPGA
where ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnableReturnRequest')
and not exists(select * from ZnodePortalGlobalAttributeValueLocale ZPGAVL where ZPGAVL.PortalGlobalAttributeValueId = ZPGA.PortalGlobalAttributeValueId
and LocaleId = 1)

update ZnodeGlobalFamilyGroupMapper set AttributeGroupDisplayOrder=1 
where GlobalAttributeGroupId=(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'WebstoreAuthentication')
update ZnodeGlobalFamilyGroupMapper set AttributeGroupDisplayOrder=2 
where GlobalAttributeGroupId=(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Redirections')
update ZnodeGlobalFamilyGroupMapper set AttributeGroupDisplayOrder=3 
where GlobalAttributeGroupId=(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'EnableBudgetManagement')
update ZnodeGlobalFamilyGroupMapper set AttributeGroupDisplayOrder=4 
where GlobalAttributeGroupId=(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'StoreAddressSettings')
update ZnodeGlobalFamilyGroupMapper set AttributeGroupDisplayOrder=5 
where GlobalAttributeGroupId=(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Captcha')
update ZnodeGlobalFamilyGroupMapper set AttributeGroupDisplayOrder=6 
where GlobalAttributeGroupId=(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'WarehouseStockLevels')
update ZnodeGlobalFamilyGroupMapper set AttributeGroupDisplayOrder=7 
where GlobalAttributeGroupId=(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'Cloudflaresetting')
update ZnodeGlobalFamilyGroupMapper set AttributeGroupDisplayOrder=8 
where GlobalAttributeGroupId=(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'QuotesSettings')
update ZnodeGlobalFamilyGroupMapper set AttributeGroupDisplayOrder=9 
where GlobalAttributeGroupId=(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'ReturnSetting')


update ZnodeGlobalAttribute set AttributeCode='LoginToSeePricingAndInventory',
HelpDescription='Enabling this will make it mandatory for customers to log in to their account, in order to see the Price and Stock information of products on the web-store.' 
where AttributeCode='LoginToSeePricing'

update ZnodeGlobalAttributeLocale set AttributeName='Login To See Pricing And Inventory'
where AttributeName='Login To See Pricing'

--ZPD-18339
update ZnodeGlobalAttributeGroupLocale 
set AttributeGroupName='Warehouse Settings' 
where AttributeGroupName ='Stock Levels By Warehouse'

insert into ZnodeGlobalAttribute
(AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsActive, DisplayOrder,HelpDescription,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate, IsSystemDefined, GlobalEntityId)
select 
(select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName ='Yes/No' ),N'EnableInventoryStockNotification',0,1,1,2,
'When this setting is set as Yes and saved, Users will have the capability to register 
	for products that are out of stock and be notified when the product is back in stock',2,getdate(),2,getdate(),0, 
(select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where not exists  (select AttributeCode from  ZnodeGlobalAttribute where AttributeCode ='EnableInventoryStockNotification')

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,
CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode='EnableInventoryStockNotification'),
'Enable Inventory Stock Notification',
NULL,2,GETDATE(),2,GETDATE()
where not exists (select * from ZnodeGlobalAttributeLocale 
	where GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='EnableInventoryStockNotification')
	and LocaleId = 1)

insert into ZnodeGlobalAttributeGroupMapper (GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'WarehouseStockLevels'),
(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableInventoryStockNotification'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeGroupMapper 
     where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'WarehouseStockLevels')
	 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableInventoryStockNotification'))

insert into ZnodePortalGlobalAttributeValue(PortalId,GlobalAttributeId,GlobalAttributeDefaultValueId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select PortalId,(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnableInventoryStockNotification'),null,null,2,getdate(),2,getdate()
from ZnodePortal ZP
where not exists(select * from ZnodePortalGlobalAttributeValue ZPGA where ZP.PortalId = ZPGA.PortalId
      and ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnableInventoryStockNotification'))

insert into ZnodePortalGlobalAttributeValueLocale(PortalGlobalAttributeValueId,	LocaleId,	AttributeValue	,CreatedBy,	CreatedDate,	ModifiedBy	,ModifiedDate)
select PortalGlobalAttributeValueId,1,'true',2,getdate(),2,getdate()
from ZnodePortalGlobalAttributeValue ZPGA
where ZPGA.GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'EnableInventoryStockNotification')
and not exists(select * from ZnodePortalGlobalAttributeValueLocale ZPGAVL where ZPGAVL.PortalGlobalAttributeValueId = ZPGA.PortalGlobalAttributeValueId
and LocaleId = 1)

--ZPD-18726

update ZnodeGlobalAttributeGroupLocale 
set AttributeGroupName='Return Settings' 
where AttributeGroupName='Return Setting'

--ZPD-19378

update ZnodeGlobalAttribute 
set HelpDescription='Enabling this will make it mandatory for the customers to login, before they perform any activities on the webstore.'
where HelpDescription='Enabling this will make it mandatory for the customers to login, before they perform any activities on the web store.'

update ZnodeGlobalAttribute 
set HelpDescription='Enabling this will make it mandatory for customers to log in to their account, in order to see the Price and Stock information of products on the webstore.'
where HelpDescription='Enabling this will make it mandatory for customers to log in to their account, in order to see the Price and Stock information of products on the web-store.'

update ZnodeGlobalAttribute 
set HelpDescription='When Yes is selected, the stock level of products from default warehouse and total stock level from all the warehouses will get displayed on the product listing page on the webstore. When No is selected, the total stock level from all warehouses will get displayed.'
where HelpDescription='When Yes is selected, the stock level of products from default warehouse and total stock level from all the warehouses will get displayed on the product listing page on the web-store. When No is selected, the total stock level from all warehouses will get displayed.'

update ZnodeGlobalAttribute 
set HelpDescription='When enabled, customers will be able to create Quote Requests from webstore.'
where HelpDescription='When enabled, customers will be able to create Quote Requests from web store.'

update ZnodeGlobalAttribute 
set HelpDescription='When enabled, customers will be able to create return requests for the eligible orders from the webstore.'
where HelpDescription='When enabled, customers will be able to create return requests for the eligible orders from the web store'

--ZPD-19378 Dt.08-June-2022
UPDATE ZnodeGlobalAttribute 
SET HelpDescription='hello world.. Enabling this will make it mandatory for the customers to login, before performing any activities on the webstore.'
WHERE HelpDescription='hello world.. Enabling this will make it mandatory for the customers to login, before performing any activities on the web store.'

--dt 10/10/2022 ZPD-8556 --> ZPD-21048

insert into ZnodeGlobalAttribute
(AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsActive, DisplayOrder,HelpDescription,CreatedBy, CreatedDate, ModifiedBy,ModifiedDate, IsSystemDefined, GlobalEntityId)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName ='Yes/No' ),'EnableTradeCentric',0,1,1,2,
'When this setting is checked and saved, Znode can be accessed through the TradeCentric.',2,getdate(),2,getdate(),0, 
(select top 1 GlobalEntityId from znodeGlobalEntity where EntityName ='Store')
where not exists  (select AttributeCode from  ZnodeGlobalAttribute where AttributeCode ='EnableTradeCentric')

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,
CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode='EnableTradeCentric'),
'Enable TradeCentric',NULL,2,GETDATE(),2,GETDATE()
where not exists (select * from ZnodeGlobalAttributeLocale 
where GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='EnableTradeCentric') and LocaleId = 1)

insert into ZnodeGlobalAttributeGroup(GroupCode,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined,GlobalEntityId)
select 'TradeCentric',null,2,getdate(),2,getdate(),0,1
where not exists(select * from ZnodeGlobalAttributeGroup where GroupCode = 'TradeCentric')

insert into ZnodeGlobalAttributeGroupLocale(LocaleId,GlobalAttributeGroupId,AttributeGroupName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeGroupId,'TradeCentric',null,2,getdate(),2,getdate()
from ZnodeGlobalAttributeGroup a
where GroupCode = 'TradeCentric'
and not exists(select * from ZnodeGlobalAttributeGroupLocale b where b.GlobalAttributeGroupId= a.GlobalAttributeGroupId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeGroupMapper (GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'TradeCentric'),
(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableTradeCentric'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeGroupMapper 
where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'TradeCentric')
and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'EnableTradeCentric'))
	 
insert into ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'TradeCentric'),
(select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'),1, 2,getdate(),2,GETDATE()
where not exists(select * from ZnodeGlobalGroupEntityMapper where GlobalAttributeGroupId=(select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'TradeCentric')
and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'))

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId, GlobalAttributeGroupId, AttributeGroupDisplayOrder,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT
(SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store'),
(SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'TradeCentric' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store')), 9r,
2, getdate(), 2, getdate()
WHERE NOT EXISTS(select * from ZnodeGlobalFamilyGroupMapper where
GlobalAttributeFamilyId = (SELECT top 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = 'Store') and
GlobalAttributeGroupId = (SELECT top 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'TradeCentric' AND GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'))
)

--dt 11/11/2022 --> ZPD-22870

INSERT INTO ZnodeAttributeInputValidationRule
	(InputValidationId,ValidationRule,ValidationName,DisplayOrder,RegExp,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1)),
	NULL,'.webp',7,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeAttributeInputValidationRule WHERE InputValidationId =(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1))
	AND ValidationName = '.webp')


INSERT INTO ZnodeGlobalAttributeValidation 
	(GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage'),
	(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1))
	,(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp')
	,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage')
	AND InputValidationId = (SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1))
	AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'))

INSERT INTO ZnodeGlobalAttributeValidation 
	(GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage'),
	(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1))
	,(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp')
	,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage')
	AND InputValidationId = (SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1))
	AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'))

INSERT INTO ZnodeGlobalAttributeValidation 
	(GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1'),
	(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1))
	,(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp')
	,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1')
	AND InputValidationId = (SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1))
	AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'))

INSERT INTO ZnodeGlobalAttributeValidation 
	(GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2'),
	(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1))
	,(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp')
	,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2')
	AND InputValidationId = (SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1))
	AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'))

--dt 29/11/2022 --> ZPD-22428
update ZnodeGlobalAttribute 
set HelpDescription='When this setting is enabled and saved, Znode can be accessed through the TradeCentric.'
where HelpDescription='When this setting is checked and saved, Znode can be accessed through the TradeCentric.'