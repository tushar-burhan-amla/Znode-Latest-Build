CREATE PROCEDURE [dbo].[Znode_GetPublishProducts](
	    @PublishCatalogId int= NULL
	  , @PublishCategoryId varchar(2000)= NULL
	  , @UserId int
	  , @NotReturnXML int= NULL
	  , @PimProductId TransferId Readonly
	  , @VersionId int= 0
	  , @IsDebug bit= 0
	  , @TokenId nvarchar(max)= ''
	  , @LocaleId TransferId READONLY 
	  , @PublishStateId INT = 0 
	  )
AS
    
/*
    Summary :	Publish Product on the basis of publish catalog
				Retrive all Product details with attributes and insert into following tables 
				1.	ZnodePublishedXml
				2.	ZnodePublishCategoryProduct
				3.	ZnodePublishProduct
				4.	ZnodePublishProductDetail

                Product details include all the type of products link, grouped, configure and bundel products (include addon) their associated products 
				collect their attributes and values into tables variables to process for publish.  
                
				Finally genrate XML for products with their attributes and inserted into ZnodePublishedXml Znode Admin process xml from sql server to mongodb
				one by one.

    Unit Testing
    
    SELECT * FROM ZnodePimCustomField WHERE CustomCode = 'Test'
    SELECT * FROM ZnodePimCatalogCategory WHERE pimCatalogId = 3 AND PimProductId = 181
    SELECT * FROM ZnodePimCustomFieldLocale WHERE PimCustomFieldId = 1
    SELECT * FROM ZnodePublishProduct WHERE PublishProductid = 213 = 30
    select * from znodepublishcatalog
    SELECT * FROM view_loadmanageProduct WHERE Attributecode = 'ProductNAme' AND AttributeValue LIKE '%Apple%'
    SELECT * FROM ZnodePimCategoryProduct WHERE  PimProductId = 181
    SELECT * FROM ZnodePimCatalogcategory WHERE pimcatalogId = 3 \
	DECLARE @ttr TransferId 
	INSERT INTO @ttr  
	SELECT 25719 
    EXEC Znode_GetPublishProducts  @PublishCatalogId = 3 ,@UserId= 2 ,@NotReturnXML= NULL,@PimProductId = @ttr,@IsDebug= 1 
    EXEC Znode_GetPublishProducts  @PublishCatalogId = null,@UserId= 2 ,@NotReturnXML= NULL,@IsDebug= 1  ,@PimProductId = 103
    EXEC Znode_GetPublishProducts  @PublishCatalogId =1,@UserId= 2 ,@RequiredXML= 1	
    SELECT * FROM 	ZnodePimCatalogCategory  WHERE pimcatalogId = 3  
    SELECT * FROM [dbo].[ZnodePimCategoryHierarchy]  WHERE pimcatalogId = 3 
 */
  
