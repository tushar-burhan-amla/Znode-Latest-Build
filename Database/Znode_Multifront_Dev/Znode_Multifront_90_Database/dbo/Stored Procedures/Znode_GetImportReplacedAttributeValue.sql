CREATE PROCEDURE [dbo].[Znode_GetImportReplacedAttributeValue]
	@ImportAttributeType Nvarchar(300)
AS
BEGIN
	BEGIN TRY
		SELECT  TargetAttributeCode,AllowAttributeValue,ReplacedAttributeValue 
		FROM ZnodeImportAttributeDefaultValue 
		WHERE ImportAttributeType = @ImportAttributeType And IsActive=1
	END TRY
	BEGIN CATCH
		DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetImportReplacedAttributeValue @ImportAttributeType = '+@ImportAttributeType ;
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName    = 'Znode_GetImportReplacedAttributeValue',
		@ErrorInProcedure = @ERROR_PROCEDURE,
		@ErrorMessage     = @ErrorMessage,
		@ErrorLine        = @ErrorLine,
		@ErrorCall        = @ErrorCall;
    END CATCH;
END