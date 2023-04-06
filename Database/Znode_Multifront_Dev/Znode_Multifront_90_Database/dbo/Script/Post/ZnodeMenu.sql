
---dt\14\10\2019  ZPD-7646
INSERT INTO ZnodeActionMenu (MenuId,ActionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select TOP 1 MenuId from ZnodeMenu where MenuName='Application Logs'), 
(select TOP 1 ActionId from ZnodeActions where ActionName = 'ImpersonationLogList'),2,getdate(),2,getdate()
where exists(select TOP 1 MenuId from ZnodeMenu where MenuName='Application Logs') and
exists(select TOP 1 ActionId from ZnodeActions where ActionName = 'ImpersonationLogList')
and not exists(select * from ZnodeActionMenu where MenuId = (select TOP 1 MenuId from ZnodeMenu where MenuName='Application Logs')
and ActionId =(select TOP 1 ActionId from ZnodeActions where ActionName = 'ImpersonationLogList'))

INSERT INTO ZnodeMenuActionsPermission (MenuId,ActionId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select TOP 1 MenuId from ZnodeMenu where MenuName='Application Logs'), 
(select TOP 1 ActionId from ZnodeActions where ActionName = 'ImpersonationLogList'),1,2,getdate(),2,getdate()
where exists(select TOP 1 MenuId from ZnodeMenu where MenuName='Application Logs') and
exists(select TOP 1 ActionId from ZnodeActions where ActionName = 'ImpersonationLogList')
and not exists(select * from ZnodeMenuActionsPermission where MenuId = (select TOP 1 MenuId from ZnodeMenu where MenuName='Application Logs')
and ActionId =(select TOP 1 ActionId from ZnodeActions where ActionName = 'ImpersonationLogList'))

--dt 14-01-2020 ZPD-8660
update ZnodeMenu set MenuName = 'DAM'
where MenuName='Media Library'

--dt 14-01-2020 ZPD-8661
update ZnodeMenu set MenuName = 'OMS'
where MenuName='Customer Service'

--dt 27-01-2020 ZPD-7813 --> ZPD-8907
insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'Reports'),'Analytics',3,null,'Analytics','AnalyticsDashboard','z-analytics-report',1,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMenu where MenuName = 'Analytics')

--dt 12-05-2020 ZPD-9739 --> ZPD-10176
insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'OMS'),'Quotes',1,null,'Quote','QuoteList','z-quote',1,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMenu where MenuName = 'Quotes')

--dt 01-06-2020 ZPD-10454
Update ZnodeMenu set ControllerName='StoreExperience',ActionName='list' where MenuName='CMS' 

--dt 02-07-2020 --> ZPD-10386
Update ZnodeMenu set MenuName ='Diagnostics & Maintenance' 
where ControllerName ='Diagnostics' and ActionName='Index' and AreaName ='Diagnostics'

--dt 07-07-2020 ZPD-11316
update ZnodeMenu set MenuName = 'BI Reports' where menuName = 'PowerBI' and ControllerName = 'PowerBI'

update  ZnodeMenu set MenuSequence = 4  where MenuName = 'Analytics' and ControllerName = 'Analytics'
update  ZnodeMenu set MenuSequence = 3  where MenuName = 'BI Reports' and ControllerName = 'PowerBI'

--dt 25-08-2020
insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'),'Attribute Families',3,null,'GlobalAttributeFamily','List','z-attribute-family',1,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMenu where MenuName = 'Attribute Families'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Global Attributes'))

update ZnodeMenu set IsActive = 0 where MenuName = 'Entity Attributes'

--dt 21-09-2020 ZPD-11573 --> ZPD-12160
update ZnodeMenu set CSSClassName = 'z-analytics' where MenuName='Analytics' and CSSClassName = 'z-analytics-report'


UPDATE Znodemenu SET MenuName = 'Content Containers',ControllerName = 'ContentContainer' WHERE MenuName = 'Content Widgets' AND CSSClassName = 'z-content-widgets'
UPDATE Znodemenu SET MenuName = 'Containers',ControllerName = 'ContentContainer' WHERE MenuName = 'Containers' AND CSSClassName = 'z-content-widgets'
UPDATE Znodemenu SET MenuName = 'Container Templates',ControllerName = 'ContainerTemplate' WHERE MenuName = 'Widget Templates' AND CSSClassName = 'z-widget-templates'

