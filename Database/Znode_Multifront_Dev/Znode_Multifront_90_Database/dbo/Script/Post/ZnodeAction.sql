
--dt\16\10\2019 ZPD-7656

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','GetLBData',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'GetLBData')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetLBData') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetLBData'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetLBData')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetLBData'))

GO

--dt\30\10\2019 ZPD-7764

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','UpdateLBData',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'UpdateLBData')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateLBData') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateLBData'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateLBData')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateLBData'))

GO

--dt\13\11\2019 ZPD-7269

INSERT  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Recommendation','GetRecommendationSetting',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Recommendation' and ActionName = 'GetRecommendationSetting')
UNION ALL 
SELECT NULL ,'Recommendation','SaveRecommendationSetting',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Recommendation' and ActionName = 'SaveRecommendationSetting')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience')	
,(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Recommendation' and ActionName= 'GetRecommendationSetting') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'GetRecommendationSetting'))
UNION ALL 
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience')	
,(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Recommendation' and ActionName= 'SaveRecommendationSetting') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'SaveRecommendationSetting'))




insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience')
,(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Recommendation' and ActionName= 'GetRecommendationSetting')	
,1,2,Getdate(),2,Getdate() where not exists 
(SELECT * FROM ZnodeMenuActionsPermission where MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Recommendation' and ActionName= 'GetRecommendationSetting'))
UNION ALL 
select 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience')
,(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Recommendation' and ActionName= 'SaveRecommendationSetting')	
,1,2,Getdate(),2,Getdate() where not exists 
(SELECT * FROM ZnodeMenuActionsPermission where MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Recommendation' and ActionName= 'SaveRecommendationSetting'))

GO
--dt\06\12\2019 ZPD-8230

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Customer','GetSalesRepList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Customer' and ActionName = 'GetSalesRepList')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer')
   ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetSalesRepList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
    (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId =
    (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetSalesRepList'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetSalesRepList')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetSalesRepList'))


--dt\06\12\2019 ZPD-8238

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'WebSite','ManageMediaWidgetConfiguration',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'WebSite' and ActionName = 'ManageMediaWidgetConfiguration')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content')
   ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'ManageMediaWidgetConfiguration') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
    (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content') and ActionId =
    (select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'ManageMediaWidgetConfiguration'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'ManageMediaWidgetConfiguration')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'ManageMediaWidgetConfiguration'))
go
--dt 13-12-2019 ZPD-8307


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Template','DownloadTemplate',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Template' and ActionName = 'DownloadTemplate')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Page Templates' AND ControllerName = 'Template')
   ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Template' and ActionName= 'DownloadTemplate') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
    (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Page Templates' AND ControllerName = 'Template') and ActionId =
    (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Template' and ActionName= 'DownloadTemplate') )

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Page Templates' AND ControllerName = 'Template'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Template' and ActionName= 'DownloadTemplate')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Page Templates' AND ControllerName = 'Template') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Template' and ActionName= 'DownloadTemplate'))
go
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'TouchPointConfiguration','GetUnAssignedTouchPointsList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName = 'GetUnAssignedTouchPointsList')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration')
   ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'GetUnAssignedTouchPointsList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
    (select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration') and ActionId =
    (select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'GetUnAssignedTouchPointsList') )

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'GetUnAssignedTouchPointsList')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'GetUnAssignedTouchPointsList'))
go
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'TouchPointConfiguration','AssignTouchPointToActiveERP',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName = 'AssignTouchPointToActiveERP')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration')
   ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'AssignTouchPointToActiveERP') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
    (select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration') and ActionId =
    (select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'AssignTouchPointToActiveERP') )

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'AssignTouchPointToActiveERP')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'AssignTouchPointToActiveERP'))

--dt 19-12-2019 ZPD-7550 --> ZPD-8199

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Store','EditAssociatedPageSetting',1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Store' and ActionName = 'EditAssociatedPageSetting')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores & Reps' AND ControllerName = 'Store')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditAssociatedPageSetting') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores & Reps' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditAssociatedPageSetting'))

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditAssociatedPageSetting') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditAssociatedPageSetting'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores & Reps' AND ControllerName = 'Store'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditAssociatedPageSetting')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores & Reps' AND ControllerName = 'Store') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditAssociatedPageSetting'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditAssociatedPageSetting')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditAssociatedPageSetting'))

GO


--dt\24\12\2019 ZPD-8372
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Store','BrandList',1,2,Getdate(),2,Getdate() 
where not exists(select * from ZnodeActions where ControllerName = 'Store' and ActionName = 'BrandList')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'BrandList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'BrandList'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'BrandList')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'BrandList'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'WebSite','GetUnAssociatedBrandList',1,2,Getdate(),2,Getdate() 
where not exists(select * from ZnodeActions where ControllerName = 'WebSite' and ActionName = 'GetUnAssociatedBrandList')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'GetUnAssociatedBrandList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'GetUnAssociatedBrandList'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'GetUnAssociatedBrandList')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'GetUnAssociatedBrandList'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Store','GetUnAssociatedBrandList',1,2,Getdate(),2,Getdate() 
where not exists(select * from ZnodeActions where ControllerName = 'Store' and ActionName = 'GetUnAssociatedBrandList')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'GetUnAssociatedBrandList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'GetUnAssociatedBrandList'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'GetUnAssociatedBrandList')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'GetUnAssociatedBrandList'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Store','AssociatePortalBrand',1,2,Getdate(),2,Getdate() 
where not exists(select * from ZnodeActions where ControllerName = 'Store' and ActionName = 'AssociatePortalBrand')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'AssociatePortalBrand') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'AssociatePortalBrand'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'AssociatePortalBrand')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'AssociatePortalBrand'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Store','UnAssociatePortalBrand',1,2,Getdate(),2,Getdate() 
where not exists(select * from ZnodeActions where ControllerName = 'Store' and ActionName = 'UnAssociatePortalBrand')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'UnAssociatePortalBrand') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'UnAssociatePortalBrand'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'UnAssociatePortalBrand')	
,4,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'UnAssociatePortalBrand'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Store','UpdateAssociatedPortalBrandDetail',1,2,Getdate(),2,Getdate() 
where not exists(select * from ZnodeActions where ControllerName = 'Store' and ActionName = 'UpdateAssociatedPortalBrandDetail')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'UpdateAssociatedPortalBrandDetail') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'UpdateAssociatedPortalBrandDetail'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'UpdateAssociatedPortalBrandDetail')	
,4,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'UpdateAssociatedPortalBrandDetail'))

GO

--dt 07-01-2020 ZPD-8499 --> ZPD-8560


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'LogMessage','ImpersonationLogList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'LogMessage' and ActionName = 'ImpersonationLogList')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'LogMessage' and ActionName= 'ImpersonationLogList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'LogMessage' and ActionName= 'ImpersonationLogList'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'LogMessage' and ActionName= 'ImpersonationLogList')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'LogMessage' and ActionName= 'ImpersonationLogList'))

GO



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'LogMessage','ImpersonationLogList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'LogMessage' and ActionName = 'ImpersonationLogList')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Application Logs' AND ControllerName = 'LogMessage')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'LogMessage' and ActionName= 'ImpersonationLogList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Application Logs' AND ControllerName = 'LogMessage')	 and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'LogMessage' and ActionName= 'ImpersonationLogList'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Application Logs' AND ControllerName = 'LogMessage')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'LogMessage' and ActionName= 'ImpersonationLogList')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Application Logs' AND ControllerName = 'LogMessage')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'LogMessage' and ActionName= 'ImpersonationLogList'))

GO


--dt 09-01-2020 ZPD-5709 --> ZPD-8620
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetAddressDetails',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetAddressDetails')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAddressDetails') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAddressDetails'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAddressDetails')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAddressDetails'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetUserAddress',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetUserAddress')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetUserAddress') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetUserAddress'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetUserAddress')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetUserAddress'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetCustomerListBySearchTerm',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetCustomerListBySearchTerm')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetCustomerListBySearchTerm') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetCustomerListBySearchTerm'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetCustomerListBySearchTerm')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetCustomerListBySearchTerm'))

GO



GO

--dt 10-01-2020 ZPD-8215 --> ZPD-8517

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Account','GetUnAssociatedCustomerList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Account' and ActionName = 'GetUnAssociatedCustomerList')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName= 'GetUnAssociatedCustomerList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName= 'GetUnAssociatedCustomerList'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName= 'GetUnAssociatedCustomerList')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName= 'GetUnAssociatedCustomerList'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Account','AssociateUsersWithAccount',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Account' and ActionName = 'AssociateUsersWithAccount')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName= 'AssociateUsersWithAccount') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName= 'AssociateUsersWithAccount'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName= 'AssociateUsersWithAccount')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName= 'AssociateUsersWithAccount'))

GO

--dt 13-01-2020 ZPD-8638

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Customer','GetImpersonationByUserId',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Customer' and ActionName = 'GetImpersonationByUserId')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationByUserId') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationByUserId'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationByUserId')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationByUserId'))

GO

--dt 13-01-2020 ZPD-8334 --> ZPD-8367

update ZnodeMenu set AreaName = 'Search', ControllerName = 'SearchReport', ActionName = 'GetTabStructureSearchReport'
where MenuName = 'Site Search'

insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'SearchReport','GetTabStructureSearchReport',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'SearchReport' and ActionName = 'GetTabStructureSearchReport')
 

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy	,CreatedDate,	ModifiedBy,	ModifiedDate )
select 
	   (select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport')	
      ,(select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetTabStructureSearchReport')	,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
       (select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport') and ActionId = 
       (select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetTabStructureSearchReport'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy	,CreatedDate,	ModifiedBy,	ModifiedDate )
select 
(select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport'),
(select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetTabStructureSearchReport')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport')	 and ActionId = 
(select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetTabStructureSearchReport'))


---------------------------------------------------------
insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'SearchReport','GetTopKeywordsReport',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'SearchReport' and ActionName = 'GetTopKeywordsReport')
 

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy	,CreatedDate,	ModifiedBy,	ModifiedDate )
select 
	   (select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport')	
      ,(select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetTopKeywordsReport')	,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
       (select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport') and ActionId = 
       (select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetTopKeywordsReport'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy	,CreatedDate,	ModifiedBy,	ModifiedDate )
select 
(select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport'),
(select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetTopKeywordsReport')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport') and ActionId = 
(select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetTopKeywordsReport'))

---------------------------------------------------------------
insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'SearchReport','GetNoResultsFoundReport',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'SearchReport' and ActionName = 'GetNoResultsFoundReport')
 

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy	,CreatedDate,	ModifiedBy,	ModifiedDate )
select 
	   (select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport')	
      ,(select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetNoResultsFoundReport')	,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
       (select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport') and ActionId = 
       (select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetNoResultsFoundReport'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy	,CreatedDate,	ModifiedBy,	ModifiedDate )
select 
(select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport'),
(select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetNoResultsFoundReport')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport') and ActionId = 
(select ActionId from ZnodeActions where ControllerName = 'SearchReport' and ActionName= 'GetNoResultsFoundReport'))
----------------
--dt ZPD-8727 --> ZPD-8683
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Customer','GetImpersonationUrl',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Customer' and ActionName = 'GetImpersonationUrl')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationUrl') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationUrl'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationUrl')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationUrl'))

GO

--dt ZPD-8727 --> ZPD-8683
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Customer','GetImpersonationUrl',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Customer' and ActionName = 'GetImpersonationUrl')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationUrl') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationUrl'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationUrl')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetImpersonationUrl'))

GO

--dt 17-01-2019 ZPD-8160 --> ZPD-8725
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','UpdatePaymentStatus',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'UpdatePaymentStatus')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdatePaymentStatus') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdatePaymentStatus'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdatePaymentStatus')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdatePaymentStatus'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','UpdateShippingHandling',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'UpdateShippingHandling')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingHandling') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingHandling'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingHandling')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingHandling'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','UpdateDiscount',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'UpdateDiscount')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateDiscount') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateDiscount'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateDiscount')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateDiscount'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetAdditionalNotes',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetAdditionalNotes')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAdditionalNotes') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAdditionalNotes'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAdditionalNotes')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAdditionalNotes'))

GO
GO

--dt ZPD-5709 --> ZPD-8717

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetPaymentMethods',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetPaymentMethods')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPaymentMethods') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPaymentMethods'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPaymentMethods')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPaymentMethods'))

GO

--dt 21-01-2020 ZPD-8812
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','AddToCartProduct ',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'AddToCartProduct ')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'AddToCartProduct ') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'AddToCartProduct '))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'AddToCartProduct ')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'AddToCartProduct '))



insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'AddToCartProduct ') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'AddToCartProduct '))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'AddToCartProduct ')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'AddToCartProduct '))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetShoppingCartItems ',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetShoppingCartItems ')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetShoppingCartItems ') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetShoppingCartItems '))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetShoppingCartItems ')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetShoppingCartItems '))



insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetShoppingCartItems ') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetShoppingCartItems '))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetShoppingCartItems ')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetShoppingCartItems '))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','CalculateShoppingCart ',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'CalculateShoppingCart ')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'CalculateShoppingCart ') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'CalculateShoppingCart '))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'CalculateShoppingCart ')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'CalculateShoppingCart '))



insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'CalculateShoppingCart ') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'CalculateShoppingCart '))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'CalculateShoppingCart ')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'CalculateShoppingCart '))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','RemoveCartItem ',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'RemoveCartItem ')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveCartItem ') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveCartItem '))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveCartItem ')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveCartItem '))



insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveCartItem ') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveCartItem '))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveCartItem ')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveCartItem '))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','UpdateTaxExemptOnCreateOrder ',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'UpdateTaxExemptOnCreateOrder ')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateTaxExemptOnCreateOrder ') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateTaxExemptOnCreateOrder '))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateTaxExemptOnCreateOrder ')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateTaxExemptOnCreateOrder '))



insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateTaxExemptOnCreateOrder ') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateTaxExemptOnCreateOrder '))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateTaxExemptOnCreateOrder ')	
,3,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	 and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateTaxExemptOnCreateOrder '))

GO

--dt 22-01-2020 ZPD-8812
Update ZnodeActions SET ActionName =  Rtrim(Ltrim(A.ActionName))
FROM ZnodeActions A where ActionName in  ('UpdateTaxExemptOnCreateOrder ', 'AddToCartProduct ' , 'GetShoppingCartItems ', 'CalculateShoppingCart ', 'UpdateCartQuantity ', 'RemoveCartItem ') 
go

--dt 28-01-2020 ZPD-5770 --> ZPD-8926
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Recommendation','GenerateRecommendationData',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Recommendation' and ActionName = 'GenerateRecommendationData')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'GenerateRecommendationData') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'GenerateRecommendationData'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'GenerateRecommendationData')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'GenerateRecommendationData'))

GO

--dt 27-01-2020 ZPD-7813 --> ZPD-8907

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Analytics','AnalyticsDashboard',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Analytics' and ActionName = 'AnalyticsDashboard')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Analytics' AND ControllerName = 'Analytics')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Analytics' and ActionName= 'AnalyticsDashboard') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Analytics' AND ControllerName = 'Analytics') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Analytics' and ActionName= 'AnalyticsDashboard'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Analytics' AND ControllerName = 'Analytics'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Analytics' and ActionName= 'AnalyticsDashboard')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Analytics' AND ControllerName = 'Analytics') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Analytics' and ActionName= 'AnalyticsDashboard'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','GetAnalyticsData',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'GetAnalyticsData')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetAnalyticsData') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetAnalyticsData'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetAnalyticsData')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetAnalyticsData'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','UpdateAnalyticsData',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'UpdateAnalyticsData')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateAnalyticsData') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateAnalyticsData'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateAnalyticsData')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateAnalyticsData'))

GO
GO
GO
--dt 30-01-2020 ZPD-8923
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','RemoveAllShoppingCartItems',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'RemoveAllShoppingCartItems')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveAllShoppingCartItems') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveAllShoppingCartItems'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveAllShoppingCartItems')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'RemoveAllShoppingCartItems'))
GO
GO

--dt 30-01-2020 ZPD-8686
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','UpdateShippingAccountNumber',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'UpdateShippingAccountNumber')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingAccountNumber') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingAccountNumber'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') ,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingAccountNumber')
,3,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingAccountNumber'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','UpdateShippingMethod',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'UpdateShippingMethod')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingMethod') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingMethod'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') ,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingMethod')
,3,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingMethod'))
GO

--dt 06-02-2020 ZPD-5770 --> ZPD-9064
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','UpdateShippingMethod',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'UpdateShippingMethod')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingMethod') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingMethod'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') ,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingMethod')
,3,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingMethod'))

--dt 12-02-2020 ZPD-5770 --> ZPD-9064
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Recommendation','CreateScheduler',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Recommendation' and ActionName = 'CreateScheduler')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'CreateScheduler') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'CreateScheduler'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') ,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'CreateScheduler')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'CreateScheduler'))
go

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Recommendation','EditScheduler',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Recommendation' and ActionName = 'EditScheduler')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'EditScheduler') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'EditScheduler'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') ,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'EditScheduler')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName= 'EditScheduler'))

---dt 18-02-2020 ZPD-9123
delete from ZnodeActionMenu where ActionId in (select ActionId from ZnodeActions where ActionName = 'GetOrderDetails')
delete from ZnodeMenuActionsPermission where ActionId in (select ActionId from ZnodeActions where ActionName = 'GetOrderDetails')
delete from ZnodeActions where ActionName = 'GetOrderDetails'

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetCartCount',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetCartCount')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetCartCount') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetCartCount'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetCartCount')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetCartCount'))

GO
---dt 19-02-2020 ZPD-9137
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'DynamicContent','GetEditorFormats',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'DynamicContent' and ActionName = 'GetEditorFormats')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Sales Reps' AND ControllerName = 'SalesRep')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'DynamicContent' and ActionName= 'GetEditorFormats') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Sales Reps' AND ControllerName = 'SalesRep') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'DynamicContent' and ActionName= 'GetEditorFormats'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Sales Reps' AND ControllerName = 'SalesRep'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'DynamicContent' and ActionName= 'GetEditorFormats')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Sales Reps' AND ControllerName = 'SalesRep') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'DynamicContent' and ActionName= 'GetEditorFormats'))

GO

----ZPD-9144

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Brand','ActiveInactiveBrand',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Brand' and ActionName = 'ActiveInactiveBrand')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Brands' AND ControllerName = 'Brand')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Brand' and ActionName= 'ActiveInactiveBrand') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Brands' AND ControllerName = 'Brand') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Brand' and ActionName= 'ActiveInactiveBrand'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Brands' AND ControllerName = 'Brand'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Brand' and ActionName= 'ActiveInactiveBrand')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Brands' AND ControllerName = 'Brand') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Brand' and ActionName= 'ActiveInactiveBrand'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'CategoryAttributeGroup','UpdateAttributeDisplayOrder',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'CategoryAttributeGroup' and ActionName = 'UpdateAttributeDisplayOrder')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Attribute Groups' AND ControllerName = 'CategoryAttributeGroup')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CategoryAttributeGroup' and ActionName= 'UpdateAttributeDisplayOrder') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Attribute Groups' AND ControllerName = 'CategoryAttributeGroup') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'CategoryAttributeGroup' and ActionName= 'UpdateAttributeDisplayOrder'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Attribute Groups' AND ControllerName = 'CategoryAttributeGroup'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CategoryAttributeGroup' and ActionName= 'UpdateAttributeDisplayOrder')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Attribute Groups' AND ControllerName = 'CategoryAttributeGroup') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CategoryAttributeGroup' and ActionName= 'UpdateAttributeDisplayOrder'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','GetWebstoreDomainsForCloudflare',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'GetWebstoreDomainsForCloudflare')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetWebstoreDomainsForCloudflare') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetWebstoreDomainsForCloudflare'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetWebstoreDomainsForCloudflare')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetWebstoreDomainsForCloudflare'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Inventory','DownloadTemplate',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Inventory' and ActionName = 'DownloadTemplate')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Inventory' AND ControllerName = 'Inventory')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Inventory' and ActionName= 'DownloadTemplate') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Inventory' AND ControllerName = 'Inventory') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Inventory' and ActionName= 'DownloadTemplate'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Inventory' AND ControllerName = 'Inventory'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Inventory' and ActionName= 'DownloadTemplate')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Inventory' AND ControllerName = 'Inventory') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Inventory' and ActionName= 'DownloadTemplate'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'MyReports','Filter',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'MyReports' and ActionName = 'Filter')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Reports' AND ControllerName = 'MyReports')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'MyReports' and ActionName= 'Filter') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Reports' AND ControllerName = 'MyReports') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'MyReports' and ActionName= 'Filter'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Reports' AND ControllerName = 'MyReports'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'MyReports' and ActionName= 'Filter')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Reports' AND ControllerName = 'MyReports') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'MyReports' and ActionName= 'Filter'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetPersonalisedAttributes',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetPersonalisedAttributes')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPersonalisedAttributes') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPersonalisedAttributes'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPersonalisedAttributes')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPersonalisedAttributes'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetProductListBySKU',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetProductListBySKU')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetProductListBySKU') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetProductListBySKU'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetProductListBySKU')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetProductListBySKU'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetQuickOrder',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetQuickOrder')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetQuickOrder') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetQuickOrder'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetQuickOrder')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetQuickOrder'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','ManagePaymentStatus',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManagePaymentStatus')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ManagePaymentStatus') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ManagePaymentStatus'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ManagePaymentStatus')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ManagePaymentStatus'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','ReOrder',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'ReOrder')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ReOrder') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ReOrder'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ReOrder')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ReOrder'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','SendPOEmail',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'SendPOEmail')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'SendPOEmail') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'SendPOEmail'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'SendPOEmail')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'SendPOEmail'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','UpdateShippingType',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'UpdateShippingType')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingType') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingType'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingType')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'UpdateShippingType'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Price','GetCulture',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Price' and ActionName = 'GetCulture')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pricing Engine' AND ControllerName = 'Price')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Price' and ActionName= 'GetCulture') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pricing Engine' AND ControllerName = 'Price') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Price' and ActionName= 'GetCulture'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pricing Engine' AND ControllerName = 'Price'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Price' and ActionName= 'GetCulture')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pricing Engine' AND ControllerName = 'Price') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Price' and ActionName= 'GetCulture'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ProductAttribute','SwatchType',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ProductAttribute' and ActionName = 'SwatchType')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Product Attributes' AND ControllerName = 'ProductAttribute')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductAttribute' and ActionName= 'SwatchType') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Product Attributes' AND ControllerName = 'ProductAttribute') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductAttribute' and ActionName= 'SwatchType'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Product Attributes' AND ControllerName = 'ProductAttribute'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductAttribute' and ActionName= 'SwatchType')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Product Attributes' AND ControllerName = 'ProductAttribute') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductAttribute' and ActionName= 'SwatchType'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ProductAttributeGroup','UpdateAttributeDisplayOrder',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ProductAttributeGroup' and ActionName = 'UpdateAttributeDisplayOrder')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Attribute Groups' AND ControllerName = 'ProductAttributeGroup')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductAttributeGroup' and ActionName= 'UpdateAttributeDisplayOrder') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Attribute Groups' AND ControllerName = 'ProductAttributeGroup') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductAttributeGroup' and ActionName= 'UpdateAttributeDisplayOrder'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Attribute Groups' AND ControllerName = 'ProductAttributeGroup'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductAttributeGroup' and ActionName= 'UpdateAttributeDisplayOrder')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Attribute Groups' AND ControllerName = 'ProductAttributeGroup') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductAttributeGroup' and ActionName= 'UpdateAttributeDisplayOrder'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Products','GetAssociatedProductCategories',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Products' and ActionName = 'GetAssociatedProductCategories')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'PIM' AND ControllerName = 'Products')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedProductCategories') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'PIM' AND ControllerName = 'Products') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedProductCategories'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'PIM' AND ControllerName = 'Products'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedProductCategories')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'PIM' AND ControllerName = 'Products') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedProductCategories'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Products','GetAssociatedProductCategories',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Products' and ActionName = 'GetAssociatedProductCategories')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Products' AND ControllerName = 'Products')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedProductCategories') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Products' AND ControllerName = 'Products') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedProductCategories'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Products' AND ControllerName = 'Products'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedProductCategories')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Products' AND ControllerName = 'Products') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedProductCategories'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Products','ProductCategoryList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Products' and ActionName = 'ProductCategoryList')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Products' AND ControllerName = 'Products')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'ProductCategoryList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Products' AND ControllerName = 'Products') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'ProductCategoryList'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Products' AND ControllerName = 'Products'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'ProductCategoryList')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Products' AND ControllerName = 'Products') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'ProductCategoryList'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Products','ProductCategoryList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Products' and ActionName = 'ProductCategoryList')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'PIM' AND ControllerName = 'Products')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'ProductCategoryList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'PIM' AND ControllerName = 'Products') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'ProductCategoryList'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'PIM' AND ControllerName = 'Products'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'ProductCategoryList')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'PIM' AND ControllerName = 'Products') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'ProductCategoryList'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Promotion','ExportPromotionCoupanData',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Promotion' and ActionName = 'ExportPromotionCoupanData')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Promotions and Coupons' AND ControllerName = 'Promotion')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionCoupanData') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Promotions and Coupons' AND ControllerName = 'Promotion') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionCoupanData'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Promotions and Coupons' AND ControllerName = 'Promotion'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionCoupanData')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Promotions and Coupons' AND ControllerName = 'Promotion') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionCoupanData'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Promotion','ExportPromotionCoupanData',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Promotion' and ActionName = 'ExportPromotionCoupanData')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Marketing' AND ControllerName = 'Promotion')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionCoupanData') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Marketing' AND ControllerName = 'Promotion') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionCoupanData'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Marketing' AND ControllerName = 'Promotion'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionCoupanData')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Marketing' AND ControllerName = 'Promotion') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionCoupanData'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Promotion','ExportPromotionData',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Promotion' and ActionName = 'ExportPromotionData')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Marketing' AND ControllerName = 'Promotion')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionData') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Marketing' AND ControllerName = 'Promotion') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionData'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Marketing' AND ControllerName = 'Promotion'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionData')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Marketing' AND ControllerName = 'Promotion') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionData'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Promotion','ExportPromotionData',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Promotion' and ActionName = 'ExportPromotionData')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Promotions and Coupons' AND ControllerName = 'Promotion')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionData') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Promotions and Coupons' AND ControllerName = 'Promotion') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionData'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Promotions and Coupons' AND ControllerName = 'Promotion'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionData')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Promotions and Coupons' AND ControllerName = 'Promotion') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Promotion' and ActionName= 'ExportPromotionData'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetOrderHistory',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetOrderHistory')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetOrderHistory') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetOrderHistory'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetOrderHistory')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'OMS' AND ControllerName = 'Order') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetOrderHistory'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Index','PublishHistory',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Index' and ActionName = 'PublishHistory')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Index' and ActionName= 'PublishHistory') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Index' and ActionName= 'PublishHistory'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Index' and ActionName= 'PublishHistory')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Index' and ActionName= 'PublishHistory'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'SEO','GetSEOTypeName',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'SEO' and ActionName = 'GetSEOTypeName')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'GetSEOTypeName') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'GetSEOTypeName'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'GetSEOTypeName')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'GetSEOTypeName'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'SEO','GetTabStructureForShippingOrigin',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'SEO' and ActionName = 'GetTabStructureForShippingOrigin')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'GetTabStructureForShippingOrigin') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'GetTabStructureForShippingOrigin'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'GetTabStructureForShippingOrigin')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'GetTabStructureForShippingOrigin'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'SEO','IsSEOCodeExist',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'SEO' and ActionName = 'IsSEOCodeExist')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'IsSEOCodeExist') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'IsSEOCodeExist'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'IsSEOCodeExist')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'SEO Setup' AND ControllerName = 'SEO') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'SEO' and ActionName= 'IsSEOCodeExist'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Vendor','ActiveInactiveVendor',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Vendor' and ActionName = 'ActiveInactiveVendor')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Vendors' AND ControllerName = 'Vendor')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Vendor' and ActionName= 'ActiveInactiveVendor') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Vendors' AND ControllerName = 'Vendor') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Vendor' and ActionName= 'ActiveInactiveVendor'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Vendors' AND ControllerName = 'Vendor'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Vendor' and ActionName= 'ActiveInactiveVendor')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Vendors' AND ControllerName = 'Vendor') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Vendor' and ActionName= 'ActiveInactiveVendor'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'WebSite','ErrorOnWidget',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'WebSite' and ActionName = 'ErrorOnWidget')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Banner Sliders' AND ControllerName = 'WebSite')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'ErrorOnWidget') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Banner Sliders' AND ControllerName = 'WebSite') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'ErrorOnWidget'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Banner Sliders' AND ControllerName = 'WebSite'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'ErrorOnWidget')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Banner Sliders' AND ControllerName = 'WebSite') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'WebSite' and ActionName= 'ErrorOnWidget'))

GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'TouchPointConfiguration','SchedulerLogDetails',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName = 'SchedulerLogDetails')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'SchedulerLogDetails') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'SchedulerLogDetails'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'SchedulerLogDetails')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'ERP Configuration' AND ControllerName = 'TouchPointConfiguration') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName= 'SchedulerLogDetails'))

GO



GO

---dt 19-03-2020 ZPD-9489
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','GetConfigurationSettings',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'GetConfigurationSettings')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetConfigurationSettings') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetConfigurationSettings'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetConfigurationSettings') 
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetConfigurationSettings'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','UpdateConfigurationSettings',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'UpdateConfigurationSettings')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateConfigurationSettings') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateConfigurationSettings'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateConfigurationSettings') 
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdateConfigurationSettings'))

go


 Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','GetConfigurationSettings',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'GetConfigurationSettings')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetConfigurationSettings') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetConfigurationSettings'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetConfigurationSettings') 
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetConfigurationSettings'))


----dt 24-03-2020 ZPD-8144 --> ZPD-9562 
update ZnodeActions set IsGlobalAccess = 0 where ControllerName = 'MediaManager' and ActionName = 'List'

--dt 27-03-2020 ZPD-7626 --> ZPD-9585

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'CMSSearchConfiguration','InsertCreateIndexData',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName = 'InsertCreateIndexData')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'InsertCreateIndexData') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'InsertCreateIndexData'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'InsertCreateIndexData') 
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'InsertCreateIndexData'))
go

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'CMSSearchConfiguration','CreateIndex',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName = 'CreateIndex')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'CreateIndex') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'CreateIndex'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'CreateIndex') 
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'CreateIndex'))

go


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'CMSSearchConfiguration','GetCmsPageSearchIndexMonitor',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName = 'GetCmsPageSearchIndexMonitor')

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'GetCmsPageSearchIndexMonitor') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'GetCmsPageSearchIndexMonitor'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport')	,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'GetCmsPageSearchIndexMonitor') 
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Site Search' AND ControllerName = 'SearchReport') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'CMSSearchConfiguration' and ActionName= 'GetCmsPageSearchIndexMonitor'))

--dt 02-04-2020 ZPD-9222  --> ZPD-9507
insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'Reports'),'BI Reports',3,null,'PowerBI','PowerBIReport','z-powerbi-report',1,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMenu where MenuName = 'BI Reports')


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','GetPowerBISettings',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'GetPowerBISettings')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetPowerBISettings') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetPowerBISettings'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetPowerBISettings')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetPowerBISettings'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','UpdatePowerBISettings',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'UpdatePowerBISettings')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdatePowerBISettings') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdatePowerBISettings'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdatePowerBISettings')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdatePowerBISettings'))

GO


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'PowerBI','GetPowerBIReport',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'PowerBI' and ActionName = 'GetPowerBIReport')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'BI Reports' AND ControllerName = 'PowerBI')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'PowerBI' and ActionName= 'GetPowerBIReport') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'BI Reports' AND ControllerName = 'PowerBI') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'PowerBI' and ActionName= 'GetPowerBIReport'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'BI Reports' AND ControllerName = 'PowerBI'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'PowerBI' and ActionName= 'GetPowerBIReport')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'BI Reports' AND ControllerName = 'PowerBI') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'PowerBI' and ActionName= 'GetPowerBIReport'))

GO

update ZnodeMenu set ActionName= 'GetPowerBIReport', CSSClassName='z-powerbi' where ControllerName='PowerBI' AND ActionName= 'GetPowerBIReport'

update ZnodeMenu set ActionName= 'GetPowerBIReport', CSSClassName='z-power-bi' where ControllerName='PowerBI' AND ActionName= 'GetPowerBIReport'

--ZPD-9865
update ZnodeActions set IsGlobalAccess = 1 where ActionName= 'GetSuggestions' AND ControllerName='Typeahead'

--dt 16-04-2020 ZPD-9976 --> ZPD-9991

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Content','IsContentPageNameExistForPortal',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Content' and ActionName = 'IsContentPageNameExistForPortal')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'CMS' AND ControllerName = 'StoreExperience')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'IsContentPageNameExistForPortal') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'CMS' AND ControllerName = 'StoreExperience') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'IsContentPageNameExistForPortal'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'CMS' AND ControllerName = 'StoreExperience') ,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'IsContentPageNameExistForPortal')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'CMS' AND ControllerName = 'StoreExperience') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'IsContentPageNameExistForPortal'))
GO
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Content','IsContentPageNameExistForPortal',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Content' and ActionName = 'IsContentPageNameExistForPortal')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'IsContentPageNameExistForPortal') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'IsContentPageNameExistForPortal'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content') ,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'IsContentPageNameExistForPortal')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'IsContentPageNameExistForPortal'))

--ZPD-10248/ ZPD-9222
update ZnodeMenu set MenuName = 'BI Reports' where ActionName = 'GetPowerBIReport'
--dt 12-05-2020 ZPD-9739 --> ZPD-10176
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','QuoteList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'QuoteList')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'QuoteList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'QuoteList'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'QuoteList')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'QuoteList'))
go
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','GetQuoteDetails',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteDetails')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteDetails') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteDetails'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteDetails')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteDetails'))

go

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','ManangeQuoteStatus',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'ManangeQuoteStatus')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'ManangeQuoteStatus') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'ManangeQuoteStatus'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'ManangeQuoteStatus')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'ManangeQuoteStatus'))

go

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','GetQuoteStateValueById',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteStateValueById')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteStateValueById') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteStateValueById'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteStateValueById')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteStateValueById'))
go
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateShippingAccountNumber',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingAccountNumber')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingAccountNumber') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingAccountNumber'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingAccountNumber')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingAccountNumber'))
go
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateShippingMethod',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingMethod')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingMethod') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingMethod'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingMethod')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingMethod'))
go
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateInHandDate',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateInHandDate')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateInHandDate') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateInHandDate'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateInHandDate')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateInHandDate'))
go
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateQuoteExpirationDate',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteExpirationDate')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteExpirationDate') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteExpirationDate'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteExpirationDate')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteExpirationDate'))
go
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','GetQuoteShippingList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteShippingList')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteShippingList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteShippingList'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteShippingList')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetQuoteShippingList'))
--dt 02-06-2020 ZPD-10380
insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 MenuId from ZnodeMenu where MenuName = 'OMS'),'Returns',2,null,'RMAReturn','List',null,1,
2,GETDATE(),2,GETDATE()
where not exists(select * from ZnodeMenu where MenuName = 'Returns')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'RMAReturn','List',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'List')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'List') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'List'))


insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'List')
,2,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'List'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'RMAReturn','ManageReturn',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'ManageReturn')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'ManageReturn') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'ManageReturn'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'ManageReturn')
,2,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'ManageReturn'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'RMAReturn','ManangeReturnStatus',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'ManangeReturnStatus')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'ManangeReturnStatus') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'ManangeReturnStatus'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'ManangeReturnStatus')
,2,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'ManangeReturnStatus'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'RMAReturn','GetAdditionalReturnNotes',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetAdditionalReturnNotes')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetAdditionalReturnNotes') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetAdditionalReturnNotes'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetAdditionalReturnNotes')
,2,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetAdditionalReturnNotes'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'RMAReturn','UpdateOrderReturnLineItem',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'UpdateOrderReturnLineItem')


insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'UpdateOrderReturnLineItem') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'UpdateOrderReturnLineItem'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'UpdateOrderReturnLineItem')
,2,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'UpdateOrderReturnLineItem'))

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'RMAReturn','UpdateOrderReturnStatus',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'UpdateOrderReturnStatus')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'UpdateOrderReturnStatus') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'UpdateOrderReturnStatus'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'UpdateOrderReturnStatus')
,2,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'UpdateOrderReturnStatus'))

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'RMAReturn','SubmitOrderReturn',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'SubmitOrderReturn')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'SubmitOrderReturn') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'SubmitOrderReturn'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'SubmitOrderReturn')
,2,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'SubmitOrderReturn'))

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'RMAReturn','PrintReturn',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'PrintReturn')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'PrintReturn') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'PrintReturn'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'PrintReturn')
,2,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'PrintReturn'))

update ZnodeMenu set CSSClassName = 'z-return' where MenuName = 'Returns'

--dt 27-05-2020 ZPD-9797 --> ZPD-10323
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Category','UpdateCategoryProductDetail',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Category' and ActionName = 'UpdateCategoryProductDetail')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Categories')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Category' and ActionName = 'UpdateCategoryProductDetail') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Categories') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Category' and ActionName = 'UpdateCategoryProductDetail'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Categories') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Category' and ActionName = 'UpdateCategoryProductDetail')
,3,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Categories') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Category' and ActionName = 'UpdateCategoryProductDetail'))

--dt 28-05-2020 ZPD-10230
Update ZnodeActions SET ActionName='DeleteAssociatedProfileCatalog' where ControllerName= 'Profiles' AND ActionName= 'DeleteAssociatedProfileCatalogs'

Update ZnodeActions SET ActionName='AssociateCatalogToProfile' where ControllerName= 'Profiles' AND ActionName= 'AssociateCatalogsToProfile'

--dt 23-06-2020 ZPD-10943 --> ZPD-10253
GO

INSERT INTO ZnodeActions (AreaName, ControllerName, ActionName, IsGlobalAccess, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT NULL, 'Order', 'UpdateInHandDate', 0, 2, GETDATE(), 2, GETDATE() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateInHandDate')

INSERT INTO ZnodeMenuActionsPermission (MenuId, ActionId, AccessPermissionId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'OMS' AND ControllerName = 'Order'),
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateInHandDate'), 2, 2, GETDATE(), 2, GETDATE() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'OMS' AND ControllerName = 'Order')
AND ActionId = (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateInHandDate'))

INSERT INTO ZnodeMenuActionsPermission (MenuId, ActionId, AccessPermissionId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Orders' AND ControllerName = 'Order'),
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateInHandDate'), 2, 2, GETDATE(), 2, GETDATE() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Orders' AND ControllerName = 'Order')
AND ActionId = (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateInHandDate'))

GO

INSERT INTO ZnodeActions (AreaName, ControllerName, ActionName, IsGlobalAccess, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT NULL, 'Order', 'UpdateJobName', 0, 2, GETDATE(), 2, GETDATE() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateJobName')

INSERT INTO ZnodeMenuActionsPermission (MenuId, ActionId, AccessPermissionId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'OMS' AND ControllerName = 'Order'),
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateJobName'), 2, 2, GETDATE(), 2, GETDATE() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'OMS' AND ControllerName = 'Order')
AND ActionId = (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateJobName'))

INSERT INTO ZnodeMenuActionsPermission (MenuId, ActionId, AccessPermissionId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Orders' AND ControllerName = 'Order'),
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateJobName'), 2, 2, GETDATE(), 2, GETDATE() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Orders' AND ControllerName = 'Order')
AND ActionId = (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateJobName'))

GO

INSERT INTO ZnodeActions (AreaName, ControllerName, ActionName, IsGlobalAccess, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT NULL, 'Order', 'UpdateShippingConstraintCode', 0, 2, GETDATE(), 2, GETDATE() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateShippingConstraintCode')

INSERT INTO ZnodeMenuActionsPermission (MenuId, ActionId, AccessPermissionId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'OMS' AND ControllerName = 'Order'),
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateShippingConstraintCode'), 2, 2, GETDATE(), 2, GETDATE() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'OMS' AND ControllerName = 'Order')
AND ActionId = (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateShippingConstraintCode'))

INSERT INTO ZnodeMenuActionsPermission (MenuId, ActionId, AccessPermissionId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Orders' AND ControllerName = 'Order'),
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateShippingConstraintCode'), 2, 2, GETDATE(), 2, GETDATE() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Orders' AND ControllerName = 'Order')
AND ActionId = (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName = 'UpdateShippingConstraintCode'))

--dt 05-06-2020 ZPD-10525 --> ZDP-3171
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Payment','IsPaymentDisplayNameExists',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Payment' and ActionName = 'IsPaymentDisplayNameExists')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Payment Methods' AND ControllerName = 'Payment')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Payment' and ActionName= 'IsPaymentDisplayNameExists') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Payment Methods' AND ControllerName = 'Payment') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Payment' and ActionName= 'IsPaymentDisplayNameExists'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Payment Methods' AND ControllerName = 'Payment') ,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Payment' and ActionName= 'IsPaymentDisplayNameExists')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Payment Methods' AND ControllerName = 'Payment') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Payment' and ActionName= 'IsPaymentDisplayNameExists'))

--dt 02-04-2020 ZPD-9222  --> ZPD-9507
insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'Reports'),'BI Reports',3,null,'PowerBI','PowerBIReport','z-powerbi-report',1,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMenu where MenuName = 'BI Reports')


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','GetPowerBISettings',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'GetPowerBISettings')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetPowerBISettings') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetPowerBISettings'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetPowerBISettings')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'GetPowerBISettings'))

GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GeneralSetting','UpdatePowerBISettings',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName = 'UpdatePowerBISettings')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdatePowerBISettings') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdatePowerBISettings'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdatePowerBISettings')	
,2,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'GeneralSetting' and ActionName= 'UpdatePowerBISettings'))

GO


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'PowerBI','GetPowerBIReport',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'PowerBI' and ActionName = 'GetPowerBIReport')


insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'BI Reports' AND ControllerName = 'PowerBI')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'PowerBI' and ActionName= 'GetPowerBIReport') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'BI Reports' AND ControllerName = 'PowerBI') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'PowerBI' and ActionName= 'GetPowerBIReport'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'BI Reports' AND ControllerName = 'PowerBI'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'PowerBI' and ActionName= 'GetPowerBIReport')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'BI Reports' AND ControllerName = 'PowerBI') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'PowerBI' and ActionName= 'GetPowerBIReport'))

GO

update ZnodeMenu set ActionName= 'GetPowerBIReport', CSSClassName='z-powerbi' where ControllerName='PowerBI' AND ActionName= 'GetPowerBIReport'

update ZnodeMenu set ActionName= 'GetPowerBIReport', CSSClassName='z-power-bi' where ControllerName='PowerBI' AND ActionName= 'GetPowerBIReport'

--ZPD-10248/ ZPD-9222
update ZnodeMenu set MenuName = 'BI Reports' where ActionName = 'GetPowerBIReport'

--dt 25-06-2020 ZPD-11042
update znodeactions set ControllerName = 'WebSite' where actionname = 'ManageMediaWidgetConfiguration' and ControllerName = 'WebSite ' 

--dt26-06-2020 ZPD-10248/ ZPD-9222
update  ZnodeMenu set ActionName = 'GetPowerBIReport' where ControllerName = 'PowerBI' and ActionName = 'PowerBIReport'
update znodeactions set ControllerName = 'WebSite' where actionname = 'ManageMediaWidgetConfiguration' and ControllerName = 'WebSite ' 

--dt 29-06-2020 ZPD-9739 --> ZPD-10176
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateShippingHandling',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingHandling')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingHandling') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingHandling'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingHandling')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingHandling'))
----------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','GetUserAddressForManageById',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetUserAddressForManageById')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetUserAddressForManageById') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetUserAddressForManageById'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetUserAddressForManageById')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetUserAddressForManageById'))
----------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','ManageCustomerAddress',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'ManageCustomerAddress')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'ManageCustomerAddress') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'ManageCustomerAddress'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'ManageCustomerAddress')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'ManageCustomerAddress'))
-------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','GetAddressWithValidation',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetAddressWithValidation')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetAddressWithValidation') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetAddressWithValidation'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetAddressWithValidation')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetAddressWithValidation'))
---------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','RemoveQuoteCartItem',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'RemoveQuoteCartItem')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'RemoveQuoteCartItem') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'RemoveQuoteCartItem'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'RemoveQuoteCartItem')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'RemoveQuoteCartItem'))
------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','GetShoppingCartItems',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetShoppingCartItems')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetShoppingCartItems') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetShoppingCartItems'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetShoppingCartItems')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetShoppingCartItems'))
---------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','CalculateShoppingCart',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CalculateShoppingCart')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CalculateShoppingCart') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CalculateShoppingCart'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CalculateShoppingCart')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CalculateShoppingCart'))
------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateQuoteCartItem',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteCartItem')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteCartItem') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteCartItem'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteCartItem')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteCartItem'))
--------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateTaxExemptForManage',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateTaxExemptForManage')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateTaxExemptForManage') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateTaxExemptForManage'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateTaxExemptForManage')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateTaxExemptForManage'))
----------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','PrintManageQuote',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'PrintManageQuote')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'PrintManageQuote') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'PrintManageQuote'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'PrintManageQuote')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'PrintManageQuote'))
-------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateShippingConstraintCode',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingConstraintCode')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingConstraintCode') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingConstraintCode'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingConstraintCode')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateShippingConstraintCode'))
---------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateJobName',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateJobName')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateJobName') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateJobName'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateJobName')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateJobName'))
------------------------------------------

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateQuote',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuote')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuote') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuote'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuote')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuote'))

-----------------------------------------------------------------------------------------------------
--dt 1-07-2020 --> ZPD-7890 >> ZPD-1111

INSERT  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'Diagnostics', 'Maintenance', 'Index', 0, 2, Getdate(), 2, Getdate() 
WHERE NOT EXISTS 
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Maintenance' AND ActionName = 'Index')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance'),
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Maintenance' AND ActionName = 'Index'),
2,Getdate(),2,Getdate()
WHERE NOT EXISTS
(SELECT * FROM ZnodeActionMenu 
WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance')
AND ActionId = (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Maintenance' 
AND ActionName = 'Index'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance'),
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Maintenance' AND ActionName = 'Index'),
2, 2, Getdate(), 2, Getdate()
WHERE NOT EXISTS
(SELECT * FROM ZnodeMenuActionsPermission 
WHERE MenuId =(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance')
AND ActionId = (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Maintenance' 
AND ActionName = 'Index'))


INSERT  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'Diagnostics', 'Maintenance', 'PurgeAllPublishedData', 0, 2, Getdate(), 2, Getdate() 
WHERE NOT EXISTS 
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Maintenance' AND ActionName = 'PurgeAllPublishedData')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance'),
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Maintenance' AND ActionName = 'PurgeAllPublishedData'),
2,Getdate(),2,Getdate()
WHERE NOT EXISTS
(SELECT * FROM ZnodeActionMenu 
WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance')
AND ActionId = (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Maintenance' 
AND ActionName = 'PurgeAllPublishedData'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance'),
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Maintenance' AND ActionName = 'PurgeAllPublishedData'),
2, 2, Getdate(), 2, Getdate()
WHERE NOT EXISTS
(SELECT * FROM ZnodeMenuActionsPermission 
WHERE MenuId =(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance')
AND ActionId = (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Maintenance' 
AND ActionName = 'PurgeAllPublishedData'))

GO
--------------------------------------------------------------------------------------

--dt ZPD-9737 --> ZPD-11288
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','CreateQuoteRequest',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CreateQuoteRequest')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CreateQuoteRequest') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CreateQuoteRequest'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CreateQuoteRequest')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CreateQuoteRequest'))
----------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','SubmitQuote',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'SubmitQuote')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'SubmitQuote') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'SubmitQuote'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'SubmitQuote')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'SubmitQuote'))
--dt 06-07-2020 ZPD-10012 --> ZPD-10839
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'BlogNews','PublishBlogNewsPage',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'BlogNews' and ActionName = 'PublishBlogNewsPage')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'BlogNews','UpdateAndPublishBlogNews',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'BlogNews' and ActionName = 'UpdateAndPublishBlogNews')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Blogs & News')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'BlogNews' and ActionName = 'PublishBlogNewsPage') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Blogs & News') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'BlogNews' and ActionName = 'PublishBlogNewsPage'))

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Blogs & News')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'BlogNews' and ActionName = 'UpdateAndPublishBlogNews') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Blogs & News') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'BlogNews' and ActionName = 'UpdateAndPublishBlogNews'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Blogs & News') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'BlogNews' and ActionName = 'PublishBlogNewsPage')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Blogs & News') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'BlogNews' and ActionName = 'PublishBlogNewsPage'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Blogs & News') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'BlogNews' and ActionName = 'UpdateAndPublishBlogNews')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Blogs & News') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'BlogNews' and ActionName = 'UpdateAndPublishBlogNews'))

--dt 08-07-2020 ZPD-9739 --> ZPD-11120, ZPD-11196
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','GetAdditionalNotes',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetAdditionalNotes')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetAdditionalNotes') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetAdditionalNotes'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetAdditionalNotes')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetAdditionalNotes'))
----------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateQuoteStatus',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteStatus')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteStatus') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteStatus'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteStatus')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteStatus'))
----------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','CalculateShippingInManage',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CalculateShippingInManage')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CalculateShippingInManage') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CalculateShippingInManage'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CalculateShippingInManage')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'CalculateShippingInManage'))
---dt 10-07-2020 ZPD-10771
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'PIM' ,'Catalog','AssociateCategoryToCatalog',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Catalog' and ActionName = 'AssociateCategoryToCatalog')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Catalogs' AND ControllerName = 'Catalog')
   ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Catalog' and ActionName= 'AssociateCategoryToCatalog') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
    (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Catalogs' AND ControllerName = 'Catalog') and ActionId =
    (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Catalog' and ActionName= 'AssociateCategoryToCatalog'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Catalogs' AND ControllerName = 'Catalog') ,
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Catalog' and ActionName= 'AssociateCategoryToCatalog')
,3,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Catalogs' AND ControllerName = 'Catalog') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Catalog' and ActionName= 'AssociateCategoryToCatalog'))

--dt 13-07-2020 ZPD-9739 --> ZPD-11416
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','GetPaymentMethods',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetPaymentMethods')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetPaymentMethods') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetPaymentMethods'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetPaymentMethods')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'GetPaymentMethods'))
----------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','SaveAndConvertQuoteToOrder',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'SaveAndConvertQuoteToOrder')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'SaveAndConvertQuoteToOrder') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'SaveAndConvertQuoteToOrder'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'SaveAndConvertQuoteToOrder')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'SaveAndConvertQuoteToOrder'))
----------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','UpdateQuoteCartItemPrice',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteCartItemPrice')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteCartItemPrice') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteCartItemPrice'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteCartItemPrice')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'UpdateQuoteCartItemPrice'))

------------------------------------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','IsCreditCardPayment',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'IsCreditCardPayment')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'IsCreditCardPayment') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'IsCreditCardPayment'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'IsCreditCardPayment')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'IsCreditCardPayment'))

--dt 21-07-2020 ZPD-10781 --> ZPd-10382
insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'OMS'),'Vouchers',4,null,'GiftCard','List','z-gift-cards',1,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMenu where MenuName = 'Vouchers')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Customer','GetUserVoucherList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Customer' and ActionName = 'GetUserVoucherList')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Users')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName = 'GetUserVoucherList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Users') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName = 'GetUserVoucherList'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Users') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName = 'GetUserVoucherList')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Users') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName = 'GetUserVoucherList'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Giftcard','ActiveDeactiveVouchers',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'ActiveDeactiveVouchers')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'ActiveDeactiveVouchers') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'ActiveDeactiveVouchers'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'ActiveDeactiveVouchers')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'ActiveDeactiveVouchers'))

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Giftcard','GetVoucherHistoryList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'GetVoucherHistoryList')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'GetVoucherHistoryList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'GetVoucherHistoryList'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'GetVoucherHistoryList')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'GetVoucherHistoryList'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','ManageApplyVoucher',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageApplyVoucher')
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','ManageRemoveVoucher',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageRemoveVoucher')


insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageRemoveVoucher') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageRemoveVoucher'))

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageRemoveVoucher') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageRemoveVoucher'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageRemoveVoucher')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageRemoveVoucher'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageRemoveVoucher')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageRemoveVoucher'))
-----------------------------------------------

UPdate ZnodeMenu SET MenuName= 'Vouchers' where ControllerName ='GiftCard'  AND ActionName ='List'

update ZnodeActionMenu set menuid = ((select min(menuid) from ZnodeMenu where MenuName = 'Vouchers'))
where MenuId = (select max(menuid) from ZnodeMenu where MenuName = 'Vouchers')
and exists(select MenuName from ZnodeMenu where MenuName = 'Vouchers' group by MenuName having count(1)>1)

update ZnodeMenuActionsPermission set menuid = ((select min(menuid) from ZnodeMenu where MenuName = 'Vouchers'))
where MenuId = (select max(menuid) from ZnodeMenu where MenuName = 'Vouchers')
and exists(select MenuName from ZnodeMenu where MenuName = 'Vouchers' group by MenuName having count(1)>1)

update ZnodeRoleMenu set menuid = ((select min(menuid) from ZnodeMenu where MenuName = 'Vouchers'))
where MenuId = (select max(menuid) from ZnodeMenu where MenuName = 'Vouchers')
and exists(select MenuName from ZnodeMenu where MenuName = 'Vouchers' group by MenuName having count(1)>1)

delete from ZnodeMenu where menuid = (
select max(menuid)
from Znodemenu zm
where  MenuName= 'Vouchers' 
and exists(select MenuName from ZnodeMenu where MenuName = 'Vouchers' group by MenuName having count(1)>1)
and not exists(select * from ZnodeActionMenu zam where zm.Menuid = ZAM.MenuId ))

update  ZnodeGiftCard set RemainingAmount = 0 where RemainingAmount  is null 

--dt 23-07-2020 ZPD-11441
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Quote','QuoteCheckoutReceipt',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Quote' and ActionName = 'QuoteCheckoutReceipt')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'QuoteCheckoutReceipt') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'QuoteCheckoutReceipt'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'QuoteCheckoutReceipt')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Quote' and ActionName = 'QuoteCheckoutReceipt'))

