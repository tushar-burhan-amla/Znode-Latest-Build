CREATE PROCEDURE [dbo].[Znode_GetPublishCategoryProducts]
( @pimCatalogId int = 0,@pimCategoryHierarchyId int = 0,@userId int,@versionId int= 0,@status int = 0 OUT,@isDebug bit = 0 ,@LocaleId TransferId READONLY , @PublishStateId INT = 0,   @ProductPublishStateId INT=NULL)  AS  /*
    Summary :	Publish Product on the basis of publish catalog and category
				Calling sp [Znode_InsertPublishProductIds] to retrive category and their child category with associated products 
				 
				1.	ZnodePublishedXml
				2.	ZnodePublishCategoryProduct
				3.	ZnodePublishProduct
				4.	ZnodePublishProductDetail

                Product details include all the type of products link, grouped, configure and bundel products (include addon) their associated products 
				collect their attributes and values into tables variables to process for publish.  
                
				Finally genrate XML for products with their attributes and inserted into ZnodePublishedXml Znode Admin process xml from sql server to mongodb
				one by one.
	
	Unit Testing
    ------------------------------------------------------------------------------------------------
	Declare @Status int 
	DECLARE @r transferid 
	INSERT INTO @r
	VALUES (1)
	,(24)
	EXEC [Znode_GetPublishCategoryProducts]  @PimCatalogId = 9
	, @PimCategoryHierarchyId = 48 
	, @UserId = 2 
	, @VersionId = 0
	, @IsDebug = 1
	, @Status  = @Status  out
	,@localeId = @r
	,@PublishStateId = 4
	Select @Status  

 */
