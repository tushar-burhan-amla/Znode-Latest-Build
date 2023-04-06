
if exists(select * from ZnodePortal where StoreCode = 'MaxwellsHardware')
begin
----Register for Business Account
insert into ZnodeCMSContentPages(PortalId,CMSTemplateId,PageName,ActivationDate,ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsPublished,PublishStateId)
select (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'),
(select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Content And Form Sections'),'Register for Business Account',
null,null,1,2,getdate(),2,getdate(),0,1
where not exists(select * from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Content And Form Sections')
and PageName = 'Register for Business Account' )

insert into ZnodeCMSContentPagesLocale(CMSContentPagesId,LocaleId,PageTitle,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Content And Form Sections')
and PageName = 'Register for Business Account' ),1,'Register for Business Account',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPagesLocale where CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Content And Form Sections')
and PageName = 'Register for Business Account' )
and LocaleId = 1 )

insert into ZnodeCMSContentPageGroup(ParentCMSContentPageGroupId,Code,CreatedBy	,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root'),
'Maxwell Hardware',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root')
and Code = 'Maxwell Hardware')

insert into ZnodeCMSContentPageGroupLocale(CMSContentPageGroupId,Name,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root')
and Code = 'Maxwell Hardware'),'Maxwell''s Hardware',1,2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPageGroupLocale where CMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root')
and Code = 'Maxwell Hardware') and LocaleId = 1)

insert into ZnodeCMSContentPageGroupMapping(CMSContentPageGroupId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware'),
	(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Content And Form Sections')
	and PageName = 'Register for Business Account' ),
	2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPageGroupMapping where CMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware')
	and CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Content And Form Sections')
	and PageName = 'Register for Business Account' ))

----Circular Saws
insert into ZnodeCMSContentPages(PortalId,CMSTemplateId,PageName,ActivationDate,ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsPublished,PublishStateId)
select (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'),
(select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video'),'Circular Saws',
null,null,1,2,getdate(),2,getdate(),0,1
where not exists(select * from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video')
and PageName = 'Circular Saws' )

insert into ZnodeCMSContentPagesLocale(CMSContentPagesId,LocaleId,PageTitle,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video')
and PageName = 'Circular Saws' ),1,'Circular Saws',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPagesLocale where CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video')
and PageName = 'Circular Saws' )
and LocaleId = 1 )

insert into ZnodeCMSContentPageGroupMapping(CMSContentPageGroupId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware'),
	(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video')
	and PageName = 'Circular Saws' ),
	2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPageGroupMapping where CMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware')
	and CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video')
	and PageName = 'Circular Saws' ))

----Cordless Woodworking
insert into ZnodeCMSContentPages(PortalId,CMSTemplateId,PageName,ActivationDate,ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsPublished,PublishStateId)
select (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'),
(select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider'),'Cordless Woodworking',
null,null,1,2,getdate(),2,getdate(),0,1
where not exists(select * from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
and PageName = 'Cordless Woodworking' )

insert into ZnodeCMSContentPagesLocale(CMSContentPagesId,LocaleId,PageTitle,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
and PageName = 'Cordless Woodworking' ),1,'Cordless Woodworking',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPagesLocale where CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
and PageName = 'Cordless Woodworking' )
and LocaleId = 1 )

insert into ZnodeCMSContentPageGroupMapping(CMSContentPageGroupId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware'),
	(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
	and PageName = 'Cordless Woodworking' ),
	2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPageGroupMapping where CMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware')
	and CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
	and PageName = 'Cordless Woodworking' ))

----Husqvarna Products	
insert into ZnodeCMSContentPages(PortalId,CMSTemplateId,PageName,ActivationDate,ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsPublished,PublishStateId)
select (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'),
(select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider'),'Husqvarna Products',
null,null,1,2,getdate(),2,getdate(),0,1
where not exists(select * from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider')
and PageName = 'Husqvarna Products' )

insert into ZnodeCMSContentPagesLocale(CMSContentPagesId,LocaleId,PageTitle,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider')
and PageName = 'Husqvarna Products' ),1,'Husqvarna Products',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPagesLocale where CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider')
and PageName = 'Husqvarna products' )
and LocaleId = 1 )

insert into ZnodeCMSContentPageGroupMapping(CMSContentPageGroupId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware'),
	(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider')
	and PageName = 'Husqvarna products' ),
	2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPageGroupMapping where CMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware')
	and CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider')
	and PageName = 'Husqvarna products' ))

-----Promotional Products
insert into ZnodeCMSContentPages(PortalId,CMSTemplateId,PageName,ActivationDate,ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsPublished,PublishStateId)
select (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'),
(select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets'),'Promotional Products',
null,null,1,2,getdate(),2,getdate(),0,1
where not exists(select * from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets')
and PageName = 'Promotional Products' )

insert into ZnodeCMSContentPagesLocale(CMSContentPagesId,LocaleId,PageTitle,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets')
and PageName = 'Promotional Products' ),1,'Promotional Products',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPagesLocale where CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets')
and PageName = 'Promotional Products' )
and LocaleId = 1 )

insert into ZnodeCMSContentPageGroupMapping(CMSContentPageGroupId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware'),
	(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets')
	and PageName = 'Promotional Products' ),
	2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSContentPageGroupMapping where CMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware')
	and CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets')
	and PageName = 'Promotional Products' ))
end

go

if exists(select * from ZnodePortal where StoreCode = 'MaxwellsHardware')
begin
insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Text'),'JobTitle',1,1,0,500,
null,2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'JobTitle')

insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Number'),'AccountNo',1,1,0,500,
null,2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'AccountNo')

insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Number'),'LastStatementBalance',1,1,0,500,
null,2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'LastStatementBalance')

insert into ZnodeGlobalAttribute(AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsActive,DisplayOrder,HelpDescription,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsSystemDefined)
select (select top 1 AttributeTypeId from ZnodeAttributeType where AttributeTypeName = 'Text'),'ReferredBy',1,1,0,500,
null,2,getdate(),2,getdate(),0
where not exists(select * from ZnodeGlobalAttribute where  AttributeCode = 'ReferredBy')

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Job Title',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'JobTitle' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Account No',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'AccountNo' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Last Statement Balance',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'LastStatementBalance' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeLocale(LocaleId,GlobalAttributeId,AttributeName,Description,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,GlobalAttributeId,'Referred By',null,2,getdate(),2,getdate()
from ZnodeGlobalAttribute a
where AttributeCode = 'ReferredBy' and 
not exists(select * from ZnodeGlobalAttributeLocale b where a.GlobalAttributeId = b.GlobalAttributeId and b.LocaleId = 1)

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'JobTitle'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'ValidationRule')
,null,null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'JobTitle')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'ValidationRule'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'JobTitle'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'UniqueValue')
,null,'',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'JobTitle')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'UniqueValue'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'JobTitle'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'RegularExpression')
,null,'',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'JobTitle')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'RegularExpression'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'JobTitle'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'RegularExpression')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'JobTitle')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'RegularExpression'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountNo'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowNegative')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountNo')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowNegative'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountNo'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowDecimals')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountNo')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowDecimals'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountNo'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MinNumber')
,null,'',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountNo')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MinNumber'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountNo'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MaxNumber')
,null,'',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountNo')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MaxNumber'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'LastStatementBalance'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowNegative')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'LastStatementBalance')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowNegative'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'LastStatementBalance'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowDecimals')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'LastStatementBalance')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'AllowDecimals'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'LastStatementBalance'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MinNumber')
,null,'',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'LastStatementBalance')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MinNumber'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'LastStatementBalance'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MaxNumber')
,null,'',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'LastStatementBalance')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'MaxNumber'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'ReferredBy'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'ValidationRule')
,null,null,2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'ReferredBy')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'ValidationRule'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'ReferredBy'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'UniqueValue')
,null,'',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'ReferredBy')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'UniqueValue'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'ReferredBy'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'RegularExpression')
,null,'',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'ReferredBy')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'RegularExpression'))

