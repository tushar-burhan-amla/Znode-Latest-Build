
CREATE VIEW [dbo].[View_AdminUserDetail]
AS
SELECT        a.UserId, a.AspNetUserId, azu.UserName, a.FirstName, a.MiddleName, a.LastName, a.Email, a.EmailOptIn, a.BudgetAmount, a.CreatedBy, CONVERT(DATETIME, 
                         a.CreatedDate) CreatedDate, A.ModifiedBy, CONVERT(DATETIME, a.ModifiedDate) ModifiedDate, c.Id RoleId, c.Name RoleName, c.IsActive, 
                         CAST(CASE WHEN ISNULL(LockoutEndDateUtc, 0) = 0 THEN  0 ELSE  1 END  AS BIT) AS IsLock, (ISNULL(a.FirstName, '') + CASE WHEN a.MiddleName IS NULL OR
                         a.MiddleName = '' THEN '' ELSE ' ' + a.MiddleName END + CASE WHEN a.LastName IS NULL OR
                         a.LastName = '' THEN '' ELSE ' ' + a.LastName END) FullName, e.Name AccountName, h.PermissionsName, h.PermissionCode, j.DepartmentName, i.DepartmentId, 
                         a.AccountId, f.AccountPermissionAccessId, a.PhoneNumber, a.ExternalId, CAST(NULL AS VARCHAR(4000)) ApprovalName, CAST(NULL AS INT) ApprovalUserId, 
                         CAST(NULL AS INT) AccountUserOrderApprovalId,a.CustomerPaymentGUID
FROM            ZnodeUser a INNER JOIN
                         AspNetUserRoles b ON (a.AspNetUserId = b.userId) INNER JOIN
                         AspNetRoles c ON (b.RoleId = c.Id) INNER JOIN
                         AspNetUsers d ON (a.AspNetUserId = d .id) LEFT JOIN
                         ZnodeAccount e ON (e.AccountId = a.AccountId) LEFT JOIN
                         ZnodeDepartmentUser i ON (i.UserId = a.UserId) LEFT JOIN
                         ZnodeDepartment j ON (j.DepartmentId = i.DepartmentId) LEFT JOIN
                         ZnodeAccountUserPermission f ON (f.UserId = a.UserId) LEFT JOIN
                         ZnodeAccountPermissionAccess g ON (g.AccountPermissionAccessId = f.AccountPermissionAccessId) LEFT JOIN
                         ZnodeAccessPermission h ON (h.AccessPermissionId = g.AccessPermissionId) LEFT JOIN
                         ZnodeUserPortal up ON (up.UserId = a.UserId) LEFT JOIN
                         AspNetZnodeUser azu ON (azu.AspNetZnodeUserId = d .UserName)
						
WHERE        c.Name NOT IN ('Customer', 'B2B') AND TypeOfRole IS NULL;
GO
