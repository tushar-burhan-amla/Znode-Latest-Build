CREATE PROCEDURE [dbo].[Znode_GetPimDownloadableProduct]
(
	@SKU [AssociatedSkus] READONLY
)
AS
BEGIN
	BEGIN TRY
		SELECT sku.Sku
		FROM ZnodePimDownloadableProduct As dwlproduct
		INNER JOIN @SKU AS sku ON dwlproduct.sku=sku.Sku
	END TRY
	BEGIN CATCH
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
		@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimDownloadableProduct'
        
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetPimDownloadableProduct',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;
 END