insert into ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'ReferredBy'),(select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'RegularExpression')
,null,'false',2,getdate(),2,getdate()
where not exists(select * from ZnodeGlobalAttributeValidation where GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'ReferredBy')
and InputValidationId = (select top 1 InputValidationId from ZnodeAttributeInputValidation where Name = 'RegularExpression'))

insert into ZnodeFormBuilderAttributeMapper(FormBuilderId,GlobalAttributeGroupId,GlobalAttributeId,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 FormBuilderId from Znodeformbuilder where FormCode = 'ProvideYourBusinessInformation'),null,
    (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'JobTitle'),3,2,getdate(),2,getdate()
where not exists(select * from ZnodeFormBuilderAttributeMapper where FormBuilderId=(select top 1 FormBuilderId from Znodeformbuilder where FormCode = 'ProvideYourBusinessInformation')
 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'JobTitle'))
 
insert into ZnodeFormBuilderAttributeMapper(FormBuilderId,GlobalAttributeGroupId,GlobalAttributeId,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 FormBuilderId from Znodeformbuilder where FormCode = 'ProvideYourBusinessInformation'),null,
    (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountNo'),4,2,getdate(),2,getdate()
where not exists(select * from ZnodeFormBuilderAttributeMapper where FormBuilderId=(select top 1 FormBuilderId from Znodeformbuilder where FormCode = 'ProvideYourBusinessInformation')
 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'AccountNo'))

insert into ZnodeFormBuilderAttributeMapper(FormBuilderId,GlobalAttributeGroupId,GlobalAttributeId,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 FormBuilderId from Znodeformbuilder where FormCode = 'ProvideYourBusinessInformation'),null,
    (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'LastStatementBalance'),12,2,getdate(),2,getdate()
where not exists(select * from ZnodeFormBuilderAttributeMapper where FormBuilderId=(select top 1 FormBuilderId from Znodeformbuilder where FormCode = 'ProvideYourBusinessInformation')
 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'LastStatementBalance'))

insert into ZnodeFormBuilderAttributeMapper(FormBuilderId,GlobalAttributeGroupId,GlobalAttributeId,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 FormBuilderId from Znodeformbuilder where FormCode = 'ProvideYourBusinessInformation'),null,
    (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'ReferredBy'),13,2,getdate(),2,getdate()
where not exists(select * from ZnodeFormBuilderAttributeMapper where FormBuilderId=(select top 1 FormBuilderId from Znodeformbuilder where FormCode = 'ProvideYourBusinessInformation')
 and GlobalAttributeId = (select top 1 GlobalAttributeId from ZnodeGlobalAttribute where  AttributeCode = 'ReferredBy'))
 end
 go
 if exists(select * from ZnodePortal where StoreCode = 'MaxwellsHardware')
begin
insert into ZnodeCMSSEODetail(CMSSEOTypeId,SEOId,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy
,ModifiedDate,IsPublish,SEOCode,PublishStateId)
select (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page'),null,1,null,(select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'),
	'provideyourbusinessform',2,getdate(),2,getdate(),0,'Register for Business Account',2
