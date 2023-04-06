
delete from ZnodeRoleMenuAccessMapper
where RoleMenuId in (select RoleMenuId from ZnodeRoleMenu 
	where RoleId = (select top 1 ID from AspNetRoles where Name = 'Sales Rep') and
	exists ( select * from ZnodeMenu where MenuName not in ('OMS','Customers','Dashboard','Users','Returns','Quotes','Pending Payment','Vouchers','Pending Orders','Orders')
	and ZnodeRoleMenu.MenuId = ZnodeMenu.MenuId))

delete from ZnodeRoleMenu 
where RoleId = (select top 1 ID from AspNetRoles where Name = 'Sales Rep') and
	exists ( select * from ZnodeMenu where MenuName not in ('OMS','Customers','Dashboard','Users','Returns','Quotes','Pending Payment','Vouchers','Pending Orders','Orders')
	and ZnodeRoleMenu.MenuId = ZnodeMenu.MenuId)

declare @TempTable table (Id int)

insert into @TempTable
select 1
Union all
select 2
Union all
select 3
Union all
select 4

Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Orders')) and b.AccessPermissionId = a.Id)

Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'Pending Orders'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Pending Orders'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Pending Orders')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Pending Orders')) and b.AccessPermissionId = a.Id)

Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Vouchers')) and b.AccessPermissionId = a.Id)

Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'Pending Payment'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Pending Payment'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Pending Payment')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Pending Payment')) and b.AccessPermissionId = a.Id)

Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')) and b.AccessPermissionId = a.Id)

Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')) and b.AccessPermissionId = a.Id)

Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'Users'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Users'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Users')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Users')) and b.AccessPermissionId = a.Id)

Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'Customers'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Customers'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Customers')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Customers')) and b.AccessPermissionId = a.Id)

Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'OMS')) and b.AccessPermissionId = a.Id)


Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'Dashboard'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Dashboard'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Dashboard')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Dashboard')) and b.AccessPermissionId = a.Id)

Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Quotes')) and b.AccessPermissionId = a.Id)

Insert into ZnodeRoleMenu(RoleId,MenuId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
Select (select top 1 Id from AspNetRoles where Name = 'Sales Rep'),
	(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns'),
	2,Getdate(),2,Getdate()
Where not exists(select * from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns'))

Insert Into ZnodeRoleMenuAccessMapper(RoleMenuId,AccessPermissionId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')), a.Id, 2, getdate(),2,getdate()
from @TempTable a
where not exists(select * from ZnodeRoleMenuAccessMapper b where RoleMenuId = 
(select top 1 RoleMenuId from ZnodeRoleMenu where RoleId = (select top 1 Id from AspNetRoles where Name = 'Sales Rep')
	AND MenuId=(select top 1 MenuId from ZnodeMenu where MenuName = 'Returns')) and b.AccessPermissionId = a.Id)
