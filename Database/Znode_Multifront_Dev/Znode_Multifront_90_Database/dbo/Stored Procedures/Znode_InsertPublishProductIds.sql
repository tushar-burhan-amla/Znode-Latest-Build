CREATE PROCEDURE [dbo].[Znode_InsertPublishProductIds]
(
	@PublishCatalogId           INT            = NULL,
	@UserId                     INT				  ,
	@PimProductId               TransferId Readonly,
	@IsCallAssociated           BIT           = 0,
	@PimCategoryHierarchyId	 INT		   = 0  ,
	@IsDebug					 INT		   = 0     
)
AS
    
/*
  Summary :	Publish Product on the basis of publish catalog
				Retrive all Product details with attributes and INSERT INTO following tables 
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
	 SELECT * FROM ZnodePimCatalogcategory WHERE pimcatalogId = 3 
     EXEC Znode_GetPublishProducts  @PublishCatalogId = 5 ,@UserId= 2 ,@NotReturnXML= NULL,@PimProductId = 117,@IsDebug= 1 
	 	DECLARE @ttr TransferId 
	INSERT INTO @ttr  
	SELECT 25719 
     EXEC Znode_InsertPublishProductIds  @PublishCatalogId = 3,@UserId= 2  ,@PimProductId = @ttr  ,@IsDebug= 1 
     EXEC Znode_GetPublishProducts  @PublishCatalogId =1,@UserId= 2 ,@RequiredXML= 1	
	 SELECT * FROM 	ZnodePimCatalogCategory  WHERE pimcatalogId = 3  
	 SELECT * FROM [dbo].[ZnodePimCategoryHierarchy]  WHERE pimcatalogId = 3 
    */ 

