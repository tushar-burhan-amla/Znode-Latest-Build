Insert  INTO ZnodeCMSWidgets (Code,DisplayName,IsConfigurable,FileName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'HomeRecommendations' ,'RecommendedProducts',0,'Product_List.png',2,Getdate(),2,Getdate() where not exists
(SELECT * FROM ZnodeCMSWidgets where Code = 'HomeRecommendations')
UNION ALL 
SELECT 'PDPRecommendations' ,'RecommendedProducts',0,'Product_List.png',2,Getdate(),2,Getdate() where not exists
(SELECT * FROM ZnodeCMSWidgets where Code = 'PDPRecommendations')
UNION ALL
SELECT 'CartRecommendations' ,'RecommendedProducts',0,'Product_List.png',2,Getdate(),2,Getdate() where not exists
(SELECT * FROM ZnodeCMSWidgets where Code = 'CartRecommendations')

go
Insert  INTO ZnodeCMSWidgets (Code,DisplayName,IsConfigurable,FileName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'ImageWidget' ,'ImageWidget',1,'Text_Editor.png',2,Getdate(),2,Getdate() where not exists
(SELECT * FROM ZnodeCMSWidgets where Code = 'ImageWidget')
go
Insert  INTO ZnodeCMSWidgets (Code,DisplayName,IsConfigurable,FileName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'VideoWidget' ,'Video Widget',1,'Text_Editor.png',2,Getdate(),2,Getdate() where not exists
(SELECT * FROM ZnodeCMSWidgets where Code = 'VideoWidget')

--dt 03-02-2020 ZPD-7565 --> ZPD-9001
insert into [ZnodeCMSWidgets] ([Code],[DisplayName],[IsConfigurable],[FileName],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate])
select 'MegaMenuNavigation','MegaMenuNavigation',0,'Top-Menu.png',2,GETDATE(),2,GETDATE()
where not exists(select * from [ZnodeCMSWidgets] where [Code] = 'MegaMenuNavigation')

-- Content container ZPD-13449 Dt-06Dec2021
GO
SET IDENTITY_INSERT [dbo].[ZnodeCMSContentContainer] ON 
GO
INSERT [dbo].[ZnodeCMSContentContainer] ([CMSContentContainerId], [Name], [ContainerKey], [FamilyId], [Tags], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [PublishStateId]) 
SELECT [CMSContentWidgetId], [Name], [WidgetKey], [FamilyId], [Tags], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], NULL
FROM ZnodeCMSContentWidget
WHERE CMSContentWidgetId NOT IN (SELECT CMSContentContainerId FROM [ZnodeCMSContentContainer])
GO
SET IDENTITY_INSERT [dbo].[ZnodeCMSContentContainer] OFF
GO


SET IDENTITY_INSERT [dbo].[ZnodeCMSContainerTemplate] ON 
GO
INSERT [dbo].[ZnodeCMSContainerTemplate] ([CMSContainerTemplateId], [Code], [Name], [FileName], [MediaId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT [CMSWidgetTemplateId], [Code], [Name], [FileName], [MediaId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]
FROM ZnodeCMSWidgetTemplate
WHERE CMSWidgetTemplateId NOT IN (SELECT CMSContainerTemplateId FROM [ZnodeCMSContainerTemplate])
GO
SET IDENTITY_INSERT [dbo].[ZnodeCMSContainerTemplate] OFF
GO

SET IDENTITY_INSERT [dbo].[ZnodeCMSContainerProfileVariant] ON 
GO
DECLARE @PublishStateId INT
SELECT @PublishStateId=PublishStateId FROM ZnodePublishState WHERE PublishStateCode='DRAFT'
INSERT [dbo].[ZnodeCMSContainerProfileVariant] ([CMSContainerProfileVariantId], [CMSContentContainerId], [ProfileId], [PortalId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [PublishStateId]) 
SELECT [CMSWidgetProfileVariantId], [CMSContentWidgetId], [ProfileId], [PortalId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], @PublishStateId
FROM ZnodeCMSWidgetProfileVariant
WHERE CMSWidgetProfileVariantId NOT IN (SELECT CMSContainerProfileVariantId FROM [ZnodeCMSContainerProfileVariant])
GO
SET IDENTITY_INSERT [dbo].[ZnodeCMSContainerProfileVariant] OFF

GO
SET IDENTITY_INSERT [dbo].[ZnodeCMSContainerProfileVariantLocale] ON 
GO
DECLARE @IsActive BIT=1
INSERT [dbo].[ZnodeCMSContainerProfileVariantLocale] ([CMSContainerProfileVariantLocaleId], [CMSContainerProfileVariantId], [CMSContainerTemplateId], [LocaleId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) 
SELECT [CMSWidgetProfileVariantLocaleId], [CMSWidgetProfileVariantId], [CMSWidgetTemplateId], [LocaleId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], @IsActive
FROM ZnodeCMSWidgetProfileVariantLocale
WHERE CMSWidgetProfileVariantLocaleId NOT IN (SELECT CMSContainerProfileVariantLocaleId FROM [ZnodeCMSContainerProfileVariantLocale])
GO
SET IDENTITY_INSERT [dbo].[ZnodeCMSContainerProfileVariantLocale] OFF

--dt 31-01-2022 ZPD-17569 --> ZPD-17179
insert into [ZnodeCMSWidgets] ([Code],[DisplayName],[IsConfigurable],[FileName],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate])
select 'ContentContainer','Content Container',1,'Text_Editor.png',2,GETDATE(),2,GETDATE()
where not exists(select * from [ZnodeCMSWidgets] where [Code] = 'ContentContainer')

--dt 06-04-2022 ZPD-18857 
UPDATE ZnodeCMSWidgetTitleConfiguration
SET CMSWidgetsId=(SELECT TOP 1 CMSWidgetsId FROM ZnodeCMSWidgets WHERE Code='LinkPanel')
WHERE TitleCode='Register for a Business Account'
AND CMSWidgetsId = (SELECT TOP 1 CMSWidgetsId FROM ZnodeCMSWidgets WHERE Code='Helps')
AND WidgetsKey='2243'
AND CMSMappingId=(SELECT TOP 1 PortalId FROM ZnodePortal WHERE StoreCode='MaxwellsHardware' OR CompanyName='Maxwell''s Hardware')
AND TypeOFMapping='PortalMapping';

--dt 07-04-2022 ZPD-18857 
UPDATE ZnodeCMSWidgetTitleConfiguration
SET CMSWidgetsId=(SELECT TOP 1 CMSWidgetsId FROM ZnodeCMSWidgets WHERE Code='LinkPanel')
WHERE TitleCode='Promotional Products'
AND CMSWidgetsId = (SELECT TOP 1 CMSWidgetsId FROM ZnodeCMSWidgets WHERE Code='Helps')
AND WidgetsKey='2253'
AND CMSMappingId=(SELECT TOP 1 PortalId FROM ZnodePortal WHERE StoreCode='MaxwellsHardware' OR CompanyName='Maxwell''s Hardware')
AND TypeOFMapping='PortalMapping'

GO
--dt 07-04-2022 ZPD-18857
IF EXISTS(SELECT TOP 1 PortalId FROM znodePortal WHERE storecode = 'MaxwellsHardware')
BEGIN
	insert into ZnodeCMSWidgetTitleConfiguration(TitleCode,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	select 'Register for a Business Account',(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel'),'2243',
	 (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'),'PortalMapping',2,getdate(),2,getdate()
	where not exists(select * from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Register for a Business Account'
	and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel') and WidgetsKey = '2243'
	and CMSMappingId = (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'))


	insert into ZnodeCMSWidgetTitleConfigurationLocale(
	CMSWidgetTitleConfigurationId,MediaId,Title,Url,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsNewTab,DisplayOrder)
	select (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Register for a Business Account')
	,0,'Register for a Business Account','/provideyourbusinessform',1,2,getdate(),2,getdate(),0,999
	where not exists(select * from ZnodeCMSWidgetTitleConfigurationLocale where CMSWidgetTitleConfigurationId = (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Register for a Business Account')
	and LocaleId = 1 )

	insert into ZnodeCMSWidgetTitleConfiguration(TitleCode,CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	select 'Promotional Products',(select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel'),'2253',
	 (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'),'PortalMapping',2,getdate(),2,getdate()
	where not exists(select * from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Promotional Products'
	and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where code = 'LinkPanel') and WidgetsKey = '2253'
	and CMSMappingId = (select top 1 PortalId from znodePortal where storecode = 'MaxwellsHardware'))


	insert into ZnodeCMSWidgetTitleConfigurationLocale(
	CMSWidgetTitleConfigurationId,MediaId,Title,Url,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsNewTab,DisplayOrder)
	select (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Promotional Products')
	,0,'Promotional Products','/promotional_products',1,2,getdate(),2,getdate(),0,999
	where not exists(select * from ZnodeCMSWidgetTitleConfigurationLocale where CMSWidgetTitleConfigurationId = (select top 1 CMSWidgetTitleConfigurationId from ZnodeCMSWidgetTitleConfiguration where TitleCode = 'Promotional Products')
	and LocaleId = 1 )
END