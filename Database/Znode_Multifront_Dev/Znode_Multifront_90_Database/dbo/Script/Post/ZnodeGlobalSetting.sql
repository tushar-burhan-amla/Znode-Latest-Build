
BEGIN
UPDATE ZnodeGlobalSetting set FeatureValues='False',ModifiedDate=GETDATE(),ModifiedBy=2 where FeatureName='IsLoggingLevelsEnabledWarning' 
UPDATE ZnodeGlobalSetting set FeatureValues='False',ModifiedDate=GETDATE(),ModifiedBy=2 where FeatureName='IsLoggingLevelsEnabledInfo' 
UPDATE ZnodeGlobalSetting set FeatureValues='False',ModifiedDate=GETDATE(),ModifiedBy=2 where FeatureName='IsLoggingLevelsEnabledDebug' 
UPDATE ZnodeGlobalSetting set FeatureValues='True',ModifiedDate=GETDATE(),ModifiedBy=2 where FeatureName='IsLoggingLevelsEnabledError' 
UPDATE ZnodeGlobalSetting set FeatureValues='False',ModifiedDate=GETDATE(),ModifiedBy=2 where FeatureName='IsLoggingLevelsEnabledAll' 
UPDATE ZnodeGlobalSetting set FeatureValues='False',ModifiedDate=GETDATE(),ModifiedBy=2 where FeatureName='IsLoggingLevelsEnabledFatal' 
END

GO

--dt\13\08\2019 ZPD-7000

INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'ClearLoadBalancerAPICacheIPs','False',NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'ClearLoadBalancerAPICacheIPs')
GO


INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'ClearLoadBalancerWebStoreCacheIPs','False',NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'ClearLoadBalancerWebStoreCacheIPs')
GO

--dt 20-11-2019 ZPD-8051
INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'DefaultProductLimitForRecommendations','12',NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'DefaultProductLimitForRecommendations')
GO

--dt 27-01-2020 ZPD-7813 --> ZPD-8907
insert into ZnodeGlobalSetting(FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'AnalyticsJSONKey','',
null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalSetting where FeatureName = 'AnalyticsJSONKey')

--dt 29-01-2020 ZPD-7813 --> ZPD-8907
update ZnodeGlobalSetting set FeatureValues = null
where FeatureName = 'AnalyticsJSONKey'

--dt 29-01-2020 ZPD-7813 --> ZPD-8907
update ZnodeGlobalSetting set FeatureValues = ''
where FeatureName = 'AnalyticsJSONKey'


--dt\19\03\2020 ZPD-9489

INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'IsCalculateTaxAfterDiscount','False',NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'IsCalculateTaxAfterDiscount')
GO

INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'AllowedPromotions','ZnodeCartPromotionAmountOffCatalog,ZnodeCartPromotionAmountOffOrder',NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'AllowedPromotions')
GO


--dt 25-03-2020 ZPD-9187
update ZnodeGlobalSetting set FeatureSubValues =  FeatureSubValues+',CustomCode'
where FeatureName = 'SaveOrderAttribute' and FeatureSubValues not like '%CustomCode%'

Go
--dt 27-03-2020 ZPD-9489
INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'CMSPageSearchableAttributes','text,pagetitle,seodescription,seotitle,seourl',NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'CMSPageSearchableAttributes')
GO

--dt 02-04-2020 ZPD-9222 --> ZPD-9507
INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'PowerBIApplicationId',null,NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'PowerBIApplicationId')
GO
INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'PowerBITenantId',null,NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'PowerBITenantId')
GO
INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'PowerBIReportId',null,NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'PowerBIReportId')
GO
INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'PowerBIGroupId',null,NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'PowerBIGroupId')
GO
INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'PowerBIUserName',null,NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'PowerBIUserName')
GO
INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'PowerBIPassword',null,NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'PowerBIPassword')
GO

----dt 30/07/2020 ZPD-10382
insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Number'),'VoucherExpirationReminderEmailInDays',1,0,1,500,
'The value indicates that the set number of days prior to the expiration date a reminder email notification will be sent to the customers to use the voucher',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'VoucherExpirationReminderEmailInDays')

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Voucher Expiration Reminder Email (In Days)',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'VoucherExpirationReminderEmailInDays' and
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeDefaultValue(GlobalAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,MediaId,SwatchText,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'VoucherExpirationReminderEmailInDays'),'30',null,null,null,null,null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeDefaultValue where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'VoucherExpirationReminderEmailInDays') and AttributeDefaultValueCode = '30')

