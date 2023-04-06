CREATE PROCEDURE [dbo].[Znode_DeletePublishCatalogEntity]
(
    @PublishCatalogId  INT = 0 
   ,@UserId int = 0
   ,@IsRevertPublish bit = 0 
   ,@NewGUID nvarchar(500) 

)
AS
/*
	To Remove all publish catalog details from related entities
	Unit Testing : 
	Declare @Status bit 
	Exec [dbo].[ZnodePublishPortalEntity]
     @PortalId  = 1 
	,@LocaleId  = 0 
	,@RevisionState = 'PRODUCTION' 
	,@UserId = 2
	,@Status = @Status 
	--Select @Status 
*/
BEGIN
SET NOCOUNT ON
BEGIN TRY 
		DECLARE @Status BIT = 0;
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

		IF @IsRevertPublish = 0 AND @PublishCatalogId > 0 
		BEGIN 
		 

		 DELETE FROM ZnodePublishProductEntity WHERE ElasticSearchEvent = 2 AND ZnodeCatalogId = @PublishCatalogId
		  
		 UPDATE ZnodePublishProductEntity 
		 SET ElasticSearchEvent = 0 
		 WHERE ZnodeCatalogId = @PublishCatalogId
		 		
		 UPDATE  a
		 SET IsCatalogPublished = 1, IsCategoryPublished = 1 , IsProductPublished = 1
			, PublishStateId = b.PublishStateId, a.ModifiedDate = @GetDate
		 FROM ZnodePublishCatalogLog a 
		 INNER JOIN ZnodePublishState b ON (b.PublishStateCode = a.Token )
		 WHERE a.PublishStateId = 6 AND a.PublishType = 'Catalog'
		 AND a.PimCatalogId =@PublishCatalogId
			
		UPDATE ZnodePublishProgressNotifierEntity SET 
            ProgressMark =100, 
            IsCompleted  = 1,
            IsFailed =0
        WHERE  JobId = @NewGUID
		END 
		ELSE 
		BEGIN 

		UPDATE a
		SET Name = b.name , a.SalesPrice = b.SalesPrice , a.RetailPrice = b.RetailPrice, CurrencyCode = b.CurrencyCode , CultureCode= b.CultureCode, CurrencySuffix = b.CurrencySuffix
		, CategoryName = b.CategoryName, a.ElasticSearchEvent = 0 , ZnodeParentCategoryIds = b.ZnodeParentCategoryIds , a.SKU = b.SKU , a.SKULower = b.SKULower,a.Attributes = b.Attributes
		FROM ZnodePublishProductEntity a 
		INNER JOIN ZnodePublishProductEntityLog b ON (b.PublishProductEntityId = a.PublishProductEntityId)

			   
		 UPDATE  a
		 SET IsCatalogPublished = 0, IsCategoryPublished = 0 , IsProductPublished = 0
			, PublishStateId = b.PublishStateId, a.ModifiedDate = @GetDate
		 FROM ZnodePublishCatalogLog a 
		 INNER JOIN ZnodePublishState b ON (b.PublishStateCode = 'PUBLISH_FAILED' )
		 WHERE a.PublishStateId = 6 AND a.PublishType = 'Catalog'
		-- AND a.PimCatalogId =@PublishCatalogId

		INSERT INTO ZnodeProceduresErrorLog (ProcedureName,ErrorInProcedure,ErrorMessage,ErrorLine,ErrorCall,CreatedBy,CreatedDate)
		SELECT 'Publish Catalog','Elastic Fail', ' Publish Failed due to Elastic data update failure or connection issue'
			   ,0, 'Publish', 2, @GetDate

		UPDATE ZnodePublishProgressNotifierEntity SET 
            ProgressMark =80, 
            IsCompleted  = 0,
            IsFailed =1 
        WHERE  JobId = @NewGUID

		-- TRUNCATE TABLE ZnodePublishProductEntityLog
		END 
		SET @Status = 1 

		DELETE FROM ZnodePublishProgressNotifierEntity WHERE ProgressMark = 100 OR IsCompleted = 1 OR IsFailed = 1 
		SELECT @PublishCatalogId AS id,@Status AS Status;   
END TRY 
BEGIN CATCH 
	SET @Status =0  
	 SELECT 1 AS ID,@Status AS Status;   
	 Rollback transaction
	 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePublishCatalogEntity 
		@PublishCatalogId = '+CAST(@PublishCatalogId  AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@IsRevertPublish='+CAST(@IsRevertPublish AS VARCHAR(10))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
	
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_DeletePublishCatalogEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END