




CREATE View [dbo].[View_GetAccountAccessPermission]
AS 
SELECT a.AccountPermissionId , isnull(a.AccountId,0) AccountId,A.AccountPermissionName,C.PermissionsName,b.AccountPermissionAccessId,c.PermissionCode,c.AccessPermissionId
FROM ZnodeAccountPermission a 
LEFT JOIN ZnodeAccountPermissionAccess  b ON (b.AccountPermissionId = a.AccountPermissionId) 
LEFT JOIN ZnodeAccessPermission c ON (b.AccessPermissionId = c.AccessPermissionId)