where not exists(select * from ZnodeCMSSEODetail where SEOCode = 'Register for Business Account' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'))

insert into ZnodeCMSSEODetailLocale(CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CanonicalURL,RobotTag)
select (select top 1 CMSSEODetailId from ZnodeCMSSEODetail where SEOCode = 'Register for Business Account' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware')),1,null,null,null,2,getdate(),2,getdate(), 'provideyourbusinessform', 'None'
where not exists(select * from ZnodeCMSSEODetailLocale where CMSSEODetailId = (select top 1 CMSSEODetailId from ZnodeCMSSEODetail where SEOCode = 'Register for Business Account' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'))
	  and LocaleId = 1)

insert into ZnodeCMSSEODetail(CMSSEOTypeId,SEOId,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy
,ModifiedDate,IsPublish,SEOCode,PublishStateId)
select (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page'),null,1,null,(select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'),
	'circular_saws_products',2,getdate(),2,getdate(),0,'Circular Saws',2
where not exists(select * from ZnodeCMSSEODetail where SEOCode = 'Circular Saws' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'))

insert into ZnodeCMSSEODetailLocale(CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CanonicalURL,RobotTag)
select (select top 1 CMSSEODetailId from ZnodeCMSSEODetail where SEOCode = 'Circular Saws' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware')),1,null,null,null,2,getdate(),2,getdate(), 'circular_saws_products', 'None'
where not exists(select * from ZnodeCMSSEODetailLocale where CMSSEODetailId = (select top 1 CMSSEODetailId from ZnodeCMSSEODetail where SEOCode = 'Circular Saws' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'))
	  and LocaleId = 1)

insert into ZnodeCMSSEODetail(CMSSEOTypeId,SEOId,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy
,ModifiedDate,IsPublish,SEOCode,PublishStateId)
select (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page'),null,1,null,(select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'),
	'cordless-woodworking',2,getdate(),2,getdate(),0,'Cordless Woodworking',2
where not exists(select * from ZnodeCMSSEODetail where SEOCode = 'Cordless Woodworking' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'))

insert into ZnodeCMSSEODetailLocale(CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CanonicalURL,RobotTag)
select (select top 1 CMSSEODetailId from ZnodeCMSSEODetail where SEOCode = 'Cordless Woodworking' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware')),1,'Cordless Woodworking','Cordless Woodworking',null,2,getdate(),2,getdate(), 'cordless-woodworking', 'None'
where not exists(select * from ZnodeCMSSEODetailLocale where CMSSEODetailId = (select top 1 CMSSEODetailId from ZnodeCMSSEODetail where SEOCode = 'Cordless Woodworking' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'))
	  and LocaleId = 1)

insert into ZnodeCMSSEODetail(CMSSEOTypeId,SEOId,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy
,ModifiedDate,IsPublish,SEOCode,PublishStateId)
select (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page'),null,1,null,(select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'),
	'husqvarna_products',2,getdate(),2,getdate(),0,'Husqvarna Products',2
where not exists(select * from ZnodeCMSSEODetail where SEOCode = 'Husqvarna Products' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'))

insert into ZnodeCMSSEODetailLocale(CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CanonicalURL,RobotTag)
select (select top 1 CMSSEODetailId from ZnodeCMSSEODetail where SEOCode = 'Husqvarna Products' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware')),1,null,null,null,2,getdate(),2,getdate(), 'husqvarna_products', 'None'
where not exists(select * from ZnodeCMSSEODetailLocale where CMSSEODetailId = (select top 1 CMSSEODetailId from ZnodeCMSSEODetail where SEOCode = 'Husqvarna Products' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'))
	  and LocaleId = 1)

insert into ZnodeCMSSEODetail(CMSSEOTypeId,SEOId,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy
,ModifiedDate,IsPublish,SEOCode,PublishStateId)
select (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page'),null,1,null,(select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'),
	'promotional_products',2,getdate(),2,getdate(),0,'Promotional Products',2
where not exists(select * from ZnodeCMSSEODetail where SEOCode = 'Promotional Products' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'))

insert into ZnodeCMSSEODetailLocale(CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CanonicalURL,RobotTag)
select (select top 1 CMSSEODetailId from ZnodeCMSSEODetail where SEOCode = 'Promotional Products' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware')),1,null,null,null,2,getdate(),2,getdate(), 'promotional_products', 'None'
where not exists(select * from ZnodeCMSSEODetailLocale where CMSSEODetailId = (select top 1 CMSSEODetailId from ZnodeCMSSEODetail where SEOCode = 'Promotional Products' and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Content Page')
      and PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware'))
	  and LocaleId = 1)

end
go
if exists(select * from ZnodePortal where StoreCode = 'MaxwellsHardware')
begin

insert into ZnodeMedia(MediaConfigurationId,Path,FileName,Size,Height,Width,Length,Type,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Version)
select (select top 1 MediaConfigurationId from ZnodeMediaConfiguration where server = 'Local'),'33724e67-ebb0-4b41-aa35-532a1b3abcc0full-hero-image_husqvarna-lawn-tractor.png','full-hero-image_husqvarna-lawn-tractor.png',
	1679358,525 ,1432,1679358,'.png',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeMedia where Path = '33724e67-ebb0-4b41-aa35-532a1b3abcc0full-hero-image_husqvarna-lawn-tractor.png' and FileName = 'full-hero-image_husqvarna-lawn-tractor.png')
insert into ZnodeMedia(MediaConfigurationId,Path,FileName,Size,Height,Width,Length,Type,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Version)
select (select top 1 MediaConfigurationId from ZnodeMediaConfiguration where server = 'Local'),'33724e67-ebb0-4b41-aa35-532a1b3abcc0full-hero-image_husqvarna-lawn-tractor.png','full-hero-image_husqvarna-lawn-tractor.png',
	1679358,525 ,1432,1679358,'.png',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeMedia where Path = '33724e67-ebb0-4b41-aa35-532a1b3abcc0full-hero-image_husqvarna-lawn-tractor.png' and FileName = 'full-hero-image_husqvarna-lawn-tractor.png')

insert into ZnodeMedia(MediaConfigurationId,Path,FileName,Size,Height,Width,Length,Type,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Version)
select (select top 1 MediaConfigurationId from ZnodeMediaConfiguration where server = 'Local'),'426a3c48-8194-4431-aba3-b743167ac3f3cordless-image.jpg','cordless-image.jpg',
	2861634,3000 ,3000,2861634 ,'.jpg',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeMedia where Path = '426a3c48-8194-4431-aba3-b743167ac3f3cordless-image.jpg' and FileName = 'cordless-image.jpg')

insert into ZnodeMedia(MediaConfigurationId,Path,FileName,Size,Height,Width,Length,Type,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Version)
select (select top 1 MediaConfigurationId from ZnodeMediaConfiguration where server = 'Local'),'072ab305-6b9b-452b-bf02-7a178b36224ffull-hero_dewalt-tools.png','full-hero_dewalt-tools.png',
	1420193,524 ,1432      ,1420193    ,'.png',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeMedia where Path = '072ab305-6b9b-452b-bf02-7a178b36224ffull-hero_dewalt-tools.png' and FileName = 'full-hero_dewalt-tools.png')

insert into ZnodeMedia(MediaConfigurationId,Path,FileName,Size,Height,Width,Length,Type,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Version)
select (select top 1 MediaConfigurationId from ZnodeMediaConfiguration where server = 'Local'),'f7199e64-c5d2-419a-8ef4-ff2f7bd34bdchero_snap-on.png','hero_snap-on.png',
	570693,524        ,716             ,570693        ,'.png',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeMedia where Path = 'f7199e64-c5d2-419a-8ef4-ff2f7bd34bdchero_snap-on.png' and FileName = 'hero_snap-on.png')

insert into ZnodeMedia(MediaConfigurationId,Path,FileName,Size,Height,Width,Length,Type,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Version)
select (select top 1 MediaConfigurationId from ZnodeMediaConfiguration where server = 'Local'),'994e156f-74db-4f78-bf96-9a9170559552Circular-Saw-Video.mp4','Circular-Saw-Video.mp4',
	7909352,null ,null      ,7909352       ,'.mp4',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeMedia where Path = '994e156f-74db-4f78-bf96-9a9170559552Circular-Saw-Video.mp4' and FileName = 'Circular-Saw-Video.mp4')

insert into ZnodeMedia(MediaConfigurationId,Path,FileName,Size,Height,Width,Length,Type,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Version)
select (select top 1 MediaConfigurationId from ZnodeMediaConfiguration where server = 'Local'),'016c571b-ccf0-4925-93ad-c863109267a1welding machine.jpg','welding machine.jpg',
	114441,1000      ,1000      ,114441    ,'.jpg',2,getdate(),2,getdate(),0
where not exists(select * from ZnodeMedia where Path = '016c571b-ccf0-4925-93ad-c863109267a1welding machine.jpg' and FileName = 'welding machine.jpg')


insert into ZnodeCMSTextWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,Text,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor'),'996079',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Content And Form Sections') and PageName = 'Register for Business Account' ),'ContentPageMapping',
'<div class="col-12 p-0">  <div class="col-12" style="text-align: center; display: block;"><img src="http://api9x.znodellc.com/Data/Media/3b7d2be1-a3ec-4dbe-84c5-95b687e11252Logo-Maxwell.svg" alt="" width="280" height="70" /></div>  <h1 class="pt-5" style="text-align: center; display: block;">Set Up a Business Account and get 10% off your first order.</h1>  <p class="pb-5 pt-2 font-16" style="text-align: center; display: block;">Grow your business with us. Set up a Maxwell Business account for quick and easy access to all of our products</p>  <h3 class="py-4" style="text-align: center; display: block;">Benefits of a Business Account :</h3>  <div class="col-12">  <div class="row">  <div class="col-12 col-sm-4" style="text-align: center; display: block;">  <div class="clock-icon dashboard-icons" style="font-size: 70px; color: #ff6f00;">  <div class="d-none">&nbsp;</div>  </div>  <h6>BULK PRICING</h6>  <p class="font-16">Buy more and save more every day.</p>  </div>  <div class="col-12 col-sm-4" style="text-align: center; display: block;">  <div class="my-profile-icon dashboard-icons" style="font-size: 70px; color: #ff6f00;">  <div class="d-none">&nbsp;</div>  </div>  <h6>DEDICATED SALES</h6>  <p class="font-16">Dedicated sales support ready to assist.</p>  </div>  <div class="col-12 col-sm-4" style="text-align: center; display: block;">  <div class="order-templates-icon dashboard-icons" style="font-size: 70px; color: #ff6f00;">  <div class="d-none">&nbsp;</div>  </div>  <h6>ORDER TEMPLATES</h6>  <p class="font-16">Create product lists for easy re-ordering.</p>  </div>  </div>  </div>  </div>',
2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSTextWidgetConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor') and TypeOFMapping = 'ContentPageMapping'
and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Content And Form Sections') and PageName = 'Register for Business Account' )
and WidgetsKey = '996079' and LocaleId = 1)

insert into ZnodeCMSMediaConfiguration(CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,MediaId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'VideoWidget'),'8786787',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video')
	and PageName = 'Circular Saws' ),'ContentPageMapping', (select top 1 MediaId from ZnodeMedia where filename = 'Circular-Saw-Video.mp4' and path = '994e156f-74db-4f78-bf96-9a9170559552Circular-Saw-Video.mp4'),
	2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSMediaConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'VideoWidget') and WidgetsKey = '8786787'
and CMSMappingId =(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video')
	and PageName = 'Circular Saws' ) and MediaId = (select top 1 MediaId from ZnodeMedia where filename = 'Circular-Saw-Video.mp4' and path = '994e156f-74db-4f78-bf96-9a9170559552Circular-Saw-Video.mp4')
	)


insert into ZnodeCMSTextWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,Text,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor'),'9998781',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video') and PageName = 'Circular Saws' ),'ContentPageMapping',
'<div style="text-align: center; margin: 0 auto; max-width: 55%;">  <h4 style="color: #ff6f00; max-width: 95%; text-align: center; margin: 0 auto;">DEWALT FLEXVOLT 60-VOLT CORDLESS CIRCULAR SAW WITH BREAK</h4>  <p>Lorem Ipsum is simply text of the printing and typesetting industry. Lorem Ipsum has been the industry''s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting,</p>  </div>',
2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSTextWidgetConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor') and TypeOFMapping = 'ContentPageMapping'
and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video') and PageName = 'Circular Saws' )
and WidgetsKey = '9998781' and LocaleId = 1)

