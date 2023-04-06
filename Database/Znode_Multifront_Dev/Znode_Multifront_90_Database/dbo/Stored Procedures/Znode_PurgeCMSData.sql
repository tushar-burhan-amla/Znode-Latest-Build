CREATE PROCEDURE [Znode_PurgeCMSData]
(
	@DeleteAllCMSData BIT = 0
)
AS
BEGIN 
BEGIN TRY
	SET NOCOUNT ON;

	--Delete CMS Data
	EXEC [Znode_PurgeData] @DeleteAllCMSData=@DeleteAllCMSData

END TRY
BEGIN CATCH
	SELECT ERROR_MESSAGE()	
	DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max) = 'EXEC Znode_PurgeCMSData'
	SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
	EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_PurgeCMSData', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
END CATCH;
END