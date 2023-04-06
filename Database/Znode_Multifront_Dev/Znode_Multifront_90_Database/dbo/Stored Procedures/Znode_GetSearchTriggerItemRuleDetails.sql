CREATE PROCEDURE [dbo].[Znode_GetSearchTriggerItemRuleDetails]
(
	@Keyword nvarchar(100) = '',
	@PublishCatalogId int
)
AS
BEGIN

	SET NOCOUNT ON;
	BEGIN TRY

	DECLARE @GetDate DATETIME= dbo.Fn_GetDate()

	IF OBJECT_ID('tempdb..#KeyWord') IS NOT NULL
		DROP TABLE #KeyWord

	SELECT *
	INTO #KeyWord
	FROM dbo.Split(@KeyWord,' ')

	DELETE FROM #KeyWord WHERE LEN(Item)<1

	IF OBJECT_ID('tempdb..#Temp') IS NOT NULL
		DROP TABLE #Temp

	SELECT DISTINCT ZSCR.SearchCatalogRuleId, ZSTR.SearchTriggerRuleId, ZSTR.SearchTriggerValue, ZSTR.SearchTriggerCondition,ZSCR.IsItemForAll, ZSCR.IsTriggerForAll
	INTO #Temp
	FROM ZnodeSearchCatalogRule ZSCR 
	INNER JOIN ZnodeSearchTriggerRule ZSTR ON ZSCR.SearchCatalogRuleId = ZSTR.SearchCatalogRuleId
	INNER JOIN #KeyWord K ON ((ZSTR.SearchTriggerCondition='Is' AND ZSTR.SearchTriggerValue=K.Item)
								OR (ZSTR.SearchTriggerCondition='Contains' AND K.Item LIKE '%'+ZSTR.SearchTriggerValue+'%')
							)
	WHERE ZSCR.PublishCatalogId = @PublishCatalogId AND ZSCR.IsGlobalRule = 0 AND IsPause <> 1
		AND CAST(@GetDate AS DATE) BETWEEN CAST(ZSCR.StartDate AS DATE) AND ISNULL(ZSCR.EndDate,'9999-01-01')

	SELECT A.*,ZSCR.IsItemForAll
	FROM
	(
		SELECT SearchCatalogRuleId,SearchItemKeyword,SearchItemCondition,SearchItemValue,SearchItemBoostValue
		FROM ZnodeSearchItemRule A
		WHERE EXISTS (SELECT TOP 1 1 FROM #Temp WHERE IsTriggerForAll=0 AND SearchCatalogRuleId=A.SearchCatalogRuleId)

		UNION ALL
		SELECT SearchCatalogRuleId,SearchItemKeyword,SearchItemCondition,SearchItemValue,SearchItemBoostValue
		FROM ZnodeSearchItemRule B
		WHERE EXISTS (SELECT TOP 1 1 FROM #Temp WHERE IsTriggerForAll=1 AND SearchCatalogRuleId=B.SearchCatalogRuleId)
			AND NOT EXISTS
				(SELECT TOP 1 1 FROM ZnodeSearchTriggerRule ZSTR
				LEFT JOIN #Temp T ON ZSTR.SearchCatalogRuleId=T.SearchCatalogRuleId AND ZSTR.SearchTriggerValue=T.SearchTriggerValue
				LEFT JOIN #KeyWord K ON K.Item LIKE '%'+ZSTR.SearchTriggerValue+'%'
				WHERE (T.IsTriggerForAll<>0 OR T.IsTriggerForAll IS NULL) AND K.Id IS NULL
				AND EXISTS(SELECT TOP 1 1 FROM #Temp WHERE SearchCatalogRuleId=ZSTR.SearchCatalogRuleId))
	) A
	INNER JOIN ZnodeSearchCatalogRule ZSCR ON A.SearchCatalogRuleId=ZSCR.SearchCatalogRuleId

	UNION ALL
	SELECT DISTINCT ZSCR.SearchCatalogRuleId,ZSIR.SearchItemKeyword, ZSIR.SearchItemCondition, ZSIR.SearchItemValue, ZSIR.SearchItemBoostValue, ZSCR.IsItemForAll
	FROM ZnodeSearchCatalogRule ZSCR 
	LEFT JOIN ZnodeSearchTriggerRule ZSTR ON ZSCR.SearchCatalogRuleId = ZSTR.SearchCatalogRuleId 
	INNER JOIN ZnodeSearchItemRule ZSIR ON ZSCR.SearchCatalogRuleId = ZSIR.SearchCatalogRuleId 
	WHERE ZSCR.PublishCatalogId = @PublishCatalogId AND ZSCR.IsGlobalRule = 1
		AND CAST(@GetDate AS DATE) BETWEEN CAST(ZSCR.StartDate AS DATE) AND ISNULL( ZSCR.EndDate,'9999-01-01') AND IsPause <> 1

	END TRY
	BEGIN CATCH
		DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSearchTriggerItemRuleDetails @Keyword = '+@Keyword+' @PublishCatalogId = '+CAST(@PublishCatalogId AS VARCHAR(50));
              			 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		   
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetSearchTriggerItemRuleDetails',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;

END