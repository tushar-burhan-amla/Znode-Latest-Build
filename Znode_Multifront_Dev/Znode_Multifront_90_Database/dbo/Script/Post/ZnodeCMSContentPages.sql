
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