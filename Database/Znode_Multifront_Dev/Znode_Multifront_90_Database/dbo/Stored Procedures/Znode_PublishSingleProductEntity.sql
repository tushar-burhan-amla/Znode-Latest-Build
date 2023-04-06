CREATE PROCEDURE [dbo].[Znode_PublishSingleProductEntity]
(
    @PimProductId TransferId READONLY
   ,@RevisionType   Varchar(30) = ''
   ,@UserId int = 0
   ,@IsAutoPublish bit = 0 
   ,@ImportGUID Varchar(500) = '' 
)
AS
/*
	To publish all catalog product and their details
	Unit Testing : 
	*/
BEGIN
BEGIN TRY 
SET NOCOUNT ON
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	Declare @Status Bit =0 
	DECLARE @PimProductIdIn INT , @PimCatalogId INT 

	SET @ImportGUID = CASE WHEN @ImportGUID = '' THEN CAST( NEWID() AS VARCHAR(200)) ELSE @ImportGUID END 

	DECLARE @PimCatalogProduct TABLE (PimProductId INT , IsDeleted BIT,  ZnodecatalogId INT  , VersionId INT )

	IF EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryProduct a 
	INNER JOIN ZnodePimCategoryHierarchy b ON (b.PimCategoryId = a.PimCategoryId)
	INNER JOIN ZnodePublishCatalogEntity y ON (y.ZnodeCatalogId = b.PimCatalogId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @PimProductId t WHERE t.Id = a.PimProductId)
	)
	-- AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishCatalogLog nt WHERE nt.PublishStateId = 6 )
	-- AND NOT EXISTS (SELECT TOP  1 1 FROM ZnodePublishProgressNotifierEntity )
	BEGIN 



	DECLARE Cur_CatalogProduct CURSOR FOR 
	SELECT DISTINCT PimProductId, b.PimCatalogId
	FROM ZnodePimCategoryProduct a 
	INNER JOIN ZnodePimCategoryHierarchy b ON (b.PimCategoryId = a.PimCategoryId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @PimProductId t WHERE t.Id = a.PimProductId)
	OPEN Cur_CatalogProduct 

	FETCH NEXT FROM Cur_CatalogProduct INTO @PimProductIdIn,@PimCatalogId

	WHILE @@FETCH_STATUS=0 
	BEGIN 

	

	INSERT INTO @PimCatalogProduct 
	EXEC Znode_PublishCatalogEntity @PimCatalogId= @PimCatalogId
									,@RevisionState =@RevisionType
									,@UserId =@UserId
									,@NewGUID = @ImportGUID
									,@IsDraftProductsOnly = 0 
									,@PimProductId =@PimProductIdIn
									,@PimCategoryHierarchyId = 0 



	FETCH NEXT FROM Cur_CatalogProduct INTO @PimProductIdIn,@PimCatalogId
	END 
	CLOSE Cur_CatalogProduct
	DEALLOCATE Cur_CatalogProduct 

	   	  
	SET @Status =1 

	END 
	ELSE 
	BEGIN 


	SET @Status = 0 
	END 


	SELECT * 
	FROM ZnodePublishProductEntity a with (nolock)
	WHERE EXISTS (SELECT TOP 1 1 FROM @PimCatalogProduct r WHERE r.VersionId = a.VersionId AND r.PimProductId = a.ZnodeProductId AND r.IsDeleted = 0)

	SELECT PimProductId ZnodeProductId , IsDeleted ,  ZnodecatalogId   , VersionId 
	FROM @PimCatalogProduct 
	WHERE IsDeleted =1 

		UPDATE ZnodePimProduct 
	SET PublishStateId =  CASE WHEN @RevisionType= 'None' OR @RevisionType= 'Production'  THEN 3 ELSE 4 END 
	WHERE PublishStateId IN (1,2)
	AND PimProductId IN ( SELECT PimProductId FROM  @PimCatalogProduct ) 



	UPDATE ZnodePublishCatalogLog 
	SET PublishStateId = CASE WHEN @RevisionType= 'None' OR @RevisionType= 'Production'  THEN 3 ELSE 4 END 
	, IsCatalogPublished = 1 
	WHERE PublishType= 'Product'
	
	UPDATE ZnodePublishProgressNotifierEntity 
	SET IsCompleted = 1 ,ProgressMark = 100 
	WHERE JobId = @ImportGUID

	SELECT 0 AS id,@Status AS Status;   

	DELETE FROM ZnodePublishProgressNotifierEntity  	WHERE JobId = @ImportGUID

END TRY 
BEGIN CATCH 
	SET @Status =0  
	 SELECT 1 AS ID,@Status AS Status;   
	 SELECT ERROR_MESSAGE()
	 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PublishSingleProductEntity
		@PimProductIds = '',@UserId='+CAST(@UserId AS VARCHAR(50))+',@RevisionType='+CAST(@RevisionType AS VARCHAR(10))
			
	INSERT INTO ZnodePublishSingleProductErrorLogEntity
	(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
	SELECT 'PublishSingleProductEntity', '' + isnull(@ErrorMessage,'') , 'Fail' , @GetDate, 
	@UserId ,''


	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_PublishSingleProductEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END