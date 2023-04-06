CREATE PROCEDURE [Znode_PurgeProductCategoryAttributeData]
(
	@DeleteAllProductCategoryAttribute BIT = 0,
	@ExceptProductCategoryAttributeId TransferId Readonly
)
AS
BEGIN 
BEGIN TRY
	SET NOCOUNT ON;

	--Delete Product AND Category Attribute Data
	EXEC [Znode_PurgeData] @DeleteAllProductCategoryAttribute=@DeleteAllProductCategoryAttribute,@ExceptProductCategoryAttributeId=@ExceptProductCategoryAttributeId

END TRY
BEGIN CATCH
	SELECT ERROR_MESSAGE()	
	DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max) = 'EXEC Znode_PurgeProductCategoryAttributeData'
	SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
	EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_PurgeProductCategoryAttributeData', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
END CATCH;
END