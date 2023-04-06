CREATE PROCEDURE [dbo].[Znode_DeleteCMSWidgetTemplates]
(
	@WidgetTemplateIds  VARCHAR(2000),
	@status  BIT OUT
)
AS
BEGIN
BEGIN TRY
BEGIN TRAN DeleteWidgetTemplates

		DECLARE @TBL_WidgetTemplate TABLE(WidgetTemplateId int)

		INSERT INTO @TBL_WidgetTemplate (WidgetTemplateId)
		SELECT item FROM dbo.Split(@WidgetTemplateIds,',')

		DECLARE @TBL_DeleteIds TABLE(WidgetTemplateId int)
			 
		INSERT INTO @TBL_DeleteIds(WidgetTemplateId)
		SELECT WT.WidgetTemplateId FROM @TBL_WidgetTemplate WT inner join
		ZnodeCMSWidgetTemplate CWT on WT.WidgetTemplateId = CWT.CMSWidgetTemplateId 
		WHERE not exists
		(
		SELECT top 1 1 FROM ZnodeCMSWidgetProfileVariant CW 
		INNER JOIN ZnodeCMSWidgetProfileVariantLocale CWL ON CW.CMSWidgetProfileVariantId = CWL.CMSWidgetProfileVariantId
		WHERE CWL.CMSWidgetTemplateId = WT.WidgetTemplateId

		)

		DELETE FROM ZnodeCMSWidgetTemplate
		WHERE exists
		(
		SELECT top 1 1 FROM @TBL_DeleteIds DI
		WHERE DI.WidgetTemplateId = ZnodeCMSWidgetTemplate.CMSWidgetTemplateId

		);
			 
		IF(SELECT COUNT(1) FROM @TBL_DeleteIds) = (SELECT COUNT(1) FROM @TBL_WidgetTemplate )  
		BEGIN
            SET @Status = 1;
            SELECT 1 AS ID,
                CAST(1 AS BIT) AS [Status];
        END;
        ELSE
        BEGIN
            SET @Status = 0;
            SELECT 1 AS ID,
                CAST(0 AS BIT) AS [Status];
        END;

COMMIT TRAN DeleteWidgetTemplates;
END TRY

BEGIN CATCH
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
	@ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteCMSWidgetTemplates 
	@WidgetTemplateIds = '+@WidgetTemplateIds+',@Status='+CAST(@Status AS VARCHAR(50));

		SET @Status =0  
		SELECT 1 AS ID,@Status AS Status;  
		ROLLBACK TRAN DeleteWidgetTemplates;
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_DeleteCMSWidgetTemplates',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
       
END CATCH

END
