
CREATE PROCEDURE [dbo].[Znode_InsertUpdateSearchCatalogRule]
(
	@SearchCatalogRuleId Int = 0, ---- 0 -- Insert, >0 -- Update
	@PublishCatalogId  Int,
	@RuleName Varchar(600),
	@StartDate DateTime,
	@EndDate DateTime = NULL, ---01/01/0001 '0001-01-01'
	@IsGlobalRule Bit = 0, -- 1 - Apply to all search for catalog
	@IsTriggerForAll Bit = 0, -- 0 for Any , 1 for All
	@SearchRuleTriggerDetail SearchRuleTriggerDetail ReadOnly,
	@IsItemForAll Bit = 0, -- 0 for Any , 1 for All
	@SearchRuleItemDetail SearchRuleItemDetail ReadOnly,
	@UserId Int,
	@Status Bit Out
)
AS
BEGIN
	
	SET NOCOUNT ON;

	DECLARE @GetDate DateTime= dbo.Fn_GetDate()

	DECLARE @InsertedSearchCatalogRule TABLE ( SearchCatalogRuleId INT ) 

	BEGIN TRY
	BEGIN TRAN

	   IF EXISTS ( SELECT * FROM ZnodeSearchCatalogRule WHERE RuleName = @RuleName AND @SearchCatalogRuleId = 0 )
	   BEGIN 
			SET @Status = 0
			SELECT 1 as Id, @Status  AS [Status], 'Data already exists' Message
	   END
	   ELSE 
	   BEGIN
			---- Updating existing Rule against catalog
			UPDATE ZnodeSearchCatalogRule
			SET RuleName = @RuleName,
				StartDate = @StartDate,
				EndDate = CASE WHEN CAST(@EndDate AS Date) = '1754-01-01' THEN NULL ELSE @EndDate END, ----'1754-01-01' Default date used for null 
				IsTriggerForAll = @IsTriggerForAll,
				IsItemForAll = @IsItemForAll,
				IsGlobalRule = @IsGlobalRule,
				ModifiedBy = @UserId,
				ModifiedDate = @GetDate
			WHERE PublishCatalogId = @PublishCatalogId 
			AND SearchCatalogRuleId = @SearchCatalogRuleId 

			---- Inserting new Rule against catalog
			INSERT INTO ZnodeSearchCatalogRule( PublishCatalogId, RuleName, StartDate, EndDate, IsTriggerForAll, IsItemForAll, IsGlobalRule, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
			OUTPUT Inserted.SearchCatalogRuleId  into @InsertedSearchCatalogRule ( SearchCatalogRuleId )
			SELECT @PublishCatalogId AS PublishCatalogId, @RuleName AS RuleName, @StartDate AS StartDate, CASE WHEN CAST(@EndDate AS Date) = '1754-01-01' THEN NULL ELSE @EndDate END AS EndDate, 
			@IsTriggerForAll AS IsTriggerForAll, @IsItemForAll AS IsItemForAll, @IsGlobalRule AS IsGlobalRule, @UserId AS CreatedBy, @GetDate AS CreatedDate,@UserId AS ModifiedBy, @GetDate AS ModifiedDate
			WHERE NOT EXISTS( SELECT * FROM ZnodeSearchCatalogRule WHERE RuleName = @RuleName )
			AND @SearchCatalogRuleId = 0

			----Deleting search trigger against catalog rule
			DELETE FROM ZnodeSearchTriggerRule 
			WHERE NOT EXISTS (SELECT * FROM @SearchRuleTriggerDetail SRTD WHERE ZnodeSearchTriggerRule.SearchTriggerRuleId = SRTD.SearchTriggerRuleId 
							  AND ZnodeSearchTriggerRule.SearchCatalogRuleId = SRTD.SearchCatalogRuleId  )
			AND ZnodeSearchTriggerRule.SearchCatalogRuleId = @SearchCatalogRuleId 

		
			---- Updating search trigger against catalog rule --- SRTD.SearchCatalogRuleId IS NOT NULL records for update
			UPDATE ZSTR 
			SET ZSTR.SearchTriggerKeyword = SRTD.SearchTriggerKeyword,
				ZSTR.SearchTriggerCondition = SRTD.SearchTriggerCondition,
				ZSTR.SearchTriggerValue = SRTD.SearchTriggerValue, 
				ZSTR.ModifiedBy = @UserId, 
				ZSTR.ModifiedDate = @GetDate
			OUTPUT Inserted.SearchCatalogRuleId  into @InsertedSearchCatalogRule ( SearchCatalogRuleId )
			FROM ZnodeSearchTriggerRule ZSTR 
			INNER JOIN @SearchRuleTriggerDetail SRTD ON ZSTR.SearchTriggerRuleId = SRTD.SearchTriggerRuleId 
			AND ZSTR.SearchCatalogRuleId = @SearchCatalogRuleId 
		                                           
			---- Inserting search trigger against new created catalog rule --- SRTD.SearchCatalogRuleId IS NULL records for insert
			INSERT INTO ZnodeSearchTriggerRule( SearchCatalogRuleId, SearchTriggerKeyword, SearchTriggerCondition, SearchTriggerValue, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
			SELECT ISCR.SearchCatalogRuleId, SRTD.SearchTriggerKeyword, SRTD.SearchTriggerCondition, SRTD.SearchTriggerValue, 
				   @UserId AS CreatedBy, @GetDate AS CreatedDate,@UserId AS ModifiedBy, @GetDate AS ModifiedDate
			FROM @SearchRuleTriggerDetail SRTD
			CROSS APPLY (SELECT TOP 1 SearchCatalogRuleId FROM @InsertedSearchCatalogRule ) ISCR 
			WHERE ISNULL(SRTD.SearchCatalogRuleId,0) = 0
		
			---- Inserting search trigger against existing catalog rule 
			INSERT INTO ZnodeSearchTriggerRule( SearchCatalogRuleId, SearchTriggerKeyword, SearchTriggerCondition, SearchTriggerValue, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
			SELECT SRTD.SearchCatalogRuleId, SRTD.SearchTriggerKeyword, SRTD.SearchTriggerCondition, SRTD.SearchTriggerValue, 
				   @UserId AS CreatedBy, @GetDate AS CreatedDate,@UserId AS ModifiedBy, @GetDate AS ModifiedDate
			FROM @SearchRuleTriggerDetail SRTD
			WHERE ISNULL(SRTD.SearchCatalogRuleId,0) <> 0
			AND NOT EXISTS ( SELECT * FROM ZnodeSearchTriggerRule ZSTR WHERE SRTD.SearchCatalogRuleId = ZSTR.SearchCatalogRuleId AND SRTD.SearchTriggerKeyword = ZSTR.SearchTriggerKeyword
							 AND SRTD.SearchTriggerCondition = ZSTR.SearchTriggerCondition AND SRTD.SearchTriggerValue = ZSTR.SearchTriggerValue)
		
			----Deleting search items against catalog rule
			DELETE FROM ZnodeSearchItemRule 
			WHERE NOT EXISTS (SELECT * FROM @SearchRuleItemDetail SRID WHERE ZnodeSearchItemRule.SearchItemRuleId = SRID.SearchItemRuleId 
							  AND ZnodeSearchItemRule.SearchCatalogRuleId = SRID.SearchCatalogRuleId  )
			AND ZnodeSearchItemRule.SearchCatalogRuleId = @SearchCatalogRuleId 

			---- Updating search items against catalog rule --- SRID.SearchCatalogRuleId IS NOT NULL records for update
			UPDATE ZSIR 
			SET ZSIR.SearchItemKeyword = SRID.SearchItemKeyword,
				ZSIR.SearchItemCondition = SRID.SearchItemCondition,
				ZSIR.SearchItemValue = SRID.SearchItemValue, 
				ZSIR.SearchItemBoostValue = SRID.SearchItemBoostValue,
				ZSIR.ModifiedBy = @UserId, 
				ZSIR.ModifiedDate = @GetDate
			OUTPUT Inserted.SearchCatalogRuleId  INTO @InsertedSearchCatalogRule ( SearchCatalogRuleId )
			FROM ZnodeSearchItemRule ZSIR 
			INNER JOIN @SearchRuleItemDetail SRID ON ZSIR.SearchItemRuleId = SRID.SearchItemRuleId 
			AND ZSIR.SearchCatalogRuleId = @SearchCatalogRuleId 
		                                        
			---- Inserting search items against new created catalog rule --- SRID.SearchCatalogRuleId IS NULL records for insert
			INSERT INTO ZnodeSearchItemRule( SearchCatalogRuleId, SearchItemKeyword, SearchItemCondition, SearchItemValue, SearchItemBoostValue, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
			SELECT ISCR.SearchCatalogRuleId, SRID.SearchItemKeyword, SRID.SearchItemCondition, SRID.SearchItemValue, SRID.SearchItemBoostValue, 
				   @UserId AS CreatedBy, @GetDate AS CreatedDate,@UserId AS ModifiedBy, @GetDate AS ModifiedDate
			FROM @SearchRuleItemDetail SRID
			CROSS APPLY (SELECT TOP 1 SearchCatalogRuleId FROM @InsertedSearchCatalogRule ) ISCR 
			WHERE ISNULL(SRID.SearchCatalogRuleId,0) = 0
		
			---- Inserting search items against existing catalog rule 
			INSERT INTO ZnodeSearchItemRule( SearchCatalogRuleId, SearchItemKeyword, SearchItemCondition, SearchItemValue, SearchItemBoostValue, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
			SELECT SRID.SearchCatalogRuleId, SRID.SearchItemKeyword, SRID.SearchItemCondition, SRID.SearchItemValue, SRID.SearchItemBoostValue, 
				   @UserId AS CreatedBy, @GetDate AS CreatedDate,@UserId AS ModifiedBy, @GetDate AS ModifiedDate
			FROM @SearchRuleItemDetail SRID
			WHERE ISNULL(SRID.SearchCatalogRuleId,0) <> 0 
			AND NOT EXISTS (SELECT * FROM ZnodeSearchItemRule ZSIR WHERE SRID.SearchCatalogRuleId = ZSIR.SearchCatalogRuleId AND SRID.SearchItemKeyword = ZSIR.SearchItemKeyword 
							AND SRID.SearchItemCondition = ZSIR.SearchItemCondition AND SRID.SearchItemValue = ZSIR.SearchItemValue )

			SET @Status = 1

			SELECT 1 as Id, @Status  AS [Status]
		END
			

	COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		SET @Status = 0 
		SELECT 1 as Id, @Status  AS [Status]
	END CATCH

END