insert into ZnodeCMSTextWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,Text,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor'),'343544',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video') and PageName = 'Circular Saws' ),'ContentPageMapping',
'<h5 class="pt-4" style="border-bottom: 1px solid #000;">Miter Saws</h5>',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSTextWidgetConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor') and TypeOFMapping = 'ContentPageMapping'
and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video') and PageName = 'Circular Saws' )
and WidgetsKey = '343544' and LocaleId = 1)

----
insert into ZnodeCMSTextWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,Text,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor'),'565767',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ),'ContentPageMapping',
'<div class="col-12 pt-3">  <h1 style="text-align: center; display: block; margin: 0 auto;">DEWALT BRUSHLESS WOODWORKING TOOLS <br /> ARE FINALLY HERE!</h1>  <p class="font-16" style="text-align: center; display: block; margin: 0 auto; max-width: 550px;">DEWALT has officially released its line of new brushless woodworking tools, and they look great. The first three to be released are the D-handle Jigsaw, the Router, and the Random Orbit Sander.!</p>  </div>',
2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSTextWidgetConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor') and TypeOFMapping = 'ContentPageMapping'
and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' )
and WidgetsKey = '565767' and LocaleId = 1)

insert into ZnodeCMSTextWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,Text,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor'),'9897878',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ),'ContentPageMapping',
'<h3 style="color: #ff6f00;">THE JIG IS&nbsp;UP</h3>  <p class="font-16">A new brushless jigsaw means you can cut more and for longer than ever before. A 1-inch stroke cuts quickly and efficiently, especially when paired with 4 different orbital patterns. The keyless blade changes for switching between wood and metal, or even when a blade is dull. A blower keeps dust out of your way while cutting, and LED headlights ensure you stay true to your lines. The top handle is easy to grip, making it all the more secure!</p>  <p class="my-4"><a class="btn btn-secondary" href="https://webstore-b2b-znode.amla.io/product/4355">View Product</a></p>',
2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSTextWidgetConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor') and TypeOFMapping = 'ContentPageMapping'
and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' )
and WidgetsKey = '9897878' and LocaleId = 1)

insert into ZnodeCMSTextWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,Text,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor'),'24445556',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ),'ContentPageMapping',
'<h3 style="color: #ff6f00;">THE ROUTE TO SUCCESS</h3>  <p class="font-16">The Brushless motor adjusts the speed of the bit while cutting, even under a heavy load. Soft start sees a comback in this tool, turning it on slowly reducing the risk of damaging your work and electric brakes stop the tool when you turn it off. It also has a quick-release clamp ensuring fast and consistent operation, a depth adjustmen ring, a big spindle lock button, and the ability to work with a plunge base</p>  <p class="my-4"><a class="btn btn-secondary" href="https://webstore-b2b-znode.amla.io/dewalt-1by4-impact-driver">View Product</a></p>',
2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSTextWidgetConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor') and TypeOFMapping = 'ContentPageMapping'
and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' )
and WidgetsKey = '24445556' and LocaleId = 1)

insert into ZnodeCMSTextWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,Text,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor'),'2333445',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ),'ContentPageMapping',
'<h3 style="font-size: 26px; text-align: center; display: block; margin: 0 auto; text-transform: uppercase;">Related Featured Products</h3>',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSTextWidgetConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor') and TypeOFMapping = 'ContentPageMapping'
and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' )
and WidgetsKey = '2333445' and LocaleId = 1)

insert into ZnodeCMSTextWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,Text,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor'),'8787878',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ),'ContentPageMapping',
'<h3 style="color: #ff6f00;">HANDY-DANDY SANDER</h3>  <p class="font-16">More power, less maintenance, and did we forget to mention it''s brushless? It also has allows for easy, single-hand adjustmants for speed and power, as well as being easled to prevent dust getting inside and gunking up the machine. There''s even a port for your dust collector hose to stop the mess before it happens</p>  <p class="my-4"><a class="btn btn-secondary" href="https://webstore-b2b-znode.amla.io/husqvarna-16-inch-gas-powered-chainsaw">View Product</a></p>',
2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSTextWidgetConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor') and TypeOFMapping = 'ContentPageMapping'
and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' )
and WidgetsKey = '8787878' and LocaleId = 1)

insert into ZnodeCMSTextWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,Text,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor'),'7878888',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider') and PageName = 'Husqvarna Products' ),'ContentPageMapping',
'<h5 class="pt-4" style="border-bottom: 1px solid #000;">All Lawn Mowers</h5>  <p>&nbsp;</p>',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSTextWidgetConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor') and TypeOFMapping = 'ContentPageMapping'
and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider') and PageName = 'Husqvarna Products' )
and WidgetsKey = '7878888' and LocaleId = 1)

insert into ZnodeCMSTextWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,Text,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor'),'455566777',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets') and PageName = 'Promotional Products' ),'ContentPageMapping',
'<h4 style="color: #ff6f00;">SNAP-ON 3/8 DRIVE SWIVEL</h4>  <h4 style="color: #ff6f00;">HEAD RATCHETS</h4>  <div style="width: 550px;">Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry''s standard dummy text ever since the 1500s,</div>  <p class="my-4"><a class="btn btn-primary" href="snap-on-3by8-drive-swivel-head-ratchet">View Product</a></p>'
,2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSTextWidgetConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor') and TypeOFMapping = 'ContentPageMapping'
and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets') and PageName = 'Promotional Products' )
and WidgetsKey = '455566777' and LocaleId = 1)

insert into ZnodeCMSTextWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,Text,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor'),'3456567878',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets') and PageName = 'Promotional Products' ),'ContentPageMapping',
'<h5 class="pt-4" style="border-bottom: 1px solid #000;">Hand Tools</h5>'
,2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSTextWidgetConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor') and TypeOFMapping = 'ContentPageMapping'
and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets') and PageName = 'Promotional Products' )
and WidgetsKey = '3456567878' and LocaleId = 1)

----


insert into ZnodeCMSSearchWidget(CMSWidgetsId,AttributeCode,SearchKeyword,LocaleId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'SearchWidget'),null,'Lawn Mower',1,'7877880',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider')
and PageName = 'Husqvarna Products' ),'ContentPageMapping',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSSearchWidget where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'SearchWidget')
and LocaleId = 1 and WidgetsKey = '7877880' and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider')
and PageName = 'Husqvarna Products' ))

insert into ZnodeCMSSearchWidget(CMSWidgetsId,AttributeCode,SearchKeyword,LocaleId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'SearchWidget'),null,'Tools',1,'1255667',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets')
and PageName = 'Promotional Products' ),'ContentPageMapping',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSSearchWidget where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'SearchWidget')
and LocaleId = 1 and WidgetsKey = '1255667' and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets')
and PageName = 'Promotional Products' ))