BEGIN   
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
			DECLARE @IsCatalogPublishInProcess INT  = 0
		DECLARE @tBL_PublishIds table (PublishProductId int,PimProductId int,PublishCatalogId int)
		DECLARE @publishCatalogId int= isnull((SELECT TOP 1 PublishCatalogId FROM ZnodePublishCatalog ZPC WHERE ZPC.PimCatalogId = @pimCatalogId),0),@publishCataloglogId int= 0;
		DECLARE @tBL_CategoryCategoryHierarchyIds table (CategoryId int,ParentCategoryId int ,PimCategoryHierarchyId INT ,ParentPimCategoryHierarchyId INT  )
		DECLARE @pimProductId TransferId
		DECLARE @insertPublishProductIds table (PublishProductId int,PimProductId int,PublishCatalogId int )

		SELECT @versionId = max(PublishCataloglogId)
		FROM ZnodePublishCatalogLog 
		WHERE PublishCatalogId =@publishCatalogId

		INSERT INTO @tBL_CategoryCategoryHierarchyIds(CategoryId,ParentCategoryId,PimCategoryHierarchyId,ParentPimCategoryHierarchyId ) 
		SELECT DISTINCT PimCategoryId, Null,PimCategoryHierarchyId,NULL  FROM ( SELECT PimCategoryId,ParentPimCategoryId,PimCategoryHierarchyId,ParentPimCategoryHierarchyId
		FROM DBO.[Fn_GetRecurciveCategoryIds_PimCategoryHierarchy](@pimCategoryHierarchyId,@pimCatalogId) 
		UNION SELECT PimCategoryId, Null,PimCategoryHierarchyId,NULL FROM ZnodePimCategoryHierarchy WHERE PimCategoryHierarchyId = @pimCategoryHierarchyId 
		UNION SELECT PimCategoryId, Null,PimCategoryHierarchyId,NULL  FROM dbo.[Fn_GetRecurciveCategoryIds_PimCategoryHierarchyIdnew] (@pimCategoryHierarchyId,@pimCatalogId) ) Category

		IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryProduct ty inner join ZnodePimCategoryHierarchy ZPCH ON ty.PimCategoryId = ZPCH.PimCategoryId
		WHERE EXISTS (SELECT TOP 1 1 FROM ( SELECT PimCategoryId,PimCategoryHierarchyId,ParentPimCategoryHierarchyId 
		FROM dbo.[Fn_GetRecurciveCategoryIds_ForChild](@pimCategoryHierarchyId,@pimCatalogId) UNION ALL SELECT NULL ,@pimCategoryHierarchyId,NULL  ) TN WHERE TN.PimCategoryHierarchyId = ZPCH.PimCategoryHierarchyId  ) AND ty.PimProductId IS NOT NULL )
		BEGIN 
			SET @IsCatalogPublishInProcess = 2 

		END 

		IF (isnull(@publishCatalogId,0) = 0 ) 
			BEGIN 
				SET @status = 1
				-- Catalog Not Published 
				RETURN 0;
			END
			
		IF @IsCatalogPublishInProcess =  0 
		BEGIN 
			-- Any other catalog was in process dont intitiate category publish	
			EXEC [Znode_GetPublishCategoryGroup] @publishCatalogId = @PublishCatalogId,@VersionId = 0,@userId =2,@isDebug = 1,@PimCategoryHierarchyId = @PimCategoryHierarchyId,@localeId =@localeID,@PublishStateId=@PublishStateId
			
		BEGIN 
			INSERT INTO @insertPublishProductIds EXEC [Dbo].[Znode_InsertPublishProductIds] @publishCatalogId = @publishCatalogId,@userid = @userid,@pimProductId = @pimProductId,@pimCategoryHierarchyId = @pimCategoryHierarchyId
			INSERT INTO @pimProductId SELECT PimProductId FROM @insertPublishProductIds

			EXEC [Dbo].[Znode_GetPublishProductbulk] @publishCatalogId = @publishCatalogId,@versionId = @versionId,@pimProductId = @pimProductId,@userid = @userid,@pimCategoryHierarchyId = @pimCategoryHierarchyId,@pimCatalogId = @pimCatalogId,@localeIds = @localeId ,@publishstateId =@publishStateId
		END
		DECLARE @tBL_PublishCatalogId table(PublishCatalogId int,PublishProductId int,PublishCategoryId int,PimProductId int,VersionId int,LocaleId INT  );
			
		INSERT INTO @tBL_PublishCatalogId (PublishCatalogId,PublishProductId,PublishCategoryId,PimProductId,VersionId ,Localeid)  
		SELECT DISTINCT ZPC.PublishCatalogId,ZPX.PublishProductId,ZPX.PublishCategoryId,ZPP.PimProductId,Max(TH.PublishCatalogLogId),TH.Localeid 
		FROM ZnodePublishCategory ZPC 
		INNER JOIN ZnodePublishCatalogLog TH ON (TH.PublishCatalogId = ZPC.PublishCatalogId)
		INNER JOIN @tBL_CategoryCategoryHierarchyIds CTC ON (ZPC.PimCategoryHierarchyId = CTC .PimCategoryHierarchyId )
		INNER JOIN ZnodePublishCategoryProduct ZPX  ON ZPC.PublishCategoryId = ZPX.PublishCategoryId AND ZPX.PublishCatalogId = ZPC.PublishCatalogId 
		INNER JOIN ZnodePublishProduct ZPP ON ZPP.PublishCatalogId = ZPC.PublishCatalogId AND ZPX.PublishProductId = ZPP.PublishProductId 
		WHERE ZPC.PublishCatalogId = @PublishCatalogId 
		AND  TH.PublishStateId = @PublishStateId
		AND EXISTS (SELECT TOP 1 1 FROM @LocaleId WHERE id = TH.LocaleId)
		GROUP BY ZPC.PublishCatalogId,ZPX.PublishProductId ,ZPX.PublishCategoryId,ZPP.PimProductId,TH.Localeid 
		
		INSERT INTO @tBL_PublishCatalogId (PublishCatalogId,PublishProductId,PublishCategoryId,PimProductId,VersionId,Localeid ) 
		SELECT IPP.PublishCatalogId,IPP.PublishProductId,0,IPP.PimProductId,max(PublishCatalogLogId) VersionId ,h.Localeid
		FROM @insertPublishProductIds IPP 
		LEFT JOIN ZnodePublishCatalogLog h ON (h.PublishCatalogId = IPP.PublishCatalogId )
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM @tBL_PublishCatalogId PCI WHERE IPP.PublishProductId = PCI.PublishProductId)
		AND EXISTS (SELECT TOP 1 1 FROM @LocaleId WHERE id = h.LocaleId)
		AND h.PublishStateId = @PublishStateId
		GROUP BY IPP.PublishCatalogId,IPP.PublishProductId,IPP.PimProductId,Localeid
			
		UPDATE ZnodePublishCatalogLog 
		SET IsProductPublished = 1,
			PublishProductId = (SELECT  COUNT(DISTINCT PublishProductId) FROM ZnodePublishCategoryProduct ZPP WHERE ZPP.PublishCatalogId = ZnodePublishCatalogLog.PublishCatalogId AND ZPP.PublishCategoryId IS NOT NULL)  
		,ModifiedBy=@userId, ModifiedDate=@GetDate
		WHERE PublishCatalogLogId IN (SELECT VersionId FROM @tBL_PublishCatalogId)

		UPDATE ZnodePimProduct 
		SET IsProductPublish = 1 ,PublishStateId = ISNULL(@ProductPublishStateId,@PublishStateId),
		ModifiedBy=@userId, ModifiedDate=@GetDate	
		WHERE EXISTS (SELECT TOP 1 1 
			FROM @tBL_PublishCatalogId ZPP
			WHERE ZPP.PimProductId = ZnodePimProduct.PimProductId
			)
		
			SELECT PublishCatalogId
				,PublishProductId
				,PublishCategoryId
				,VersionId,LocaleId
		FROM @tBL_PublishCatalogId
		END 
			IF @IsCatalogPublishInProcess = 1 
			BEGIN 
			SELECT 1 Id , 'Single category publish request cannot be processed as catalog or category publish is in progress. Please try after publish is complete.' MessageDetails,  CAST(0 AS BIT ) Status
			END 
			ELSE
				IF @IsCatalogPublishInProcess = 2 
			BEGIN
				
			SELECT 1 Id , 'Please associate products to the category or to at least one child category to publish the category.' MessageDetails,  CAST(0 AS BIT ) Status
			END 
			ELSE 
			BEGIN 
			SELECT 1 Id , ' Publish Successfull' MessageDetails, CAST(1 AS BIT ) Status
			END 
	END TRY
	BEGIN CATCH
		SELECT error_message()
			,error_procedure();
		UPDATE ZnodePublishCatalogLog 
		SET IsCatalogPublished = 0 
		WHERE PublishCatalogLogId = @versionId
		SET @status = 0;
		DECLARE @error_procedure varchar(1000)= error_procedure(),@errorMessage nvarchar(max)= error_message(),@errorLine varchar(100)= error_line(),@errorCall nvarchar(max)= 'EXEC Znode_GetPublishProducts @PimCatalogId = '+cast(@pimCatalogId AS varchar(max))+',@@PimCategoryHierarchyId='+@pimCategoryHierarchyId+',@UserId='+cast(@userId AS varchar(50))+',@UserId = '+cast(@userId AS varchar(50))+',@VersionId='+cast(@versionId AS varchar(50))+',@Status='+cast(@status AS varchar(10));
		SELECT 0 AS ID
			,cast(0 AS bit) AS Status;
		--ROLLBACK TRAN GetPublishProducts;
		EXEC Znode_InsertProcedureErrorLog @procedureName = 'Znode_GetPublishCategoryProducts',@errorInProcedure = @error_procedure,@errorMessage = @errorMessage,@errorLine = @errorLine,@errorCall = @errorCall;
	END CATCH;
END;