Update Znodemenu SET MenuName = 'Containers',ControllerName = 'ContentContainer' where MenuName = 'Widgets' AND CSSClassName = 'z-content-widgets'
and MenuId = (select min(MenuId) from Znodemenu where MenuName = 'Widgets' AND CSSClassName = 'z-content-widgets')

Update Znodemenu SET MenuName = 'Content Containers',ControllerName = 'ContentContainer' where MenuName = 'Widgets' AND CSSClassName = 'z-content-widgets'
and MenuId = (select max(MenuId) from Znodemenu where MenuName = 'Widgets' AND CSSClassName = 'z-content-widgets')


--dt 28-09-2020

insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'),'Containers',3,null,'ContentWidget','List','z-content-widgets',1,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMenu where MenuName = 'Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS'))

insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')),
'Content Containers',3,null,'ContentWidget','List','z-content-widgets',1,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMenu where MenuName = 'Content Containers'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))

insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')),
'Container Templates',2,null,'WidgetTemplate','List','z-widget-templates',1,
2,getdate(),2,getdate()
where not exists(select * from ZnodeMenu where MenuName = 'Container Templates'
and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId = ( select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')))

--dt 13-10-2020
update ZnodeMenu set MenuSequence = 2 where ParentMenuId = (select Top 1 MenuId from  ZnodeMenu where MenuName = 'CMS') and MenuName = 'Containers'

-- dt 22-Dec-2021
insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =
( select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')),
'Content Containers',2,null,'ContentContainer','List','z-widget-templates',1,
2,getdate(),2,getdate()
where NOT EXISTS (SELECT * FROM ZnodeMenu where MenuName='Content Containers')

insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =
( select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')),
'Content Containers',2,null,'ContentContainer','List','z-widget-templates',1,
2,getdate(),2,getdate()
where NOT EXISTS (SELECT * FROM ZnodeMenu where MenuName='Content Containers')

insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =
( select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')),
'Container Templates',2,null,'ContainerTemplate','List','z-widget-templates',1,
2,getdate(),2,getdate()
where NOT EXISTS (SELECT * FROM ZnodeMenu where MenuName='Container Templates')

insert into ZnodeMenu(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select Top 1 MenuId from ZnodeMenu where MenuName = 'Containers' and ParentMenuId =
( select Top 1 MenuId from ZnodeMenu where MenuName = 'CMS')),
'Containers',2,null,'ContentContainer','List','z-widget-templates',1,
2,getdate(),2,getdate()
where NOT EXISTS (SELECT * FROM ZnodeMenu where MenuName='Containers')

--30-Dec-2021
UPDATE Znodemenu SET MenuName = 'Content Containers',ControllerName = 'ContentContainer' WHERE MenuName = 'Content Widgets' AND CSSClassName = 'z-content-widgets'
UPDATE Znodemenu SET MenuName = 'Content Containers',ControllerName = 'ContentContainer' WHERE MenuName = 'Content Containers' AND CSSClassName = 'z-content-widgets'
UPDATE Znodemenu SET MenuName = 'Containers',ControllerName = 'ContentContainer' WHERE MenuName = 'Containers' AND CSSClassName = 'z-content-widgets'
UPDATE Znodemenu SET MenuName = 'Container Templates',ControllerName = 'ContainerTemplate' WHERE MenuName = 'Widget Templates' AND CSSClassName = 'z-widget-templates'
UPDATE Znodemenu SET MenuName = 'Container Templates',ControllerName = 'ContainerTemplate' WHERE MenuName = 'Container Templates' AND CSSClassName = 'z-widget-templates'

-- ZPD-17961 Dt-21-Feb-2022
UPDATE ZnodeMenu
SET MenuName='Commerce Connector'
WHERE MenuId=(SELECT MenuId FROM ZnodeMenu WHERE MenuName = 'Extension Engine');
--22-Feb-20222 ZPD-12653
update ZnodeMenu set MenuName = 'Email & SMS Templates' where MenuName = 'Email Templates'

--ZPD-21101 Dt-21-July-2022
INSERT INTO ZnodeMenu
	(ParentMenuId,MenuName,MenuSequence,AreaName,ControllerName,ActionName,CSSClassName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
	( select Top 1 MenuId FROM ZnodeMenu WHERE MenuName = 'Dev Center'),
'Export',13,NULL,'Export','List','z-import-export',1,
2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT * FROM ZnodeMenu WHERE MenuName='Export')