insert into ZnodeCMSSearchWidget(CMSWidgetsId,AttributeCode,SearchKeyword,LocaleId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'SearchWidget'),null,'Saw',1,'1255667',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video')
and PageName = 'Circular Saws' ),'ContentPageMapping',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSSearchWidget where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'SearchWidget')
and LocaleId = 1 and WidgetsKey = '1255667' and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Video')
and PageName = 'Circular Saws' ))

----

insert into ZnodeCMSSlider(Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsPublished,PublishStateId)
select 'Husqvarna banner',2,getdate(),2,getdate(),0,2
where not exists(select * from ZnodeCMSSlider where Name='Husqvarna banner')

insert into ZnodeCMSSliderBanner(CMSSliderId,TextAlignment,BannerSequence,ActivationDate,ExpirationDate,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSSliderId from ZnodeCMSSlider where Name='Husqvarna banner'),'Left Align',null,null,null,2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSSliderBanner where CMSSliderId = (select top 1 CMSSliderId from ZnodeCMSSlider where Name='Husqvarna banner')
      and TextAlignment = 'Left Align')

insert into ZnodeCMSSliderBannerLocale(CMSSliderBannerId,LocaleId,Description,ImageAlternateText,MediaId,Title,ButtonLabelName,ButtonLink,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSSliderBannerId from ZnodeCMSSliderBanner where CMSSliderId = (select top 1 CMSSliderId from ZnodeCMSSlider where Name='Husqvarna banner') and TextAlignment = 'Left Align'),
     1, '<h5 class="d-none d-md-flex flex-column" style="color: #ff6f00; padding-left: 15px;">HUSQVARNA 42 INCH MULCHING<br /> CAPABILTY LAWN TRACTOR</h5>  <p class="d-none d-md-flex flex-column" style="color: #fff; max-width: 450px; padding-left: 15px;">Husqvarna''s yard tractors offer premium performance with quality results. Their compact size makes them easy to maneuver and require less space for storage.</p>  <p class="d-none d-md-flex"><a class="btn btn-primary" style="padding-right: 30px;" href="husqvarna-42-inch-mulching-capability-lawn-tractor">View Product</a></p>',
	 null,(select top 1 MediaId from ZnodeMedia where Path = '33724e67-ebb0-4b41-aa35-532a1b3abcc0full-hero-image_husqvarna-lawn-tractor.png' and FileName = 'full-hero-image_husqvarna-lawn-tractor.png'),
	 'Husqvarna banner',null,null,2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSSliderBannerLocale where CMSSliderBannerId = (select top 1 CMSSliderBannerId from ZnodeCMSSliderBanner where CMSSliderId = (select top 1 CMSSliderId from ZnodeCMSSlider where Name='Husqvarna banner') and TextAlignment = 'Left Align')
	and LocaleId = 1 and MediaId = (select top 1 MediaId from ZnodeMedia where Path = '33724e67-ebb0-4b41-aa35-532a1b3abcc0full-hero-image_husqvarna-lawn-tractor.png' and FileName = 'full-hero-image_husqvarna-lawn-tractor.png'))
insert into ZnodeCMSWidgetSliderBanner(CMSSliderId,Type,Navigation,AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,CMSWidgetsId,WidgetsKey
,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSSliderId from ZnodeCMSSlider where Name='Husqvarna banner'),'Boxed','Dots',0,null,0,'fade',(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'BannerSlider'),
	'889900',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider')
	and PageName = 'Husqvarna Products' ),'ContentPageMapping',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSWidgetSliderBanner where CMSSliderId = (select top 1 CMSSliderId from ZnodeCMSSlider where Name='Husqvarna banner')
and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'BannerSlider') and WidgetsKey = '889900' and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Content And Banner Slider')
	and PageName = 'Husqvarna Products' ))
--------------


------

insert into ZnodeCMSMediaConfiguration(CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,MediaId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'ImageWidget'),'34455667677',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets')
and PageName = 'Promotional Products' ),'ContentPageMapping', (select top 1 MediaId from ZnodeMedia where filename = 'hero_snap-on.png' and path = 'f7199e64-c5d2-419a-8ef4-ff2f7bd34bdchero_snap-on.png'),
	2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSMediaConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'ImageWidget') and WidgetsKey = '34455667677'
and CMSMappingId =(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Simple Search With Image And Multiple Text Widgets')
and PageName = 'Promotional Products' ) and MediaId = (select top 1 MediaId from ZnodeMedia where filename = 'hero_snap-on.png' and path = 'f7199e64-c5d2-419a-8ef4-ff2f7bd34bdchero_snap-on.png')
	)

insert into ZnodeCMSMediaConfiguration(CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,MediaId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'ImageWidget'),'6787889',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
and PageName = 'Cordless Woodworking' ),'ContentPageMapping', (select top 1 MediaId from ZnodeMedia where filename = 'full-hero_dewalt-tools.png' and path = '072ab305-6b9b-452b-bf02-7a178b36224ffull-hero_dewalt-tools.png'),
	2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSMediaConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'ImageWidget') and WidgetsKey = '6787889'
and CMSMappingId =(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
and PageName = 'Cordless Woodworking' ) and MediaId = (select top 1 MediaId from ZnodeMedia where filename = 'full-hero_dewalt-tools.png' and path = '072ab305-6b9b-452b-bf02-7a178b36224ffull-hero_dewalt-tools.png')
)

insert into ZnodeCMSMediaConfiguration(CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,MediaId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'ImageWidget'),'98988999',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
and PageName = 'Cordless Woodworking' ),'ContentPageMapping', (select top 1 MediaId from ZnodeMedia where filename = 'cordless-image.jpg' and path = '426a3c48-8194-4431-aba3-b743167ac3f3cordless-image.jpg'),
	2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSMediaConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'ImageWidget') and WidgetsKey = '98988999'
and CMSMappingId =(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
and PageName = 'Cordless Woodworking' ) and MediaId = (select top 1 MediaId from ZnodeMedia where filename = 'cordless-image.jpg' and path = '426a3c48-8194-4431-aba3-b743167ac3f3cordless-image.jpg')
)

insert into ZnodeCMSMediaConfiguration(CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,MediaId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'ImageWidget'),'56778899',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
and PageName = 'Cordless Woodworking' ),'ContentPageMapping', (select top 1 MediaId from ZnodeMedia where filename = 'cordless-image.jpg' and path = '426a3c48-8194-4431-aba3-b743167ac3f3cordless-image.jpg'),
	2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSMediaConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'ImageWidget') and WidgetsKey = '56778899'
and CMSMappingId =(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
and PageName = 'Cordless Woodworking' ) and MediaId = (select top 1 MediaId from ZnodeMedia where filename = 'cordless-image.jpg' and path = '426a3c48-8194-4431-aba3-b743167ac3f3cordless-image.jpg')
)

insert into ZnodeCMSMediaConfiguration(CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,MediaId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'ImageWidget'),'809898',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
and PageName = 'Cordless Woodworking' ),'ContentPageMapping', (select top 1 MediaId from ZnodeMedia where filename = 'welding machine.jpg' and path = '016c571b-ccf0-4925-93ad-c863109267a1welding machine.jpg'),
	2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSMediaConfiguration where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code  = 'ImageWidget') and WidgetsKey = '809898'
