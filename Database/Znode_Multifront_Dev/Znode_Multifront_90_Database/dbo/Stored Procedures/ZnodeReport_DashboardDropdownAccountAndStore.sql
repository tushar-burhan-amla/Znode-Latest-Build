CREATE PROCEDURE [dbo].[ZnodeReport_DashboardDropdownAccountAndStore]
AS
BEGIN
SET NOCOUNT ON
BEGIN TRY
	
	--Getting account and store list for dropdown on dashboard
	SELECT AccountId AS Id,Name AS EntityName,'Account' AS EntityType FROM ZnodeAccount WITH (NOLOCK)
	UNION ALL
	SELECT PortalId AS Id, StoreName AS EntityName, 'Store' AS EntityType FROM ZnodePortal WITH (NOLOCK)

END TRY
BEGIN CATCH      
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashbordDropdownAccountAndStore ';

	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'ZnodeReport_DashboardDropdownAccountAndStore',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH
END