Update ZnodeActions SET ActionName= 'ManageQuoteStatus' where ControllerName ='Quote'  AND ActionName ='ManangeQuoteStatus' 

--dt 27-07-2020 ZPD-11677
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','ManageApplyVoucher',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageApplyVoucher')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageApplyVoucher') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageApplyVoucher'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageApplyVoucher')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ManageApplyVoucher'))

--dt 17-08-2020 ZPD-11501 and ZPD-11610 
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Catalog','IsCatalogCodeExists',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Catalog' and ActionName = 'IsCatalogCodeExists')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Catalogs')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Catalog' and ActionName = 'IsCatalogCodeExists') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Catalogs') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Catalog' and ActionName = 'IsCatalogCodeExists'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Catalogs') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Catalog' and ActionName = 'IsCatalogCodeExists')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Catalogs') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Catalog' and ActionName = 'IsCatalogCodeExists'))
--------------------------------

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Account','IsAccountCodeExists',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Account' and ActionName = 'IsAccountCodeExists')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Accounts')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName = 'IsAccountCodeExists') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Accounts') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName = 'IsAccountCodeExists'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Accounts') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName = 'IsAccountCodeExists')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Accounts') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Account' and ActionName = 'IsAccountCodeExists'))

--dt 25-08-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','List',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'List')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'List') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'List'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'List')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'List'))


--dt 25-08-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','Create',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Create')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Create') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Create'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Create')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Create'))


--dt 25-08-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','Edit',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Edit')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Edit') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Edit'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Edit')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Edit'))


--dt 25-08-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','Delete',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Delete')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Delete') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Delete'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Delete')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'Delete'))


--dt 25-08-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','GetAssignedAttributeGroups',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetAssignedAttributeGroups')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetAssignedAttributeGroups') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetAssignedAttributeGroups'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetAssignedAttributeGroups')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetAssignedAttributeGroups'))


--dt 25-08-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','GetUnassignedAttributeGroups',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetUnassignedAttributeGroups')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetUnassignedAttributeGroups') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetUnassignedAttributeGroups'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetUnassignedAttributeGroups')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetUnassignedAttributeGroups'))


--dt 25-08-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','AssignAttributeGroups',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'AssignAttributeGroups')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'AssignAttributeGroups') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'AssignAttributeGroups'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'AssignAttributeGroups')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'AssignAttributeGroups'))


--dt 25-08-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','UnassignAttributeGroups',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'UnassignAttributeGroups')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'UnassignAttributeGroups') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'UnassignAttributeGroups'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'UnassignAttributeGroups')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'UnassignAttributeGroups'))


--dt 25-08-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','UpdateAttributeGroupDisplayOrder',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'UpdateAttributeGroupDisplayOrder')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'UpdateAttributeGroupDisplayOrder') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'UpdateAttributeGroupDisplayOrder'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'UpdateAttributeGroupDisplayOrder')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'UpdateAttributeGroupDisplayOrder'))


--dt 25-08-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','GetTabStructure',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetTabStructure')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetTabStructure') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetTabStructure'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetTabStructure')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'GetTabStructure'))



--dt 25-08-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','CreateAttributeFamilyLocale',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'CreateAttributeFamilyLocale')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'CreateAttributeFamilyLocale') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'CreateAttributeFamilyLocale'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'CreateAttributeFamilyLocale')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'CreateAttributeFamilyLocale'))

--ZPD-12141 dt 09-14-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','ApplyVoucher',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'ApplyVoucher')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ApplyVoucher') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ApplyVoucher'))

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ApplyVoucher') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ApplyVoucher'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ApplyVoucher')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ApplyVoucher'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ApplyVoucher')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'ApplyVoucher'))

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','RemoveVoucher',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Giftcard' and ActionName = 'RemoveVoucher')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'RemoveVoucher') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'RemoveVoucher'))

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'RemoveVoucher') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'RemoveVoucher'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'RemoveVoucher')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'RemoveVoucher'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'RemoveVoucher')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS') and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName = 'RemoveVoucher'))

--dt 16-09-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'GlobalAttributeFamily','IsFamilyCodeExist',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'IsFamilyCodeExist')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'IsFamilyCodeExist') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'IsFamilyCodeExist'))


insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'IsFamilyCodeExist')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes')) and ActionId =
(select top 1 ActionId from ZnodeActions where ControllerName = 'GlobalAttributeFamily' and ActionName = 'IsFamilyCodeExist'))

--dt 28-09-2020
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','List',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'List')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','List',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'List')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'List') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'List'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'List')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId =(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'List'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','Create',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'Create')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','Create',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'Create')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Create') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId =(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Create'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Create')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId =(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Create'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','Edit',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'Edit')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','Edit',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'Edit')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Edit') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId =(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Edit'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Edit')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId =(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Edit'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','GetVariants',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'GetVariants')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','GetVariants',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'GetVariants')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetVariants') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId =(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetVariants'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetVariants')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId =(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetVariants'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','GetUnassociatedProfiles',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'GetUnassociatedProfiles')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','GetUnassociatedProfiles',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'GetUnassociatedProfiles')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetUnassociatedProfiles') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetUnassociatedProfiles'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetUnassociatedProfiles')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetUnassociatedProfiles'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','AssociateVariants',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'AssociateVariants')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','AssociateVariants',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'AssociateVariants')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateVariants') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateVariants'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateVariants')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateVariants'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','AssociateWidgetTemplate',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'AssociateWidgetTemplate')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','AssociateWidgetTemplate',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'AssociateWidgetTemplate')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateWidgetTemplate') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateWidgetTemplate'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateWidgetTemplate')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateWidgetTemplate'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','GetEntityAttributeDetails',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'GetEntityAttributeDetails')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','GetEntityAttributeDetails',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'GetEntityAttributeDetails')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetEntityAttributeDetails') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetEntityAttributeDetails'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetEntityAttributeDetails')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetEntityAttributeDetails'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','SaveEntityDetails',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'SaveEntityDetails')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','SaveEntityDetails',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'SaveEntityDetails')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'SaveEntityDetails') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'SaveEntityDetails'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'SaveEntityDetails')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'SaveEntityDetails'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','DeleteAssociatedVariant',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'DeleteAssociatedVariant')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','DeleteAssociatedVariant',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'DeleteAssociatedVariant')


insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'DeleteAssociatedVariant') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'DeleteAssociatedVariant'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'DeleteAssociatedVariant')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'DeleteAssociatedVariant'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','Delete',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'Delete')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','Delete',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'Delete')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Delete') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Delete'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Delete')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'Delete'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','IsWidgetExist',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'IsWidgetExist')

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContentContainer','IsWidgetExist',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContentContainer' and ActionName = 'IsWidgetExist')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'IsWidgetExist') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'IsWidgetExist'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'IsWidgetExist')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'IsWidgetExist'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'WidgetTemplate','List',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'List')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'List') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'List'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'List')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'List'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'WidgetTemplate','Create',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Create')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Create') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Create'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Create')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Create'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'WidgetTemplate','Edit',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Edit')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Edit') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Edit'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Edit')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Edit'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'WidgetTemplate','Delete',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Delete')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Delete') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Delete'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Delete')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Delete'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'WidgetTemplate','Copy',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Copy')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Copy') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Copy'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Copy')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'Copy'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'WidgetTemplate','DownloadWidgetTemplate',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'DownloadWidgetTemplate')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'DownloadWidgetTemplate') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'DownloadWidgetTemplate'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'DownloadWidgetTemplate')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'DownloadWidgetTemplate'))



Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'WidgetTemplate','IsWidgetTemplateExist',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'IsWidgetTemplateExist')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
,(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'IsWidgetTemplateExist') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'IsWidgetTemplateExist'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))) ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'IsWidgetTemplateExist')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =(select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'WidgetTemplate' and ActionName = 'IsWidgetTemplateExist'))


--ZPD-12333 --> ZPD-10403
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'MediaManager' ,'MediaConfiguration','GenerateImages',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'MediaConfiguration' and ActionName = 'GenerateImages')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Media Settings')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'MediaConfiguration' and ActionName = 'GenerateImages') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Media Settings')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'MediaConfiguration' and ActionName = 'GenerateImages'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Media Settings'),
(select top 1 ActionId from ZnodeActions where ControllerName = 'MediaConfiguration' and ActionName = 'GenerateImages')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Media Settings')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'MediaConfiguration' and ActionName = 'GenerateImages'))

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'MediaManager' ,'MediaManager','GenerateImageOnEdit',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'MediaManager' and ActionName = 'GenerateImageOnEdit')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'DAM')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'MediaManager' and ActionName = 'GenerateImageOnEdit') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'DAM')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'MediaManager' and ActionName = 'GenerateImageOnEdit'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'DAM'),
(select top 1 ActionId from ZnodeActions where ControllerName = 'MediaManager' and ActionName = 'GenerateImageOnEdit')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'DAM')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'MediaManager' and ActionName = 'GenerateImageOnEdit'))

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 'MediaManager' ,'MediaManager','GenerateImageOnEdit',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'MediaManager' and ActionName = 'GenerateImageOnEdit')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Media Explorer')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'MediaManager' and ActionName = 'GenerateImageOnEdit') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Media Explorer')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'MediaManager' and ActionName = 'GenerateImageOnEdit'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Media Explorer'),
(select top 1 ActionId from ZnodeActions where ControllerName = 'MediaManager' and ActionName = 'GenerateImageOnEdit')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Media Explorer')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'MediaManager' and ActionName = 'GenerateImageOnEdit'))

---------------- ZPD-9657 --> ZPD-13470  dt-12/01/2021

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select '' ,'RMAReturn','CheckOrderEligibileForReturn',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'CheckOrderEligibileForReturn')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'CheckOrderEligibileForReturn') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'CheckOrderEligibileForReturn'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns'),
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'CheckOrderEligibileForReturn')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'CheckOrderEligibileForReturn'))
-----------------------------------------------------------

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select '' ,'RMAReturn','GetOrderDetailsForReturn',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetOrderDetailsForReturn')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetOrderDetailsForReturn') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetOrderDetailsForReturn'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns'),
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetOrderDetailsForReturn')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetOrderDetailsForReturn'))
-----------------------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select '' ,'RMAReturn','CalculateOrderReturn',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'CalculateOrderReturn')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'CalculateOrderReturn') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'CalculateOrderReturn'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns'),
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'CalculateOrderReturn')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'CalculateOrderReturn'))
-----------------------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select '' ,'RMAReturn','SubmitCreateReturn',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'SubmitCreateReturn')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'SubmitCreateReturn') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'SubmitCreateReturn'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns'),
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'SubmitCreateReturn')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'SubmitCreateReturn'))
-----------------------------------------------------------
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select '' ,'RMAReturn','GetReturnDetails',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetReturnDetails')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetReturnDetails') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetReturnDetails'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns'),
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetReturnDetails')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'GetReturnDetails'))

---------------- ZPD-9657 --> ZPD-13470  dt-12/01/2021

Update ZnodeActions SET ActionName='CheckOrderEligibleForReturn' where ControllerName= 'RMAReturn' AND ActionName= 'CheckOrderEligibileForReturn'

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select '' ,'RMAReturn','IsValidReturnItems',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'IsValidReturnItems')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'IsValidReturnItems') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'IsValidReturnItems'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns'),
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'IsValidReturnItems')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'IsValidReturnItems'))

---------------- ZPD-9657 --> ZPD-13616  dt-25/01/2021
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select '' ,'RMAReturn','PrintCreateReturnReceipt',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'PrintCreateReturnReceipt')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'PrintCreateReturnReceipt') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'PrintCreateReturnReceipt'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns'),
(select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'PrintCreateReturnReceipt')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'RMAReturn' and ActionName = 'PrintCreateReturnReceipt'))

---------------ZPD-13095  dt-04/02/2021
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select '' ,'Recommendation','CreateScheduler',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Recommendation' and ActionName = 'CreateScheduler')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Store Experience')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName = 'CreateScheduler') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Store Experience')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName = 'CreateScheduler'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Store Experience'),
(select top 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName = 'CreateScheduler')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Store Experience')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'Recommendation' and ActionName = 'CreateScheduler'))
insert into ZnodeActions (Areaname,ControllerName,ActionName,IsGlobalAccess,CreatedBy, CreatedDate,ModifiedBy,ModifiedDate)
select null,'Customer','GetSalesRepListForAccount',0,2,GETUTCDATE(),2,GETUTCDATE()
where not exists(select * from ZnodeActions where ControllerName = 'Customer' and ActionName = 'GetSalesRepListForAccount')

 insert into ZnodeActionMenu (MenuId,ActionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetSalesRepListForAccount') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetSalesRepListForAccount'))

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetSalesRepListForAccount')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Accounts' AND ControllerName = 'Account') and ActionId = 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'GetSalesRepListForAccount'))


insert into ZnodeActions (Areaname,ControllerName,ActionName,IsGlobalAccess,CreatedBy, CreatedDate,ModifiedBy,ModifiedDate)
SELECT null,'Dashboard','GetDashboardDetails',0,2,GETUTCDATE(),2,GETUTCDATE()
WHERE not exists(Select * from ZnodeActions where ControllerName='Dashboard' and ActionName='GetDashboardDetails')

 insert into ZnodeActionMenu (MenuId,ActionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardDetails') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardDetails'))