and CMSMappingId =(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider')
and PageName = 'Cordless Woodworking' ) and MediaId = (select top 1 MediaId from ZnodeMedia where filename = 'welding machine.jpg' and path = '016c571b-ccf0-4925-93ad-c863109267a1welding machine.jpg')
)

insert into ZnodeCMSFormWidgetConfiguration(LocaleId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,FormBuilderId,FormTitle,ButtonText,IsTextMessage
,TextMessage,RedirectURL,IsShowCaptcha,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'FormWidget'),'72772',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Content And Form Sections')
	and PageName = 'Register for Business Account' ),'ContentPageMapping',(select top 1 FormBuilderId from Znodeformbuilder where FormCode = 'ProvideYourBusinessInformation')
	,'Provide Your Business Information','Submit',1,'Thank you for submitted','',0,2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSFormWidgetConfiguration where LocaleId = 1 and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'FormWidget')
    and WidgetsKey='72772' and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Content And Form Sections')
	and PageName = 'Register for Business Account' ))

insert into ZnodeCMSWidgetTitleConfiguration(TitleCode,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'Circular Saws',(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel'),'2253',
 (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'),'PortalMapping',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Circular Saws'
and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel') and WidgetsKey = '2253'
and CMSMappingId = (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'))

insert into ZnodeCMSWidgetTitleConfiguration(TitleCode,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'Husqvarna Products',(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel'),'2253',
 (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'),'PortalMapping',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Husqvarna Products'
and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel') and WidgetsKey = '2253'
and CMSMappingId = (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'))

insert into ZnodeCMSWidgetTitleConfiguration(TitleCode,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'Check Order Status',(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel'),'2253',
 (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'),'PortalMapping',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Check Order Status'
and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel') and WidgetsKey = '2253'
and CMSMappingId = (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'))

insert into ZnodeCMSWidgetTitleConfiguration(TitleCode,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'Cordless Woodworking',(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel'),'2253',
 (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'),'PortalMapping',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Cordless Woodworking'
and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel') and WidgetsKey = '2253'
and CMSMappingId = (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'))

insert into ZnodeCMSWidgetTitleConfiguration(TitleCode,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'Track Order',(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel'),'22530',
 (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'),'PortalMapping',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Track Order'
and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel') and WidgetsKey = '22530'
and CMSMappingId = (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'))

insert into ZnodeCMSWidgetTitleConfiguration(TitleCode,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'Track Order',(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel'),'22530',
 (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'),'PortalMapping',2,getdate(),2,getdate()
where not exists(select * from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Track Order'
and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel') and WidgetsKey = '22530'
and CMSMappingId = (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'))


insert into ZnodeCMSWidgetTitleConfigurationLocale(
CMSWidgetTitleConfigurationId,MediaId,Title,Url,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsNewTab,DisplayOrder)
select (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Circular Saws')
,0,'Circular Saws','/circular_saws_products',1,2,getdate(),2,getdate(),0,999
where not exists(select * from ZnodeCMSWidgetTitleConfigurationLocale where CMSWidgetTitleConfigurationId = (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Circular Saws')
and LocaleId = 1 )

insert into ZnodeCMSWidgetTitleConfigurationLocale(
CMSWidgetTitleConfigurationId,MediaId,Title,Url,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsNewTab,DisplayOrder)
select (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Husqvarna Products')
,0,'Husqvarna Products','/husqvarna_products',1,2,getdate(),2,getdate(),0,999
where not exists(select * from ZnodeCMSWidgetTitleConfigurationLocale where CMSWidgetTitleConfigurationId = (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Husqvarna Products')
and LocaleId = 1 )


insert into ZnodeCMSWidgetTitleConfigurationLocale(
CMSWidgetTitleConfigurationId,MediaId,Title,Url,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsNewTab,DisplayOrder)
select (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Check Order Status')
,0,'Check Order Status','/User/GetOrderDetails',1,2,getdate(),2,getdate(),0,999
where not exists(select * from ZnodeCMSWidgetTitleConfigurationLocale where CMSWidgetTitleConfigurationId = (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Check Order Status')
and LocaleId = 1 )

insert into ZnodeCMSWidgetTitleConfigurationLocale(
CMSWidgetTitleConfigurationId,MediaId,Title,Url,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsNewTab,DisplayOrder)
select (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Cordless Woodworking')
,0,'Cordless Woodworking','/cordless-woodworking',1,2,getdate(),2,getdate(),0,999
where not exists(select * from ZnodeCMSWidgetTitleConfigurationLocale where CMSWidgetTitleConfigurationId = (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Cordless Woodworking')
and LocaleId = 1 )

insert into ZnodeCMSWidgetTitleConfigurationLocale(
CMSWidgetTitleConfigurationId,MediaId,Title,Url,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsNewTab,DisplayOrder)
select (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Track Order')
,0,'Track Order','https://webstore-b2b-znode.amla.io/User/GetOrderDetails',1,2,getdate(),2,getdate(),0,999
where not exists(select * from ZnodeCMSWidgetTitleConfigurationLocale where CMSWidgetTitleConfigurationId = (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Track Order')
and LocaleId = 1 )

-----

insert into ZnodeMediaCategory(MediaPathId,MediaId,MediaAttributeFamilyId,Path,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'MAXWELLS HARDWARE'),
(select top 1 MediaId from ZnodeMedia where Path = '994e156f-74db-4f78-bf96-9a9170559552Circular-Saw-Video.mp4' and FileName = 'Circular-Saw-Video.mp4'),
(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Video'), null,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaCategory where MediaPathId=(select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'MAXWELLS HARDWARE')
and MediaId = (select top 1 MediaId from ZnodeMedia where Path = '994e156f-74db-4f78-bf96-9a9170559552Circular-Saw-Video.mp4' and FileName = 'Circular-Saw-Video.mp4')
and MediaAttributeFamilyId=(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Video')
)

insert into ZnodeMediaCategory(MediaPathId,MediaId,MediaAttributeFamilyId,Path,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'MAXWELLS HARDWARE'),
(select  top 1 MediaId  from ZnodeMedia where Path = '33724e67-ebb0-4b41-aa35-532a1b3abcc0full-hero-image_husqvarna-lawn-tractor.png' and FileName = 'full-hero-image_husqvarna-lawn-tractor.png'),
(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Image'), null,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaCategory where MediaPathId=(select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'MAXWELLS HARDWARE')
and MediaId = (select  top 1 MediaId  from ZnodeMedia where Path = '33724e67-ebb0-4b41-aa35-532a1b3abcc0full-hero-image_husqvarna-lawn-tractor.png' and FileName = 'full-hero-image_husqvarna-lawn-tractor.png')
and MediaAttributeFamilyId=(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Image')
)

insert into ZnodeMediaCategory(MediaPathId,MediaId,MediaAttributeFamilyId,Path,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'MAXWELLS HARDWARE'),
(select top 1 MediaId from ZnodeMedia where Path = '426a3c48-8194-4431-aba3-b743167ac3f3cordless-image.jpg' and FileName = 'cordless-image.jpg'),
(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Image'), null,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaCategory where MediaPathId=(select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'MAXWELLS HARDWARE')
and MediaId = (select top 1 MediaId from ZnodeMedia where Path = '426a3c48-8194-4431-aba3-b743167ac3f3cordless-image.jpg' and FileName = 'cordless-image.jpg')
and MediaAttributeFamilyId=(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Image')
)

insert into ZnodeMediaCategory(MediaPathId,MediaId,MediaAttributeFamilyId,Path,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'MAXWELLS HARDWARE'),
(select top 1 MediaId from ZnodeMedia where Path = '072ab305-6b9b-452b-bf02-7a178b36224ffull-hero_dewalt-tools.png' and FileName = 'full-hero_dewalt-tools.png'),
(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Image'), null,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaCategory where MediaPathId=(select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'MAXWELLS HARDWARE')
and MediaId = (select top 1 MediaId from ZnodeMedia where Path = '072ab305-6b9b-452b-bf02-7a178b36224ffull-hero_dewalt-tools.png' and FileName = 'full-hero_dewalt-tools.png')
and MediaAttributeFamilyId=(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Image')
)

insert into ZnodeMediaCategory(MediaPathId,MediaId,MediaAttributeFamilyId,Path,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'MAXWELLS HARDWARE'),
(select top 1 MediaId  from ZnodeMedia where Path = 'f7199e64-c5d2-419a-8ef4-ff2f7bd34bdchero_snap-on.png' and FileName = 'hero_snap-on.png'),
(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Image'), null,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaCategory where MediaPathId=(select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'MAXWELLS HARDWARE')
and MediaId =(select top 1 MediaId  from ZnodeMedia where Path = 'f7199e64-c5d2-419a-8ef4-ff2f7bd34bdchero_snap-on.png' and FileName = 'hero_snap-on.png')
and MediaAttributeFamilyId=(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Image')
)

insert into ZnodeMediaCategory(MediaPathId,MediaId,MediaAttributeFamilyId,Path,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'Root'),
(select top 1 MediaId from ZnodeMedia where Path = '016c571b-ccf0-4925-93ad-c863109267a1welding machine.jpg' and FileName = 'welding machine.jpg'),
(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Image'), null,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMediaCategory where MediaPathId=(select top 1 MediaPathId from ZnodeMediaPathLocale where PathName = 'Root')
and MediaId =(select top 1 MediaId from ZnodeMedia where Path = '016c571b-ccf0-4925-93ad-c863109267a1welding machine.jpg' and FileName = 'welding machine.jpg')
and MediaAttributeFamilyId=(select top 1 MediaAttributeFamilyId from ZnodeMediaAttributeFamily where FamilyCode = 'Image')
)
end
go

--dt 30-07-2020 ZPD-11731 --> ZPD-11579
if exists(select * from ZnodePortal where StoreCode = 'MaxwellsHardware')
begin
	insert into ZnodeCMSWidgetProduct(PublishProductId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DisplayOrder,SKU)
	select null,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList'),'9794455',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ),
	'ContentPageMapping',2,getdate(),2,getdate(),1,'z-1234580'
	where not exists(select * from ZnodeCMSWidgetProduct where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList') and
	WidgetsKey = '9794455' and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' )
	and SKU = 'z-1234580')

	insert into ZnodeCMSWidgetProduct(PublishProductId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DisplayOrder,SKU)
	select null,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList'),'9794455',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ),
	'ContentPageMapping',2,getdate(),2,getdate(),1,'z-1234581'
	where not exists(select * from ZnodeCMSWidgetProduct where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList') and
	WidgetsKey = '9794455' and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' )
	and SKU = 'z-1234581')

	insert into ZnodeCMSWidgetProduct(PublishProductId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DisplayOrder,SKU)
	select null,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList'),'9794455',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ),
	'ContentPageMapping',2,getdate(),2,getdate(),1,'z-1234567'
	where not exists(select * from ZnodeCMSWidgetProduct where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList') and
	WidgetsKey = '9794455' and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' )
	and SKU = 'z-1234567')

	insert into ZnodeCMSWidgetProduct(PublishProductId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DisplayOrder,SKU)
	select null,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList'),'9794455',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ),
	'ContentPageMapping',2,getdate(),2,getdate(),1,'z-1234568'
	where not exists(select * from ZnodeCMSWidgetProduct where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList') and
	WidgetsKey = '9794455' and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' )
	and SKU = 'z-1234568')

	insert into ZnodeCMSWidgetProduct(PublishProductId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DisplayOrder,SKU)
	select null,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList'),'9794455',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ),
	'ContentPageMapping',2,getdate(),2,getdate(),1,'z-1234582'
	where not exists(select * from ZnodeCMSWidgetProduct where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList') and
	WidgetsKey = '9794455' and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' )
	and SKU = 'z-1234582')

	insert into ZnodeCMSWidgetProduct(PublishProductId,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DisplayOrder,SKU)
	select null,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList'),'9794455',(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ),
	'ContentPageMapping',2,getdate(),2,getdate(),1,'z-1234579'
	where not exists(select * from ZnodeCMSWidgetProduct where CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ProductList') and
	WidgetsKey = '9794455' and CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
	CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' )
	and SKU = 'z-1234579')
end

--dt 31-07-2020 ZPD-11730 / ZPD-11731
if exists(select * from ZnodePortal where StoreCode = 'MaxwellsHardware')
begin
update ZnodeCMSTextWidgetConfiguration set text ='<h3 style="font-size: 26px; text-align: center; display: block; margin: 0 auto; text-transform: uppercase;">Related Featured Products</h3>'
where CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ) 
and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor')
and WidgetsKey = '8787878'

update ZnodeCMSTextWidgetConfiguration set text ='<h3 style="color: #ff6f00;">HANDY-DANDY SANDER</h3>  <p class="font-16">More power, less maintenance, and did we forget to mention it''s brushless? It also has allows for easy, single-hand adjustmants for speed and power, as well as being easled to prevent dust getting inside and gunking up the machine. There''s even a port for your dust collector hose to stop the mess before it happens</p>  <p class="my-4"><a class="btn btn-secondary" href="https://webstore-b2b-znode.amla.io/husqvarna-16-inch-gas-powered-chainsaw">View Product</a></p>'
where CMSMappingId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = 'Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider') and PageName = 'Cordless Woodworking' ) 
and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TemplateTextEditor')
and WidgetsKey = '2333445'

update a set DisplayOrder = 6
from ZnodeFormBuilderAttributeMapper a
inner join ZnodeGlobalAttribute b on a.GlobalAttributeId = b.GlobalAttributeId
where b.AttributeCode = 'WorkEmailAddress' and FormBuilderId = (select top 1 FormBuilderId from ZnodeFormBuilder where FormCode = 'ProvideYourBusinessInformation')
 
update a set DisplayOrder = 7
from ZnodeFormBuilderAttributeMapper a
inner join ZnodeGlobalAttribute b on a.GlobalAttributeId = b.GlobalAttributeId
where b.AttributeCode = 'WorkPhoneNumber' and FormBuilderId = (select top 1 FormBuilderId from ZnodeFormBuilder where FormCode = 'ProvideYourBusinessInformation')
 
update a set DisplayOrder = 8
from ZnodeFormBuilderAttributeMapper a
inner join ZnodeGlobalAttribute b on a.GlobalAttributeId = b.GlobalAttributeId
where b.AttributeCode = 'Address' and FormBuilderId = (select top 1 FormBuilderId from ZnodeFormBuilder where FormCode = 'ProvideYourBusinessInformation')
 
update a set DisplayOrder = 9
from ZnodeFormBuilderAttributeMapper a
inner join ZnodeGlobalAttribute b on a.GlobalAttributeId = b.GlobalAttributeId
where b.AttributeCode = 'City' and FormBuilderId = (select top 1 FormBuilderId from ZnodeFormBuilder where FormCode = 'ProvideYourBusinessInformation')
  
update a set DisplayOrder = 10
from ZnodeFormBuilderAttributeMapper a
inner join ZnodeGlobalAttribute b on a.GlobalAttributeId = b.GlobalAttributeId
where b.AttributeCode = 'State' and FormBuilderId = (select top 1 FormBuilderId from ZnodeFormBuilder where FormCode = 'ProvideYourBusinessInformation')
  
update a set DisplayOrder = 11
from ZnodeFormBuilderAttributeMapper a
inner join ZnodeGlobalAttribute b on a.GlobalAttributeId = b.GlobalAttributeId
where b.AttributeCode = 'ZipCode' and FormBuilderId = (select top 1 FormBuilderId from ZnodeFormBuilder where FormCode = 'ProvideYourBusinessInformation')
  
update a set DisplayOrder = 12
from ZnodeFormBuilderAttributeMapper a
inner join ZnodeGlobalAttribute b on a.GlobalAttributeId = b.GlobalAttributeId
where b.AttributeCode = 'LastStatementBalance' and FormBuilderId = (select top 1 FormBuilderId from ZnodeFormBuilder where FormCode = 'ProvideYourBusinessInformation')
  
update a set DisplayOrder = 13
from ZnodeFormBuilderAttributeMapper a
inner join ZnodeGlobalAttribute b on a.GlobalAttributeId = b.GlobalAttributeId
where b.AttributeCode = 'ReferredBy' and FormBuilderId = (select top 1 FormBuilderId from ZnodeFormBuilder where FormCode = 'ProvideYourBusinessInformation')
end   

UPDATE A SET TextMessage = 'Thank you for submission'
FROM ZnodeCMSFormWidgetConfiguration a
INNER JOIN ZnodeCMSContentPages b on b.CMSContentPagesId = a.CMSMappingId
INNER JOIN ZnodeCMSTemplate c ON b.CMSTemplateId = c.CMSTemplateId
WHERE A.TypeOFMapping = 'ContentPageMapping' AND B.PageName = 'Register for Business Account'
AND c.FileName = 'LandingFormInfoTemplate'


insert into ZnodeCMSContentPages(PortalId,CMSTemplateId,PageName,ActivationDate,ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsPublished,PublishStateId) 
select ZP.PortalId , (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = '404'),'404', null,null,1,2,getdate(),2,getdate(),0,1 
from ZnodePortal ZP 
where StoreCode in ('MaxwellsHardware','MaxwellsPowerTools','MaxwellsSafetyGear')
and not exists(select * from ZnodeCMSContentPages x where x.PortalId = ZP.PortalId and x.CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = '404') 
and x.PageName = '404' ) 

insert into ZnodeCMSContentPagesLocale(CMSContentPagesId,LocaleId,PageTitle,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate) 
select CMSContentPagesId,1,'404',2,getdate(),2,getdate() 
from ZnodeCMSContentPages a 
where a.PageName = '404' and not exists(select * from ZnodeCMSContentPagesLocale x where x.CMSContentPagesId = a.CMSContentPagesId and x.LocaleId = 1 )
and PortalId in (select PortalId from ZnodePortal where StoreCode in ('MaxwellsHardware','MaxwellsPowerTools','MaxwellsSafetyGear'))

insert into ZnodeCMSContentPageGroupMapping(CMSContentPageGroupId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate) 
select (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware'),     
(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and     
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = '404')     and PageName = '404' ),     2,getdate(),2,getdate() 
where not exists(select * from ZnodeCMSContentPageGroupMapping where CMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware')     
and CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') 
and     CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = '404')     and PageName = '404' ))
and exists(select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware')
and exists(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsHardware') and     
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = '404')     and PageName = '404' )

insert into ZnodeCMSContentPageGroupMapping(CMSContentPageGroupId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate) 
select (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware'),     
(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsPowerTools') and     
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = '404')     and PageName = '404' ),     2,getdate(),2,getdate() 
where not exists(select * from ZnodeCMSContentPageGroupMapping where CMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware')     
and CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsPowerTools') 
and     CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = '404')     and PageName = '404' ))
and exists(select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware')
and exists(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsPowerTools') and     
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = '404')     and PageName = '404' )

insert into ZnodeCMSContentPageGroupMapping(CMSContentPageGroupId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate) 
select (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware'),     
(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsSafetyGear') and     
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = '404')     and PageName = '404' ),     2,getdate(),2,getdate() 
where not exists(select * from ZnodeCMSContentPageGroupMapping where CMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware')     
and CMSContentPagesId = (select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsSafetyGear') 
and     CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = '404')     and PageName = '404' ))
and exists(select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where ParentCMSContentPageGroupId = (select top 1 CMSContentPageGroupId from ZnodeCMSContentPageGroup where Code = 'Root') and Code = 'Maxwell Hardware')
and exists(select top 1 CMSContentPagesId from ZnodeCMSContentPages where PortalId = (select top 1 PortalId from ZnodePortal where StoreCode = 'MaxwellsSafetyGear') and     
CMSTemplateId = (select top 1 CMSTemplateId from ZnodeCMSTemplate where name = '404')     and PageName = '404' )

insert into [ZnodeCMSTextWidgetConfiguration]
(LocaleId
,CMSWidgetsId
,WidgetsKey
,CMSMappingId
,TypeOFMapping
,Text
,CreatedBy
,CreatedDate
,ModifiedBy
,ModifiedDate)
select 1,(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TextEditor'),'7771123',CMSContentPagesId,'ContentPageMapping'
,'<div>&nbsp; &nbsp; &lt;div &gt;<br>&nbsp; &nbsp; &nbsp; &nbsp; &lt;h3 class="text-center"&gt;Page not found&lt;/h3&gt;<br>&nbsp; &nbsp; &lt;/div&gt;</div>',2,getdate(),2,getdate()
from ZnodeCMSContentPages a where PageName = '404'
and PortalId in (select PortalId from ZnodePortal where StoreCode in ('MaxwellsHardware','MaxwellsPowerTools','MaxwellsSafetyGear'))
and not exists(select * from [ZnodeCMSTextWidgetConfiguration] b where b.CMSMappingId = a.CMSContentPagesId and b.LocaleId=1 and WidgetsKey = '7771123'
and b.CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'TextEditor')
and TypeOFMapping = 'ContentPageMapping')

insert into ZnodeCMSSeoDetail(CMSSEOTypeId
,SEOId
,IsRedirect
,MetaInformation
,PortalId
,SEOUrl
,CreatedBy
,CreatedDate
,ModifiedBy
,ModifiedDate
,IsPublish
,SEOCode
,PublishStateId)
select (select top 1 CMSSEOTypeId from ZnodeCMSSEOType WHERE Name = 'Content Page'),null,0,null,PortalId,'404',2,getdate(),2,getdate(),0,'404',2
from ZnodePortal a
where StoreCode in ('MaxwellsHardware','MaxwellsPowerTools','MaxwellsSafetyGear') and
not exists(select * from ZnodeCMSSeoDetail b where CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType WHERE Name = 'Content Page')
and a.PortalId = b.PortalId and SEOUrl = '404')

insert into ZnodeCMSSeoDetailLocale(CMSSEODetailId
,LocaleId
,SEOTitle
,SEODescription
,SEOKeywords
,CreatedBy
,CreatedDate
,ModifiedBy
,ModifiedDate
,CanonicalURL
,RobotTag)
select a.CMSSEODetailId,1,null,null,null,2,getdate(),2,getdate(),'404','None'
from ZnodeCMSSeoDetail A WHERE CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType WHERE Name = 'Content Page')
and SEOUrl = '404'
and not exists(select * from ZnodeCMSSeoDetailLocale b where a.CMSSEODetailId = b.CMSSEODetailId and b.LocaleId = 1)
and a.PortalId in (select PortalId from ZnodePortal where StoreCode in ('MaxwellsHardware','MaxwellsPowerTools','MaxwellsSafetyGear'))
