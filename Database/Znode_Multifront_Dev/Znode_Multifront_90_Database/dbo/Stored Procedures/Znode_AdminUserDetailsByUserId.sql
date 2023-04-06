CREATE PROCEDURE [dbo].[Znode_AdminUserDetailsByUserId]
(	
	@UserId INT
)
AS
/* 
    Summary: SP used to get the User details using UserId
    EXEC [Znode_AdminUserDetailsByUserId] @UserId = 1005
*/
BEGIN
BEGIN TRY
SET NOCOUNT ON;
	if OBJECT_ID('tempdb..#View_CustomerUserAddDetail') is not null
		drop table #View_CustomerUserAddDetail

	SELECT top 1 a.userId,a.AspNetuserId,azu.UserName,a.FirstName,a.MiddleName,a.LastName,a.PhoneNumber,
			a.Email,a.EmailOptIn,a.CreatedBy,a.SMSOptIn,CONVERT( DATE, a.CreatedDate) CreatedDate,A.ModifiedBy,
			CONVERT( DATE, a.ModifiedDate) ModifiedDate,ur.RoleId,r.Name RoleName,
			CAST(CASE WHEN ISNULL(LockoutEndDateUtc, 0) = 0 THEN  0 ELSE  1 END  AS    BIT) AS IsLock,
			e.Name+' | '+e.AccountCode AccountName,a.AccountId,a.ExternalId,
			CAST(CASE WHEN a.AccountId IS NULL THEN 0 ELSE 1 END AS    BIT) AS IsAccountCustomer,
			a.BudgetAmount,r.TypeOfRole,CAST(CASE WHEN a.AspNetuserId IS NULL THEN 1 ELSE 0 END AS    BIT) AS IsGuestUser,
			a.CustomerPaymentGUID,
			(ISNULL(RTRIM(LTRIM(a.FirstName)), '')+' '+ISNULL(RTRIM(LTRIM(a.MiddleName)), '')
			+CASE
				WHEN ISNULL(RTRIM(LTRIM(a.MiddleName)), '') = ''
				THEN ''
				ELSE ' '
			END+ISNULL(RTRIM(LTRIM(a.LastName)), '')) FullName
	,CASE WHEN zp.StoreName IS NULL THEN 'ALL' ELSE zp.StoreName END StoreName,
	CASE WHEN a.AccountId IS NULL THEN up.PortalId ELSE ZPA.PortalId END as PortalId
	into #View_CustomerUserAddDetail
	FROM ZnodeUser a
	left JOIN ASPNetUsers B ON(a.AspNetuserId = b.Id)
	LEFT JOIN ZnodeAccount e ON(e.AccountId = a.AccountId)
	LEFT JOIN AspNetUserRoles ur ON(ur.UserId = a.AspNetUserId)
	LEFT JOIN AspNetRoles r ON(r.Id = ur.RoleId)                       
	LEFT JOIN AspNetZnodeUser azu ON(azu.AspNetZnodeUserId = b.UserName)
	LEFT JOIN ZnodeUserPortal up ON(up.UserId = a.UserId)  
	LEFT JOIN ZnodePortal zp ON (up.PortalId = zp.PortalId)
	LEFT JOIN ZnodePortalAccount ZPA ON(ZPA.AccountId = a.AccountId) 
	WHERE a.UserId = @UserId


	alter table #View_CustomerUserAddDetail 
	Add DepartmentId int, AccountPermissionAccessId int,SalesRepUserName varchar(600),SalesRepId Int,
	PermissionsName varchar(200), PermissionCode varchar(200),SalesRepFullName varchar(1000)

	--To get data for DepartmentId
	update CUD SET DepartmentId = i.DepartmentId
	from #View_CustomerUserAddDetail cud
	INNER JOIN ZnodeDepartmentUser i ON(i.UserId = cud.UserId)

	--To get data for AccountPermissionAccessId
	update CUD SET AccountPermissionAccessId = f.AccountPermissionAccessId
	from #View_CustomerUserAddDetail cud
	INNER JOIN ZnodeAccountUserPermission f ON(f.UserId = cud.UserId)

	----Updating SalesRep for user if any 
	update CUAD
	set CUAD.SalesRepUserName = azu.UserName, SalesRepId = SRCUP.SalesRepUserId,
	CUAD.SalesRepFullName = (ISNULL(RTRIM(LTRIM(ZU.FirstName)), '')+' '+ISNULL(RTRIM(LTRIM(ZU.MiddleName)), '')
								+CASE
									WHEN ISNULL(RTRIM(LTRIM(ZU.MiddleName)), '') = ''
									THEN ''
									ELSE ' '
								END+ISNULL(RTRIM(LTRIM(ZU.LastName)), '')) 
	from #View_CustomerUserAddDetail CUAD
	inner join ZnodeSalesRepCustomerUserPortal SRCUP ON CUAD.UserId = SRCUP.CustomerUserid 
	inner join ZnodeUser ZU ON SRCUP.SalesRepUserId = ZU.UserId
	inner join ASPNetUsers ANU ON(ZU.AspNetuserId = ANU.Id)
	inner join AspNetZnodeUser azu ON(azu.AspNetZnodeUserId = ANU.UserName)

	--To get data for PermissionsName
	update CUD SET PermissionsName = h.PermissionsName, PermissionCode = h.PermissionCode
	from #View_CustomerUserAddDetail cud
    INNER JOIN ZnodeAccountUserPermission f ON(f.UserId = cud.UserId)
    INNER JOIN ZnodeAccountPermissionAccess g ON(g.AccountPermissionAccessId = f.AccountPermissionAccessId)
    INNER JOIN ZnodeAccessPermission h ON(h.AccessPermissionId = g.AccessPermissionId)

	SELECT UserId,AspNetuserId,UserName,FirstName,MiddleName,LastName,FullName,PhoneNumber, Email,EmailOptIn,SMSOptIn,CreatedBy,CreatedDate,
		   ModifiedBy, ModifiedDate,RoleId,RoleName,IsLock,AccountName,AccountId,ExternalId,IsAccountCustomer,BudgetAmount,
		   TypeOfRole,IsGuestUser,CustomerPaymentGUID,StoreName, PortalId,DepartmentId, AccountPermissionAccessId,
		   SalesRepUserName,SalesRepFullName,isnull(SalesRepId,0) as SalesRepId, PermissionsName, PermissionCode
	FROM #View_CustomerUserAddDetail

	if OBJECT_ID('tempdb..#View_CustomerUserAddDetail') is not null
		drop table #View_CustomerUserAddDetail

END TRY
BEGIN CATCH
	DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_AdminUserDetailsByUserId @UserId ='+CAST(@UserId AS VARCHAR(10));
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName    = 'Znode_AdminUserDetailsByUserId',
	@ErrorInProcedure = @ERROR_PROCEDURE,
	@ErrorMessage     = @ErrorMessage,
	@ErrorLine        = @ErrorLine,
	@ErrorCall        = @ErrorCall;
END CATCH;
END;