insert into ZnodeMenuActionsPermission(MenuId,ActionId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardDetails')	
,1,2,Getdate(),2,Getdate() where not exists (select * from ZnodeMenuActionsPermission where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardDetails'))

insert into ZnodeActions (Areaname,ControllerName,ActionName,IsGlobalAccess,CreatedBy, CreatedDate,ModifiedBy,ModifiedDate)
SELECT null,'Dashboard','GetDashboardOrders',0,2,GETUTCDATE(),2,GETUTCDATE()
WHERE not exists(Select * from ZnodeActions where ControllerName='Dashboard' and ActionName='GetDashboardOrders')

 insert into ZnodeActionMenu (MenuId,ActionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardOrders') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardOrders'))

insert into ZnodeMenuActionsPermission(MenuId,ActionId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardOrders')	
,1,2,Getdate(),2,Getdate() where not exists (select * from ZnodeMenuActionsPermission where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardOrders'))

insert into ZnodeActions (Areaname,ControllerName,ActionName,IsGlobalAccess,CreatedBy, CreatedDate,ModifiedBy,ModifiedDate)
SELECT null,'Dashboard','GetDashboardReturns',0,2,GETUTCDATE(),2,GETUTCDATE()
WHERE not exists(Select * from ZnodeActions where ControllerName='Dashboard' and ActionName='GetDashboardReturns')

 insert into ZnodeActionMenu (MenuId,ActionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardReturns') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardReturns'))

insert into ZnodeMenuActionsPermission(MenuId,ActionId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardReturns')	
,1,2,Getdate(),2,Getdate() where not exists (select * from ZnodeMenuActionsPermission where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardReturns'))

insert into ZnodeActions (Areaname,ControllerName,ActionName,IsGlobalAccess,CreatedBy, CreatedDate,ModifiedBy,ModifiedDate)
SELECT null,'Dashboard','GetDashboardQuotes',0,2,GETUTCDATE(),2,GETUTCDATE()
WHERE not exists(Select * from ZnodeActions where ControllerName='Dashboard' and ActionName='GetDashboardQuotes')

 insert into ZnodeActionMenu (MenuId,ActionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardQuotes') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardQuotes'))

insert into ZnodeMenuActionsPermission(MenuId,ActionId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardQuotes')	
,1,2,Getdate(),2,Getdate() where not exists (select * from ZnodeMenuActionsPermission where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardQuotes'))

insert into ZnodeActions (Areaname,ControllerName,ActionName,IsGlobalAccess,CreatedBy, CreatedDate,ModifiedBy,ModifiedDate)
SELECT null,'Dashboard','GetDashboardSaleDetails',0,2,GETUTCDATE(),2,GETUTCDATE()
WHERE not exists(Select * from ZnodeActions where ControllerName='Dashboard' and ActionName='GetDashboardSaleDetails')

 insert into ZnodeActionMenu (MenuId,ActionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardSaleDetails') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardSaleDetails'))

insert into ZnodeMenuActionsPermission(MenuId,ActionId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardSaleDetails')	
,1,2,Getdate(),2,Getdate() where not exists (select * from ZnodeMenuActionsPermission where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardSaleDetails'))

insert into ZnodeActions (Areaname,ControllerName,ActionName,IsGlobalAccess,CreatedBy, CreatedDate,ModifiedBy,ModifiedDate)
SELECT null,'Dashboard','GetDashboardSaleDetails',0,2,GETUTCDATE(),2,GETUTCDATE()
WHERE not exists(Select * from ZnodeActions where ControllerName='Dashboard' and ActionName='GetDashboardSaleDetails')

 insert into ZnodeActionMenu (MenuId,ActionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardSaleDetails') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardSaleDetails'))

insert into ZnodeMenuActionsPermission(MenuId,ActionId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardSaleDetails')	
,1,2,Getdate(),2,Getdate() where not exists (select * from ZnodeMenuActionsPermission where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardSaleDetails'))

insert into ZnodeActions (Areaname,ControllerName,ActionName,IsGlobalAccess,CreatedBy, CreatedDate,ModifiedBy,ModifiedDate)
SELECT null,'Dashboard','GetDashboardTopAccounts',0,2,GETUTCDATE(),2,GETUTCDATE()
WHERE not exists(Select * from ZnodeActions where ControllerName='Dashboard' and ActionName='GetDashboardTopAccounts')

 insert into ZnodeActionMenu (MenuId,ActionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardTopAccounts') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardTopAccounts'))

insert into ZnodeMenuActionsPermission(MenuId,ActionId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select 
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard')	
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardTopAccounts')	
,1,2,Getdate(),2,Getdate() where not exists (select * from ZnodeMenuActionsPermission where MenuId = 
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Dashboard' AND ControllerName = 'Dashboard') and ActionId = 
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Dashboard' and ActionName= 'GetDashboardTopAccounts'))


update ZnodeActions set IsGlobalAccess=0 where ActionName='CreateOrder'

update ZnodeActions set IsGlobalAccess=1 where ActionName = 'GetSuggestions' and ControllerName= 'Typeahead'


INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'SEO','DeleteSeoDetail',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'SEO' and ActionName = 'DeleteSeoDetail')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Products' AND ControllerName = 'SEO')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'SEO' and ActionName = 'DeleteSeoDetail') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Products' AND ControllerName = 'SEO') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'SEO' and ActionName = 'DeleteSeoDetail'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Products' AND ControllerName = 'SEO'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'SEO' and ActionName = 'DeleteSeoDetail')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Products' AND ControllerName = 'SEO') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'SEO' and ActionName = 'DeleteSeoDetail'))



--ZPD-15085
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'GiftCard','ActivateDeactivateVouchers',0,2,Getdate(),2,Getdate() 
WHERE NOT EXISTS(SELECT * FROM ZnodeActions WHERE ControllerName = 'GiftCard' and ActionName = 'ActivateDeactivateVouchers')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Vouchers' AND ControllerName = 'GiftCard'),
	(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GiftCard' and ActionName = 'ActivateDeactivateVouchers') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId =(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Vouchers' AND ControllerName = 'GiftCard') 
	AND ActionId =(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GiftCard' and ActionName = 'ActivateDeactivateVouchers'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Vouchers' AND ControllerName = 'GiftCard'),
	(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GiftCard' and ActionName = 'ActivateDeactivateVouchers'),1,2,Getdate(),2,Getdate() 
WHERE NOT EXISTS(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId =(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Vouchers' AND ControllerName = 'GiftCard') 
AND ActionId =(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GiftCard' and ActionName = 'ActivateDeactivateVouchers'))

--ZPD-15108
INSERT  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Account','GetParentAccountsList',0,2,Getdate(),2,Getdate() 
WHERE NOT EXISTS(SELECT * FROM ZnodeActions WHERE ControllerName = 'Account' and ActionName = 'GetParentAccountsList')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Accounts' AND ControllerName = 'Account')
  ,(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Account' and ActionName= 'GetParentAccountsList') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Accounts' AND ControllerName = 'Account') and ActionId =
   (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Account' and ActionName= 'GetParentAccountsList'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Accounts' AND ControllerName = 'Account') ,
	(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Account' and ActionName= 'GetParentAccountsList')
	,3,2,Getdate(),2,Getdate() 
WHERE NOT EXISTS(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Accounts' AND ControllerName = 'Account') and ActionId =
	(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Account' and ActionName= 'GetParentAccountsList'))
----------ZPD-14893
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Products','GetAssociatedBundleProducts',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Products' and ActionName = 'GetAssociatedBundleProducts')

 

INSERT INTO ZnodeMenuActionsPermission (MenuId,ActionId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select TOP 1 MenuId from ZnodeMenu where MenuName='PIM'), 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts'),1,2,getdate(),2,getdate()
where exists(select TOP 1 MenuId from ZnodeMenu where MenuName='PIM') and
exists(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts')
and not exists(select * from ZnodeMenuActionsPermission where MenuId = (select TOP 1 MenuId from ZnodeMenu where MenuName='PIM')
and ActionId =(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts'))



insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT (select TOP 1 MenuId from ZnodeMenu where MenuName='Products'), 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts'),1,2,getdate(),2,getdate()
where exists(select TOP 1 MenuId from ZnodeMenu where MenuName='Products') and
exists(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts')
and not exists(select * from ZnodeMenuActionsPermission where MenuId = (select TOP 1 MenuId from ZnodeMenu where MenuName='Products')
and ActionId =(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts'))




GO
--ZPD-15085
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'GiftCard','ActivateDeactivateVouchers',0,2,Getdate(),2,Getdate() 
WHERE NOT EXISTS(SELECT * FROM ZnodeActions WHERE ControllerName = 'GiftCard' and ActionName = 'ActivateDeactivateVouchers')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Vouchers' AND ControllerName = 'GiftCard'),
	(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GiftCard' and ActionName = 'ActivateDeactivateVouchers') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId =(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Vouchers' AND ControllerName = 'GiftCard') 
	AND ActionId =(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GiftCard' and ActionName = 'ActivateDeactivateVouchers'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Vouchers' AND ControllerName = 'GiftCard'),
	(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GiftCard' and ActionName = 'ActivateDeactivateVouchers'),1,2,Getdate(),2,Getdate() 
WHERE NOT EXISTS(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId =(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Vouchers' AND ControllerName = 'GiftCard') 
AND ActionId =(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GiftCard' and ActionName = 'ActivateDeactivateVouchers'))

--ZPD-15108
INSERT  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Account','GetParentAccountsList',0,2,Getdate(),2,Getdate() 
WHERE NOT EXISTS(SELECT * FROM ZnodeActions WHERE ControllerName = 'Account' and ActionName = 'GetParentAccountsList')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Accounts' AND ControllerName = 'Account')
  ,(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Account' and ActionName= 'GetParentAccountsList') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Accounts' AND ControllerName = 'Account') and ActionId =
   (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Account' and ActionName= 'GetParentAccountsList'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Accounts' AND ControllerName = 'Account') ,
	(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Account' and ActionName= 'GetParentAccountsList')
	,3,2,Getdate(),2,Getdate() 
WHERE NOT EXISTS(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Accounts' AND ControllerName = 'Account') and ActionId =
	(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Account' and ActionName= 'GetParentAccountsList'))


----------------ZPD-15239
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Customer','GetCountryBasedOnPortalId',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Customer' and ActionName = 'GetCountryBasedOnPortalId')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Users' AND ControllerName = 'Customer')
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Customer' and ActionName = 'GetCountryBasedOnPortalId') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId =
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Users' AND ControllerName = 'Customer') and ActionId =
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Customer' and ActionName = 'GetCountryBasedOnPortalId'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Users' AND ControllerName = 'Customer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Customer' and ActionName = 'GetCountryBasedOnPortalId')
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId =
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Users' AND ControllerName = 'Customer') and ActionId =
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Customer' and ActionName = 'GetCountryBasedOnPortalId'))
----------ZPD-14893
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Products','GetAssociatedBundleProducts',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Products' and ActionName = 'GetAssociatedBundleProducts')

 

INSERT INTO ZnodeMenuActionsPermission (MenuId,ActionId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select TOP 1 MenuId from ZnodeMenu where MenuName='PIM'), 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts'),1,2,getdate(),2,getdate()
where exists(select TOP 1 MenuId from ZnodeMenu where MenuName='PIM') and
exists(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts')
and not exists(select * from ZnodeMenuActionsPermission where MenuId = (select TOP 1 MenuId from ZnodeMenu where MenuName='PIM')
and ActionId =(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts'))



insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT (select TOP 1 MenuId from ZnodeMenu where MenuName='Products'), 
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts'),1,2,getdate(),2,getdate()
where exists(select TOP 1 MenuId from ZnodeMenu where MenuName='Products') and
exists(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts')
and not exists(select * from ZnodeMenuActionsPermission where MenuId = (select TOP 1 MenuId from ZnodeMenu where MenuName='Products')
and ActionId =(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Products' and ActionName= 'GetAssociatedBundleProducts'))




GO

--ZPD-16057
Update ZnodeActionMenu set MenuId = (select TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'General' and ControllerName = 'MyReports') 
where ActionId = (select top 1 ActionId from ZnodeActions where ControllerName='MyReports'and ActionName='List')

--ZPD-16284
Update ZnodeActionMenu set MenuId = (select TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'General' and ControllerName = 'MyReports') 
where ActionId = (select top 1 ActionId from ZnodeActions where ControllerName='MyReports'and ActionName='DynamicReport')
GO
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','SaveAssociatedVariantData',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'SaveAssociatedVariantData')

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','SaveAssociatedVariantData',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'SaveAssociatedVariantData')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'SaveAssociatedVariantData') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'SaveAssociatedVariantData'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'SaveAssociatedVariantData')	
,2,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'SaveAssociatedVariantData'))


INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','EditAssociatedVariant',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditAssociatedVariant')

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','EditAssociatedVariant',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditAssociatedVariant')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditAssociatedVariant') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditAssociatedVariant'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditAssociatedVariant')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditAssociatedVariant'))




INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','GetGlobalAttributesForDefaultData',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetGlobalAttributesForDefaultData')

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','GetGlobalAttributesForDefaultData',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetGlobalAttributesForDefaultData')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetGlobalAttributesForDefaultData') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetGlobalAttributesForDefaultData'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetGlobalAttributesForDefaultData')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetGlobalAttributesForDefaultData'))



INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','GetAssociatedVariantList',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetAssociatedVariantList')

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','GetAssociatedVariantList',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetAssociatedVariantList')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetAssociatedVariantList') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetAssociatedVariantList'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetAssociatedVariantList')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetAssociatedVariantList'))

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'TouchPointConfiguration','Recurring',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName = 'Recurring')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Site Search')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName = 'Recurring') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Site Search')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName = 'Recurring'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Site Search') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName = 'Recurring')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Site Search')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'TouchPointConfiguration' and ActionName = 'Recurring'))
GO

--ZPD-16622
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Attributes','ValidationRuleRegularExpression',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Attributes' and ActionName = 'ValidationRuleRegularExpression')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attributes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Attributes' and ActionName = 'ValidationRuleRegularExpression') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attributes')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'Attributes' and ActionName = 'ValidationRuleRegularExpression'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attributes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Attributes' and ActionName = 'ValidationRuleRegularExpression')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attributes')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'Attributes' and ActionName = 'ValidationRuleRegularExpression'))

-- ZPD-15479
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Order','FailedOrderTransactionList',1,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Order' and ActionName = 'FailedOrderTransactionList')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Users' AND ControllerName = 'Customer')
,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Order' and ActionName = 'FailedOrderTransactionList') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId =
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Users' AND ControllerName = 'Customer') and ActionId =
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Order' and ActionName = 'FailedOrderTransactionList'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Users' AND ControllerName = 'Customer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Order' and ActionName = 'FailedOrderTransactionList')
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId =
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Users' AND ControllerName = 'Customer') and ActionId =
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Order' and ActionName = 'FailedOrderTransactionList'))


