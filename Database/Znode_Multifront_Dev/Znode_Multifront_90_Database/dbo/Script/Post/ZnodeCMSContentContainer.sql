DECLARE @PublishStateId INT;
SELECT @PublishStateId=PublishStateId FROM ZnodePublishState WHERE PublishStateCode='DRAFT'

--ZnodeGlobalEntity
INSERT INTO ZnodeGlobalEntity (EntityName,IsActive,TableName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsFamilyUnique)
SELECT 'Content Containers',1,'ZnodeWidgetGlobalAttributeValue',2, GETDATE(), 2, GETDATE(), 0
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalEntity WHERE GlobalEntityId=(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers'))

--ZnodeGlobalAttributeFamily
INSERT [dbo].[ZnodeGlobalAttributeFamily] ([FamilyCode], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'HomepageMarketingContentBlock', 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomepageMarketingContentBlock')

INSERT [dbo].[ZnodeGlobalAttributeFamily] ([FamilyCode], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'HomePageTicker', 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageTicker')

INSERT [dbo].[ZnodeGlobalAttributeFamily] ([FamilyCode], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'HomePageAdSpace', 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageAdSpace')

INSERT [dbo].[ZnodeGlobalAttributeFamily] ([FamilyCode], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'CartPageAdSpace', 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace')


--ZnodeGlobalAttributeFamilyLocale
INSERT [dbo].[ZnodeGlobalAttributeFamilyLocale] ([LocaleId], [GlobalAttributeFamilyId], [AttributeFamilyName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomepageMarketingContentBlock'),
N'HomePage Promo', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeFamilyLocale WHERE GlobalAttributeFamilyId=(SELECT GlobalAttributeFamilyId 
	FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomepageMarketingContentBlock'))

INSERT [dbo].[ZnodeGlobalAttributeFamilyLocale] ([LocaleId], [GlobalAttributeFamilyId], [AttributeFamilyName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageTicker'),
N'Home Page Ticker', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeFamilyLocale WHERE GlobalAttributeFamilyId=(SELECT GlobalAttributeFamilyId 
	FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageTicker'))

INSERT [dbo].[ZnodeGlobalAttributeFamilyLocale] ([LocaleId], [GlobalAttributeFamilyId], [AttributeFamilyName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageAdSpace'),
N'Home Page Ad Space', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeFamilyLocale WHERE GlobalAttributeFamilyId=(SELECT GlobalAttributeFamilyId 
	FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageAdSpace'))

INSERT [dbo].[ZnodeGlobalAttributeFamilyLocale] ([LocaleId], [GlobalAttributeFamilyId], [AttributeFamilyName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate])  
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace'),
N'Cart Page Ad Space', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeFamilyLocale WHERE GlobalAttributeFamilyId=(SELECT GlobalAttributeFamilyId 
	FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace'))


-- ZnodeCMSContentContainer
INSERT [dbo].[ZnodeCMSContentContainer] ([Name], [ContainerKey], [FamilyId], [Tags], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [PublishStateId]) 
SELECT N'Home Page Promo', N'HomePagePromo', 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomepageMarketingContentBlock'), 
N'', 2, GETDATE(), 2, GETDATE(), @PublishStateId
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')

INSERT [dbo].[ZnodeCMSContentContainer] ([Name], [ContainerKey], [FamilyId], [Tags], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [PublishStateId]) 
SELECT N'Home Page Ticker', N'HomePageTicker', 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageTicker'), 
N'', 2, GETDATE(), 2, GETDATE(), @PublishStateId
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePageTicker')

INSERT [dbo].[ZnodeCMSContentContainer] ([Name], [ContainerKey], [FamilyId], [Tags], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [PublishStateId]) 
SELECT N'Home Page Ad Space', N'AdSpace', 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageAdSpace'), 
NULL, 2, GETDATE(), 2, GETDATE(), @PublishStateId
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')

INSERT [dbo].[ZnodeCMSContentContainer] ([Name], [ContainerKey], [FamilyId], [Tags], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [PublishStateId]) 
SELECT N'Cart Page Ad Spaces', N'CartPageAdSpace', 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace'), 
NULL, 2, GETDATE(), 2, GETDATE(), @PublishStateId
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')


--ZnodeCMSContainerProfileVariant
INSERT [dbo].[ZnodeCMSContainerProfileVariant] ([CMSContentContainerId], [ProfileId], [PortalId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [PublishStateId]) 
SELECT (SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo'), 
NULL, NULL, 2, GETDATE(), 2, GETDATE(), @PublishStateId
WHERE EXISTS (SELECT * FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
AND NOT EXISTS (SELECT * FROM ZnodeCMSContainerProfileVariant WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)

INSERT [dbo].[ZnodeCMSContainerProfileVariant] ([CMSContentContainerId], [ProfileId], [PortalId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [PublishStateId]) 
SELECT (SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePageTicker'), 
NULL, NULL, 2, GETDATE(), 2, GETDATE(), @PublishStateId
WHERE EXISTS (SELECT * FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePageTicker')
AND NOT EXISTS (SELECT * FROM ZnodeCMSContainerProfileVariant WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer 
	WHERE ContainerKey='HomePageTicker') AND ProfileId IS NULL AND PortalId IS NULL)

INSERT [dbo].[ZnodeCMSContainerProfileVariant] ([CMSContentContainerId], [ProfileId], [PortalId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [PublishStateId]) 
SELECT (SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey IN ('AdSpace')), 
NULL, NULL, 2, GETDATE(), 2, GETDATE(), @PublishStateId
WHERE EXISTS (SELECT * FROM ZnodeCMSContentContainer WHERE ContainerKey IN ('AdSpace') )
AND NOT EXISTS (SELECT * FROM ZnodeCMSContainerProfileVariant WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer 
	WHERE ContainerKey IN ('AdSpace')) AND ProfileId IS NULL AND PortalId IS NULL)

INSERT [dbo].[ZnodeCMSContainerProfileVariant] ([CMSContentContainerId], [ProfileId], [PortalId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [PublishStateId]) 
SELECT (SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey IN ('CartPageAdSpace')), 
NULL, NULL, 2, GETDATE(), 2, GETDATE(), @PublishStateId
WHERE EXISTS (SELECT * FROM ZnodeCMSContentContainer WHERE ContainerKey IN ('CartPageAdSpace') )
AND NOT EXISTS (SELECT * FROM ZnodeCMSContainerProfileVariant WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer 
	WHERE ContainerKey IN ('CartPageAdSpace')) AND ProfileId IS NULL AND PortalId IS NULL)


-- Update Publish Status
UPDATE ZnodeCMSContentContainer
SET PublishStateId=@PublishStateId
WHERE ContainerKey IN ('HomePagePromo','HomePageTicker','AdSpace','CartPageAdSpace')
	AND PublishStateId<>@PublishStateId

UPDATE CPV
SET CPV.PublishStateId=@PublishStateId
FROM ZnodeCMSContainerProfileVariant CPV
INNER JOIN ZnodeCMSContentContainer CC ON CPV.CMSContentContainerId=CC.CMSContentContainerId
WHERE CC.ContainerKey IN ('HomePagePromo','HomePageTicker','AdSpace','CartPageAdSpace')
	AND CPV.PublishStateId<>@PublishStateId


--ZnodeCMSContainerProfileVariantLocale
INSERT [dbo].[ZnodeCMSContainerProfileVariantLocale] ([CMSContainerProfileVariantId], [CMSContainerTemplateId], [LocaleId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) 
SELECT 
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL),
NULL, 
1, 2, GETDATE(), 2, GETDATE(), 1
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSContainerProfileVariantLocale WHERE CMSContainerProfileVariantId=(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL))

INSERT [dbo].[ZnodeCMSContainerProfileVariantLocale] ([CMSContainerProfileVariantId], [CMSContainerTemplateId], [LocaleId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) 
SELECT 
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL),
NULL, 
1, 2, GETDATE(), 2, GETDATE(), 1
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSContainerProfileVariantLocale WHERE CMSContainerProfileVariantId=(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL))

INSERT [dbo].[ZnodeCMSContainerProfileVariantLocale] ([CMSContainerProfileVariantId], [CMSContainerTemplateId], [LocaleId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) 
SELECT 
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePageTicker') AND ProfileId IS NULL AND PortalId IS NULL),
NULL, 
1, 2, GETDATE(), 2, GETDATE(), 1
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSContainerProfileVariantLocale WHERE CMSContainerProfileVariantId=(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePageTicker') AND ProfileId IS NULL AND PortalId IS NULL))

INSERT [dbo].[ZnodeCMSContainerProfileVariantLocale] ([CMSContainerProfileVariantId], [CMSContainerTemplateId], [LocaleId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) 
SELECT 
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
NULL, 
1, 2, GETDATE(), 2, GETDATE(), 1
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSContainerProfileVariantLocale WHERE CMSContainerProfileVariantId=(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL))

INSERT [dbo].[ZnodeCMSContainerProfileVariantLocale] ([CMSContainerProfileVariantId], [CMSContainerTemplateId], [LocaleId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) 
SELECT 
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
NULL, 
1, 2, GETDATE(), 2, GETDATE(), 1
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSContainerProfileVariantLocale WHERE CMSContainerProfileVariantId=(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL))



--ZnodeGlobalEntityFamilyMapper

INSERT INTO ZnodeGlobalEntityFamilyMapper (GlobalAttributeFamilyId,GlobalEntityId,GlobalEntityValueId)
SELECT 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomepageMarketingContentBlock'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalEntityFamilyMapper
	WHERE GlobalAttributeFamilyId=(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomepageMarketingContentBlock')
	AND GlobalEntityId=(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
	AND GlobalEntityValueId=(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
)

INSERT INTO ZnodeGlobalEntityFamilyMapper (GlobalAttributeFamilyId,GlobalEntityId,GlobalEntityValueId)
SELECT 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageTicker'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePageTicker') AND ProfileId IS NULL AND PortalId IS NULL)
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalEntityFamilyMapper
	WHERE GlobalAttributeFamilyId=(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageTicker')
	AND GlobalEntityId=(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
	AND GlobalEntityValueId=(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePageTicker') AND ProfileId IS NULL AND PortalId IS NULL)
)

INSERT INTO ZnodeGlobalEntityFamilyMapper (GlobalAttributeFamilyId,GlobalEntityId,GlobalEntityValueId)
SELECT 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalEntityFamilyMapper
	WHERE GlobalAttributeFamilyId=(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace')
	AND GlobalEntityId=(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
	AND GlobalEntityValueId=(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
)





INSERT INTO ZnodeGlobalEntityFamilyMapper (GlobalAttributeFamilyId,GlobalEntityId,GlobalEntityValueId)
SELECT 
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageAdSpace'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalEntityFamilyMapper
	WHERE GlobalAttributeFamilyId=(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageAdSpace')
	AND GlobalEntityId=(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
	AND GlobalEntityValueId=(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
)


--ZnodeMediaServerMaster
INSERT INTO ZnodeMediaServerMaster (ServerName,PartialViewName,IsOtherServer,ThumbnailFolderName,ClassName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'Local','Local',0,'Thumbnail','LocalAgent',2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeMediaServerMaster WHERE MediaServerMasterId=(SELECT TOP 1 MediaServerMasterId FROM ZnodeMediaServerMaster WHERE PartialViewName='Local'))


-- ZnodeMedia
INSERT [dbo].[ZnodeMedia] ([MediaConfigurationId], [Path], [FileName], [Size], [Height], [Width], [Length], [Type], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Version]) 
SELECT (SELECT TOP 1 MediaConfigurationId FROM ZnodeMediaConfiguration 
	WHERE MediaServerMasterId=(SELECT TOP 1 MediaServerMasterId FROM ZnodeMediaServerMaster WHERE PartialViewName='Local') AND IsActive=1),
N'42bff3d2-0dbf-4b6d-b1be-321537912ad03945ac03-7e20-488b-a690-028e7bea2325ad-dewalt-lawncare.jpg', N'3945ac03-7e20-488b-a690-028e7bea2325ad-dewalt-lawncare.jpg', N'239347', N'500       ', N'750       ', N'239347    ', N'.jpg', 2, GETDATE(), 2, GETDATE(), 0
WHERE NOT EXISTS (SELECT * FROM ZnodeMedia WHERE [FileName]='3945ac03-7e20-488b-a690-028e7bea2325ad-dewalt-lawncare.jpg')

INSERT [dbo].[ZnodeMedia] ([MediaConfigurationId], [Path], [FileName], [Size], [Height], [Width], [Length], [Type], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Version]) 
SELECT (SELECT TOP 1 MediaConfigurationId FROM ZnodeMediaConfiguration
	WHERE MediaServerMasterId=(SELECT TOP 1 MediaServerMasterId FROM ZnodeMediaServerMaster WHERE PartialViewName='Local') AND IsActive=1),
N'847cd3a7-bcbd-4c94-b881-e67872a7fad93225d329-4e61-4158-bd65-5402a20dfc5bad-100off-tools.jpg', N'3225d329-4e61-4158-bd65-5402a20dfc5bad-100off-tools.jpg', N'107304', N'500       ', N'750       ', N'107304    ', N'.jpg', 2, GETDATE(), 2, GETDATE(), 0
WHERE NOT EXISTS (SELECT * FROM ZnodeMedia WHERE [FileName]='3225d329-4e61-4158-bd65-5402a20dfc5bad-100off-tools.jpg')

INSERT [dbo].[ZnodeMedia] ([MediaConfigurationId], [Path], [FileName], [Size], [Height], [Width], [Length], [Type], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Version]) 
SELECT(SELECT TOP 1 MediaConfigurationId FROM ZnodeMediaConfiguration 
	WHERE MediaServerMasterId=(SELECT TOP 1 MediaServerMasterId FROM ZnodeMediaServerMaster WHERE PartialViewName='Local') AND IsActive=1),
N'fd3a3779-19c9-47a7-82db-5ec81554abc3d4e1e90e-b63b-421b-a4b1-607bfb644f0brouter-short.jpg', N'd4e1e90e-b63b-421b-a4b1-607bfb644f0brouter-short.jpg', N'81401', N'449       ', N'1400      ', N'81401     ', N'.jpg', 2, GETDATE(), 2, GETDATE(), 0
WHERE NOT EXISTS (SELECT * FROM ZnodeMedia WHERE [FileName]='d4e1e90e-b63b-421b-a4b1-607bfb644f0brouter-short.jpg')

INSERT [dbo].[ZnodeMedia] ([MediaConfigurationId], [Path], [FileName], [Size], [Height], [Width], [Length], [Type], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Version]) 
SELECT (SELECT TOP 1 MediaConfigurationId FROM ZnodeMediaConfiguration 
	WHERE MediaServerMasterId=(SELECT TOP 1 MediaServerMasterId FROM ZnodeMediaServerMaster WHERE PartialViewName='Local') AND IsActive=1),
N'846b4bc5-a7db-4489-8305-7b07d4f6c45a9d640c1a-53d0-479a-b614-487ac5ca9d13router-small.jpg', N'9d640c1a-53d0-479a-b614-487ac5ca9d13router-small.jpg', N'52407', N'449       ', N'800       ', N'52407     ', N'.jpg', 2, GETDATE(), 2, GETDATE(), 1
WHERE NOT EXISTS (SELECT * FROM ZnodeMedia WHERE [FileName]='9d640c1a-53d0-479a-b614-487ac5ca9d13router-small.jpg')


--ZnodeGlobalAttributeGroup

INSERT [dbo].[ZnodeGlobalAttributeGroup] ([GroupCode], [DisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'HomepageMarketingContentBlock', NULL, 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock')

INSERT [dbo].[ZnodeGlobalAttributeGroup] ([GroupCode], [DisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'DisplayOptions', NULL, 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroup WHERE GroupCode='DisplayOptions')

INSERT [dbo].[ZnodeGlobalAttributeGroup] ([GroupCode], [DisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'HomePageTicker', NULL, 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomePageTicker')

INSERT [dbo].[ZnodeGlobalAttributeGroup] ([GroupCode], [DisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'AdSpace', NULL, 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace')

INSERT [dbo].[ZnodeGlobalAttributeGroup] ([GroupCode], [DisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'AdSpace2', NULL, 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2')

INSERT [dbo].[ZnodeGlobalAttributeGroup] ([GroupCode], [DisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'CartPageAdSpace1', NULL, 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace1')

INSERT [dbo].[ZnodeGlobalAttributeGroup] ([GroupCode], [DisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'CartPageAdSpace2', NULL, 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace2')

INSERT [dbo].[ZnodeGlobalAttributeGroup] ([GroupCode], [DisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT N'CartPageAdSpace', NULL, 2, GETDATE(), 2, GETDATE(), 0,
	(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace')









-- ZnodeGlobalAttributeGroupLocale

INSERT [dbo].[ZnodeGlobalAttributeGroupLocale] ([LocaleId], [GlobalAttributeGroupId], [AttributeGroupName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT
1, 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock'),
N'Marketing Content Block', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupLocale WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock'))

INSERT [dbo].[ZnodeGlobalAttributeGroupLocale] ([LocaleId], [GlobalAttributeGroupId], [AttributeGroupName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate])
SELECT
1, 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='DisplayOptions'),
N'Display Options', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupLocale WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='DisplayOptions'))

INSERT [dbo].[ZnodeGlobalAttributeGroupLocale] ([LocaleId], [GlobalAttributeGroupId], [AttributeGroupName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT
1, 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomePageTicker'),
N'Home Page Ticker', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupLocale WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomePageTicker'))

INSERT [dbo].[ZnodeGlobalAttributeGroupLocale] ([LocaleId], [GlobalAttributeGroupId], [AttributeGroupName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT
1, 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace'),
N'Ad Space', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupLocale WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace'))

INSERT [dbo].[ZnodeGlobalAttributeGroupLocale] ([LocaleId], [GlobalAttributeGroupId], [AttributeGroupName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT
1, 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2'),
N'Ad Space2', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupLocale WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2'))

INSERT [dbo].[ZnodeGlobalAttributeGroupLocale] ([LocaleId], [GlobalAttributeGroupId], [AttributeGroupName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT
1, 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace1'),
N'Cart Page Ad Space1', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupLocale WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace1'))

INSERT [dbo].[ZnodeGlobalAttributeGroupLocale] ([LocaleId], [GlobalAttributeGroupId], [AttributeGroupName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT
1, 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace2'),
N'Cart Page Ad Space2', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupLocale WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace2'))

INSERT [dbo].[ZnodeGlobalAttributeGroupLocale] ([LocaleId], [GlobalAttributeGroupId], [AttributeGroupName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT
1, 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace'),
N'Cart Page Ad Space', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupLocale WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace'))


--ZnodeGlobalFamilyGroupMapper
INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId,GlobalAttributeGroupId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomepageMarketingContentBlock'),
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock'),
999, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalFamilyGroupMapper WHERE GlobalAttributeFamilyId=
	(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomepageMarketingContentBlock')
	AND GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId,GlobalAttributeGroupId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomepageMarketingContentBlock'),
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='DisplayOptions'),
999, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalFamilyGroupMapper WHERE GlobalAttributeFamilyId=
	(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomepageMarketingContentBlock')
	AND GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='DisplayOptions')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId,GlobalAttributeGroupId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageTicker'),
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomePageTicker'),
999, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalFamilyGroupMapper WHERE GlobalAttributeFamilyId=
	(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageTicker')
	AND GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomePageTicker')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId,GlobalAttributeGroupId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageAdSpace'),
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace'),
999, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalFamilyGroupMapper WHERE GlobalAttributeFamilyId=
	(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageAdSpace')
	AND GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId,GlobalAttributeGroupId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageAdSpace'),
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2'),
999, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalFamilyGroupMapper WHERE GlobalAttributeFamilyId=
	(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='HomePageAdSpace')
	AND GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId,GlobalAttributeGroupId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace'),
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace1'),
999, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalFamilyGroupMapper WHERE GlobalAttributeFamilyId=
	(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace')
	AND GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace1')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId,GlobalAttributeGroupId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace'),
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace2'),
999, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalFamilyGroupMapper WHERE GlobalAttributeFamilyId=
	(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace')
	AND GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace2')
)

INSERT INTO ZnodeGlobalFamilyGroupMapper (GlobalAttributeFamilyId,GlobalAttributeGroupId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT
(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace'),
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace'),
999, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalFamilyGroupMapper WHERE GlobalAttributeFamilyId=
	(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode='CartPageAdSpace')
	AND GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace')
)





--ZnodeGlobalAttribute

INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId]) 
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'), 
N'PromoTitle1', 0, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoTitle1')

INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'),
N'PromoLargeImage', 0, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoLargeImage')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'),
N'PromoSmallImage', 0, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoSmallImage')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'),
N'PromoCTAText', 0,
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTAText')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'),
N'PromoCTALinkURL', 0, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTALinkURL')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'), 
N'HomePageTicker', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='HomePageTicker')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'), 
N'AdSpaceImage1', 0, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage1')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'),
N'TitleForImage1', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage1')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'),  
N'TextForImage1', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='TextForImage1')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'),
N'CTALinkURL1', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL1')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'), 
N'AdSpaceImage2', 0, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage2')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'), 
N'TitleForImage2', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage2')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'), 
N'TextorImage2', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='TextorImage2')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'),
N'CTALinkURL2', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL2')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'), 
N'CartPageTitle1', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle1')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'), 
N'CartPageText1', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText1')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'),
N'C2', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='C2')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'), 
N'CartPageTitle2', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle2')
																																																					  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'), 
N'CartPageText3', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText3')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'), 
N'CartPageTitle3', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle3')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'), 
N'CartPageText03', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText03')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'),
N'C3', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='C3')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'),
N'BackgroundColor', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='BackgroundColor')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'),
N'ContentAlignment', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='ContentAlignment')
																																																													  
INSERT [dbo].[ZnodeGlobalAttribute] ([AttributeTypeId], [AttributeCode], [IsRequired], [IsLocalizable], [IsActive], [DisplayOrder], [HelpDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSystemDefined], [GlobalEntityId])  
SELECT 
(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'),
N'C1', 1, 
1, 0, 500, NULL, 2, GETDATE(), 2, GETDATE(), 0,
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttribute WHERE AttributeCode='C1')



--ZnodeGlobalAttributeLocale


INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='BackgroundColor'),
N'BackgroundColor', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='BackgroundColor'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ContentAlignment'),
N'ContentAlignment', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ContentAlignment'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle1'),
N'Title', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle1'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle2'),
N'Title', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle2'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle3'),
N'Title', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle3'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText3'),
N'Description', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText3'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText1'),
N'Description', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText1'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText03'),
N'Description', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText03'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C2'),
N'CTA Link URL', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C2'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C3'),
N'CTA Link URL', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C3'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C1'),
N'CTA Link URL', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C1'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage1'),
N'Ad Space Image 1', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage1'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage2'),
N'Ad Space Image 2', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage2'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage1'),
N'Title For Image 1', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage1'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage2'),
N'Title For Image 2', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage2'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextForImage1'),
N'Text For Image 1', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextForImage1'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextorImage2'),
N'Text For Image 2', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextorImage2'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL1'),
N'CTA Link URL 1', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL1'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL2'),
N'CTA Link URL 2', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL2'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoSmallImage'),
N'Promo Small Image', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoSmallImage'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoLargeImage'),
N'Promo Large Image', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoLargeImage'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTAText'),
N'Promo CTA Text', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTAText'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTALinkURL'),
N'Prom CTA Link URL', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTALinkURL'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='HomePageTicker'),
N'Home Page Ticker', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='HomePageTicker'))

INSERT [dbo].[ZnodeGlobalAttributeLocale] ([LocaleId], [GlobalAttributeId], [AttributeName], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
1, 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoTitle1'),
N'Promo Title', NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeLocale WHERE GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoTitle1'))






--ZnodeGlobalAttributeGroupMapper

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoTitle1'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoTitle1'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoLargeImage'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoLargeImage'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoSmallImage'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoSmallImage'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTAText'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTAText'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTALinkURL'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomepageMarketingContentBlock')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTALinkURL'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomePageTicker'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='HomePageTicker'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='HomePageTicker')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='HomePageTicker'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage1'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage1'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage1'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage1'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextForImage1'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextForImage1'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL1'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL1'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage2'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage2'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage2'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage2'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextorImage2'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextorImage2'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL2'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='AdSpace2')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL2'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace1'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle1'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace1')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle1'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace1'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText1'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace1')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText1'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace2'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C2'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace2')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C2'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace2'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle2'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace2')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle2'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace2'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText03'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace2')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText03'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText3'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText3'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle3'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle3'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C3'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C3'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='DisplayOptions'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='BackgroundColor'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='DisplayOptions')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='BackgroundColor'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='DisplayOptions'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ContentAlignment'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='DisplayOptions')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ContentAlignment'))