BEGIN
--  BEGIN TRAN InsertPublishProductIds;
BEGIN TRY
SET NOCOUNT ON;
		
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate(); 
	DECLARE @PimCatalogId int= ISNULL((SELECT PimCatalogId FROM ZnodePublishcatalog WHERE PublishCatalogId = @PublishCatalogId), 0);  --- this variable is used to carry y pim catalog id by using published catalog id
	DECLARE 
	@ProductNamePimAttributeId INT = dbo.Fn_GetProductNameAttributeId(),
	@DefaultLocaleId INT= Dbo.Fn_GetDefaultLocaleId(),
	@LocaleId INT = 0,
	@SkuPimAttributeId  INT =  dbo.Fn_GetProductSKUAttributeId(), 
	@IsActivePimAttributeId INT =  dbo.Fn_GetProductIsActiveAttributeId(),
	@ProductTypeAttributeId INT = dbo.Fn_GetProductTypeAttributeId()

	DECLARE @TBL_LocaleId  TABLE (RowId INT IDENTITY(1,1) PRIMARY KEY  , LocaleId INT )

	INSERT INTO @TBL_LocaleId (LocaleId) 
	SELECT  LocaleId
	FROM ZnodeLocale MT 
	WHERE IsActive = 1
	AND EXISTS(SELECT * FROM ZnodePortalCatalog ZPC 
			INNER JOIN ZnodePortalLocale ZPL ON ZPC.PortalId = ZPL.PortalId
			WHERE ZPC.PublishCatalogId = @PublishCatalogId AND MT.LocaleId = ZPL.LocaleId )
	
	IF NOT EXISTS(SELECT * FROM @TBL_LocaleId)
		INSERT INTO @TBL_LocaleId  
		SELECT  LocaleId
		FROM ZnodeLocale 
		WHERE IsActive = 1-- AND IsDefault = 1
			 
	-- This variable used to carry the locale in loop 
	-- This variable is used to carry the default locale which is globaly set
	DECLARE @Counter INT =1 ,@maxCountId INT = (SELECT max(RowId) FROM @TBL_LocaleId ) 
	DECLARE @DeletePublishProductId VARCHAR(MAX)= '', @PimProductIds VARCHAR(MAX)= '', @PimAttributeId VARCHAR(MAX)= '';
	DECLARE @TBL_CategoryHierarchyIds TABLE (CategoryId int,ParentCategoryId int ) 
	DECLARE @TBL_PublishCategoryIds TABLE (PublishCategoryId  int ) 
		
	-- This table will used to hold the all currently active locale ids  
			 
	IF Object_ID ('tempdb..#ActiveProduct') is not null
		DROP TABLE #ActiveProduct

	IF Object_ID ('tempdb..#TBL_PimProductIds') is not null
		DROP TABLE #TBL_PimProductIds
		
	IF Object_ID ('tempdb..#ZnodePublishCategoryProduct') is not null
		DROP TABLE #ZnodePublishCategoryProduct;

	SELECT ZPCP.PimCategoryId, ZPCP.PimProductId, ZPC.PublishCategoryId,ZPP.PublishProductId
	INTO #ZnodePublishCategoryProduct
	FROM [dbo].[ZnodePimCategoryProduct] ZPCP
	INNER JOIN ZnodePublishCategory ZPC ON ZPCP.PimCategoryId = ZPC.PimCategoryId
	INNER JOIN ZnodePublishProduct ZPP ON ZPCP.PimProductId = ZPP.PimProductId
	WHERE EXISTS( SELECT TOP 1 1 FROM @PimProductId SP WHERE SP.Id = ZPCP.PimProductId)

	DELETE FROM ZnodePublishCategoryProduct
	WHERE NOT EXISTS (SELECT TOP 1 1 from #ZnodePublishCategoryProduct Z where Z.PublishProductId = ZnodePublishCategoryProduct.PublishProductId and Z.PublishCategoryId = ZnodePublishCategoryProduct.PublishCategoryId)
	AND EXISTS(SELECT TOP 1 1 FROM #ZnodePublishCategoryProduct Z where Z.PublishProductId = ZnodePublishCategoryProduct.PublishProductId)
	
	-- This table hold the complete xml of product with other information like category and catalog
	CREATE TABLE #TBL_PimProductIds(PimProductId INT  ,PimCategoryId INT,PimCatalogId INT,PublishCatalogId INT,IsParentProducts BIT ,DisplayOrder INT,ProductName NVARCHAR(MAX),SKU  NVARCHAR(MAX),
		IsActive NVARCHAR(MAX),PimAttributeFamilyId INT ,ProfileId   VARCHAR(MAX),CategoryDisplayOrder INT ,ProductIndex INT,PimCategoryHierarchyId INT ,PRIMARY KEY (PimCatalogId,PimCategoryId,PimCategoryHierarchyId,PimProductId)  )

	-- This table is used to hold the product which publish in current process 
	Create TABLE #TBL_PublishProductIds (PublishProductId  INT  ,PimProductId INT,PublishCatalogId  INT
			,PublishCategoryId VARCHAR(MAX),CategoryProfileIds VARCHAR(max),VersionId INT , PRIMARY KEY (PimProductId,PublishProductId,PublishCatalogId)); 
	 
	--Retrive category data : parent / client
	---------------
	-- this check is used when this procedure is call by internal procedure to publish only product and no need to return publish xml;    
	--Collected list of products for  publish 
       
	If @PimCategoryHierarchyId = 0 
	BEGIN

		INSERT INTO #TBL_PimProductIds ( PimProductId, PimCategoryId, IsParentProducts, DisplayOrder, PimCatalogId,CategoryDisplayOrder,PublishCatalogId,PimCategoryHierarchyId )
		SELECT DISTINCT ZPCC.PimProductId, ZPCC.PimCategoryId, 1 AS IsParentProducts, NULL AS DisplayOrder, ZPCH.PimCatalogId,ZPCC.DisplayOrder ,ZPC.PublishCatalogId,ISNULL(ZPCH.PimCategoryHierarchyId,0)
		FROM ZnodePimCategoryProduct AS ZPCC
		INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC.PimCategoryId = ZPCH.PimCategoryId
		INNER JOIN ZnodePublishCatalog ZPC ON ZPC.PimCatalogId = ZPCH.PimCatalogId
		WHERE  (ZPCH.PimCatalogId = @PimCatalogId OR EXISTS( SELECT TOP 1 1 FROM @PimProductId SP WHERE SP.Id = ZPCC.PimProductId) ) AND ZPCC.PimProductId IS NOT NULL
		--AND EXISTS ( SELECT * FROM #ActiveProduct PAV WHERE ZPCC.PimProductId = PAV.PimProductId )

	END
	ELSE
	BEGIN
				
		INSERT INTO @TBL_CategoryHierarchyIds(CategoryId , ParentCategoryId )
		Select Distinct PimCategoryId , Null FROM (
		SELECT PimCategoryId,ParentPimCategoryId from DBO.[Fn_GetRecurciveCategoryIds](@PimCategoryHierarchyId,@PimCatalogId)
		Union 
		Select PimCategoryId , null  from ZnodePimCategoryHierarchy where PimCategoryHierarchyId = @PimCategoryHierarchyId 
		Union 
		Select PimCategoryId , null  from [Fn_GetRecurciveCategoryIds_new] (@PimCategoryHierarchyId,@PimCatalogId) ) Category  

		INSERT INTO  @TBL_PublishCategoryIds 
		select ZPC.PublishCategoryId from ZnodePublishCategory ZPC 
		Inner join  @TBL_CategoryHierarchyIds CT1 On 
		ZPC.PimCategoryId = CT1.CategoryId 	
			
		INSERT INTO #TBL_PimProductIds ( PimProductId, PimCategoryId, IsParentProducts, DisplayOrder, PimCatalogId,CategoryDisplayOrder,PublishCatalogId,PimCategoryHierarchyId )
		SELECT DISTINCT ZPCC.PimProductId, ZPCC.PimCategoryId, 1 AS IsParentProducts, NULL AS DisplayOrder, ZPCH.PimCatalogId,ZPCC.DisplayOrder ,ZPC.PublishCatalogId,ISNULL(ZPCH.PimCategoryHierarchyId,0)
		FROM ZnodePimCategoryProduct AS ZPCC
		INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC.PimCategoryId = ZPCH.PimCategoryId
		INNER JOIN ZnodePublishCatalog ZPC ON ZPC.PimCatalogId = ZPCH.PimCatalogId
		WHERE  (ZPCH.PimCatalogId = @PimCatalogId OR EXISTS( SELECT TOP 1 1 FROM @PimProductId SP WHERE SP.Id = ZPCC.PimProductId) ) AND ZPCC.PimProductId IS NOT NULL
		--AND EXISTS ( SELECT * FROM #ActiveProduct PAV WHERE ZPCC.PimProductId = PAV.PimProductId )
		AND (ZPCC.PimCategoryId in(Select CategoryId from @TBL_CategoryHierarchyIds) ) 


		SELECT ZPCP.PublishCatalogId,THO.PimProductId,PimCategoryHierarchyId,ProductIndex
		INTO #TBL_PublishCategoryProduct 
		FROM ZnodePublishCategoryProduct ZPCP 
		INNER JOIN ZnodePublishProduct THO ON (THO.PublishProductId = ZPCP.PublishProductId  AND ZPCP.PublishCatalogId = THO.PublishCatalogId)
		WHERE ZPCP.PublishCatalogId = @PublishCatalogId
		AND EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductIds TYU WHERE TYU.PimProductId  =  THO.PimProductId )

		UPDATE  #TBL_PimProductIds 
		SET ProductIndex = CASE WHEN EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCategoryProduct TH WHERE TH.PimProductId = #TBL_PimProductIds.PimProductId 
		AND #TBL_PimProductIds.PimCategoryHierarchyId = TH.PimCategoryHierarchyId  ) THEN (SELECT TOP  1 ProductIndex FROM #TBL_PublishCategoryProduct TM WHERE TM.PimProductId = #TBL_PimProductIds.PimProductId 
		AND #TBL_PimProductIds.PimCategoryHierarchyId = TM.PimCategoryHierarchyId  )

		WHEN EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCategoryProduct TH WHERE TH.PimProductId = #TBL_PimProductIds.PimProductId 
		AND #TBL_PimProductIds.PimCategoryHierarchyId <> TH.PimCategoryHierarchyId  )  
		THEN (SELECT TOP  1 MAX(isnull(ProductIndex,0))+1  FROM #TBL_PublishCategoryProduct TM1 WHERE TM1.PimProductId = #TBL_PimProductIds.PimProductId 
		) ELSE  1  END 
			
	END
					
	--Collected list of link products for  publish
	INSERT INTO #TBL_PimProductIds( PimProductId, PimCategoryId, IsParentProducts, DisplayOrder, PimCatalogId , PublishCatalogId,PimCategoryHierarchyId)
	SELECT ZPLPD.PimProductId, Isnull(ZPCC.PimCategoryId,0), 0 AS IsParentProducts, NULL AS DisplayOrder, CTPP.PimCatalogId,CTPP.PublishCatalogId,isnull(ZPCH.PimCategoryHierarchyId,0)
	FROM ZnodePimLinkProductDetail AS ZPLPD
	INNER JOIN #TBL_PimProductIds AS CTPP ON ZPLPD.PimParentProductId = CTPP.PimProductId AND  IsParentProducts = 1 
	LEFT JOIN ZnodePimCategoryProduct AS ZPCC ON ZPCC.PimProductId = ZPLPD.PimProductId 
	LEFT JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCH.PimCatalogId = CTPP.PimCatalogId and ZPCC.PimCategoryId = ZPCH.PimCategoryId
	WHERE NOT EXISTS ( SELECT TOP 1 1 FROM #TBL_PimProductIds AS CTPPI WHERE CTPPI.PimProductId = ZPLPD.PimProductId) 
	GROUP BY ZPLPD.PimProductId, ZPCC.PimCategoryId,CTPP.PimCatalogId,CTPP.PublishCatalogId ,ZPCH.PimCategoryHierarchyId

		
	--Collected list of Addon products for  publish
  
	INSERT INTO #TBL_PimProductIds( PimProductId, PimCategoryId, IsParentProducts, DisplayOrder, PimCatalogId,PublishCatalogId,PimCategoryHierarchyId)
	SELECT ZPAPD.PimChildProductId, ISNULL(ZPCC.PimCategoryId,0) AS PublishCategoryId, 0 AS IsParentProducts, null AS DisplayOrder,CTALP.PimCatalogId,CTALP.PublishCatalogId,ISNULL(ZPCH.PimCategoryHierarchyId,0)
	FROM ZnodePimAddOnProductDetail AS ZPAPD 
	INNER JOIN ZnodePimAddOnProduct AS ZPAP ON ZPAP.PimAddOnProductId = ZPAPD.PimAddOnProductId
	INNER JOIN #TBL_PimProductIds AS CTALP ON CTALP.PimProductId = ZPAP.PimProductId AND  IsParentProducts = 1
	LEFT JOIN ZnodePimCategoryProduct AS ZPCC ON ZPCC.PimProductId = ZPAPD.PimChildProductId 
	LEFT JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCH.PimCatalogId = CTALP.PimCatalogId and ZPCH.PimCategoryId = ZPCC.PimCategoryId
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductIds AS CTALPI WHERE CTALPI.PimProductId = ZPAPD.PimChildProductId) 
	GROUP BY ZPAPD.PimChildProductId, ZPCC.PimCategoryId , CTALP.PimCatalogId,CTALP.PublishCatalogId,ZPCH.PimCategoryHierarchyId

	
	--Collected list of Bundle / Group / Config products for  publish
	INSERT INTO #TBL_PimProductIds(PimProductId,PimCategoryId,IsParentProducts,DisplayOrder,PimCatalogId,PublishCatalogId,PimCategoryHierarchyId)
	SELECT ZPTA.PimProductId,ISNULL(ZPCC.PimCategoryId,0),0 AS IsParentProducts,NULL DisplayOrder,CTAAP.PimCatalogId,CTAAP.PublishCatalogId,ISNULL(ZPCH.PimCategoryHierarchyId,0)
	FROM ZnodePimProductTypeAssociation AS ZPTA INNER JOIN #TBL_PimProductIds AS CTAAP ON CTAAP.PimProductId = ZPTA.PimParentProductId AND IsParentProducts = 1
	LEFT JOIN ZnodePimCategoryProduct AS ZPCC ON ZPCC.PimProductId = ZPTA.PimProductId 
	LEFT JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCH.PimCatalogId = CTAAP.PimCatalogId AND ZPCC.PimCategoryId = ZPCH.PimCategoryId
	WHERE NOT EXISTS( SELECT TOP 1 1 FROM #TBL_PimProductIds AS CTAAPI WHERE CTAAPI.PimProductId = ZPTA.PimProductId)
	GROUP BY ZPTA.PimProductId,ZPCC.PimCategoryId,CTAAP.PimCatalogId,CTAAP.PublishCatalogId,ZPCH.PimCategoryHierarchyId
        				

	UPDATE TBPP
	SET PublishCatalogId = ZPC.PublishCatalogId 
	FROM #TBL_PimProductIds TBPP 
	INNER JOIN ZnodePublishCatalog ZPC ON ZpC.PimCatalogId = TBPP.PimCatalogId;
        
	DECLARE @PublishProductId TRANSFERId 
				

	IF @PublishCatalogId IS NOT NULL AND @PublishCatalogId <> 0 
	BEGIN
		If @PimCategoryHierarchyId = 0 
		BEGIN
									 				 	
			INSERT INTO @PublishProductId
			SELECT DISTINCT ZPP.PublishProductId 
			FROM ZnodePublishProduct AS ZPP 
			Left JOIN ZnodePublishCategoryProduct ZPPC ON (ZPPC.PublishProductId = ZPP.PublishProductId AND ZPPC.PublishCatalogId = ZPP.PublishCatalogId)
			--INNER JOIN ZnodePublishCategory ZPC ON (ZPC.PublishCategoryId = ZPPC.PublishCategoryId)
			WHERE NOT EXISTS
			(SELECT TOP 1 1 FROM #TBL_PimProductIds AS TBP WHERE ZPP.PimProductId = TBP.PimProductId 
			AND TBP.PublishCatalogId = ZPP.PublishCatalogId 
			AND ISNULL(TBP.PimCategoryHierarchyId,0) = ISNULL(ZPPC.PimCategoryHierarchyId,0) )
			AND ZPP.PublishCatalogId = @PublishCatalogId
			--Remove extra products from catalog
				
		END
		ELSE 
		BEGIN
		
			INSERT INTO @PublishProductId
			SELECT DISTINCT ZPP.PublishProductId 
			FROM ZnodePublishProduct AS ZPP 
			INNER JOIN ZnodePublishCategoryProduct ZPPC ON (ZPPC.PublishProductId = ZPP.PublishProductId AND ZPPC.PublishCatalogId = ZPP.PublishCatalogId)
			INNER JOIN ZnodePublishCategory ZPC ON (ZPC.PublishCatalogId = ZPPC.PublishCatalogId  AND   ZPC.PublishCategoryId = ZPPC.PublishCategoryId)
			WHERE NOT EXISTS
			(SELECT TOP 1 1 FROM #TBL_PimProductIds AS TBP WHERE ZPP.PimProductId = TBP.PimProductId 
			AND TBP.PublishCatalogId = ZPP.PublishCatalogId 
			AND ISNULL(TBP.PimCategoryHierarchyId,0) = ISNULL(ZPPC.PimCategoryHierarchyId,0))
			AND ZPP.PublishCatalogId = @PublishCatalogId
			AND ZPC.PimCategoryId  in 
			(
			Select CategoryId from @TBL_CategoryHierarchyIds
			)
			
		END
	END
	ELSE IF @IsCallAssociated = 0 
	BEGIN 
		
		DECLARE @TBL_ProductIdscollect TABLE(PublishProductId INT , PimproductId INT , PublishcatalogId  INT  , ProductType NVARCHAr(max))
		If @PimCategoryHierarchyId = 0 
		BEGIN

			INSERT INTO @TBL_ProductIdscollect (PublishProductId,PimproductId,PublishcatalogId,ProductType)
			SELECT PublishProductId,ZPAV.PimproductId,TBPOCI.PublishcatalogId,ZPATF.AttributeDefaultValueCode
			FROM ZnodePimAttributeValue ZPAV 
			INNER JOIN ZnodePimProductAttributeDefaultValue ZPADV ON (ZPADV.PimAttributeValueId = ZPAV.PimAttributeValueId )
			INNER JOIN #TBL_PimProductIds TBLIDF ON (TBLIDF.PimProductId = ZPAV.PimProductId )
			INNER JOIN ZnodePublishProduct TBPOCI ON (TBPOCI.PimProductId = TBLIDF.PimProductId AND TBPOCI.PublishCatalogId = TBLIDF.PublishCatalogId 	)
			INNER JOIN ZnodePimAttributeDefaultValue ZPATF ON (ZPATF.PimAttributeId =  @ProductTypeAttributeId 
			AND ZPADV.PimAttributeDefaultValueId = ZPATF.PimAttributeDefaultValueId )
			WHERE  IsParentProducts = 1	
			AND LocaleId =@DefaultLocaleId
		END 
		Else 
		BEGIN
					
			INSERT INTO @TBL_ProductIdscollect (PublishProductId,PimproductId,PublishcatalogId,ProductType)
			SELECT TBPOCI.PublishProductId,ZPAV.PimproductId,TBPOCI.PublishcatalogId,ZPATF.AttributeDefaultValueCode
			FROM ZnodePimAttributeValue ZPAV 
			INNER JOIN ZnodePimProductAttributeDefaultValue ZPADV ON (ZPADV.PimAttributeValueId = ZPAV.PimAttributeValueId )
			INNER JOIN #TBL_PimProductIds TBLIDF ON (TBLIDF.PimProductId = ZPAV.PimProductId )
			INNER JOIN ZnodePublishProduct TBPOCI ON (TBPOCI.PimProductId = TBLIDF.PimProductId AND TBPOCI.PublishCatalogId = TBLIDF.PublishCatalogId 	)
			INNER JOIN ZnodePimAttributeDefaultValue ZPATF ON (ZPATF.PimAttributeId =  @ProductTypeAttributeId 
			AND ZPADV.PimAttributeDefaultValueId = ZPATF.PimAttributeDefaultValueId )
			INNER JOIN ZnodePublishCategoryProduct  ZPCP ON ZPCP.PublishCatalogId = TBPOCI.PublishCatalogId AND 
			ZPCP.PublishProductId = TBPOCI.PublishProductId
			INNER JOIN ZnodePublishCategory ZPC ON  (ZPC.PublishCatalogId = ZPCP.PublishCatalogId  AND ZPC.PublishCategoryId = ZPCP.PublishCategoryId)
			WHERE  IsParentProducts = 1	AND LocaleId =@DefaultLocaleId
			AND ZPC.PimCategoryId  in ( Select CategoryId from @TBL_CategoryHierarchyIds ) 
		END 
		
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_ProductIdscollect WHERE ProductType IN ('GroupedProduct','BundleProduct','ConfigurableProduct','SimpleProduct') ) 
		BEGIN 
	
			DECLARE @TBL_DeleteTrackProduct TABLE (PublishProductId INT,AssociatedZnodeProductId INT  ,PublishCatalogId INT,PublishCatalogLogId INT ,IsDelete BIT , PublishCategoryId int  )

			;With Cte_PublishProduct AS
			(
				SELECT TBL.PublishProductId,PimproductId,TBL.PublishcatalogId,ProductType ,MAx(PublishCatalogLogId) PublishCatalogLogId
				FROM  @TBL_ProductIdscollect TBL 
				INNER JOIN ZnodePublishCatalogLog TBLG ON (TBLG.PublishCatalogId = TBL.PublishcatalogId)
				WHERE IsCatalogPublished = 1 
				GROUP BY TBL.PublishProductId,PimproductId,TBL.PublishcatalogId,ProductType

			)
			, Cte_ConfigData AS 
			(
				SELECT ZPP2.PublishProductId  AssociatedZnodeProductId,ZPP.PublishProductId,ZPXML.PimproductId,ZPP.PublishcatalogId,ProductType,CTR.PublishCatalogLogId
				FROM ZnodePublishAssociatedProduct ZPXML 
				INNER JOIN ZnodePublishProduct ZPP ON (ZPP.PimProductId = ZPXML.ParentPimProductId)
				INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId  = ZPP.PublishCatalogId AND ZPC.PimCatalogID = ZPXML.PimCatalogId )
				INNER JOIN Cte_PublishProduct CTR ON ( CTR.PublishProductId = ZPP.PublishProductId)
				INNER JOIN ZnodePublishProduct ZPP2 ON (ZPP2.PimProductId  = ZPXML.PimProductId AND ZPP2.PublishCatalogId = ZPP.PublishCatalogId )
				LEFT JOIN ZnodePublishCategoryProduct ZPPC ON (ZPPC.PublishProductId = ZPP2.PublishProductId  AND ZPPC.PublishCatalogId = ZPP.PublishCatalogId )
				WHERE  (ZPPC.PublishCategoryId in (Select PublishCategoryId from @TBL_PublishCategoryIds) OR @PimCategoryHierarchyId = 0 ) 
			
			)
			INSERT INTO @TBL_DeleteTrackProduct (PublishProductId,AssociatedZnodeProductId,PublishcatalogId,PublishCatalogLogId)
			SELECT ZPP.PublishProductId,AssociatedZnodeProductId,PublishcatalogId,PublishCatalogLogId 
			FROM Cte_ConfigData ZPP	
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM  #TBL_PublishProductIds TBLP WHERE TBLP.PublishProductId = ZPP.AssociatedZnodeProductId)

			;With Cte_updateStatus AS
			(
		 
				SELECT  PublishProductId,PublishcatalogId
				FROM @TBL_DeleteTrackProduct CTR 
				WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishAssociatedProduct ZPXML 
				INNER JOIN ZnodePublishProduct ZPP ON (ZPP.PimProductId = ZPXML.ParentPimProductId)
				INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId  = ZPP.PublishCatalogId AND ZPC.PimCatalogID = ZPXML.PimCatalogId )
				INNER JOIN ZnodePublishProduct ZPP2 ON (ZPP2.PimProductId  = ZPXML.PimProductId AND ZPP2.PublishCatalogId = ZPP.PublishCatalogId )
				WHERE  CTR.PublishProductId = ZPP2.PublishProductId 
				AND CTR.PublishCatalogId = ZPP.PublishCatalogId ) 
		
			)

			UPDATE a 
			SET IsDelete = CASE WHEN TYR.PublishProductId IS NULL THEN 1 ELSE 0 END 
			FROM @TBL_DeleteTrackProduct a 
			LEFT JOIN Cte_updateStatus TYR ON (TYR.PublishProductId = a.PublishProductId AND TYR.PublishCatalogId = a.PublishCatalogId)

		
			INSERT INTO @PublishProductId 
			SELECT DISTINCT AssociatedZnodeProductId 
			FROM @TBL_DeleteTrackProduct
			WHERE IsDelete =1  
			--	AND 1=0

		END 

	
		INSERT INTO @PublishProductId
		SELECT distinct PublishProductid
		FROM ZnodePublishProduct ZPP
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId =  ZPP.PublishCatalogId )
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryProduct ZPPP inner join ZnodePimCategoryHierarchy ZPCH ON ZPPP.PimCategoryId =ZPCH.PimCategoryId
		WHERE (ZPCH.PimCatalogid = ZPc.PimCatalogId AND ZPPP.PimProductId = ZPP.PimProductId))  
		AND EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductIds TYR WHERE TYR.PimProductId = ZPP.PimProductId )
		AND NOT EXISTS (SELECT TOP 1 1 FROM @PublishProductId YTR WHERE YTR.Id = ZPP.PublishProductId  )
		AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishCategoryProduct TY WHERE TY.PublishProductId = ZPP.PublishProductId AND TY.PublishCatalogId = ZPP.PublishcatalogId  )
	--AND  1=0	
	END  

		
	EXEC dbo.Znode_DeletePublishCatalogProduct  @PublishProductIds = @PublishProductId,@PublishCatalogId = @PublishCatalogId ,
	@PimCategoryHierarchyId  =@PimCategoryHierarchyId  ,
	@PimCatalogId  = @PimCatalogId 

		
	UPDATE ZPP SET ZPP.CreatedBy = @UserId, ZPP.CreatedDate = @GetDate, ZPP.ModifiedBy = @UserId, ZPP.ModifiedDate = @GetDate
	OUTPUT INSERTED.PublishProductId, INSERTED.PimProductId, INSERTED.PublishCatalogId INTO #TBL_PublishProductIds(PublishProductId, PimProductId, PublishCatalogId)		
	FROM ZnodePublishProduct ZPP
	WHERE EXISTS (SELECT * FROM #TBL_PimProductIds TBP WHERE TBP.PimProductId= ZPP.PimProductId AND TBP.PublishCatalogId = ZPP.PublishCatalogId)

	INSERT INTO ZnodePublishProduct(PimProductId, PublishCatalogId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
	OUTPUT INSERTED.PublishProductId, INSERTED.PimProductId, INSERTED.PublishCatalogId INTO #TBL_PublishProductIds(PublishProductId, PimProductId, PublishCatalogId)
	SELECT DISTINCT TBP.PimProductId, TBP.PublishCatalogId, @UserId, @GetDate, @UserId, @GetDate 
	FROM #TBL_PimProductIds AS TBP
	WHERE NOT EXISTS(SELECT * FROM ZnodePublishProduct ZPP WHERE TBP.PimProductId= ZPP.PimProductId AND TBP.PublishCatalogId = ZPP.PublishCatalogId )

	-- Here used the ouput clause to catch what data inserted or updated into variable table
	
	IF Object_ID ('tempdb..#TB_CategoryProduct') is not null
		DROP TABLE #TB_CategoryProduct

	Create TABLE #TB_CategoryProduct(PublishProductId int , PublishCategoryId int, PublishCatalogId int , PimCategoryHierarchyId int ,ProductIndex int  )
	
	IF Isnull(@PublishCatalogId,0)  = 0  
	BEGIN
		INSERT INTO #TB_CategoryProduct 
		(PublishProductId, PublishCategoryId, PublishCatalogId, PimCategoryHierarchyId,	ProductIndex )
		SELECT PublishProductId,
			ISNULL(ZPC.PublishCategoryId,0)PublishCategoryId,
			TBP.PublishCatalogId,ZPC.PimCategoryHierarchyId,
			CASE WHEN ISNULL(@PimCategoryHierarchyId,0) <> 0  THEN TBP.ProductIndex 
			ELSE ROW_NUMBER()Over(Partition BY TBPP.PublishProductId Order BY ISNULL(ZPC.PublishCategoryId,0)) END  ProductIndex
		FROM #TBL_PimProductIds AS TBP 
		LEFT JOIN ZnodePublishCategory AS ZPC ON (ISNULL(TBP.PimCategoryId, 0) = ISNULL(ZPC.PimCategoryId, -1) AND ZPC.PublishCatalogId = TBP.PublishCatalogId 
		AND ISNULL(ZPC.PimCategoryHierarchyId, 0) = ISNULL(TBP.PimCategoryHierarchyId, -1))
		INNER JOIN #TBL_PublishProductIds AS TBPP ON TBP.PimProductId = TBPP.PimProductId
		AND TBP.PublishCatalogId = TBPP.PublishCatalogId
		Where (ZPC.PimCategoryHierarchyId <> 0 )
	GROUP BY PublishProductId, ZPC.PublishCategoryId, TBP.PublishCatalogId,ZPC.PimCategoryHierarchyId,TBP.ProductIndex
	END
	ELSE 	
	BEGIN
		INSERT INTO #TB_CategoryProduct 
		(PublishProductId, PublishCategoryId, PublishCatalogId, PimCategoryHierarchyId,	ProductIndex )
		SELECT PublishProductId,
			ISNULL(ZPC.PublishCategoryId,0)PublishCategoryId,
			TBP.PublishCatalogId,ZPC.PimCategoryHierarchyId,CASE WHEN ISNULL(@PimCategoryHierarchyId,0) <> 0  
			THEN TBP.ProductIndex ELSE     ROW_NUMBER()Over(Partition BY TBPP.PublishProductId 
			Order BY ISNULL(ZPC.PublishCategoryId,0)) END  ProductIndex
		FROM #TBL_PimProductIds AS TBP 
		LEFT JOIN ZnodePublishCategory AS ZPC ON (ISNULL(TBP.PimCategoryId, 0) = ISNULL(ZPC.PimCategoryId, -1) AND ZPC.PublishCatalogId = TBP.PublishCatalogId 
		AND ISNULL(ZPC.PimCategoryHierarchyId, 0) = ISNULL(TBP.PimCategoryHierarchyId, -1))
		INNER JOIN #TBL_PublishProductIds AS TBPP ON TBP.PimProductId = TBPP.PimProductId
		AND TBP.PublishCatalogId = TBPP.PublishCatalogId
		GROUP BY PublishProductId, ZPC.PublishCategoryId, TBP.PublishCatalogId,ZPC.PimCategoryHierarchyId,TBP.ProductIndex
	END
	
	UPDATE TARGET SET TARGET.PublishCategoryId = CASE WHEN SOURCE.PublishCategoryId = 0 THEN NULL ELSE SOURCE.PublishCategoryId END 
	,TARGET.CreatedBy = @UserId, TARGET.CreatedDate = @GetDate, TARGET.ModifiedBy = @UserId, 
	TARGET.ModifiedDate = @GetDate,TARGET.PimCategoryHierarchyId = SOURCE.PimCategoryHierarchyId				
	,ProductIndex = case when Source.ProductIndex is null then 1 else  Source.ProductIndex end
	FROM ZnodePublishCategoryProduct TARGET
	INNER JOIN #TB_Categoryproduct SOURCE ON TARGET.PublishCatalogId = SOURCE.PublishCatalogId AND ISNULL(TARGET.PublishCategoryId, 0) = ISNULL(SOURCE.PublishCategoryId, 0) AND TARGET.PublishProductId = SOURCE.PublishProductId 

			
	INSERT INTO ZnodePublishCategoryProduct(PublishProductId,PublishCategoryId,PublishCatalogId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PimCategoryHierarchyId,ProductIndex) 
	SELECT SOURCE.PublishProductId,CASE WHEN SOURCE.PublishCategoryId =0 THEN NULL ELSE SOURCE.PublishCategoryId  END , SOURCE.PublishCatalogId,@UserId,@GetDate,@userId,@GetDate,SOURCE.PimCategoryHierarchyId,case when Source.ProductIndex is null then 1 else  Source.ProductIndex end
	FROM #TB_Categoryproduct SOURCE
	WHERE NOT EXISTS(SELECT * FROM ZnodePublishCategoryProduct TARGET WHERE TARGET.PublishCatalogId = SOURCE.PublishCatalogId AND ISNULL(TARGET.PublishCategoryId, 0) = ISNULL(SOURCE.PublishCategoryId, 0) AND TARGET.PublishProductId = SOURCE.PublishProductId )

	--Getting default locale data for attributes ProductName, SKU and IsActive
	SELECT A.PimProductId,A.PimAttributeId,B.AttributeValue,B.ZnodePimAttributeValueLocaleId,B.LocaleId --,Row_Number() Over(Partition By a.PimProductId,a.PimAttributeId ORDER BY a.PimProductId,a.PimAttributeId  ) RowId
	INTO #TBL_AttributeVAlue
	FROM ZnodePimAttributeValue a   
	INNER JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )  
	INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
	WHERE LocaleId = @DefaultLocaleId 
	AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishProductIds ZPP WHERE (ZPP.PimProductId = A.PimProductId) )
	AND (C.PimAttributeId in ( @ProductNamePimAttributeId  ,  @SKUPimAttributeId , @IsActivePimAttributeId  ))

	
	WHILE @Counter <= @maxCountId
	BEGIN 
		SET @LocaleId = (SELECT TOP 1 LocaleId FROM @TBL_LocaleId WHERE RowId = @Counter)
		
		IF @DefaultLocaleId <> @LocaleId
		BEGIN
			DELETE FROM #TBL_AttributeVAlue WHERE LocaleId = @LocaleId
			
			INSERT INTO #TBL_AttributeVAlue
			SELECT A.PimProductId,A.PimAttributeId,B.AttributeValue,B.ZnodePimAttributeValueLocaleId,B.LocaleId --,Row_Number() Over(Partition By a.PimProductId,a.PimAttributeId ORDER BY a.PimProductId,a.PimAttributeId  ) RowId
			FROM ZnodePimAttributeValue a   
			INNER JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )  
			INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
			WHERE LocaleId = @LocaleId
			AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishProductIds ZPP WHERE (ZPP.PimProductId = A.PimProductId) )
			AND (C.PimAttributeId in ( @ProductNamePimAttributeId  ,  @SKUPimAttributeId , @IsActivePimAttributeId  ))
		END
		
		SELECT A.PimProductId,A.PimAttributeId,A.AttributeValue,A.ZnodePimAttributeValueLocaleId,A.LocaleId,Row_Number() Over(Partition By a.PimProductId,a.PimAttributeId ORDER BY a.PimProductId,a.PimAttributeId  ) RowId
		INTO #TBL_AttributeVAlue1
		FROM #TBL_AttributeVAlue A
		
		SELECT   ZPP.PublishProductId ,TBLA.AttributeValue PRoductName,TBLAI.AttributeValue SKU ,ISNULL(TBLAII.AttributeValue,'0') IsActive
		INTO #ZnodePublishProductDetail
		FROM  #TBL_PublishProductIds zpp
		INNER JOIN #TBL_AttributeVAlue1 TBLA ON (TBLA.PimAttributeId = @ProductNamePimAttributeId AND TBLA.PimProductId = ZPP.PimProductId AND TBLA.LocaleId  = CASE WHEN TBLA.RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END )
		INNER JOIN #TBL_AttributeVAlue1 TBLAI ON (TBLAI.PimAttributeId = @SKUPimAttributeId AND TBLAI.PimProductId = ZPP.PimProductId AND TBLAI.LocaleId  = CASE WHEN TBLAI.RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END )
		INNER JOIN #TBL_AttributeVAlue1 TBLAII ON (TBLAII.PimAttributeId = @IsActivePimAttributeId AND TBLAII.PimProductId = ZPP.PimProductId AND TBLAII.LocaleId  = CASE WHEN TBLAII.RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END )
		GROUP BY ZPP.PublishProductId,TBLA.AttributeValue,TBLAI.AttributeValue,TBLAII.AttributeValue 
		
		UPDATE TARGET SET TARGET.ProductName   = SOURCE.ProductName
		,TARGET.SKU			 = SOURCE.SKU
		,TARGET.IsActive	= SOURCE.IsActive
		,TARGET.ModifiedBy	 = @userid
		,TARGET.ModifiedDate  = @GetDate
		FROM ZnodePublishProductDetail TARGET
		INNER JOIN #ZnodePublishProductDetail SOURCE ON TARGET.PublishProductId = SOURCE.PublishProductId 
		WHERE TARGET.LocaleId = @LocaleId 

		INSERT INTO ZnodePublishProductDetail (PublishProductId,ProductName,SKU,IsActive,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT SOURCE.PublishProductId ,SOURCE.ProductName ,SOURCE.SKU ,SOURCE.IsActive ,@LocaleId ,@userId ,@GetDate ,@userId ,@GetDate
		FROM #ZnodePublishProductDetail SOURCE
		WHERE NOT EXISTS(SELECT * FROM ZnodePublishProductDetail TARGET WHERE TARGET.PublishProductId = SOURCE.PublishProductId AND TARGET.LocaleId = @LocaleId )

		IF OBJECT_ID('TEMPDB..#TBL_AttributeVAlue1') IS NOT NULL 
			DROP TABLE #TBL_AttributeVAlue1 
		IF OBJECT_ID('TEMPDB..#ZnodePublishProductDetail') IS NOT NULL 
			DROP TABLE #ZnodePublishProductDetail
		
		SET @Counter = @counter + 1 
	END 
	   
	IF @PublishCatalogId IS NULL OR @PublishCatalogId =0 
	BEGIN 
		SELECT PublishProductId, PimProductId, PublishCatalogId 
		FROM #TBL_PublishProductIds
	END 
	IF (ISnull(@PimCategoryHierarchyId ,0) <> 0 ) 
	BEGIN
		SELECT PublishProductId, PimProductId, PublishCatalogId 
		FROM #TBL_PublishProductIds
	End 

--COMMIT TRAN InsertPublishProductIds;
END TRY 
BEGIN CATCH 
	SELECT ERROR_MESSAGE()
	UPDATE ZnodePublishCatalogLog 
	SET IsCatalogPublished = 0 
	,IscategoryPublished = 0 
	,IsProductPublished = 0 
	,PublishStateId = 1 
	WHERE PublishCatalogLogId IN (SELECT Max(PublishCatalogLogId) FROM ZnodePublishCatalogLog WHERE PublishCatalogId = @PublishCatalogId  GROUP BY PublishStateId , PublishCatalogId )
END CATCH 
END