--ZPD-16622
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Attributes','ValidationRuleRegularExpression',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Attributes' and ActionName = 'ValidationRuleRegularExpression')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attributes')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'Attributes' and ActionName = 'ValidationRuleRegularExpression') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attributes')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'Attributes' and ActionName = 'ValidationRuleRegularExpression'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attributes') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'Attributes' and ActionName = 'ValidationRuleRegularExpression')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Attributes')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'Attributes' and ActionName = 'ValidationRuleRegularExpression'))

update ZnodeMenu Set ParentMenuId = (select top 1 MenuId from ZnodeMenu WHERE MenuName = 'CMS' AND ControllerName = 'StoreExperience')
where MenuName = 'Containers' and CSSClassName = 'z-content-widgets' AND ControllerName = 'ContentContainer'

update ZnodeMenu 
Set ParentMenuId = (select top 1 MenuId from ZnodeMenu WHERE MenuName = 'Containers' AND ControllerName = 'ContentContainer'
					and ParentMenuId = (select top 1 MenuId from ZnodeMenu WHERE MenuName = 'CMS' AND ControllerName = 'StoreExperience'))
where MenuName = 'Content Containers' and CSSClassName = 'z-content-widgets' AND ControllerName = 'ContentContainer'

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContainerTemplate','Create',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Create')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Create') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Create'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Create')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Create'))


Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContainerTemplate','Edit',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Edit')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Edit') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Edit'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Edit')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Edit'))

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ContainerTemplate','Copy',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Copy')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates')
,(select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Copy') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Copy'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates') ,
(select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Copy')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select top 1 MenuId from ZnodeMenu where MenuName = 'Container Templates')
and ActionId = (select top 1 ActionId from ZnodeActions where ControllerName = 'ContainerTemplate' and ActionName = 'Copy'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','AssociateContainerTemplate',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateContainerTemplate')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateContainerTemplate') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateContainerTemplate'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateContainerTemplate')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'AssociateContainerTemplate'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','ActivateDeactivateVariant',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'ActivateDeactivateVariant')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'ActivateDeactivateVariant') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'ActivateDeactivateVariant'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'ActivateDeactivateVariant')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'ActivateDeactivateVariant'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','IsContainerExist',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'IsContainerExist')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'IsContainerExist') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'IsContainerExist'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'IsContainerExist')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'IsContainerExist'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','UpdateAndPublishContentContainer',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'UpdateAndPublishContentContainer')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'UpdateAndPublishContentContainer') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'UpdateAndPublishContentContainer'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'UpdateAndPublishContentContainer')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'UpdateAndPublishContentContainer'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','PublishContentContainer',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'PublishContentContainer')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'PublishContentContainer') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'PublishContentContainer'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'PublishContentContainer')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'PublishContentContainer'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','PublishContainerVariant',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'PublishContainerVariant')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'PublishContainerVariant') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'PublishContainerVariant'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'PublishContainerVariant')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'PublishContainerVariant'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','UpdateAndPublishContainerVariant',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'UpdateAndPublishContainerVariant')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'UpdateAndPublishContainerVariant') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'UpdateAndPublishContainerVariant'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'UpdateAndPublishContainerVariant')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'UpdateAndPublishContainerVariant'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','EditAssociatedVariant',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditAssociatedVariant')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditAssociatedVariant') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditAssociatedVariant'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditAssociatedVariant')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditAssociatedVariant'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContainerTemplate','List',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'List')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'List') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'List'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'List')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'List'))


insert INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Hangfire','Dashboard',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Hangfire' and ActionName = 'Dashboard')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance' AND ControllerName = 'Diagnostics')
  ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Hangfire' and ActionName = 'Dashboard') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId =
   (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance' AND ControllerName = 'Diagnostics') and ActionId =
   (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Hangfire' and ActionName = 'Dashboard'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance' AND ControllerName = 'Diagnostics'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Hangfire' and ActionName = 'Dashboard')
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId =
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Diagnostics & Maintenance' AND ControllerName = 'Diagnostics') and ActionId =
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Hangfire' and ActionName = 'Dashboard'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContainerTemplate','Delete',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'Delete')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'Delete') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'Delete'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'Delete')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'Delete'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContainerTemplate','DownloadWidgetTemplate',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'DownloadWidgetTemplate')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'DownloadWidgetTemplate') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'DownloadWidgetTemplate'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'DownloadWidgetTemplate')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'DownloadWidgetTemplate'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContainerTemplate','IsContainerTemplateExist',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'IsContainerTemplateExist')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'IsContainerTemplateExist') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'IsContainerTemplateExist'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'IsContainerTemplateExist')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Container Templates' AND ControllerName = 'ContainerTemplate') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContainerTemplate' and ActionName = 'IsContainerTemplateExist'))


INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','GetAttributesDataOnLocaleChange',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetAttributesDataOnLocaleChange')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetAttributesDataOnLocaleChange') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetAttributesDataOnLocaleChange'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetAttributesDataOnLocaleChange')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'GetAttributesDataOnLocaleChange'))


--ZPD-17089
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'BlogNews','ActivateDeactivateBlogNews',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'BlogNews' and ActionName = 'ActivateDeactivateBlogNews')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Blogs & News' AND ControllerName = 'BlogNews')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'BlogNews' and ActionName = 'ActivateDeactivateBlogNews') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Blogs & News' AND ControllerName = 'BlogNews') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'BlogNews' and ActionName = 'ActivateDeactivateBlogNews'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Blogs & News' AND ControllerName = 'BlogNews'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'BlogNews' and ActionName = 'ActivateDeactivateBlogNews')	
,2,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Blogs & News' AND ControllerName = 'BlogNews') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'BlogNews' and ActionName = 'ActivateDeactivateBlogNews'))

-- dt 16/02/2022 ZPD-17179 --> ZPD-17781
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'WebSite','ManageCMSContentContainerWidget',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'ManageCMSContentContainerWidget')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'ManageCMSContentContainerWidget') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'ManageCMSContentContainerWidget'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'ManageCMSContentContainerWidget')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'ManageCMSContentContainerWidget'))
								
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'WebSite','SaveContainerDetails',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'SaveContainerDetails')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'SaveContainerDetails') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'SaveContainerDetails'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'SaveContainerDetails')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'SaveContainerDetails'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'WebSite','RemoveWidgetDataFromContentPage',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'RemoveWidgetDataFromContentPage')

INSERT INTO ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience')	
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'RemoveWidgetDataFromContentPage') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'RemoveWidgetDataFromContentPage'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy ,CreatedDate,	ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'RemoveWidgetDataFromContentPage')	
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Store Experience' AND ControllerName = 'StoreExperience') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'WebSite' and ActionName = 'RemoveWidgetDataFromContentPage'))

--ZPD-16954
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Order','ReorderCompleteOrder',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Order' and ActionName = 'ReorderCompleteOrder')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'OMS' AND ControllerName = 'Order')
   ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Order' and ActionName = 'ReorderCompleteOrder') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId =
    (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'OMS' AND ControllerName = 'Order') and ActionId =
    (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Order' and ActionName = 'ReorderCompleteOrder'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'OMS' AND ControllerName = 'Order'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Order' and ActionName = 'ReorderCompleteOrder')
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId =
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'OMS' AND ControllerName = 'Order') and ActionId =
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Order' and ActionName = 'ReorderCompleteOrder'))


----ZPD-16959-Dt-23March-2022
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Store','EditStoreKlaviyo',1,2,Getdate(),2,Getdate()
where not exists(select * from ZnodeActions where ControllerName = 'Store' and ActionName = 'EditStoreKlaviyo')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditStoreKlaviyo') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditStoreKlaviyo'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditStoreKlaviyo')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditStoreKlaviyo'))

--ZPD-18615
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Store','EditStoreSMSNotification',1,2,Getdate(),2,Getdate()
where not exists(select * from ZnodeActions where ControllerName = 'Store' and ActionName = 'EditStoreSMSNotification')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditStoreSMSNotification') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditStoreSMSNotification'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditStoreSMSNotification')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'EditStoreSMSNotification'))


---ZPD-17149
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Store','GetAssociatedInvoiceManagementPaymentList',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'GetAssociatedInvoiceManagementPaymentList')

INSERT INTO ZnodeActionMenu ( MenuId,        ActionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store')        
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'GetAssociatedInvoiceManagementPaymentList') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'GetAssociatedInvoiceManagementPaymentList'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,        ActionId, AccessPermissionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'GetAssociatedInvoiceManagementPaymentList')        
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'GetAssociatedInvoiceManagementPaymentList'))

---ZPD-16959
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Store','EditStoreKlaviyo',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'EditStoreKlaviyo')

INSERT INTO ZnodeActionMenu ( MenuId,        ActionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store')        
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'EditStoreKlaviyo') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'EditStoreKlaviyo'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,        ActionId, AccessPermissionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'EditStoreKlaviyo')        
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'EditStoreKlaviyo'))


INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','CreateScheduler',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'CreateScheduler')

INSERT INTO ZnodeActionMenu ( MenuId,        ActionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer')        
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'CreateScheduler') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'CreateScheduler'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,        ActionId, AccessPermissionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'CreateScheduler')        
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'CreateScheduler'))


INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'ContentContainer','EditScheduler',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditScheduler')

INSERT INTO ZnodeActionMenu ( MenuId,        ActionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer')        
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditScheduler') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditScheduler'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,        ActionId, AccessPermissionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditScheduler')        
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Content Containers' AND ControllerName = 'ContentContainer') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'ContentContainer' and ActionName = 'EditScheduler'))

--ZPD-18881
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Store','GetProviderTypeForm',1,2,Getdate(),2,Getdate()
where not exists(select * from ZnodeActions where ControllerName = 'Store' and ActionName = 'GetProviderTypeForm')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'GetProviderTypeForm') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'GetProviderTypeForm'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'GetProviderTypeForm')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'GetProviderTypeForm'))

--ZPD-18908
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Store','GetUnassociatedPaymentListForInvoice',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'GetUnassociatedPaymentListForInvoice')

INSERT INTO ZnodeActionMenu ( MenuId,        ActionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store')        
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'GetUnassociatedPaymentListForInvoice') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'GetUnassociatedPaymentListForInvoice'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,        ActionId, AccessPermissionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'GetUnassociatedPaymentListForInvoice')        
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Stores' AND ControllerName = 'Store') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Store' and ActionName = 'GetUnassociatedPaymentListForInvoice'))

--ZPD-18605
update ZnodeMenuActionsPermission set MenuId=(select Top 1 MenuId from ZnodeMenu where MenuName='Orders') where ActionId=(select Top 1 ActionId from ZnodeActions where ActionName='FailedOrderTransactionList' AND ControllerName='Customer');
update ZnodeActionMenu set MenuId= (select Top 1 MenuId from ZnodeMenu where MenuName='Orders') where ActionId=(select Top 1 ActionId from ZnodeActions where ActionName='FailedOrderTransactionList' AND ControllerName='Customer');
update ZnodeActions set ControllerName='Order' where ActionName='FailedOrderTransactionList' AND ControllerName='Customer';

--ZPD-19176
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'ProductFeed','IsFileNameExist',1,2,Getdate(),2,Getdate()
where not exists(select * from ZnodeActions where ControllerName = 'ProductFeed' and ActionName = 'IsFileNameExist')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Product Feeds' AND ControllerName = 'ProductFeed')
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductFeed' and ActionName= 'IsFileNameExist') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Product Feeds' AND ControllerName = 'ProductFeed') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductFeed' and ActionName= 'IsFileNameExist'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Product Feeds' AND ControllerName = 'ProductFeed')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductFeed' and ActionName= 'IsFileNameExist')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Product Feeds' AND ControllerName = 'ProductFeed') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'ProductFeed' and ActionName= 'IsFileNameExist'))

--ZPD-19459
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Store','AssociateOfflinePaymentSetting',1,2,Getdate(),2,Getdate()
where not exists(select * from ZnodeActions where ControllerName = 'Store' and ActionName = 'AssociateOfflinePaymentSetting')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'AssociateOfflinePaymentSetting') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'AssociateOfflinePaymentSetting'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'AssociateOfflinePaymentSetting')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Stores' AND ControllerName = 'Store') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Store' and ActionName= 'AssociateOfflinePaymentSetting'))

---ZPD-19494
INSERT INTO ZnodeActions ( AreaName, ControllerName, ActionName, IsGlobalAccess, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) 
SELECT
NULL, 'SearchConfiguration', 'IsSynonymCodeExists', 0,2,Getdate(), 2, Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'SearchConfiguration' AND ActionName = 'IsSynonymCodeExists')

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
SELECT
(SELECT top 1 MenuId FROM ZnodeMenu WHERE MenuName='Site Search' AND ControllerName = 'SearchReport'),
(SELECT top 1 ActionId FROM ZnodeActions WHERE ControllerName='SearchConfiguration' AND  ActionName='IsSynonymCodeExists'),
1,2,Getdate(), 2, Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId =
(SELECT top 1 MenuId FROM ZnodeMenu WHERE MenuName='Site Search'AND ControllerName = 'SearchReport') AND ActionId =
(SELECT top 1 ActionId FROM ZnodeActions WHERE ControllerName='SearchConfiguration' AND  ActionName='IsSynonymCodeExists'))