INSERT [dbo].[ZnodeGlobalAttributeGroupMapper] ([GlobalAttributeGroupId], [GlobalAttributeId], [AttributeDisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace1'),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C1'),
NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeGlobalAttributeGroupMapper 
	WHERE GlobalAttributeGroupId=(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode='CartPageAdSpace1')
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C1'))


--ZnodeWidgetGlobalAttributeValue

INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoTitle1'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoTitle1'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoLargeImage'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoLargeImage'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoSmallImage'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoSmallImage'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTAText'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTAText'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTALinkURL'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTALinkURL'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePageTicker'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePageTicker') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='HomePageTicker'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePageTicker')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePageTicker') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='HomePageTicker'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage1'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage1'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage1'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage1'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextForImage1'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextForImage1'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL1'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL1'))

INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage2'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage2'))

INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage2'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage2'))

INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextorImage2'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextorImage2'))

INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL2'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL2'))

INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle1'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle1'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText1'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText1'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C2'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C2'))

INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle2'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle2'))

INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText03'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText03'))

INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText3'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText3'))

INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle3'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle3'))

INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C3'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C3'))





INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='BackgroundColor'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='BackgroundColor'))


INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ContentAlignment'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ContentAlignment'))





