
CREATE VIEW [dbo].[View_UserRoles]
AS
SELECT        UserId, AspNetUserId, UserName, FirstName, MiddleName, LastName, Email, EmailOptIn, BudgetAmount, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, 
                         RoleId, RoleName, IsActive, IsLock, FullName, AccountName, PermissionsName, PermissionCode, DepartmentName, DepartmentId, AccountId, 
                         AccountPermissionAccessId, PhoneNumber, ExternalId, ApprovalName, ApprovalUserId, AccountUserOrderApprovalId, CustomerPaymentGUID
FROM            dbo.View_AdminUserDetail
GO