insert into ZnodeGlobalAttributeDefaultValueLocale(LocaleId,GlobalAttributeDefaultValueId,AttributeDefaultValue,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 GlobalAttributeDefaultValueId from ZnodeGlobalAttributeDefaultValue where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'VoucherExpirationReminderEmailInDays') and AttributeDefaultValueCode = '30')
,'30',null,2,GETDATE(),2,GETDATE()
where not exists(select * from ZnodeGlobalAttributeDefaultValueLocale where LocaleId = 1
and GlobalAttributeDefaultValueId = (select top 1 GlobalAttributeDefaultValueId from ZnodeGlobalAttributeDefaultValue where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'VoucherExpirationReminderEmailInDays') and AttributeDefaultValueCode = '30')
)

insert into ZnodeGlobalAttributeGroup(GroupCode, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsSystemDefined)
select 'VoucherSettings', NULL, 2, getdate(), 2, getdate(), 0
where not exists(select * from ZnodeGlobalAttributeGroup where GroupCode = 'VoucherSettings')

insert into ZnodeGlobalAttributeGroupLocale(LocaleId,GlobalAttributeGroupId,AttributeGroupName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeGroupId,'Voucher Settings',null,2,getdate(),2,getdate()
from ZnodeGlobalAttributeGroup a
where a.GroupCode = 'VoucherSettings' and
not exists(select * from ZnodeGlobalAttributeGroupLocale b where a.GlobalAttributeGroupId = b.GlobalAttributeGroupId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeGroupMapper (GlobalAttributeGroupId,GlobalAttributeId,AttributeDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'VoucherSettings'),
(select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'VoucherExpirationReminderEmailInDays'),null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeGroupMapper 
     where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'VoucherSettings')
	 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where AttributeCode = 'VoucherExpirationReminderEmailInDays'))

insert into ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'VoucherSettings'),
      (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'), 999,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalGroupEntityMapper where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'VoucherSettings')
     and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Store'))

insert into ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'VoucherSettings'),
      (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account'), 999,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalGroupEntityMapper where GlobalAttributeGroupId = (select top 1 GlobalAttributeGroupId from ZnodeGlobalAttributeGroup where GroupCode = 'VoucherSettings')
     and GlobalEntityId = (select top 1 GlobalEntityId from ZnodeGlobalEntity where EntityName = 'Account'))

-----------------------------------------------------------------------------------------------------------------------------------------------------
--dt 28-10-02020 -- ZPD-12812
INSERT  INTO ZnodeGlobalSetting (FeatureName, FeatureValues, FeatureSubValues, CreatedBy,CreatedDate, ModifiedBy,ModifiedDate)
SELECT 'OldOrderIdentifierOrderId',(SELECT MAX(ISNULL(OmsOrderId,0)) FROM ZnodeOmsOrder), null, 2, Getdate(), 2, Getdate() 
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalSetting WHERE FeatureName = 'OldOrderIdentifierOrderId')

--dt 29-01-2021 -- ZPD-12812
Update ZnodeGlobalSetting SET FeatureName = 'OldOrderIdentifierOrderId' Where FeatureName = 'OldOrderIndentifierOrderId'
     

update  ZnodeGlobalSetting set FeatureValues = 15000 where FeatureName = 'ProductImportBulk'

--dt 11-03-2022 ZPD-18152
INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'DeleteAlreadySentEmails','5',NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'DeleteAlreadySentEmails')
GO

INSERT INTO znodeglobalsetting (FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'DeletePendingEmails','90',NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM znodeglobalsetting WHERE FeatureName = 'DeletePendingEmails')
GO
INSERT INTO ZnodeGlobalSetting(FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'CatalogCategoryHierarchyAssociationAutoCreate','False',NULL,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeGlobalSetting WHERE FeatureName = 'CatalogCategoryHierarchyAssociationAutoCreate')

--ZPD-23324 Dt:06-12-2022, Rollback on 26-Dec-2022
--INSERT INTO ZnodeGlobalSetting 
--	(FeatureName,FeatureValues,FeatureSubValues,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
--SELECT 'SavedTimeZone','UTC',NULL,2, GETDATE(),2 , GETDATE()
--WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeGlobalSetting WHERE FeatureName = 'SavedTimeZone');