INSERT [dbo].[ZnodeWidgetGlobalAttributeValue] ([CMSContentContainerId], [CMSContainerProfileVariantId], [GlobalAttributeId], [GlobalAttributeDefaultValueId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
SELECT 
(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace'),
(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL),
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C1'),
NULL, NULL, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
AND CMSContainerProfileVariantId=
	(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
WHERE CMSContentContainerId=(SELECT TOP 1 [CMSContentContainerId] FROM ZnodeCMSContentContainer
	WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
AND GlobalAttributeId=
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C1'))


--ZnodeWidgetGlobalAttributeValueLocale

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoTitle1')
),
1, N'Getting the fine edges with <br />The Cordless Dewalt Router ZX-3000', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoTitle1')
	) AND LocaleId=1
)


INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTAText')
),
1, N'View Product', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTAText')
	) AND LocaleId=1
)



INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTALinkURL')
),
1, N'/mountain-plumbing-10-inch-traditional-round-rain-head', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoCTALinkURL')
	) AND LocaleId=1
)




INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoLargeImage')
),
1, NULL, 2, GETDATE(), 2, GETDATE(), NULL, 
(SELECT TOP 1 MediaId FROM ZnodeMedia WHERE [Path]='fd3a3779-19c9-47a7-82db-5ec81554abc3d4e1e90e-b63b-421b-a4b1-607bfb644f0brouter-short.jpg'),
N'fd3a3779-19c9-47a7-82db-5ec81554abc3d4e1e90e-b63b-421b-a4b1-607bfb644f0brouter-short.jpg'
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoLargeImage')
	--AND MediaId=(SELECT TOP 1 MediaId FROM ZnodeMedia WHERE [Path]='fd3a3779-19c9-47a7-82db-5ec81554abc3d4e1e90e-b63b-421b-a4b1-607bfb644f0brouter-short.jpg')
	) AND LocaleId=1
)


INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoSmallImage')
),
1, NULL, 2, GETDATE(), 2, GETDATE(), NULL, 
(SELECT TOP 1 MediaId FROM ZnodeMedia WHERE [Path]='846b4bc5-a7db-4489-8305-7b07d4f6c45a9d640c1a-53d0-479a-b614-487ac5ca9d13router-small.jpg'),
N'846b4bc5-a7db-4489-8305-7b07d4f6c45a9d640c1a-53d0-479a-b614-487ac5ca9d13router-small.jpg'
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='PromoSmallImage')
	--AND MediaId=(SELECT TOP 1 MediaId FROM ZnodeMedia WHERE [Path]='846b4bc5-a7db-4489-8305-7b07d4f6c45a9d640c1a-53d0-479a-b614-487ac5ca9d13router-small.jpg')
	) AND LocaleId=1
)


INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePageTicker')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePageTicker') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='HomePageTicker')
),
1, N'Save 20% during the holidays! Enter Holidays2021 at checkout.', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePageTicker')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePageTicker') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='HomePageTicker')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage1')
),
1, N'DeWalt® Guaranteed Tough one', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage1')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextForImage1')
),
1, N'Outdoor Power Equipment Designed to Handle the Most Demanding Yards', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextForImage1')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL1')
),
1, N'#', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL1')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage2')
),
1, N'Up To $100 OFF', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TitleForImage2')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextorImage2')
),
1, N'Select Tools + Free Delivery', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='TextorImage2')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL2')
),
1, N'/cordless-woodworking', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CTALinkURL2')
	) AND LocaleId=1
)




INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage1')
),
1, NULL, 2, GETDATE(), 2, GETDATE(), NULL, 
(SELECT TOP 1 MediaId FROM ZnodeMedia WHERE [Path]=N'42bff3d2-0dbf-4b6d-b1be-321537912ad03945ac03-7e20-488b-a690-028e7bea2325ad-dewalt-lawncare.jpg'),
N'42bff3d2-0dbf-4b6d-b1be-321537912ad03945ac03-7e20-488b-a690-028e7bea2325ad-dewalt-lawncare.jpg'
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage1')
	--AND MediaId=(SELECT TOP 1 MediaId FROM ZnodeMedia WHERE [Path]='42bff3d2-0dbf-4b6d-b1be-321537912ad03945ac03-7e20-488b-a690-028e7bea2325ad-dewalt-lawncare.jpg')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage2')
),
1, NULL, 2, GETDATE(), 2, GETDATE(), NULL, 
(SELECT TOP 1 MediaId FROM ZnodeMedia WHERE [Path]=N'847cd3a7-bcbd-4c94-b881-e67872a7fad93225d329-4e61-4158-bd65-5402a20dfc5bad-100off-tools.jpg'),
N'847cd3a7-bcbd-4c94-b881-e67872a7fad93225d329-4e61-4158-bd65-5402a20dfc5bad-100off-tools.jpg'
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='AdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='AdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='AdSpaceImage2')
	--AND MediaId=(SELECT TOP 1 MediaId FROM ZnodeMedia WHERE [Path]='847cd3a7-bcbd-4c94-b881-e67872a7fad93225d329-4e61-4158-bd65-5402a20dfc5bad-100off-tools.jpg')
	) AND LocaleId=1
)



INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle1')
),
1, N'Buy Online. Pick-up in store.', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle1')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath]) 
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText1')
),
1, N'Pick-up in store in under 2 hours', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText1')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath]) 
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C2')
),
1, N'Get up to $100 off!', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C2')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath])
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle2')
),
1, N'Get a Maxwell''s Credit Card and receive $25 off your purchase of $25+', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle2')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath]) 
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText03')
),
1, N'#', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText03')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath]) 
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText3')
),
1, N'Parts Service Repair', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageText3')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath]) 
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle3')
),
1, N'Find a service pro near you', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='CartPageTitle3')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath]) 
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C3')
),
1, N'#', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C3')
	) AND LocaleId=1
)



INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath]) 
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='BackgroundColor')
),
1, N'grey', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='BackgroundColor')
	) AND LocaleId=1
)

INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath]) 
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ContentAlignment')
),
1, N'Vertical', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='HomePagePromo')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='HomePagePromo') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='ContentAlignment')
	) AND LocaleId=1
)



INSERT [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ([WidgetGlobalAttributeValueId], [LocaleId], [AttributeValue], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [GlobalAttributeDefaultValueId], [MediaId], [MediaPath]) 
SELECT 
(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C1')
),
1, N'#', 2, GETDATE(), 2, GETDATE(), NULL, NULL, NULL
WHERE NOT EXISTS (SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WHERE WidgetGlobalAttributeValueId=
	(SELECT TOP 1 WidgetGlobalAttributeValueId FROM ZnodeWidgetGlobalAttributeValue WHERE CMSContentContainerId=
	(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer WHERE ContainerKey='CartPageAdSpace')
	AND CMSContainerProfileVariantId=
		(SELECT TOP 1 CMSContainerProfileVariantId FROM ZnodeCMSContainerProfileVariant
	WHERE CMSContentContainerId=(SELECT TOP 1 CMSContentContainerId FROM ZnodeCMSContentContainer
		WHERE ContainerKey='CartPageAdSpace') AND ProfileId IS NULL AND PortalId IS NULL)
	AND GlobalAttributeId=(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode='C1')
	) AND LocaleId=1
)



--ZnodeGlobalAttributeValidation

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BackgroundColor'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BackgroundColor')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BackgroundColor'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BackgroundColor')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BackgroundColor'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BackgroundColor')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BackgroundColor'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'BackgroundColor')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'ContentAlignment'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'ContentAlignment')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'ContentAlignment'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'ContentAlignment')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'ContentAlignment'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'ContentAlignment')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'ContentAlignment'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'ContentAlignment')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))


INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageTitle1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageTitle1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'ContentAlignment'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'ContentAlignment')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageTitle2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageTitle2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageTitle2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageTitle2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageTitle3'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageTitle3')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageTitle3'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageTitle3')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText3'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText3')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText3'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText3')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText03'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText03')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText03'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CartPageText03')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))


INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C3'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C3')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C3'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C3')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C3'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C3')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C3'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C3')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'C1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))



INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxFileSize' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,'20',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxFileSize'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'IsAllowMultiUpload'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'IsAllowMultiUpload'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxFileSize' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,'20',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxFileSize'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'IsAllowMultiUpload'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'IsAllowMultiUpload'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))


INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TitleForImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TitleForImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TitleForImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TitleForImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TitleForImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TitleForImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TitleForImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TitleForImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TextForImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TextForImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TextForImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TextForImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TextorImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TextorImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TextorImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'TextorImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))



INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'CTALinkURL2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))



INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxFileSize' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,'20',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxFileSize'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'IsAllowMultiUpload'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'IsAllowMultiUpload'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxFileSize' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,'20',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxFileSize'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'IsAllowMultiUpload'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'IsAllowMultiUpload'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')))



INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTAText'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTAText')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTAText'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTAText')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTAText'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTAText')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTAText'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTAText')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTALinkURL'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTALinkURL')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'ValidationRule'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTALinkURL'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTALinkURL')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'RegularExpression'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTALinkURL'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTALinkURL')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTALinkURL'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoCTALinkURL')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'UniqueValue'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text')))




INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'HomePageTicker'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'HomePageTicker')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'HomePageTicker'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'HomePageTicker')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoTitle1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'MaxCharacters' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoTitle1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'MaxCharacters'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoTitle1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'WYSIWYGEnabledProperty' 
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area'))
,NULL,'false',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoTitle1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'WYSIWYGEnabledProperty'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Text Area')))

--Start For all extensions for Image Data Type
IF EXISTS (	SELECT TOP 1 1 FROM ZnodeGlobalAttributeValidation GAV
			INNER JOIN ZnodeGlobalAttribute GA ON GAV.GlobalAttributeId=GA.GlobalAttributeId
			INNER JOIN ZnodeAttributeInputValidation AV ON GAV.InputValidationId=AV.InputValidationId
			WHERE GA.GlobalEntityId=(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
				AND AV.AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')
				AND GAV.InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name='Extensions'
					AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
				AND InputValidationRuleId IS NULL)
BEGIN
	DELETE GAV
	FROM ZnodeGlobalAttributeValidation GAV
	INNER JOIN ZnodeGlobalAttribute GA ON GAV.GlobalAttributeId=GA.GlobalAttributeId
	INNER JOIN ZnodeAttributeInputValidation AV ON GAV.InputValidationId=AV.InputValidationId
	WHERE GA.GlobalEntityId=(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName='Content Containers')
		AND AV.AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')
		AND GAV.InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name='Extensions'
			AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
		AND InputValidationRuleId IS NULL
END

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpg'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.png')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.png'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.gif')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.gif'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpeg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpeg'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.ico')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.ico'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.svg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage1')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.svg'))



INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpg'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.png')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.png'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.gif')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.gif'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpeg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpeg'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.ico')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.ico'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.svg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'AdSpaceImage2')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.svg'))




INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpg'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.png')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.png'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.gif')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.gif'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpeg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpeg'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.ico')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.ico'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.svg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoSmallImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.svg'))



INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpg'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.png')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.png'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.gif')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.gif'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpeg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.jpeg'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.ico')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.ico'))

INSERT INTO ZnodeGlobalAttributeValidation (GlobalAttributeId,InputValidationId,InputValidationRuleId,Name,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage'),
(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
,(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.svg')
,NULL,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalAttributeValidation WHERE GlobalAttributeId = 
	(SELECT TOP 1 GlobalAttributeId FROM ZnodeGlobalAttribute WHERE AttributeCode = 'PromoLargeImage')
AND InputValidationId = (SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image'))
AND InputValidationRuleId=(SELECT TOP 1 InputValidationRuleId FROM ZnodeAttributeInputValidationRule WHERE InputValidationId=(SELECT TOP 1 InputValidationId FROM ZnodeAttributeInputValidation  WHERE Name = 'Extensions'
	AND AttributeTypeId=(SELECT TOP 1 AttributeTypeId FROM ZnodeAttributeType WHERE AttributeTypeName='Image')) AND ValidationName='.svg'))

--End For all extensions for Image Data Type

--ZnodeGlobalGroupEntityMapper
INSERT INTO ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'HomepageMarketingContentBlock'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'),999, 2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalGroupEntityMapper WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'HomepageMarketingContentBlock')
      AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'))

INSERT INTO ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'DisplayOptions'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'),999, 2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalGroupEntityMapper WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'DisplayOptions')
      AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'))

INSERT INTO ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'HomePageTicker'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'),999, 2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalGroupEntityMapper WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'HomePageTicker')
      AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'))

INSERT INTO ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'AdSpace'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'),999, 2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalGroupEntityMapper WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'AdSpace')
      AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'))

INSERT INTO ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'AdSpace2'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'),999, 2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalGroupEntityMapper WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'AdSpace2')
      AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'))

INSERT INTO ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'CartPageAdSpace1'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'),999, 2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalGroupEntityMapper WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'CartPageAdSpace1')
      AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'))

INSERT INTO ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'CartPageAdSpace2'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'),999, 2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalGroupEntityMapper WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'CartPageAdSpace2')
      AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'))

INSERT INTO ZnodeGlobalGroupEntityMapper(GlobalAttributeGroupId,GlobalEntityId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'CartPageAdSpace'),
(SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'),999, 2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeGlobalGroupEntityMapper WHERE GlobalAttributeGroupId=
	(SELECT TOP 1 GlobalAttributeGroupId FROM ZnodeGlobalAttributeGroup WHERE GroupCode = 'CartPageAdSpace')
      AND GlobalEntityId = (SELECT TOP 1 GlobalEntityId FROM ZnodeGlobalEntity WHERE EntityName = 'Content Containers'))

-- Dt.03-Oct-2022
INSERT INTO ZnodeCMSContainerTemplate
    (Code,Name,FileName,MediaId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'SampleWidgetTemplate','Sample Widget Template','DemoControls',(SELECT TOP 1 MediaId FROM ZnodeMedia),
    2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeCMSContainerTemplate WHERE Code='SampleWidgetTemplate')
