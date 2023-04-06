CREATE PROCEDURE [dbo].[Znode_GetSearchTriggerItemRuleForEdit]
(
	@SearchCatalogRuleId  Int
)
AS
/*
	exec [Znode_GetSearchTriggerItemRuleForEdit] @SearchCatalogRuleId = 6
*/
BEGIN
	
	SET NOCOUNT ON;

	BEGIN TRY
		----Getting Search Catalog Rule Data
		SELECT ZSCR.SearchCatalogRuleId, ZSCR.PublishCatalogId,PC.CatalogName,	ZSCR.RuleName,	ZSCR.StartDate,	ZSCR.EndDate, ZSCR.IsTriggerForAll, ZSCR.IsItemForAll,	ZSCR.IsGlobalRule, ZSCR.IsPause
		FROM ZnodeSearchCatalogRule ZSCR
		INNER JOIN ZnodePimCatalog PC on ZSCR.PublishCatalogId = PC.PimCatalogId 
		WHERE SearchCatalogRuleId = @SearchCatalogRuleId 

		----Getting Search Trigger Rule Data
		SELECT ZSTR.SearchTriggerRuleId, ZSTR.SearchCatalogRuleId, ZSTR.SearchTriggerKeyword, ZSTR.SearchTriggerCondition, ZSTR.SearchTriggerValue 
		FROM ZnodeSearchTriggerRule ZSTR 
		WHERE ZSTR.SearchCatalogRuleId = @SearchCatalogRuleId 

		----Getting Search Item Rule Data
		SELECT ZSIR.SearchItemRuleId, ZSIR.SearchCatalogRuleId, ZSIR.SearchItemKeyword, ZSIR.SearchItemCondition, ZSIR.SearchItemValue, ZSIR.SearchItemBoostValue
		FROM ZnodeSearchItemRule ZSIR
		WHERE ZSIR.SearchCatalogRuleId = @SearchCatalogRuleId 

	 END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
			         @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
					 @ErrorLine VARCHAR(100)= ERROR_LINE(), 
					 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCategoryHierarchy @SearchCatalogRuleId = '+CAST(@SearchCatalogRuleId AS VARCHAR(50));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		   
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCategoryHierarchy',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;

END