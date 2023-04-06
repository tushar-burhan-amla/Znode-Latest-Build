CREATE PROCEDURE [Znode_PurgeStoreData]
(
	@DeleteAllStore BIT = 0,
	@ExceptStoreId TransferId Readonly 
)
AS
BEGIN 
BEGIN TRY
	SET NOCOUNT ON;

	--Delete store data
	EXEC [Znode_PurgeData] @DeleteAllStore = @DeleteAllStore,@ExceptStoreId=@ExceptStoreId
	
END TRY
BEGIN CATCH
	SELECT ERROR_MESSAGE()	
	DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max) = 'EXEC Znode_PurgeStoreData'
	SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
	EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_PurgeStoreData', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
END CATCH;
END