INSERT INTO ZnodeCMSContainerConfiguration (CMSWidgetsId,WidgetKey,CMSMappingId,TypeOFMapping,ContentContainerId,ContainerKey,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ContentContainer') ,1786,PortalId,'PortalMapping',0,'HomePageTicker',2,Getdate(),2,Getdate() 
From ZnodePortal ZP
WHERE NOT EXISTS
	(SELECT * FROM ZnodeCMSContainerConfiguration Z WHERE ContainerKey = 'HomePageTicker'
	and Z.CMSMappingId = ZP.PortalId
	and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ContentContainer')
	and TypeOFMapping = 'PortalMapping')

INSERT INTO ZnodeCMSContainerConfiguration (CMSWidgetsId,WidgetKey,CMSMappingId,TypeOFMapping,ContentContainerId,ContainerKey,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ContentContainer') ,1787,PortalId,'PortalMapping',0,'AdSpace',2,Getdate(),2,Getdate() 
From ZnodePortal ZP
WHERE NOT EXISTS
	(SELECT * FROM ZnodeCMSContainerConfiguration Z WHERE ContainerKey = 'AdSpace'
	and Z.CMSMappingId = ZP.PortalId
	and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ContentContainer')
	and TypeOFMapping = 'PortalMapping')

INSERT INTO ZnodeCMSContainerConfiguration (CMSWidgetsId,WidgetKey,CMSMappingId,TypeOFMapping,ContentContainerId,ContainerKey,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ContentContainer') ,1788,PortalId,'PortalMapping',0,'HomePagePromo',2,Getdate(),2,Getdate() 
From ZnodePortal ZP
WHERE NOT EXISTS
	(SELECT * FROM ZnodeCMSContainerConfiguration Z WHERE ContainerKey = 'HomePagePromo'
	and Z.CMSMappingId = ZP.PortalId
	and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ContentContainer')
	and TypeOFMapping = 'PortalMapping')

INSERT INTO ZnodeCMSContainerConfiguration (CMSWidgetsId,WidgetKey,CMSMappingId,TypeOFMapping,ContentContainerId,ContainerKey,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ContentContainer') ,1789,PortalId,'PortalMapping',0,'CartPageAdSpace',2,Getdate(),2,Getdate() 
From ZnodePortal ZP
WHERE NOT EXISTS
	(SELECT * FROM ZnodeCMSContainerConfiguration Z WHERE ContainerKey = 'CartPageAdSpace'
	and Z.CMSMappingId = ZP.PortalId
	and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ContentContainer')
	and TypeOFMapping = 'PortalMapping')
	
INSERT INTO ZnodeCMSContainerConfiguration (CMSWidgetsId,WidgetKey,CMSMappingId,TypeOFMapping,ContentContainerId,ContainerKey,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ContentContainer') ,67878898,CMSContentPagesId,
'ContentPageMapping',0,'HomePageTicker',2,Getdate(),2,Getdate() 
From ZnodeCMSContentPages CCPP
WHERE 
NOT EXISTS
	(SELECT * FROM ZnodeCMSContainerConfiguration Z WHERE ContainerKey = 'HomePageTicker'
	and Z.CMSMappingId = CCPP.CMSContentPagesId
	and CMSWidgetsId = (select top 1 CMSWidgetsId from ZnodeCMSWidgets where Code = 'ContentContainer')
	and TypeOFMapping = 'ContentPageMapping')
and 
CMSTemplateId= 
(select top 1 CMSTemplateId from ZnodeCMSTemplate where Name='Landing Page With Banner Slider Multiple Content And Image Sections and Product List Slider'
)
	 
