CREATE PROCEDURE [Znode_PurgePublishedData]
(
	@DeleteAllPublishedData BIT = 0
)
AS
BEGIN 
BEGIN TRY
	SET NOCOUNT ON;

	--Delete store data
	EXEC [Znode_PurgeData] @DeleteAllPublishedData = @DeleteAllPublishedData
	
END TRY
BEGIN CATCH
	SELECT ERROR_MESSAGE()	
	DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max) = 'EXEC Znode_PurgePublishedData'
	SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
	EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_PurgePublishedData', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
END CATCH;
END