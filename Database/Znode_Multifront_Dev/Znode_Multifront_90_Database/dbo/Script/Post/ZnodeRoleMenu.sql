

--dt\27\09\2019  B2B


INSERT INTO ASpNetRoles(Id,Name,IsActive,TypeOfRole,IsSystemDefined,IsDefault,
CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NEwID(),'Sales Rep',1,NULL,1,0,2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ASpNetRoles WHERE Name = 'Sales Rep')

GO


INSERT INTO [ZnodeRoleMenu] ( [RoleId], [MenuId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate])
SELECT (select Id FROM AspNetRoles WHERE NAMe = 'Sales Rep') ,MenuId ,2,GETDATE(),2,GETDATE()
FROM ZnodeRoleMenu RM
WHERE RoleId = (SELECT TOP 1 Id FROM AspNetRoles NR WHERE Name = 'Admin' AND RM.RoleId = NR.Id)
AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeRoleMenu ZM  
WHERE MenuId = (select TOP 1 MenuId from ZnodeMenu where ZM.MenuId = ZnodeMenu.MenuId) 
AND RoleId = (select TOP 1 Id FROM AspNetRoles RR WHERE Name = 'Sales Rep' AND RR.Id = ZM.RoleId ))


INSERT INTO ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Id FROM AspNetRoles WHERE NAMe = 'Sales Rep'),(select MenuId from ZnodeMenu where MenuName = 'Sales Reps' AND ControllerName = 'SalesRep')
,2,getdate(),2,getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeRoleMenu WHERE MenuId = (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Sales Reps' AND ControllerName = 'SalesRep')
AND RoleId = (select TOP 1 Id FROM AspNetRoles WHERE NAMe = 'Sales Rep'))


INSERT INTO ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT (select Id FROM AspNetRoles WHERE NAMe = 'Admin'),(select MenuId from ZnodeMenu where MenuName = 'Sales Reps' AND ControllerName = 'SalesRep')
,2,getdate(),2,getdate()
WHERE NOT EXISTS (SELECT * FROM ZnodeRoleMenu WHERE MenuId = (select TOP 1 MenuId from ZnodeMenu where MenuName = 'Sales Reps' AND ControllerName = 'SalesRep')
AND RoleId = (select TOP 1 Id FROM AspNetRoles WHERE NAMe = 'Admin'))

GO


IF NOT EXISTS (SELECT * FROM ZnodeRoleMenuAccessMapper MAM
WHERE RoleMenuId = (SELECT TOP 1 RoleMenuId 
FROM ZnodeRoleMenu RMM
WHERE MenuId = (select TOP 1 MenuId from ZnodeMenu where RMM.MenuId = ZnodeMenu.MenuId)
AND RoleId = (SELECT TOP 1 RoleId FROM AspNetRoles WHERE Name = 'Sales Rep' and AspNetRoles.Id = RMM.RoleId )
AND MAM.RoleMenuId = RMM.RoleMenuId))
BEGIN

INSERT INTO ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT RoleMenuId ,1,2,GETDATE(),2,GETDATE()
FROM ZnodeRoleMenu RM
WHERE MenuId = (select TOP 1 MenuId from ZnodeMenu where RM.MenuId = ZnodeMenu.MenuId)
AND RoleId = (SELECT TOP 1 RoleId FROM AspNetRoles WHERE Name = 'Sales Rep' and AspNetRoles.Id = RM.RoleId )
--AND NOT EXISTS (SELECT TOP  1 1 FROM ZnodeRoleMenuAccessMapper MAM
--WHERE RoleMenuId = (SELECT TOP 1 RoleMenuId 
--FROM ZnodeRoleMenu RMM
--WHERE MenuId = (select TOP 1 MenuId from ZnodeMenu where RMM.MenuId = ZnodeMenu.MenuId)
--AND RoleId = (SELECT TOP 1 RoleId FROM AspNetRoles WHERE Name = 'Sales Rep' and AspNetRoles.Id = RMM.RoleId )
--AND MAM.RoleMenuId = RMM.RoleMenuId))

INSERT INTO ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT RoleMenuId ,2,2,GETDATE(),2,GETDATE()
FROM ZnodeRoleMenu RM
WHERE MenuId = (select TOP 1 MenuId from ZnodeMenu where RM.MenuId = ZnodeMenu.MenuId)
AND RoleId = (SELECT TOP 1 RoleId FROM AspNetRoles WHERE Name = 'Sales Rep' and AspNetRoles.Id = RM.RoleId )

INSERT INTO ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT RoleMenuId ,3,2,GETDATE(),2,GETDATE()
FROM ZnodeRoleMenu RM
WHERE MenuId = (select TOP 1 MenuId from ZnodeMenu where RM.MenuId = ZnodeMenu.MenuId)
AND RoleId = (SELECT TOP 1 RoleId FROM AspNetRoles WHERE Name = 'Sales Rep' and AspNetRoles.Id = RM.RoleId )

INSERT INTO ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT RoleMenuId ,4,2,GETDATE(),2,GETDATE()
FROM ZnodeRoleMenu RM
WHERE MenuId = (select TOP 1 MenuId from ZnodeMenu where RM.MenuId = ZnodeMenu.MenuId)
AND RoleId = (SELECT TOP 1 RoleId FROM AspNetRoles WHERE Name = 'Sales Rep' and AspNetRoles.Id = RM.RoleId )

END

GO

--dt 02-07-2020 ZPD-11174
insert into ZnodeRoleMenu(RoleId,	MenuId,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate)
select (select top 1 Id from AspNetRoles where Name ='Admin'),ZM.MenuId,2,Getdate(),2,Getdate() 
from ZnodeMenu ZM where not exists(
select * from ZnodeRoleMenu ZRM where ZM.MenuId = ZRM.MenuId and
ZRM.RoleId = (select top 1 Id from AspNetRoles where Name ='Admin'))

insert into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select RoleMenuId,1,2,Getdate(),2,Getdate()  
from ZnodeRoleMenu ZRM 
where ZRM.RoleId = (select top 1 Id from AspNetRoles where Name ='Admin')
and not exists(select * from ZnodeRoleMenuAccessMapper ZRMAM WHERE ZRMAM.RoleMenuId = ZRM.RoleMenuId and AccessPermissionId = 1 )

insert into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select RoleMenuId,2,2,Getdate(),2,Getdate()  
from ZnodeRoleMenu ZRM 
where ZRM.RoleId = (select top 1 Id from AspNetRoles where Name ='Admin')
and not exists(select * from ZnodeRoleMenuAccessMapper ZRMAM WHERE ZRMAM.RoleMenuId = ZRM.RoleMenuId and AccessPermissionId = 2 )

insert into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select RoleMenuId,3,2,Getdate(),2,Getdate()  
from ZnodeRoleMenu ZRM 
where ZRM.RoleId = (select top 1 Id from AspNetRoles where Name ='Admin')
and not exists(select * from ZnodeRoleMenuAccessMapper ZRMAM WHERE ZRMAM.RoleMenuId = ZRM.RoleMenuId and AccessPermissionId = 3 )

insert into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select RoleMenuId,4,2,Getdate(),2,Getdate()  
from ZnodeRoleMenu ZRM 
where ZRM.RoleId = (select top 1 Id from AspNetRoles where Name ='Admin')
and not exists(select * from ZnodeRoleMenuAccessMapper ZRMAM WHERE ZRMAM.RoleMenuId = ZRM.RoleMenuId and AccessPermissionId = 4 )