BEGIN
	
	BEGIN TRY
		SET NOCOUNT ON;

			 truncate table ZnodePublishedXml

			
			 DECLARE @IsCatalogPublishInProcess BIT = 0
	         DECLARE @TBL_PublishIds TABLE (PublishProductId INT , PimProductId INT , PublishCatalogId INT)
			 DECLARE @PublishStateidForPriview INT = [dbo].[Fn_GetPublishStateIdForPreview]()
			 DECLARE @DefaultLocaleId INT= Dbo.Fn_GetDefaultLocaleId()
			 
			 --User cananot modify / update UDT, Need to declare additinal table variable for editing in other store procedure.
			 DECLARE @PimProductId_Editable TransferId
			 
			
			 IF EXISTS (SELECT TOP 1 1  FROM ZnodePublishCatalogLog a 
			   INNER JOIN ZnodePimCategoryHierarchy b ON (b.PimCatalogId =a.PimCatalogId )
			   INNER JOIN ZnodePimCategoryProduct ZPCP ON b.PimCategoryId = ZPCP.PimCategoryId
			   WHERE EXISTS ( SELECT TOP 1 1 FROM @PimProductId SP WHERE  ZPCP.PimProductId = SP.Id )
			   AND a.IsCatalogPublished IS NULL 
			   ) 
			   BEGIN 
				 SET   @IsCatalogPublishInProcess =1 
			   END 
			     
	         IF (( @PublishCatalogId IS NULL  OR @PublishCatalogId = 0 ) AND @IsCatalogPublishInProcess = 0 )
			 BEGIN 
			   -- Process call single product publish
			   INSERT INTO @TBL_PublishIds 
			   EXEC [dbo].[Znode_InsertPublishProductIds] @PublishCatalogId,@userid,@PimProductId
			
			   INSERT INTO @PimProductId_Editable
			   SELECT distinct PimProductId FROM @TBL_PublishIds
			   -- initiate single product publish 
			   EXEC Znode_GetPublishSingleProduct @PublishCatalogId,@VersionId,@PimProductId_Editable,@UserId,@TokenId , @LocaleId,@PublishStateId
			  -- SELECT 	@PimProductId	
			 END 
			 ELSE IF  @IsCatalogPublishInProcess = 0 
			 BEGIN 
				-- Process call catalog publish (include category, products with multiple types)
			     EXEC [dbo].[Znode_InsertPublishProductIds] @PublishCatalogId,@userid,@PimProductId

				  ----Associated Products Publish Details
			      EXEC [Znode_PublishLatestAssociatedProduct] @PublishCatalogId, @PimProductId, @UserId, @PublishStateId 


				 EXEC Znode_GetPublishProductbulk @PublishCatalogId=@PublishCatalogId,@VersionId=@VersionId,@PimProductId=@PimProductId,@userid=@userid,@LocaleIds= @LocaleId,@PublishStateId = @PublishStateId
				 UPDATE ZnodePimProduct SET IsProductPublish = 1,PublishStateId =  @PublishStateId 
				 WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePublishProduct ZPP WHERE ZPP.PimProductId = ZnodePimProduct.PimProductId AND ZPP.PublishCatalogId = @PublishCatalogId)
			 END 
			
			 DECLARE @TBL_PublishCatalogId TABLE(PublishCatalogId INT,PublishProductId INT,PimProductId  INT , VersionId INT,LocaleId INT  );
			 INSERT INTO @TBL_PublishCatalogId 
				 SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId,MAX(PublishCatalogLogId) VersionId, ZPCP.LocaleId  
				 FROM ZnodePublishProduct ZPP 
				 LEFT JOIN ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)
				 WHERE (EXISTS (SELECT TOP 1 1 FROM @TBL_PublishIds SP WHERE SP.PimProductId = ZPP.PimProductId  ))
				 and exists (select top 1 1 from @LocaleId yu where yu.Id = zpcp.LocaleId)
				 AND IsCatalogPublished = 1 
				 AND PublishStateId = @PublishStateId
				 GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId , LocaleId 

			IF EXISTS (SELECT TOP 1 1 FROM @PimProductId WHERE ID IS NOT NULL AND ID <> '')

			--IF @PimProductId IS NOT NULL AND @PimProductId <> ''
			BEGIN
				SELECT PublishedXML ProductXml
				FROM ZnodePublishedXml ZPX
				WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PublishCatalogId TBLP WHERE TBLP.VersionId = ZPX.PublishCatalogLogId AND TBLP.PublishProductId = ZPX.PublishedId  )
				AND IsProductXML = 1
				AND @IsCatalogPublishInProcess = 0
				;

				SELECT TPC.PublishProductId  ,TPC.PublishCatalogId ,VersionId,TPC.LocaleId ,PP.SKU, ZCSD.SEOUrl
				FROM @TBL_PublishCatalogId TPC
				INNER JOIN ZnodePublishProductDetail PP ON (PP.PublishProductId = TPC.PublishProductId)
				inner join ZnodePublishCatalog ZPC ON TPC.PublishCatalogId = ZPC.PublishCatalogId
				left join ZnodePortalCatalog ZPCat on ZPC.PimCatalogId = ZPCat.PortalCatalogId
				left join ZnodeCMSSEODetail ZCSD on PP.SKU = ZCSD.SEOCode and ZPCat.PortalId = ZCSD.PortalId 
				                            AND ZCSD.CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Product')
				WHERE @IsCatalogPublishInProcess = 0
				GROUP BY TPC.PublishProductId  ,TPC.PublishCatalogId ,VersionId,TPC.LocaleId,PP.SKU, ZCSD.SEOUrl

				IF 	 @IsCatalogPublishInProcess = 1 
				BEGIN 
				SELECT 1 Id , 'Single product publish request cannot be processed as catalog or category publish is in progress. Please try after publish is complete.' MessageDetails,  CAST(0 AS BIT ) Status
				END 
				ELSE 
				BEGIN 
				SELECT 1 Id , ' Publish Successfull' MessageDetails, CAST(1 AS BIT ) Status
				END
				
		
				-- dataset for SEO implementation
				SELECT  SKU 
				FROM ZnodePublishProductDetail PPD 
				INNER JOIN @TBL_PublishCatalogId TPC ON (TPC.PublishProductId = PPD.PublishProductId )
				WHERE EXISTS (SELECT TOP 1 1 FROM @PimProductId tb WHERE tb.ID = TPC.PimProductId)
				AND PPD.localeid = @DefaultLocaleId
				GROUP BY SKU;
				 

			END
			
			if object_id('tempdb..#Cte_PublishCatalog') is not null
				drop table #Cte_PublishCatalog

			CREATE TABLE #Cte_PublishCatalog(PublishCatalogLogId INT, LocaleId INT, PublishCatalogId int)
			
			 insert into #Cte_PublishCatalog
			 SELECT max(PublishCatalogLogId) PublishCatalogLogId,a.LocaleId,PublishCatalogId 
			 FROM ZnodePublishCatalogLog  a
			 WHERE a.PublishCatalogId = @PublishCatalogId
			 GROUP BY LocaleId,PublishCatalogId			
			

			
			UPDATE ZnodePublishCatalogLog 
			SET IsProductPublished = 1 
			,PublishProductId = (SELECT  COUNT(DISTINCT PublishProductId) FROM ZnodePublishCategoryProduct ZPP WHERE ZPP.PublishCatalogId = ZnodePublishCatalogLog.PublishCatalogId AND ZPP.PublishCategoryId IS NOT NULL) 
			WHERE EXISTS (SELECT TOP 1 1 FROM #Cte_PublishCatalog  TY  WHERE  TY.PublishCatalogLogId =ZnodePublishCatalogLog.PublishCatalogLogId )  


			DECLARE @PimAttributeIsPublish VARCHAR(50) =  [dbo].[Fn_GetAttributeIsPublish]() 

			UPDATE ZnodePimProduct 
			SET PublishStateId =  @PublishStateId
			WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PublishCatalogId ZPP WHERE ZPP.PimProductId = ZnodePimProduct.PimProductId)

			--END 
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE(), ERROR_PROCEDURE();
	
		DECLARE @Status bit;
		SET @Status = 0;
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_GetPublishProducts @PublishCatalogId = '+CAST(@PublishCatalogId AS varchar(max))+',@PublishCategoryId='+@PublishCategoryId+',@UserId='+CAST(@UserId AS Varchar(50))+',@NotReturnXML='+CAST(@NotReturnXML AS Varchar(50))+',@UserId = '+CAST(@UserId AS Varchar(50))+',

		@VersionId='+CAST(@VersionId AS Varchar(50))+',@TokenId='+CAST(@TokenId AS varchar(max))+',@Status='+CAST(@Status AS varchar(10));
		SELECT 0 AS ID, CAST(0 AS bit) AS Status;
		
		if object_id('tempdb..#Cte_PublishCatalog') is not null
			UPDATE ZnodePublishCatalogLog 
			SET IsCatalogPublished = 0 
			,IsCategoryPublished = 0,IsProductPublished= 0  
			WHERE EXISTS (SELECT TOP 1 1 from #Cte_PublishCatalog TR WHERE TR.PublishCatalogLogId = ZnodePublishCatalogLog.PublishCatalogLogId) 
			EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_GetPublishProducts', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END;