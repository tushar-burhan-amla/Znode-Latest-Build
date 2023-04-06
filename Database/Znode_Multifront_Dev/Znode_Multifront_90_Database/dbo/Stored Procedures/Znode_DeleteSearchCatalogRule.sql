CREATE PROCEDURE [dbo].[Znode_DeleteSearchCatalogRule]
( 
	@SearchCatalogRuleId VARCHAR(MAX),
	@Status      BIT OUT
)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRAN DeleteSearchCatalogRule;
    BEGIN TRY

		DELETE FROM ZnodeSearchItemRule
		WHERE EXISTS 
			  ( SELECT * FROM ZnodeSearchCatalogRule ZSCR WHERE EXISTS(SELECT * FROM dbo.Split(@SearchCatalogRuleId,',')S WHERE ZSCR.SearchCatalogRuleId = S.Item )
				AND ZSCR.SearchCatalogRuleId = ZnodeSearchItemRule.SearchCatalogRuleId )

		DELETE FROM ZnodeSearchTriggerRule
		WHERE EXISTS 
			  ( SELECT * FROM ZnodeSearchCatalogRule ZSCR WHERE EXISTS(SELECT * FROM dbo.Split(@SearchCatalogRuleId,',')S WHERE ZSCR.SearchCatalogRuleId = S.Item )
				AND ZSCR.SearchCatalogRuleId = ZnodeSearchTriggerRule.SearchCatalogRuleId )

		DELETE FROM ZnodeSearchCatalogRule 
		WHERE EXISTS ( SELECT * FROM dbo.Split(@SearchCatalogRuleId,',')S WHERE ZnodeSearchCatalogRule.SearchCatalogRuleId = S.Item )
		
		SELECT 1 AS ID,CAST(1 AS BIT) AS [Status];
        SET @Status = 1;

	COMMIT TRAN DeleteSearchCatalogRule;
    END TRY
    BEGIN CATCH
        DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteCMSSlider @CMSSliderId = '+CAST(@SearchCatalogRuleId AS VARCHAR(100))+',@Status='+CAST(@Status AS VARCHAR(50));
        SELECT 0 AS ID,
            CAST(0 AS BIT) AS [Status];
        SET @Status = 0;
		ROLLBACK TRAN DeleteCMSSlider;
        EXEC Znode_InsertProcedureErrorLog
            @ProcedureName = 'Znode_DeleteSearchCatalogRule',
            @ErrorInProcedure = @Error_procedure,
            @ErrorMessage = @ErrorMessage,
            @ErrorLine = @ErrorLine,
            @ErrorCall = @ErrorCall;
        ROLLBACK TRAN DeleteSearchCatalogRule;
    END CATCH;
END;