INSERT INTO ZnodeActionMenu (MenuId, ActionId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) 
SELECT
(SELECT top 1 MenuId FROM ZnodeMenu WHERE MenuName='Site Search'AND ControllerName = 'SearchReport'),
(SELECT top 1 ActionId FROM ZnodeActions WHERE ControllerName='SearchConfiguration' AND  ActionName='IsSynonymCodeExists'),
2,Getdate(), 2, Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActionMenu WHERE MenuId =
(SELECT top 1 MenuId FROM ZnodeMenu WHERE MenuName='Site Search') AND ActionId =
(SELECT top 1 ActionId FROM ZnodeActions WHERE ControllerName='SearchConfiguration' AND  ActionName='IsSynonymCodeExists'))

--ZPD-19264
update ZnodeMenuActionsPermission set MenuId=(select Top 1 MenuId from ZnodeMenu where MenuName='Orders')
where ActionId=(select Top 1 ActionId from ZnodeActions where ActionName='FailedOrderTransactionList');

update ZnodeActionMenu set MenuId= (select Top 1 MenuId from ZnodeMenu where MenuName='Orders') 
where ActionId=(select Top 1 ActionId from ZnodeActions where ActionName='FailedOrderTransactionList');

--ZPD-20233 --14/6/22
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'GeneralSetting','GetStockNoticeSettings',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'GeneralSetting' and ActionName = 'GetStockNoticeSettings')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting')
   ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GeneralSetting' and ActionName = 'GetStockNoticeSettings') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId =
    (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId =
    (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GeneralSetting' and ActionName = 'GetStockNoticeSettings'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GeneralSetting' and ActionName = 'GetStockNoticeSettings')
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId =
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId =
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'GeneralSetting' and ActionName = 'GetStockNoticeSettings'))

--ZPD--19402
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','ReorderCompleteOrder',1,2,Getdate(),2,Getdate()
where not exists(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'ReorderCompleteOrder')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'ORDERS' AND ControllerName = 'Order')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ReorderCompleteOrder') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'ORDERS' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ReorderCompleteOrder'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'ORDERS' AND ControllerName = 'Order')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ReorderCompleteOrder')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'ORDERS' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'ReorderCompleteOrder'))

--ZPD-19301

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Customer','UpdateUsernameForRegisteredUser',1,2,Getdate(),2,Getdate()
where not exists(select * from ZnodeActions where ControllerName = 'Customer' and ActionName = 'UpdateUsernameForRegisteredUser')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
 (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer')
    ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'UpdateUsernameForRegisteredUser') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
     (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'UpdateUsernameForRegisteredUser'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'UpdateUsernameForRegisteredUser')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Users' AND ControllerName = 'Customer') and ActionId =
     (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Customer' and ActionName= 'UpdateUsernameForRegisteredUser'))

INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Search','PublishSearchProfile',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Search' and ActionName = 'PublishSearchProfile')

INSERT INTO ZnodeActionMenu ( MenuId,        ActionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'SITE SEARCH' AND ControllerName = 'SearchReport')        
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Search' and ActionName = 'PublishSearchProfile') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'SITE SEARCH' AND ControllerName = 'SearchReport') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Search' and ActionName = 'PublishSearchProfile'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,        ActionId, AccessPermissionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'SITE SEARCH' AND ControllerName = 'SearchReport'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Search' and ActionName = 'PublishSearchProfile')        
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'SITE SEARCH' AND ControllerName = 'SearchReport') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Search' and ActionName = 'PublishSearchProfile'))


INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Search','GetUnassociatedCatalogList',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Search' and ActionName = 'GetUnassociatedCatalogList')

INSERT INTO ZnodeActionMenu ( MenuId,        ActionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'SITE SEARCH' AND ControllerName = 'SearchReport')        
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Search' and ActionName = 'GetUnassociatedCatalogList') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'SITE SEARCH' AND ControllerName = 'SearchReport') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Search' and ActionName = 'GetUnassociatedCatalogList'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,        ActionId, AccessPermissionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'SITE SEARCH' AND ControllerName = 'SearchReport'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Search' and ActionName = 'GetUnassociatedCatalogList')        
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'SITE SEARCH' AND ControllerName = 'SearchReport') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Search' and ActionName = 'GetUnassociatedCatalogList'))


-- ZPD-20919 Dt-11-July-2022
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetAuthorizeNetToken',1,2,Getdate(),2,Getdate()
where not exists(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetAuthorizeNetToken')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAuthorizeNetToken') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAuthorizeNetToken'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAuthorizeNetToken')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetAuthorizeNetToken'))

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Order','GetPaymentGatewayToken',1,2,Getdate(),2,Getdate()
where not exists(select * from ZnodeActions where ControllerName = 'Order' and ActionName = 'GetPaymentGatewayToken')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPaymentGatewayToken') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPaymentGatewayToken'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order')
,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPaymentGatewayToken')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Orders' AND ControllerName = 'Order') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Order' and ActionName= 'GetPaymentGatewayToken'))

 --dt\13\07\2022 ZPD-19817
 INSERT INTO [dbo].[ZnodeActions]
    ([AreaName],[ControllerName],[ActionName],[IsGlobalAccess],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate])
SELECT NULL,'GiftCard','ActiveDeactiveSingleVoucher',1,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeActions WHERE ControllerName='GiftCard' AND ActionName='ActiveDeactiveSingleVoucher')

 --dt\12\09\2022 ZPD-19702
Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Import','GetPromotionTypeList',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Import' and ActionName = 'GetPromotionTypeList')

insert into ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Import' AND ControllerName = 'Import')
   ,(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Import' and ActionName= 'GetPromotionTypeList') ,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId =
    (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Import' AND ControllerName = 'Import') and ActionId =
    (select TOP 1 ActionId from ZnodeActions where ControllerName = 'Import' and ActionName= 'GetPromotionTypeList'))

insert into ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
select
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Import' AND ControllerName = 'Import'),
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Import' and ActionName= 'GetPromotionTypeList')
,1,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeMenuActionsPermission where MenuId =
(select TOP 1 MenuId from ZnodeMenu where MenuName = 'Import' AND ControllerName = 'Import') and ActionId =
(select TOP 1 ActionId from ZnodeActions where ControllerName = 'Import' and ActionName= 'GetPromotionTypeList'))

 --dt\13\09\2022 ZPD-22290
INSERT INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Export','List',0,2,Getdate(),2,Getdate() WHERE NOT EXISTS
(SELECT * FROM ZnodeActions WHERE ControllerName = 'Export' and ActionName = 'List')

INSERT INTO ZnodeActionMenu ( MenuId,        ActionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
 (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Export' AND ControllerName = 'Export')        
    ,(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Export' and ActionName = 'List') ,2,Getdate(),2,Getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId = 
     (SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Export' AND ControllerName = 'Export') and ActionId = 
     (SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Export' and ActionName = 'List'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId,        ActionId, AccessPermissionId,        CreatedBy ,CreatedDate,        ModifiedBy, ModifiedDate )
SELECT 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Export' AND ControllerName = 'Export'),
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Export' and ActionName = 'List')        
,1,2,Getdate(),2,Getdate() WHERE NOT EXISTS 
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId from ZnodeMenu WHERE MenuName = 'Export' AND ControllerName = 'Export') and ActionId = 
(SELECT TOP 1 ActionId from ZnodeActions WHERE ControllerName = 'Export' and ActionName = 'List'))

--dt\21\09\2022 ZPD-19118
INSERT INTO ZnodeActions(ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
    SELECT 'Order','GetIframeViewWithToken',1,2,GETDATE(),2,GETDATE()
    WHERE NOT EXISTS(SELECT 1 FROM ZnodeActions WHERE ControllerName='Order' AND ActionName='GetIframeViewWithToken')

--dt\28\09\2022 ZPD-22597
INSERT  INTO ZnodeActions(AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT NULL ,'Import','DownloadPDF',1,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS(SELECT * FROM ZnodeActions WHERE ControllerName = 'Import' and ActionName = 'DownloadPDF')

INSERT INTO ZnodeActionMenu ( MenuId, ActionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Import' AND ControllerName = 'Import')
,(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Import' and ActionName= 'DownloadPDF') ,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeActionMenu WHERE MenuId =
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Import' AND ControllerName = 'Import') and ActionId =
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Import' and ActionName= 'DownloadPDF'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Import' AND ControllerName = 'Import')
,(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Import' and ActionName= 'DownloadPDF')
,1,2,GETDATE(),2,GETDATE() WHERE not exists
(SELECT * FROM ZnodeMenuActionsPermission WHERE MenuId =
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Import' AND ControllerName = 'Import') and ActionId =
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Import' and ActionName= 'DownloadPDF'))

--Dt-11-Oct-2022 ZPD-21319
DELETE FROM ZnodeActionMenu
WHERE ActionId = (SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Order' AND ActionName= 'CheckoutReceipt')
AND MenuId = (SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName='OMS' AND ParentMenuId IS NULL)
 
--Dt 22 Nov 2022 ZPD-22969
DELETE FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Analytics' AND ControllerName = 'Analytics') and ActionId = 
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Analytics' and ActionName= 'AnalyticsDashboard')

DELETE FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'GeneralSetting' and ActionName= 'GetAnalyticsData')

DELETE FROM ZnodeMenuActionsPermission WHERE MenuId = 
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'GeneralSetting' and ActionName= 'UpdateAnalyticsData')

DELETE FROM ZnodeActionMenu WHERE MenuId = 
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Analytics' AND ControllerName = 'Analytics') and ActionId = 
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Analytics' and ActionName= 'AnalyticsDashboard')

DELETE FROM ZnodeActionMenu WHERE MenuId = 
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'GeneralSetting' and ActionName= 'GetAnalyticsData')

DELETE FROM ZnodeActionMenu WHERE MenuId = 
(SELECT TOP 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Global Settings' AND ControllerName = 'GeneralSetting') and ActionId = 
(SELECT TOP 1 ActionId FROM ZnodeActions WHERE ControllerName = 'GeneralSetting' and ActionName= 'UpdateAnalyticsData')

DELETE FROM ZnodeActions WHERE ControllerName = 'Analytics' and ActionName = 'AnalyticsDashboard'

DELETE FROM ZnodeActions WHERE ControllerName = 'GeneralSetting' and ActionName = 'GetAnalyticsData'

DELETE FROM ZnodeActions WHERE ControllerName = 'GeneralSetting' and ActionName = 'UpdateAnalyticsData'

DELETE  from ZnodeRoleMenuAccessMapper where RoleMenuId in ( select RoleMenuId from ZnodeRoleMenu WHERE MenuId IN (SELECT MenuId FROM ZnodeMenu WHERE MenuName = 'Analytics') )

DELETE  FROM ZnodeRoleMenu WHERE MenuId IN (SELECT MenuId FROM ZnodeMenu WHERE MenuName = 'Analytics')

DELETE FROM ZnodeMenu WHERE MenuName = 'Analytics' AND ControllerName = 'Analytics'

--dt\29\11\2022 ZPD-20700
INSERT INTO ZnodeActions(ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
    SELECT 'Import','ManageCustomImportTemplateList',1,2,GETDATE(),2,GETDATE()
    WHERE NOT EXISTS(SELECT 1 FROM ZnodeActions WHERE ControllerName='Import' AND ActionName='ManageCustomImportTemplateList')

--dt\29\11\2022 ZPD-20700
INSERT INTO ZnodeActions(ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
    SELECT 'Import','DeleteImportTemplate',1,2,GETDATE(),2,GETDATE()
    WHERE NOT EXISTS(SELECT 1 FROM ZnodeActions WHERE ControllerName='Import' AND ActionName='DeleteImportTemplate')


--dt\09\12\2022 ZPD-23587

INSERT  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
    SELECT NULL ,'Export','DownloadExportFile',0,2,Getdate(),2,Getdate() 
    WHERE NOT EXISTS (SELECT 1 from ZnodeActions where ControllerName = 'Export' and ActionName = 'DownloadExportFile')

INSERT  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
    SELECT NULL ,'Export','Deletelogs',0,2,Getdate(),2,Getdate() 
    WHERE NOT EXISTS (SELECT 1 from ZnodeActions where ControllerName = 'Export' and ActionName = 'Deletelogs')


--dt\13\12\2022 ZPD-23587

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT top 1 MenuId   FROM ZnodeMenu    WHERE MenuName = 'Export') ,
(SELECT top 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Export' and ActionName = 'DownloadExportFile')
,1,2,Getdate(),2,Getdate() 
WHERE NOT EXISTS
(SELECT * from ZnodeMenuActionsPermission WHERE MenuId =
(SELECT top 1 MenuId from ZnodeMenu WHERE MenuName = 'Export') and ActionId =
(SELECT top 1 ActionId from ZnodeActions WHERE ControllerName = 'Export' and ActionName = 'DownloadExportFile'))

INSERT INTO ZnodeMenuActionsPermission ( MenuId, ActionId, AccessPermissionId, CreatedBy ,CreatedDate, ModifiedBy, ModifiedDate )
SELECT
(SELECT top 1 MenuId   FROM ZnodeMenu    WHERE MenuName = 'Export') ,
(SELECT top 1 ActionId FROM ZnodeActions WHERE ControllerName = 'Export' and ActionName = 'Deletelogs')
,1,2,Getdate(),2,Getdate() 
WHERE NOT EXISTS
(SELECT * from ZnodeMenuActionsPermission WHERE MenuId =
(SELECT top 1 MenuId from ZnodeMenu WHERE MenuName = 'Export') and ActionId =
(SELECT top 1 ActionId from ZnodeActions WHERE ControllerName = 'Export' and ActionName = 'Deletelogs'))
