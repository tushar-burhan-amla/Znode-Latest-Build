
--dt\09\10\2019 ZBT-369

INSERT INTO ZnodePimFamilyGroupMapper (PimAttributeFamilyId,PimAttributeGroupId,PimAttributeId,GroupDisplayOrder,IsSystemDefined
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT PimAttributeFamilyId, (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductInfo'),
(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'IsObsolete'),500,1,2,GETDATE(),2,GETDATE()
FROM  ZnodePimAttributeFamily PAF
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper PFG WHERE 
PimAttributeId = (SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'IsObsolete')
AND PimAttributeGroupId = (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductInfo') 
AND PFG.PimAttributeFamilyId = PAF.PimAttributeFamilyId)
AND PAF.IsCategory = 0
go

--dt \14\10\2019 ZBT-358

DECLARE @InsertedPimAttributeIds TABLE (PimAttributeId int ,AttributeTypeId int,AttributeCode nvarchar(300))
INSERT INTO ZnodePimAttribute (AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined
,IsConfigurable,IsPersonalizable,IsShowOnGrid,DisplayOrder,HelpDescription,IsCategory,IsHidden,IsSwatch,
CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
OUTPUT Inserted.PimAttributeId,Inserted.AttributeTypeId,Inserted.AttributeCode INTO @InsertedPimAttributeIds  		
SELECT (SELECT AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName = 'Text')
,'UPC',0,1,1,0,0,0,0,10,null,0,0,null,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'UPC')
		
INSERT INTO ZnodePimAttributeLocale (LocaleId,PimAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1 ,IPAS.PimAttributeId, 'UPC Code', null, 2,GETDATE(),2,GETDATE()   
FROM @InsertedPimAttributeIds IPAS 
		
insert into ZnodePimFrontendProperties 
(PimAttributeId,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select IPA.PimAttributeId,1 IsComparable, 1 IsUseInSearch, 0 IsHtmlTags,0 IsFacets,2,getdate(),2,getdate()
from @InsertedPimAttributeIds IPA
		
INSERT INTO ZnodePimAttributeGroupMapper
(PimAttributeGroupId,PimAttributeId,AttributeDisplayOrder,IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'ProductDetails'),(select PimAttributeId from znodePimattribute where AttributeCode = 'UPC'),null,1,2,getdate(),2,getdate()
WHERE NOT EXISTS (select * from ZnodePimAttributeGroupMapper where PimAttributeGroupId =(select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'ProductDetails') AND
PimAttributeId = (select PimAttributeId from znodePimattribute where AttributeCode = 'UPC') )
GO
--dt 16/10/2019 ZPD-7674
update ZnodePimAttribute set IsSystemDefined = 1 where AttributeCode='UPC'

go
--dt 16/10/2019 ZPD-7673
INSERT INTO ZnodePimFamilyGroupMapper (PimAttributeFamilyId,PimAttributeGroupId,PimAttributeId,GroupDisplayOrder,IsSystemDefined
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT PimAttributeFamilyId, (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductDetails'),
(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'UPC'),500,1,2,GETDATE(),2,GETDATE()
FROM  ZnodePimAttributeFamily PAF
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper PFG WHERE 
PimAttributeId = (SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'UPC')
AND PimAttributeGroupId = (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductDetails') 
AND PFG.PimAttributeFamilyId = PAF.PimAttributeFamilyId)
AND PAF.IsCategory = 0

--dt 17/10/2019 ZPD-7674
update ZnodePimAttributeGroupMapper set IsSystemDefined = 1
where PimAttributeId = (select Top 1 PimAttributeId from ZnodePimAttribute where AttributeCode  = 'upc')

update  ZnodePimFamilyGroupMapper set IsSystemDefined = 1  
where PimAttributeId = (select Top 1 PimAttributeId from ZnodePimAttribute where AttributeCode  = 'upc')

--dt 08/11/2019 ZPD-7815 --> ZPD-7800
update ZnodePimAttribute set HelpDescription= 'Enter the minimum quantity that can be selected when adding an item to the cart.' where AttributeCode = 'MinimumQuantity'
GO
update ZnodePimAttribute set HelpDescription= 'Enter the maximum quantity that can be selected when adding an item to the cart.' where AttributeCode = 'MaximumQuantity'

--dt 26-03-2020 ZPD-7632 -- > ZPD-8048
DECLARE @InsertedPimAttributeIds TABLE (PimAttributeId int ,AttributeTypeId int,AttributeCode nvarchar(300))
INSERT INTO ZnodePimAttribute (AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined
,IsConfigurable,IsPersonalizable,IsShowOnGrid,DisplayOrder,HelpDescription,IsCategory,IsHidden,IsSwatch,
CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
OUTPUT Inserted.PimAttributeId,Inserted.AttributeTypeId,Inserted.AttributeCode INTO @InsertedPimAttributeIds  		
SELECT (SELECT AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName = 'Yes/No')
,'HideFromSearch',0,1,1,0,0,0,0,500,null,0,0,null,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'HideFromSearch')
		
INSERT INTO ZnodePimAttributeLocale (LocaleId,PimAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1 ,IPAS.PimAttributeId, 'Hide From Search ', null, 2,GETDATE(),2,GETDATE()   
FROM @InsertedPimAttributeIds IPAS 
		
insert into ZnodePimFrontendProperties 
(PimAttributeId,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select IPA.PimAttributeId,1 IsComparable, 1 IsUseInSearch, 0 IsHtmlTags,0 IsFacets,2,getdate(),2,getdate()
from @InsertedPimAttributeIds IPA
		
INSERT INTO ZnodePimAttributeGroupMapper
(PimAttributeGroupId,PimAttributeId,AttributeDisplayOrder,IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'ProductInfo'),(select PimAttributeId from znodePimattribute where AttributeCode = 'HideFromSearch'),null,0,2,getdate(),2,getdate()
WHERE NOT EXISTS (select * from ZnodePimAttributeGroupMapper where PimAttributeGroupId =(select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'ProductInfo') AND
PimAttributeId = (select PimAttributeId from znodePimattribute where AttributeCode = 'HideFromSearch') )
GO

INSERT INTO ZnodePimFamilyGroupMapper (PimAttributeFamilyId,PimAttributeGroupId,PimAttributeId,GroupDisplayOrder,IsSystemDefined
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT PimAttributeFamilyId, (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductInfo'),
(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideFromSearch'),500,1,2,GETDATE(),2,GETDATE()
FROM  ZnodePimAttributeFamily PAF
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper PFG WHERE 
PimAttributeId = (SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideFromSearch')
AND PimAttributeGroupId = (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductInfo') 
AND PFG.PimAttributeFamilyId = PAF.PimAttributeFamilyId)
AND PAF.IsCategory = 0 and PAF.FamilyCode = 'Default'

--dt 15-06-2020 ZPD-10689 -- > ZPD-10255
DECLARE @InsertedPimAttributeIds TABLE (PimAttributeId int ,AttributeTypeId int,AttributeCode nvarchar(300))
INSERT INTO ZnodePimAttribute (AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined
,IsConfigurable,IsPersonalizable,IsShowOnGrid,DisplayOrder,HelpDescription,IsCategory,IsHidden,IsSwatch,
CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
OUTPUT Inserted.PimAttributeId,Inserted.AttributeTypeId,Inserted.AttributeCode INTO @InsertedPimAttributeIds  		
SELECT (SELECT AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName = 'Number')
,'TypicalLeadTime',0,0,1,1,0,0,0,5,null,0,0,null,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'TypicalLeadTime')
		
INSERT INTO ZnodePimAttributeLocale (LocaleId,PimAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1 ,IPAS.PimAttributeId, 'Typical Lead Time', null, 2,GETDATE(),2,GETDATE()   
FROM @InsertedPimAttributeIds IPAS 
		
insert into ZnodePimFrontendProperties 
(PimAttributeId,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select IPA.PimAttributeId,0 IsComparable, 0 IsUseInSearch, 0 IsHtmlTags,0 IsFacets,2,getdate(),2,getdate()
from @InsertedPimAttributeIds IPA
		
INSERT INTO ZnodePimAttributeGroupMapper
(PimAttributeGroupId,PimAttributeId,AttributeDisplayOrder,IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'ProductSetting'),(select PimAttributeId from znodePimattribute where AttributeCode = 'TypicalLeadTime'),null,0,2,getdate(),2,getdate()
WHERE NOT EXISTS (select * from ZnodePimAttributeGroupMapper where PimAttributeGroupId =(select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'ProductSetting') AND
PimAttributeId = (select PimAttributeId from znodePimattribute where AttributeCode = 'TypicalLeadTime') )
GO

INSERT INTO ZnodePimFamilyGroupMapper (PimAttributeFamilyId,PimAttributeGroupId,PimAttributeId,GroupDisplayOrder,IsSystemDefined
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT PimAttributeFamilyId, (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductSetting'),
(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'TypicalLeadTime'),500,1,2,GETDATE(),2,GETDATE()
FROM  ZnodePimAttributeFamily PAF
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper PFG WHERE 
PimAttributeId = (SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'TypicalLeadTime')
AND PimAttributeGroupId = (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductSetting') 
AND PFG.PimAttributeFamilyId = PAF.PimAttributeFamilyId)
AND PAF.IsCategory = 0 and PAF.FamilyCode = 'Default'

--dt 19-06-2020 ZPD-10932 
insert into ZnodePimAttributeValidation(PimAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime'),
(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowNegative' ),null,'false',2,GETDATE(),2,GETDATE()
where not exists(select * from ZnodePimAttributeValidation where PimAttributeId=(select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime')
and InputValidationId=(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowNegative' )
)

insert into ZnodePimAttributeValidation(PimAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime'),
(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowDecimals' ),null,'false',2,GETDATE(),2,GETDATE()
where not exists(select * from ZnodePimAttributeValidation where PimAttributeId=(select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime')
and InputValidationId=(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowDecimals' )
)

insert into ZnodePimAttributeValidation(PimAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime'),
(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MinNumber' ),null,'',2,GETDATE(),2,GETDATE()
where not exists(select * from ZnodePimAttributeValidation where PimAttributeId=(select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime')
and InputValidationId=(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MinNumber' )
)

insert into ZnodePimAttributeValidation(PimAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime'),
(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MaxNumber' ),null,'',2,GETDATE(),2,GETDATE()
where not exists(select * from ZnodePimAttributeValidation where PimAttributeId=(select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime')
and InputValidationId=(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MaxNumber' )
)

--dt 21-07-2020 ZPD-11600 --> ZPD-10255
declare @PimAttributeGroupId int
set @PimAttributeGroupId = (select top 1 PimAttributeGroupId  from ZnodePimAttributeGroupMapper where PimAttributeId = (select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime')
and PimAttributeGroupId = (select top 1 PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'ProductSetting'))

insert into ZnodePimFamilyGroupMapper(PimAttributeFamilyId,PimAttributeGroupId,PimAttributeId,GroupDisplayOrder,IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select PimAttributeFamilyId,@PimAttributeGroupId,(select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime'),500,1,2,getdate(),2,getdate()
from znodepimattributefamily zpaf
where not exists(select * from ZnodePimFamilyGroupMapper zpfgm where zpfgm.PimAttributeFamilyId = zpaf.PimAttributeFamilyId
	and zpfgm.PimAttributeGroupId = @PimAttributeGroupId and zpfgm.PimAttributeId = (select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime'))

update ZnodePimFamilyGroupMapper  set IsSystemDefined = 1 where PimAttributeId = (select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime')
update ZnodePimAttributeGroupMapper set IsSystemDefined = 1 where PimAttributeId = (select top 1 PimAttributeId from ZnodePimAttribute where AttributeCode = 'TypicalLeadTime')

--dt 14-10-2020 ZPD-12600
declare @PimAttributeId int =(select top 1 PimAttributeId from znodepimattribute where AttributeCode = 'ShipSeparately')
update  znodepimattribute set IsSystemDefined = 0 where AttributeCode = 'ShipSeparately'
delete FROM ZnodePimAttributeGroupMapper where PimAttributeId = @PimAttributeId
delete from ZnodePimAttributeValueLocale where PimAttributeValueId in (select PimAttributeValueId FROM ZnodePimAttributeValue where PimAttributeId = @PimAttributeId)
delete FROM ZnodePimAttributeValue where PimAttributeId = @PimAttributeId
delete FROM ZnodePimFamilyGroupMapper where PimAttributeId = @PimAttributeId
delete FROM ZnodePimLinkProductDetail where isnull(PimAttributeId,0) = @PimAttributeId
delete FROM ZnodePimProductImage where isnull(PimAttributeId,0) = @PimAttributeId
delete FROM ZnodePimProductTypeAssociation where isnull(PimAttributeId,0) = @PimAttributeId

exec [Znode_DeletePimAttribute] @PimAttributeId = @PimAttributeId , @Status=0

--dt 22-10-2020 ZPD-12682
insert into ZnodePimAttributegroupmapper
select PimAttributeGroupId,PimAttributeId, null,0,2,getdate(),2,getdate()
from ZnodePimAttributeGroup PAG 
inner join ZnodePimAttribute ZPA on  GroupCode ='ShippingSettings'and attributecode = 'FreeShipping'
where not exists (select top 1 1 from ZnodePimAttributegroupmapper ZPAG where ZPAG.PimAttributeGroupId = PAG.PimAttributeGroupId and ZPA.PimAttributeId = ZPAG.PimAttributeId)

insert into ZnodePimFamilyGroupMapper
select PimAttributeFamilyId, PimAttributeGroupId,PimAttributeId, 500,0,2,getdate(),2,getdate()
from ZnodePimAttributeFamily ZAF 
inner join ZnodePimAttributeGroup PAG  on  GroupCode ='ShippingSettings'
inner join ZnodePimAttribute ZPA on   attributecode = 'FreeShipping'
where not exists (select top 1 1 from ZnodePimFamilyGroupMapper ZPAG where ZPAG.PimAttributeFamilyId = ZAF.PimAttributeFamilyId and ZPAG.PimAttributeGroupId = PAG.PimAttributeGroupId and ZPA.PimAttributeId= ZPAG.PimAttributeId)

update ZnodePimAttributeGroupMapper set IsSystemDefined = 1
where PimAttributeId = (select top 1 PimAttributeId from znodePimAttribute where AttributeCode = 'FreeShipping' and IsSystemDefined = 1)
and isnull(IsSystemDefined,0) = 0

update ZnodePimFamilyGroupMapper set IsSystemDefined = 1
where PimAttributeId = (select top 1 PimAttributeId from znodePimAttribute where AttributeCode = 'FreeShipping' and IsSystemDefined = 1)
and isnull(IsSystemDefined,0) = 0

UPDATE a
SET a.IsSystemDefined = 1
from ZnodePimAttributeGroupMapper a
INNER JOIN ZnodePimAttribute b ON a.PimAttributeId = b.PimAttributeId
WHERE AttributeCode IN ('CategoryBanner','CategoryName','CategoryTitle','CategoryImage','Brand','Vendor','Highlights')
and b.IsSystemDefined = 1


--ZPD-16953
DECLARE @InsertedPimAttributeIds TABLE (PimAttributeId int ,AttributeTypeId int,AttributeCode nvarchar(300))
INSERT INTO ZnodePimAttribute (AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined
,IsConfigurable,IsPersonalizable,IsShowOnGrid,DisplayOrder,HelpDescription,IsCategory,IsHidden,IsSwatch,
CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
OUTPUT Inserted.PimAttributeId,Inserted.AttributeTypeId,Inserted.AttributeCode INTO @InsertedPimAttributeIds  		
SELECT (SELECT AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName = 'Yes/No')
,'DisplayVariantsOnGrid',0,0,0,1,0,0,0,7,'When enabled, all the product variants are displayed on grid on the Quick View and Product Details Page on the web store for bulk ordering',0,0,null,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'DisplayVariantsOnGrid')
		
INSERT INTO ZnodePimAttributeLocale (LocaleId,PimAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1 ,IPAS.PimAttributeId, 'Display Variants On Grid', null, 2,GETDATE(),2,GETDATE()   
FROM @InsertedPimAttributeIds IPAS 
		
insert into ZnodePimFrontendProperties 
(PimAttributeId,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select IPA.PimAttributeId,0 IsComparable, 0 IsUseInSearch, 0 IsHtmlTags,0 IsFacets,2,getdate(),2,getdate()
from @InsertedPimAttributeIds IPA
		
INSERT INTO ZnodePimAttributeGroupMapper
(PimAttributeGroupId,PimAttributeId,AttributeDisplayOrder,IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'ProductSetting'),(select PimAttributeId from znodePimattribute where AttributeCode = 'DisplayVariantsOnGrid'),null,0,2,getdate(),2,getdate()
WHERE NOT EXISTS (select * from ZnodePimAttributeGroupMapper where PimAttributeGroupId =(select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'ProductSetting') AND
PimAttributeId = (select PimAttributeId from znodePimattribute where AttributeCode = 'DisplayVariantsOnGrid') )
GO

INSERT INTO ZnodePimFamilyGroupMapper (PimAttributeFamilyId,PimAttributeGroupId,PimAttributeId,GroupDisplayOrder,IsSystemDefined
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT PimAttributeFamilyId, (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductSetting'),
(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'DisplayVariantsOnGrid'),500,1,2,GETDATE(),2,GETDATE()
FROM  ZnodePimAttributeFamily PAF
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper PFG WHERE 
PimAttributeId = (SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'DisplayVariantsOnGrid')
AND PimAttributeGroupId = (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductSetting') 
AND PFG.PimAttributeFamilyId = PAF.PimAttributeFamilyId)
AND PAF.IsCategory = 0 

-- ZPD-17524 Dt.23-Feb-2022
DECLARE @LocaleId INT=(SELECT TOP 1 LocaleId FROM ZnodeLocale WHERE Code='en-US')

DECLARE @InsertedPimAttributeIds TABLE (PimAttributeId INT ,AttributeTypeId INT,AttributeCode NVARCHAR(300))

INSERT INTO ZnodePimAttribute
	(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,IsConfigurable,IsPersonalizable,
		IsShowOnGrid,DisplayOrder,HelpDescription,IsCategory,IsHidden,IsSwatch,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
OUTPUT Inserted.PimAttributeId,Inserted.AttributeTypeId,Inserted.AttributeCode
INTO @InsertedPimAttributeIds
SELECT (SELECT AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName = 'Yes/No')
,'HideCategoryonMenu',0,1,1,0,0,0,0,500,'Hide Category on Menu',1,0,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'HideCategoryonMenu')

INSERT INTO ZnodePimAttributeLocale 
	(LocaleId,PimAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT @LocaleId,IPAS.PimAttributeId,'Hide Category on Menu', NULL, 2,GETDATE(),2,GETDATE()   
FROM @InsertedPimAttributeIds IPAS

INSERT INTO ZnodePimFrontendProperties 
	(PimAttributeId,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT IPA.PimAttributeId,1 IsComparable, 1 IsUseInSearch, 0 IsHtmlTags,0 IsFacets,2,GETDATE(),2,GETDATE()
FROM @InsertedPimAttributeIds IPA

INSERT INTO ZnodePimAttributeGroupMapper
	(PimAttributeGroupId,PimAttributeId,AttributeDisplayOrder,IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'GeneralInfo'),
	(SELECT PimAttributeId FROM znodePimattribute WHERE AttributeCode = 'HideCategoryonMenu'),NULL,1,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodePimAttributeGroupMapper
	WHERE PimAttributeGroupId =(SELECT PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'GeneralInfo')
		AND PimAttributeId = (SELECT PimAttributeId FROM znodePimattribute WHERE AttributeCode = 'HideCategoryonMenu'))

INSERT INTO ZnodePimFamilyGroupMapper
	(PimAttributeFamilyId,PimAttributeGroupId,PimAttributeId,GroupDisplayOrder,IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT PimAttributeFamilyId, 
	(SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'GeneralInfo'),
	(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu'),
	500,1,2,GETDATE(),2,GETDATE()
FROM ZnodePimAttributeFamily PAF
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper PFG
	WHERE PimAttributeId = (SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu')
		AND PimAttributeGroupId = (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'GeneralInfo') 
		AND PFG.PimAttributeFamilyId = PAF.PimAttributeFamilyId)
	AND PAF.IsCategory = 1

INSERT INTO ZnodePimAttributeDefaultValue
	(PimAttributeId,AttributeDefaultValueCode,IsEditable,DisplayOrder,IsDefault,SwatchText,MediaId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu'),
'false',NULL,NULL,NULL,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeDefaultValue
	WHERE PimAttributeId=(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu')
		AND AttributeDefaultValueCode='false')

INSERT INTO ZnodePimAttributeDefaultValueLocale
	(LocaleId,PimAttributeDefaultValueId,AttributeDefaultValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT @LocaleId,
(SELECT TOP 1 PimAttributeDefaultValueId FROM ZnodePimAttributeDefaultValue
		WHERE PimAttributeId=(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu') AND AttributeDefaultValueCode='false'),
'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeDefaultValueLocale
	WHERE PimAttributeDefaultValueId=(SELECT TOP 1 PimAttributeDefaultValueId FROM ZnodePimAttributeDefaultValue
		WHERE PimAttributeId=(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu') AND AttributeDefaultValueCode='false'))

DECLARE @PimCategoryAttributeValueId TABLE (PimCategoryAttributeValueId INT,PimCategoryId INT,PimAttributeId INT,PimAttributeFamilyId INT);
INSERT INTO ZnodePimCategoryAttributeValue
	(PimCategoryId,PimAttributeFamilyId,PimAttributeId,PimAttributeDefaultValueId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
OUTPUT INSERTED.PimCategoryAttributeValueId,INSERTED.PimCategoryId,INSERTED.PimAttributeId,INSERTED.PimAttributeFamilyId
INTO @PimCategoryAttributeValueId
SELECT PimCategoryId,
(SELECT TOP 1 PimAttributeFamilyId FROM  ZnodePimAttributeFamily WHERE FamilyCode='DefaultCategory'),
(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu'),
(SELECT TOP 1 PimAttributeDefaultValueId FROM ZnodePimAttributeDefaultValue
		WHERE PimAttributeId=(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu') AND AttributeDefaultValueCode='false'),
2,GETDATE(),2,GETDATE()
FROM ZnodePimCategory ZPC
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryAttributeValue PCAV 
	WHERE PCAV.PimCategoryId=ZPC.PimCategoryId
	AND PimAttributeFamilyId=(SELECT TOP 1 PimAttributeFamilyId FROM  ZnodePimAttributeFamily WHERE FamilyCode='DefaultCategory')
	AND PimAttributeId=(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu')
	AND PimAttributeDefaultValueId=(SELECT TOP 1 PimAttributeDefaultValueId FROM ZnodePimAttributeDefaultValue
		WHERE PimAttributeId=(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu') AND AttributeDefaultValueCode='false')
	)

INSERT INTO ZnodePimCategoryAttributeValueLocale
	(LocaleId,PimCategoryAttributeValueId,CategoryValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT DISTINCT
	@LocaleId,
	PimCategoryAttributeValueId,
	'false',2,GETDATE(),2,GETDATE()
FROM @PimCategoryAttributeValueId A
WHERE NOT EXISTS
(
	SELECT TOP 1 1
	FROM ZnodePimCategoryAttributeValueLocale B
	WHERE B.PimCategoryAttributeValueId = A.PimCategoryAttributeValueId
	AND B.LocaleId = @LocaleId
)

--ZPD-17524 Dt.10-March-2022
UPDATE ZnodePimAttribute 
SET IsSystemDefined = 1 
WHERE AttributeCode='HideCategoryonMenu'
	AND ISNULL(IsSystemDefined,0) = 0

UPDATE ZnodePimAttributeGroupMapper 
SET IsSystemDefined = 1
WHERE PimAttributeId = (SELECT TOP 1 PimAttributeId FROM znodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu' AND IsSystemDefined = 1)
	AND ISNULL(IsSystemDefined,0) = 0

UPDATE ZnodePimFamilyGroupMapper 
SET IsSystemDefined = 1
WHERE PimAttributeId = (SELECT TOP 1 PimAttributeId FROM znodePimAttribute WHERE AttributeCode = 'HideCategoryonMenu' AND IsSystemDefined = 1)
	AND ISNULL(IsSystemDefined,0) = 0

--dt 16-03-2022 ZPD-17705
UPDATE ZnodePimAttribute
SET IsSystemDefined = 1
WHERE AttributeCode='HideFromSearch' AND ISNULL(IsSystemDefined,0) = 0

UPDATE ZnodePimAttributeGroupMapper
SET IsSystemDefined = 1
WHERE PimAttributeId = (SELECT TOP 1 PimAttributeId FROM znodePimAttribute WHERE AttributeCode = 'HideFromSearch' AND IsSystemDefined = 1)
AND ISNULL(IsSystemDefined,0) = 0

UPDATE ZnodePimFamilyGroupMapper
SET IsSystemDefined = 1
WHERE PimAttributeId = (SELECT TOP 1 PimAttributeId FROM znodePimAttribute WHERE AttributeCode = 'HideFromSearch' AND IsSystemDefined = 1)
AND ISNULL(IsSystemDefined,0) = 0

INSERT INTO ZnodePimFamilyGroupMapper (PimAttributeFamilyId,PimAttributeGroupId,PimAttributeId,GroupDisplayOrder,IsSystemDefined
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT PimAttributeFamilyId, (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductInfo'),
(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideFromSearch'),500,1,2,GETDATE(),2,GETDATE()
FROM  ZnodePimAttributeFamily PAF
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper PFG WHERE
PimAttributeId = (SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'HideFromSearch')
AND PimAttributeGroupId = (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'ProductInfo')
AND PFG.PimAttributeFamilyId = PAF.PimAttributeFamilyId)
AND PAF.IsCategory = 0

--dt 01-08-2022 ZPD-18441
GO
DECLARE @InsertedPimAttributeIds TABLE (PimAttributeId int ,AttributeTypeId int,AttributeCode nvarchar(300))
INSERT INTO ZnodePimAttribute (AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined
,IsConfigurable,IsPersonalizable,IsShowOnGrid,DisplayOrder,HelpDescription,IsCategory,IsHidden,IsSwatch,
CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
OUTPUT Inserted.PimAttributeId,Inserted.AttributeTypeId,Inserted.AttributeCode INTO @InsertedPimAttributeIds  		
SELECT (SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName = 'Text')
,'Video1',0,0,1,1,0,0,0,500,null,0,0,null,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'Video1')
		
INSERT INTO ZnodePimAttributeLocale (LocaleId,PimAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1 ,IPAS.PimAttributeId, 'Video 1', null, 2,GETDATE(),2,GETDATE()   
FROM @InsertedPimAttributeIds IPAS 
		
insert into ZnodePimFrontendProperties (PimAttributeId,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select IPAZnodePimAttribute.PimAttributeId,0 IsComparable, 0 IsUseInSearch, 0 IsHtmlTags,0 IsFacets,2,getdate(),2,getdate()
from @InsertedPimAttributeIds IPAZnodePimAttribute
		
INSERT INTO ZnodePimAttributeGroupMapper
(PimAttributeGroupId,PimAttributeId,AttributeDisplayOrder,IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select TOP 1 PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'Image'),
(select TOP 1 PimAttributeId from znodePimattribute where AttributeCode = 'Video1'),null,1,2,getdate(),2,getdate()
WHERE NOT EXISTS (select * from ZnodePimAttributeGroupMapper where PimAttributeGroupId =(select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'Image') AND
PimAttributeId = (select PimAttributeId from znodePimattribute where AttributeCode = 'Video1') )

INSERT INTO ZnodePimFamilyGroupMapper (PimAttributeFamilyId,PimAttributeGroupId,PimAttributeId,GroupDisplayOrder,IsSystemDefined
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT PimAttributeFamilyId, (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'Image'),
(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'Video1'),500,1,2,GETDATE(),2,GETDATE()
FROM  ZnodePimAttributeFamily PAF
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper PFG WHERE 
PimAttributeId = (SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'Video1')
AND PimAttributeGroupId = (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'Image') 
AND PFG.PimAttributeFamilyId = PAF.PimAttributeFamilyId)
AND PAF.IsCategory = 0

GO

DECLARE @InsertedPimAttributeIds TABLE (PimAttributeId int ,AttributeTypeId int,AttributeCode nvarchar(300))
INSERT INTO ZnodePimAttribute (AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined
,IsConfigurable,IsPersonalizable,IsShowOnGrid,DisplayOrder,HelpDescription,IsCategory,IsHidden,IsSwatch,
CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
OUTPUT Inserted.PimAttributeId,Inserted.AttributeTypeId,Inserted.AttributeCode INTO @InsertedPimAttributeIds 		
SELECT (SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName = 'Text')
,'Video2',0,0,1,1,0,0,0,500,null,0,0,null,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'Video2')
		
INSERT INTO ZnodePimAttributeLocale (LocaleId,PimAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 1 ,IPAS.PimAttributeId, 'Video 2', null, 2,GETDATE(),2,GETDATE()   
FROM @InsertedPimAttributeIds IPAS 
		
insert into ZnodePimFrontendProperties (PimAttributeId,IsComparable,IsUseInSearch,IsHtmlTags,IsFacets,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select IPAZnodePimAttribute.PimAttributeId,0 IsComparable, 0 IsUseInSearch, 0 IsHtmlTags,0 IsFacets,2,getdate(),2,getdate()
from @InsertedPimAttributeIds IPAZnodePimAttribute
		
INSERT INTO ZnodePimAttributeGroupMapper
(PimAttributeGroupId,PimAttributeId,AttributeDisplayOrder,IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select TOP 1 PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'Image'),
(select TOP 1 PimAttributeId from znodePimattribute where AttributeCode = 'Video2'),null,1,2,getdate(),2,getdate()
WHERE NOT EXISTS (select * from ZnodePimAttributeGroupMapper where PimAttributeGroupId =(select PimAttributeGroupId from ZnodePimAttributeGroup where GroupCode = 'Image') AND
PimAttributeId = (select PimAttributeId from znodePimattribute where AttributeCode = 'Video2') )

INSERT INTO ZnodePimFamilyGroupMapper (PimAttributeFamilyId,PimAttributeGroupId,PimAttributeId,GroupDisplayOrder,IsSystemDefined
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT PimAttributeFamilyId, (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'Image'),
(SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'Video2'),500,1,2,GETDATE(),2,GETDATE()
FROM  ZnodePimAttributeFamily PAF
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper PFG WHERE 
PimAttributeId = (SELECT TOP 1 PimAttributeId FROM  ZnodePimAttribute WHERE AttributeCode = 'Video2')
AND PimAttributeGroupId = (SELECT TOP 1 PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE GroupCode = 'Image') 
AND PFG.PimAttributeFamilyId = PAF.PimAttributeFamilyId)
AND PAF.IsCategory = 0

--Date 04-Nov-2022 ZPD-22934/ZPD-22279
UPDATE PAL
SET PAL.AttributeName='Product Status'
FROM ZnodePimAttributeLocale PAL
INNER JOIN ZnodePimAttribute PA ON PAL.PimAttributeId=PA.PimAttributeId
WHERE PA.AttributeCode='IsActive' AND PA.IsCategory=0 --AND PAL.LocaleId=1


--dt 11/11/2022 --> ZPD-22870

INSERT INTO ZnodePimAttributeValidation 
	(PimAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'CategoryImage'),
	(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1)),
	(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'),
	'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodePimAttributeValidation WHERE PimAttributeId=(SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'CategoryImage')
	AND InputValidationId=(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image'))
	AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'))

INSERT INTO ZnodePimAttributeValidation 
	(PimAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'ProductImage'),
	(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1)),
	(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'),
	'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodePimAttributeValidation WHERE PimAttributeId=(SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'ProductImage')
	AND InputValidationId=(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image'))
	AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'))

INSERT INTO ZnodePimAttributeValidation 
	(PimAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	(SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'GalleryImages'),
	(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image' AND IsList=1)),
	(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'),
	'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodePimAttributeValidation WHERE PimAttributeId=(SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'GalleryImages')
	AND InputValidationId=(SELECT TOP 1 InputValidationId FROM dbo.ZnodeAttributeInputValidation WHERE AttributeTypeId =(SELECT TOP 1 AttributeTypeId FROM dbo.ZnodeAttributeType WHERE AttributeTypeName='Image'))
	AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM [dbo].[ZnodeAttributeInputValidationRule] WHERE ValidationName='.webp'))
