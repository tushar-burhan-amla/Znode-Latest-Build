CREATE PROCEDURE Znode_GetUserDetailsByUserName
(
	@UserName Varchar(600),
	@PortalId Int
)
AS
/* 
    Summary: SP used to get the User details using UserName
    EXEC [Znode_GetUserDetailsByUserName] @UserName = 'danial450@yopmail.com', @PortalId=1,@PublishCatalogId=@PublishCatalogId 
*/
BEGIN
BEGIN TRY
SET NOCOUNT ON;

	SELECT AspNet.AspNetZnodeUserId,AspNet.UserName,AspNet.PortalId, ZU.UserId , ZU.AspNetUserId
	INTO #UserDetail
	FROM AspNetUsers Asp
	INNER JOIN AspNetZnodeUSer AspNet ON Asp.UserName = AspNet.AspNetZnodeUserId
	INNER JOIN ZnodeUser ZU ON Asp.Id = ZU.AspNetUserId
	WHERE AspNet.UserName = @UserName AND (AspNet.PortalId = @PortalId OR AspNet.PortalId IS NULL)

	SELECT AspNetZnodeUserId,UserName,PortalId FROM #UserDetail

	SELECT Roles.Name 
	FROM #UserDetail UD
	INNER JOIN AspNetUserRoles UserRole ON (UserRole.UserId = UD.AspNetUserId)
	INNER JOIN AspNetRoles Roles ON (Roles.Id = UserRole.RoleId)   

	SELECT ZAP.PermissionCode
	from #UserDetail UD
    INNER JOIN ZnodeAccountUserPermission ZAUP ON (ZAUP.UserId = UD.UserId)
    INNER JOIN ZnodeAccountPermissionAccess ZAPA ON (ZAPA.AccountPermissionAccessId = ZAUP.AccountPermissionAccessId)
    INNER JOIN ZnodeAccessPermission ZAP ON(ZAP.AccessPermissionId = ZAPA.AccessPermissionId)

	SELECT Top 1 PublishCatalogId 
	FROM ZnodePortalCatalog WHERE PortalId = @PortalId

END TRY
BEGIN CATCH
	DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetUserDetailsByUserName @UserName ='+CAST(@UserName AS VARCHAR(600))+',@PortalId ='+CAST(@PortalId AS VARCHAR(10));
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName    = 'Znode_GetUserDetailsByUserName',
	@ErrorInProcedure = @ERROR_PROCEDURE,
	@ErrorMessage     = @ErrorMessage,
	@ErrorLine        = @ErrorLine,
	@ErrorCall        = @ErrorCall;
END CATCH;
END