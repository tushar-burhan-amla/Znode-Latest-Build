CREATE PROCEDURE [dbo].[Znode_DeleteImportTemplates]
(
	@ImportTemplateIds	NVARCHAR(MAX)=0,
	@Status				BIT OUT
)
/*
	Summary: 
		This procedure is used to remove Custom Import Template details with respective mapping details associated to it.

	Unit Testing:
		EXEC dbo.Znode_DeleteImportTemplates @ImportTemplateIds='17', @Status=0
		EXEC dbo.Znode_DeleteImportTemplates @ImportTemplateIds='19,20', @Status=0
*/
AS
BEGIN
	BEGIN TRAN DeleteImportTemplates;
	BEGIN TRY
		SET NOCOUNT ON;

		IF OBJECT_ID('tempdb..#ImportTemplateIds') IS NOT NULL
			DROP TABLE #ImportTemplateIds;

		SELECT Item As ImportTemplateId
		INTO #ImportTemplateIds
		FROM dbo.Split(@ImportTemplateIds,',');

		DELETE 
		FROM #ImportTemplateIds
		WHERE ImportTemplateId IN (SELECT ImportTemplateId FROM ZnodeImportProcessLog WHERE Status = 'Started');

		DELETE
		FROM ZnodeImportLog
		WHERE ImportProcessLogId IN (	SELECT ImportProcessLogId 
										FROM ZnodeImportProcessLog
										WHERE ImportTemplateId IN (SELECT ImportTemplateId FROM #ImportTemplateIds)
									);

		DELETE
		FROM ZnodeImportProcessLog
		WHERE ImportTemplateId IN (SELECT ImportTemplateId FROM #ImportTemplateIds);

		DELETE
		FROM ZnodeImportTemplateMapping
		WHERE ImportTemplateId IN (SELECT ImportTemplateId FROM #ImportTemplateIds);

		DELETE
		FROM ZnodeImportTemplate
		WHERE ImportTemplateId IN (SELECT ImportTemplateId FROM #ImportTemplateIds);

		DROP TABLE #ImportTemplateIds;

		SELECT 1 As ID, CAST(1 AS BIT) AS Status

		SET @Status = 1;
	COMMIT TRAN DeleteImportTemplates;
	END TRY
	BEGIN CATCH
		DECLARE @Error_Procedure VARCHAR(1000) = ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteImportTemplates 
					@ImportTemplateIds='''+CAST(@ImportTemplateIds AS NVARCHAR(MAX))+''',
					@Status='+CAST(@Status AS VARCHAR(50));
			
		SELECT 0 As ID, CAST(0 AS BIT) AS Status
        SET @Status = 0;

        ROLLBACK TRAN DeleteImportTemplates;

        EXEC Znode_InsertProcedureErrorLog
            @ProcedureName = 'Znode_DeleteImportTemplates',
            @ErrorInProcedure = @Error_procedure,
            @ErrorMessage = @ErrorMessage,
            @ErrorLine = @ErrorLine,
            @ErrorCall = @ErrorCall;

		SELECT ERROR_MESSAGE();
	END CATCH
END