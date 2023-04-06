CREATE PROCEDURE [dbo].[Znode_GetPublishProductJson]  
(  
	@PublishCatalogId INT = 0   
	,@PimProductId     TransferId Readonly  
	,@UserId     INT = 0   
	,@PimCatalogId     INT = 0   
	,@VersionIdString  VARCHAR(2000)= ''  
	,@Status     Bit  OutPut  
	,@RevisionState   Varchar(50) = ''  
	,@IsDraftProductsOnly BIT = 1  
)  
WITH RECOMPILE 
AS  
/*  
DECLARE @rrte transferId   
INSERT INTO @rrte  
SELECT 1  
  
EXEC [_POC_Znode_GetPublishProductbulk] @PublishCatalogId=9,@UserId= 2 ,@localeIDs = @rrte,@PublishStateId = 3   
  
*/  
BEGIN  
BEGIN Try   
	SET NOCOUNT ON;  
	SET @Status = 0    
	Declare  @RevisionType VARCHAR(50) = ''   
	Declare @VersionId int = 0   
	DECLARE @PortalId INT = (SELECT TOP 1 POrtalId FROM ZnodePortalCatalog WHERE PublishCatalogId = @PublishCatalogId)  
	DECLARE @PriceListId INT = (SELECT TOP 1 PriceListId FROM ZnodePriceListPortal WHERE PortalId = @PortalId )  
	DECLARE @DomainUrl varchar(max) = (SELECT TOp 1 URL FROM ZnodeMediaConfiguration WHERE IsActive =1)  
	DECLARE @MaxSmallWidth INT  = (SELECT TOP 1  MAX(MaxSmallWidth) FROM ZnodeGlobalMediaDisplaySetting)  
	DECLARE @PimMediaAttributeId INT = dbo.Fn_GetProductImageAttributeId()  
  
	DECLARE @TBL_LocaleId  TABLE (RowId INT IDENTITY(1,1) PRIMARY KEY  , LocaleId INT , VersionId int,RevisionType varchar(50)  )  
	DECLARE @LocaleIds transferId   

	INSERT INTO @TBL_LocaleId (LocaleId,VersionId,RevisionType)  
	SELECT PV.LocaleId , PV.VersionId , PV.RevisionType  
	FROM ZnodePublishVersionEntity PV INNER JOIN Split(@VersionIdString,',') S ON PV.VersionId = S.Item  

	 --Getting active local for catalog which are associated to store
	INSERT INTO @LocaleIds (Id) 
	SELECT LocaleId FROM DBO.FN_GetCatalogPortalActiveLocale(@PublishCatalogId)
   
	DECLARE --@ProductNamePimAttributeId INT = dbo.Fn_GetProductNameAttributeId(),  
	@DefaultLocaleId INT= Dbo.Fn_GetDefaultLocaleId(),@LocaleId INT = 0   
	--,@SkuPimAttributeId  INT =  dbo.Fn_GetProductSKUAttributeId() , @IsActivePimAttributeId INT =  dbo.Fn_GetProductIsActiveAttributeId()  
	DECLARE @GetDate DATETIME =dbo.Fn_GetDate()  
  
	DECLARE @DefaultPortal int, @IsAllowIndexing int  
	SELECT @DefaultPortal = ZPC.PortalId, @IsAllowIndexing = 1 FROM ZnodePimCatalog ZPC INNER JOIN ZnodePublishCatalog ZPC1 ON ZPC.PimCatalogId = ZPC1.PimCatalogId WHERE ZPC1.PublishCatalogId =  @PublishCatalogId AND isnull(ZPC.IsAllowIndexing,0) = 1   
     
	-----DELETE unwanted publish data  
	DELETE ZPC FROM ZnodePublishCategoryProduct ZPC  
	WHERE NOT EXISTS(SELECT * FROM ZnodePublishCategory ZC WHERE ZPC.PublishCategoryId = ZC.PublishCategoryId )  
  
	DELETE ZPP FROM ZnodePublishCategoryProduct ZPP  
	WHERE NOT EXISTS(SELECT * FROM ZnodePublishProduct ZP WHERE ZPP.PublishProductId = ZP.PublishProductId )  
  
	DELETE ZPP FROM ZnodePublishCatalogProductDetail ZPP  
	WHERE NOT EXISTS(SELECT * FROM ZnodePublishProduct ZP WHERE ZPP.PublishProductId = ZP.PublishProductId )  
  
	DELETE ZPCP FROM ZnodePublishCatalogProductDetail ZPCP  
	INNER JOIN ZnodePublishProduct b on ZPCP.PublishProductId = b.PublishProductId   
	WHERE NOT EXISTS(SELECT * FROM ZnodePimCategoryProduct a  
	INNER JOIN ZnodePimCategoryHierarchy ZPCH on ZPCH.PimCategoryID = a.PimCategoryId   
	WHERE b.PimProductId = A.PimProductId AND ZPCP.PimCategoryHierarchyId = ZPCH.PimCategoryHierarchyId)  
	AND isnull(ZPCP.PimCategoryHierarchyId,0) <> 0 AND b.PublishCatalogId = @PublishCatalogId  
	---------  
  
	DECLARE @Counter INT =1 ,@maxCountId INT = (SELECT max(RowId) FROM @TBL_LocaleId )   
  
	CREATE TABLE #ZnodePrice (RetailPrice numeric(28,13),SalesPrice numeric(28,13),CurrencyCode varchar(100), CultureCode varchar(100), CurrencySuffix varchar(100), PublishProductId int)  
   
	CREATE TABLE #ProductSKU (SEOUrl nvarchar(max), SEODescription  nvarchar(max),SEOKeywords  nvarchar(max),SEOTitle  nvarchar(max), PublishProductId int)  
  
	CREATE TABLE #ProductImages (PublishProductId int, ImageSmallPath  varchar(max))  
  
	EXEC Znode_InsertUpdateAttributeDefaultValueJson 1   
	EXEC Znode_InsertUpdateCustomeFieldJson 1   
	EXEC Znode_InsertUpdatePimAttributeJson 1   
  
	---To draft all catalog products AND associated products for full catalog publish  
	IF @IsDraftProductsOnly = 0  
	BEGIN  
		EXEC Znode_CatalogProductDraftForPublish @PublishCatalogId=@PublishCatalogId   
	END  

	UPDATE ZnodePublishProductEntity SET ElasticSearchEvent = 0

	EXEC [Znode_InsertUpdatePimCatalogProductDetailJson] @PublishCatalogId=@PublishCatalogId,@LocaleId=@LocaleIds ,@UserId=@UserId ,@IsDraftProductsOnly = @IsDraftProductsOnly 


	IF (@IsAllowIndexing=1)  
	BEGIN   
		INSERT INTO #ZnodePrice  
		SELECT RetailPrice,SalesPrice,ZC.CurrencyCode,ZCC.CultureCode ,ZCC.Symbol CurrencySuffix,TYU.PublishProductId  
		FROM ZnodePrice ZP   
		INNER JOIN ZnodePriceList ZPL ON (ZPL.PriceListId = ZP.PriceListId)  
		INNER JOIN ZnodeCurrency ZC oN (ZC.CurrencyId = ZPL.CurrencyId )  
		INNER JOIN ZnodeCulture ZCC ON (ZCC.CultureId = ZPL.CultureId)  
		INNER JOIN ZnodePublishProductDetail TY ON (TY.SKU = ZP.SKU )   
		INNER JOIN ZnodePublishProduct TYU ON (TYU.PublishProductId = TY.PublishProductId)   
		WHERE ZP.PriceListId = @PriceListId   
		AND TY.LocaleId = @DefaultLocaleId  
		AND TYU.PublishCatalogId = @PublishCatalogId  
		AND EXISTS (SELECT TOP 1 1 FROM ZnodePriceListPortal ZPLP   
		INNER JOIN ZnodePimCatalog ZPC on ZPC.PortalId=ZPLP.PortalId WHERE ZPLP.PriceListId=ZP.PriceListId )  
		AND EXISTS(SELECT * FROM ZnodePimProduct ZPP1 WHERE TYU.PimProductId = ZPP1.PimProductId )  
		--+ CAST(@DefaultPortal AS VARCHAr(100)) + '/'  
		INSERT INTO #ProductImages  
		SELECT  TYU.PublishProductId , @DomainUrl +'Catalog/'  + CAST(@MaxSmallWidth AS VARCHAR(100)) + '/' + RT.MediaPath AS ImageSmallPath     
		FROM ZnodePimAttributeValue ZPAV   
		INNER JOIN ZnodePublishProduct TYU ON (TYU.PimProductId  = ZPAV.PimProductId)  
		INNER JOIN ZnodePimProductAttributeMedia  RT ON ( RT.PimAttributeValueId = ZPAV.PimAttributeValueId )  
		WHERE  TYU.PublishCatalogId = @PublishCatalogId  
		AND RT.LocaleId = @DefaultLocaleId  
		AND ZPAV.PimAttributeId = (SELECT TOp 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'ProductImage')  
		AND EXISTS(SELECT * FROM ZnodePimProduct ZPP1 WHERE TYU.PimProductId = ZPP1.PimProductId )  
   
		INSERT INTO #ProductSKU   
		SELECT ZCSD.SEOUrl , ZCDL.SEODescription,ZCDL.SEOKeywords ,ZCDL.SEOTitle, TYU.PublishProductId  
		FROM ZnodeCMSSEODetail ZCSD   
		INNER JOIN ZnodeCMSSEODetailLocale ZCDL ON (ZCDL.CMSSEODetailId = ZCSD.CMSSEODetailId)  
		INNER JOIN ZnodePublishProductDetail TY ON (TY.SKU = ZCSD.SEOCode AND ZCDL.LocaleId = TY.LocaleId)   
		INNER JOIN ZnodePublishProduct TYU ON (TYU.PublishProductId = TY.PublishProductId)  
		WHERE CMSSEOTypeId = (SELECT TOP 1 CMSSEOTypeId FROM ZnodeCMSSEOType WHERE Name = 'Product')   
		AND ZCDL.LocaleId = @DefaultLocaleId  
		AND TYU.PublishCatalogId = @PublishCatalogId  
		--AND ZCSD.PublishStateId = @PublishStateId  
		AND ZCSD.PortalId = @DefaultPortal  
		AND EXISTS(SELECT * FROM ZnodePimProduct ZPP1 WHERE TYU.PimProductId = ZPP1.PimProductId )  
  
	END  
   
	CREATE NONCLUSTERED INDEX Idx_#ProductSKU_PublishProductId ON [dbo].[#ProductSKU] ([PublishProductId])  
	CREATE NONCLUSTERED INDEX Idx_#ProductImages_PublishProductId ON [dbo].#ProductImages ([PublishProductId])  
	CREATE NONCLUSTERED INDEX Idx_#ZnodePrice_PublishProductId ON [dbo].#ZnodePrice ([PublishProductId])  
	
	SELECT DISTINCT PublishProductId, x.LocaleId 
	INTO #PublishProducts 
	FROM ZnodePublishProduct ZPP
	INNER JOIN ##AttributeValueLocale x ON X.PimProductId = ZPP.PimProductId
	WHERE ZPP.PublishCatalogId = @PublishCatalogId

	CREATE INDEX #PublishProducts_PublishProductId on #PublishProducts(PublishProductId,LocaleId)
	--Creating products complete attribute json
	SELECT ZPP.Pimproductid, 
			ZPCPD.PimCatalogProductDetailId,ZPCPD.PublishProductId
			,ZPCPD.PublishCatalogId,ZPCPD.PimCategoryHierarchyId
			,ZPCPD.SKU,ZPCPD.ProductName,ZPCPD.CategoryName,ZPCPD.CatalogName
			,ZPCPD.LocaleId,ZPCPD.IsActive,ZPCPD.ProfileIds,ZPCPD.ProductIndex, 
		'[' +  
		(SELECT STUFF((SELECT ','+ AttributeEntity FROM ##AttributeValueLocale a   
		WHERE a.pimproductid = ZPP.pimproductid AND a.LocaleId = ZPCPD.LocaleId   
		FOR XML Path (''),Type).value('.', 'varchar(max)') ,1,1,'')  )   
		+ ']' AS ProductXML  
	INTO #ProductAttributeXML  
	FROM [ZnodePublishCatalogProductDetail] ZPCPD   
	INNER JOIN ZnodePublishProduct ZPP ON ZPCPD.PublishProductId = ZPP.PublishProductId AND ZPCPD.PublishCatalogId = ZPP.PublishCatalogId --WHERE TY.PimProductId = ZPP.PimProductId  AND TY.LocaleId = ZPCPD.LocaleId   
	WHERE ZPCPD.PublishCatalogId = @PublishCatalogId  
	AND EXISTS(SELECT * FROM #PublishProducts X WHERE ZPCPD.PublishProductId = X.PublishProductId AND ZPCPD.LocaleId = X.LocaleId)
	AND ZPCPD.IsActive = 1 
	GROUP BY ZPP.Pimproductid,ZPCPD.PimCatalogProductDetailId,ZPCPD.PublishProductId
			,ZPCPD.PublishCatalogId,ZPCPD.PimCategoryHierarchyId
			,ZPCPD.SKU,ZPCPD.ProductName,ZPCPD.CategoryName,ZPCPD.CatalogName
			,ZPCPD.LocaleId,ZPCPD.IsActive,ZPCPD.ProfileIds,ZPCPD.ProductIndex

	CREATE NONCLUSTERED INDEX Idx_#ProductAttributeXML_PimProductId_LocaleId  
	ON [dbo].#ProductAttributeXML (PimProductId,LocaleId)  
	
	IF (SELECT COUNT(*) FROM @LocaleIds) > 1
	BEGIN
		--INSERT INTO #ProductAttributeXML(Pimproductid,LocaleId,PublishProductId,ProductXML)
		SELECT ZPP.Pimproductid, 
			ZPCPD.PimCatalogProductDetailId,ZPCPD.PublishProductId
			,ZPCPD.PublishCatalogId,ZPCPD.PimCategoryHierarchyId
			,ZPCPD.SKU,ZPCPD.ProductName,ZPCPD.CategoryName,ZPCPD.CatalogName
			,ZPCPD.LocaleId,ZPCPD.IsActive,ZPCPD.ProfileIds,ZPCPD.ProductIndex,
			'[' +  
			(SELECT STUFF((SELECT ','+ AttributeEntity FROM ##AttributeValueLocale a   
			WHERE a.pimproductid = ZPP.pimproductid AND a.LocaleId = @DefaultLocaleId   
			FOR XML Path (''),Type).value('.', 'varchar(max)') ,1,1,'')  )   
			+ ']' AS ProductXML  
		INTO #ProductAttributeXML_LocaleWise
		FROM [ZnodePublishCatalogProductDetail] ZPCPD   
		INNER JOIN ZnodePublishProduct ZPP ON ZPCPD.PublishProductId = ZPP.PublishProductId AND ZPCPD.PublishCatalogId = ZPP.PublishCatalogId --WHERE TY.PimProductId = ZPP.PimProductId  AND TY.LocaleId = ZPCPD.LocaleId   
		WHERE ZPCPD.PublishCatalogId = @PublishCatalogId  
		AND EXISTS(SELECT * FROM #PublishProducts X WHERE ZPCPD.PublishProductId = X.PublishProductId)
		AND NOT EXISTS(SELECT * FROM #ProductAttributeXML X WHERE ZPCPD.PublishProductId = X.PublishProductId AND X.LocaleId = ZPCPD.LocaleId )
		AND ZPCPD.LocaleId <> @DefaultLocaleId 
		and ZPCPD.IsActive = 1 
		GROUP BY ZPP.Pimproductid, 
			ZPCPD.PimCatalogProductDetailId,ZPCPD.PublishProductId
			,ZPCPD.PublishCatalogId,ZPCPD.PimCategoryHierarchyId
			,ZPCPD.SKU,ZPCPD.ProductName,ZPCPD.CategoryName,ZPCPD.CatalogName
			,ZPCPD.LocaleId,ZPCPD.IsActive,ZPCPD.ProfileIds,ZPCPD.ProductIndex

		CREATE NONCLUSTERED INDEX Idx_#ProductAttributeXML_LocaleWise_PimProductId_LocaleId  
		ON [dbo].#ProductAttributeXML (PimProductId,LocaleId) 
	END
	


	DECLARE @MaxCount INT, @MinRow INT, @MaxRow INT, @Rows numeric(10,2);  
	SELECT @MaxCount = COUNT(*) FROM [ZnodePublishCatalogProductDetail] ZPCPD WHERE PublishCatalogId = @PublishCatalogId  
	AND EXISTS(SELECT * FROM #PublishProducts X WHERE ZPCPD.PublishProductId = X.PublishProductId)
  
	SELECT @Rows = 50000  
          
	SELECT @MaxCount = CEILING(@MaxCount / @Rows);  
  
	SELECT PimCatalogProductDetailId, PublishProductId,Row_Number() over(Order by PublishProductId) ID into #ZnodePublishCatalogProductDetail 
	FROM [ZnodePublishCatalogProductDetail] ZPCPD WHERE PublishCatalogId = @PublishCatalogId  
	AND EXISTS(SELECT * FROM #PublishProducts X WHERE ZPCPD.PublishProductId = X.PublishProductId)
  
	UPDATE ZPPAX SET ZPPAX.ElasticSearchEvent = 2
	FROM ZnodePublishProductEntity ZPPAX
	WHERE NOT EXISTS(SELECT * from [ZnodePublishCatalogProductDetail] ZLW WHERE ZPPAX.ZnodeProductId = ZLW.PublishProductId 
						AND ZPPAX.LocaleId = ZLW.LocaleId AND ZPPAX.ZnodeCatalogId = ZLW.PublishCatalogId )
	AND ZPPAX.ZnodeCatalogId = @PublishCatalogId 
	AND EXISTS(SELECT * FROM @TBL_LocaleId V WHERE ZPPAX.VersionId = V.VersionId)

	UPDATE ZPPAX SET ZPPAX.ElasticSearchEvent = 2
    FROM ZnodePublishProductEntity ZPPAX Inner join [ZnodePublishCatalogProductDetail] ZLW  
    ON ZPPAX.ZnodeProductId = ZLW.PublishProductId  
    AND ZPPAX.LocaleId = ZLW.LocaleId AND ZPPAX.ZnodeCatalogId = ZLW.PublishCatalogId AND ZLW.IsActive = 0  
    AND EXISTS(SELECT * FROM @TBL_LocaleId V WHERE ZPPAX.VersionId = V.VersionId)

	--Delete products from category from publish tables which are removed from one category and added into other category
	--UPDATE ZPPE SET ZPPE.ElasticSearchEvent = 2 FROM ZnodePublishProductEntity ZPPE
	--WHERE NOT EXISTS(SELECT * FROM ZnodePublishCategoryProduct ZPCP WHERE ZPPE.ZnodeCategoryIds = ZPCP.PublishCategoryId AND ZPPE.ZnodeProductId = ZPCP.PublishProductId
	--AND ZPPE.ZnodeCatalogId = ZPCP.PublishCatalogId)
	--AND ZPPE.ZnodeCatalogId = @PublishCatalogId
	--AND Not Exists (Select TOP 1 1 From ZnodePublishConfigurableProductEntity X Where X.AssociatedZnodeProductId = ZPPE.ZnodeProductId )
	--AND Isnull(ZPPE.ZnodeCategoryIds ,'') = '0'
	
	UPDATE ZPPE SET ZPPE.ElasticSearchEvent = 2 FROM ZnodePublishProductEntity ZPPE
	WHERE NOT EXISTS(SELECT * FROM ZnodePublishCategoryProduct ZPCP WHERE ZPPE.ZnodeCategoryIds = ZPCP.PublishCategoryId AND ZPPE.ZnodeProductId = ZPCP.PublishProductId
	AND ZPPE.ZnodeCatalogId = ZPCP.PublishCatalogId)
	AND ZPPE.ZnodeCatalogId = @PublishCatalogId
	AND Isnull(ZPPE.ZnodeCategoryIds ,'') <> ''
	AND EXISTS(SELECT * FROM @TBL_LocaleId V WHERE ZPPE.VersionId = V.VersionId)


 --   --Delete data which are not present in catalog publish
	--Declare @Batch Bigint
	--SET @Batch = 1;
	--WHILE @Batch > 0
	--BEGIN 
	--	--BEGIN TRAN DelProd
	--		DELETE top (5000)  ZPPAX
	--		from ZnodePublishProductEntity ZPPAX
	--		WHERE NOT EXISTS(SELECT * from [ZnodePublishCatalogProductDetail] ZLW WHERE ZPPAX.ZnodeProductId = ZLW.PublishProductId 
	--							AND ZPPAX.LocaleId = ZLW.LocaleId AND ZPPAX.ZnodeCatalogId = ZLW.PublishCatalogId )
	--		AND ZPPAX.ZnodeCatalogId = @PublishCatalogId  
	--	--COMMIT TRAN DelProd
	--	SET @Batch = @@ROWCOUNT;
	--END
	
	SELECT ZPCP.PublishCategoryId,ZPCP.PublishProductId,ZPCP.PublishCatalogId,ZPCP.PimCategoryHierarchyId,ZPC.PimCategoryId,ZPCCF.PimProductId ,ZPCCF.DisplayOrder
	INTO #TempPublishCategoryProduct
	FROM ZnodePublishCategoryProduct ZPCP 
	INNER JOIN ZnodePublishProduct ZPP ON ZPCP.PublishProductId = ZPP.PublishProductId AND ZPCP.PublishCatalogId = ZPP.PublishCatalogId
	INNER JOIN ZnodePublishCategory ZPC ON (ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND   ZPC.PublishCategoryId = ZPCP.PublishCategoryId AND ZPCP.PimCategoryHierarchyId = ZPC.PimCategoryHierarchyId)
	INNER JOIN ZnodePimCategoryProduct ZPCCF ON (ZPCCF.PimCategoryId = ZPC.PimCategoryId  AND ZPCCF.PimProductId = ZPP.PimProductId )
	WHERE EXISTS(SELECT * FROM [ZnodePublishCatalogProductDetail] ZPCPD WHERE (ZPCP.PublishProductId = ZPCPD.PublishProductId AND ZPCP.PublishCatalogId = ZPCPD.PublishCatalogId AND ZPCP.PimCategoryHierarchyId = ZPCPD.PimCategoryHierarchyId))		
	AND EXISTS(SELECT * FROM #PublishProducts X WHERE ZPCP.PublishProductId = X.PublishProductId)
	AND ZPCP.PublishCatalogId = @PublishCatalogId 

	CREATE NONCLUSTERED INDEX Id_#TempPublishCategoryProduct_1 ON #TempPublishCategoryProduct(PublishProductId,PublishCatalogId,PimCategoryHierarchyId)

	IF OBJECT_ID('tempdb..#Temp_ImportLoop') IS NOT NULL  
		DROP TABLE #Temp_ImportLoop;  
          
	---- To get the min AND max rows for import in loop  
	;WITH cte AS   
	(  
		SELECT RowId = 1,   
		MinRow = 1,   
		MaxRow = cast(@Rows as int)  
		UNION ALL  
		SELECT RowId + 1,   
		MinRow + cast(@Rows as int),   
		MaxRow + cast(@Rows as int)  
		FROM cte  
		WHERE RowId + 1 <= @MaxCount  
	)  
	SELECT RowId, MinRow, MaxRow  
	INTO #Temp_ImportLoop  
	FROM cte  
	OPTION (maxrecursion 0);  

	DECLARE cur_BulkData CURSOR LOCAL FAST_FORWARD  
	FOR SELECT MinRow, MaxRow, B.LocaleId , B.RevisionType  ,b.VersionId
	FROM #Temp_ImportLoop L  
	CROSS APPLY @TBL_LocaleId B;  
  
	OPEN cur_BulkData;  
	FETCH NEXT FROM cur_BulkData INTO  @MinRow, @MaxRow,@LocaleId, @RevisionType ,@VersionId 
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		IF OBJECT_ID('TEMPDB..#ProductEntity') IS NOT NULL
				DROP TABLE #ProductEntity

		IF OBJECT_ID('TEMPDB..#ProductEntity_LocaleWise') IS NOT NULL
			DROP TABLE #ProductEntity_LocaleWise
			
		SELECT   
			CAST(@VersionId AS VARCHAR(50)) VersionId --1   
			,CAST(ZPCPD.ProductIndex AS VARCHAr(100)) + CAST(ISNULL(ZPCP.PublishCategoryId,'')  AS VARCHAR(50))  + CAST(Isnull(ZPCPD.PublishCatalogId ,'')  AS VARCHAR(50)) + CAST( Isnull(ZPCPD.LocaleId,'') AS VARCHAR(50)) IndexId  --2  
			,CAST(ZPCPD.PublishProductId AS VARCHAR(50)) PublishProductId,CAST(ZPCPD.PublishCatalogId  AS VARCHAR(50)) PublishCatalogId  --3   
			,CAST(ISNULL(ZPCPD.SKU ,'') AS NVARCHAR(2000)) SKU,CAST( Isnull(ZPCPD.LocaleId,'') AS VARCHAR(50)) LocaleId -- 4   
			,CAST(isnull(ZPCPD.ProductName,'') AS NVARCHAR(2000) )  ProductName ,CAST(ISNULL(ZPCP.PublishCategoryId,'')  AS VARCHAR(50)) PublishCategoryId  -- 5   
			--'{"Attributes":[' + PAX.ProductXML + ']'  
			,CAST(ISNULL(ZPCPD.IsActive ,'0') AS VARCHAR(50)) IsActive ,ZPCPD.ProductXML,'[]' Brands,CAST(isnull(ZPCPD.CategoryName,'') AS NVARCHAR(2000)) CategoryName  --6  
			,CAST(Isnull(ZPCPD.CatalogName,'')  AS NVARCHAR(2000)) CatalogName,CAST(ISNULL(ZPCP.DisplayOrder,'') AS VARCHAR(50)) DisplayOrder  -- 7   
			,@RevisionType RevisionType, 0 AssociatedProductDisplayOrder,-- pending  -- 8   
			ZPCPD.ProductIndex,  -- 9  
			Case When @IsAllowIndexing = 1 then  ISNULL(CAST(TBZP.SalesPrice  AS VARCHAr(500)),'') else '' end SalesPrice ,   
			Case When @IsAllowIndexing = 1 then  ISNULL(CAST(TBZP.RetailPrice  AS VARCHAr(500)),'') else '' end RetailPrice ,   
			Case When @IsAllowIndexing = 1 then  ISNULL(TBZP.CultureCode ,'') else '' end CultureCode ,   
			Case When @IsAllowIndexing = 1 then  ISNULL(TBZP.CurrencySuffix ,'') else '' end CurrencySuffix ,   
			Case When @IsAllowIndexing = 1 then  ISNULL(TBZP.CurrencyCode ,'') else '' end CurrencyCode ,   
			Case When @IsAllowIndexing = 1 then  ISNULL(TBPS.SEODescription,'') else '' end SEODescriptionForIndex,  
			Case When @IsAllowIndexing = 1 then  ISNULL(TBPS.SEOKeywords,'') else '' end SEOKeywords,  
			Case When @IsAllowIndexing = 1 then  ISNULL(SEOTitle,'') else '' end SEOTitle,  
			Case When @IsAllowIndexing = 1 then  ISNULL(TBPS.SEOUrl ,'') else '' end SEOUrl,  
			Case When @IsAllowIndexing = 1 then  ISNULL(TBPI.ImageSmallPath,'') else '' end ImageSmallPath,  
			CAST(ISNULL(LOWER(ZPCPD.SKU) ,'') AS NVARCHAR(2000)) Lower_SKU--, z.Id    		
		INTO #ProductEntity
		FROM #ProductAttributeXML ZPCPD  
		INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPCPD.PublishCatalogId)  
		--INNER JOIN #ProductAttributeXML PAX ON PAX.PublishProductId = ZPCPD.PublishProductId  AND PAX.LocaleId = ZPCPD.LocaleId   
		LEFT JOIN  #ZnodePrice TBZP ON (TBZP.PublishProductId = ZPCPD.PublishProductId)  
		LEFT JOIN  #ProductSKU TBPS ON (TBPS.PublishProductId = ZPCPD.PublishProductId)  
		LEFT JOIN  #ProductImages TBPI ON (TBPI.PublishProductId = ZPCPD.PublishProductId)  
		LEFT JOIN #TempPublishCategoryProduct ZPCP  ON (ZPCP.PublishProductId = ZPCPD.PublishProductId AND ZPCP.PublishCatalogId = ZPCPD.PublishCatalogId AND ZPCP.PimCategoryHierarchyId = ZPCPD.PimCategoryHierarchyId)
		WHERE ZPCPD.LocaleId = @LocaleId  
		AND EXISTS(SELECT * FROM #ZnodePublishCatalogProductDetail z WHERE ZPCPD.PimCatalogProductDetailId = z.PimCatalogProductDetailId AND z.Id BETWEEN @MinRow AND @MaxRow)
		AND ZPCPD.PublishCatalogId = @PublishCatalogId  
		AND EXISTS(SELECT * FROM #PublishProducts X WHERE ZPCPD.PublishProductId = X.PublishProductId)

		CREATE INDEX Idx_#ProductEntity ON #ProductEntity (PublishProductId,PublishCatalogId,LocaleId,PublishCategoryId)

		UPDATE A SET a.SKU = b.SKU,A.SKULower = Lower_SKU,A.Name = B.ProductName, A.Brands = B.Brands, A.CategoryName = B.CategoryName, A.CatalogName = B.CatalogName,
			A.DisplayOrder = B.DisplayOrder, A.AssociatedProductDisplayOrder = B.AssociatedProductDisplayOrder,
			A.SalesPrice = B.SalesPrice, A.RetailPrice = B.RetailPrice, A.SeoDescription=B.SEODescriptionForIndex,
			A.SeoKeywords = B.SEOKeywords, A.SeoTitle = B.SEOTitle, A.SeoUrl = B.SEOUrl, A.ImageSmallPath = B.ImageSmallPath,
			A.ProductIndex = B.ProductIndex, A.IsActive = B.IsActive, A.IndexId = B.IndexId, A.Attributes = B.ProductXML,
			--A.ElasticSearchEvent = 1
			A.ElasticSearchEvent = Case When B.IsActive =1 Then  1 else 2 End 
		FROM ZnodePublishProductEntity A
		INNER JOIN #ProductEntity B ON A.ZnodeProductId = B.PublishProductId AND A.ZnodeCatalogId = B.PublishCatalogId AND A.LocaleId = B.LocaleId 
			AND ISNULL(A.ZnodeCategoryIds,0) = ISNULL(B.PublishCategoryId,0) AND A.VersionId = B.VersionId AND A.ElasticSearchEvent <> 2
		AND EXISTS(SELECT * FROM @TBL_LocaleId V WHERE A.VersionId = V.VersionId)

		INSERT INTO ZnodePublishProductEntity (  
			VersionId, --1  
			IndexId, --2   
			ZnodeProductId,ZnodeCatalogId, --3  
			SKU,LocaleId, --4   
			Name,ZnodeCategoryIds, --5  
			IsActive,Attributes,Brands,CategoryName, --6   
			CatalogName,DisplayOrder, --7   
			RevisionType,AssociatedProductDisplayOrder, --8  
			ProductIndex,--9  
			SalesPrice,RetailPrice,CultureCode,CurrencySuffix,CurrencyCode,SeoDescription,SeoKeywords,SeoTitle,SeoUrl,ImageSmallPath,SKULower,ElasticSearchEvent)  
		SELECT VersionId, --1  
			IndexId, --2   
			PublishProductId,PublishCatalogId, --3  
			SKU,LocaleId, --4   
			ProductName,PublishCategoryId, --5  
			IsActive,ProductXML,Brands,CategoryName, --6   
			CatalogName,DisplayOrder, --7   
			RevisionType,AssociatedProductDisplayOrder, --8  
			ProductIndex,--9  
			SalesPrice,RetailPrice,CultureCode,CurrencySuffix,CurrencyCode,SEODescriptionForIndex,SeoKeywords,SeoTitle,SeoUrl,ImageSmallPath,Lower_SKU,1 AS ElasticSearchEvent
		FROM #ProductEntity B
		WHERE NOT EXISTS(SELECT * FROM ZnodePublishProductEntity A WHERE A.ZnodeProductId = B.PublishProductId AND A.ZnodeCatalogId = B.PublishCatalogId AND A.LocaleId = B.LocaleId 
			AND ISNULL(A.ZnodeCategoryIds,0) = ISNULL(B.PublishCategoryId,0) AND A.VersionId = B.VersionId AND A.ElasticSearchEvent <> 2)
			--select * from #ProductEntity where PublishProductId = 191 
		IF @LocaleId <> @DefaultLocaleId
		BEGIN
			SELECT 
				CAST(@VersionId AS VARCHAR(50)) VersionId --1 
				,CAST(ZPCPD.ProductIndex AS VARCHAr(100)) + CAST(ISNULL(ZPCP.PublishCategoryId,'')  AS VARCHAR(50))  + CAST(Isnull(ZPCPD.PublishCatalogId ,'')  AS VARCHAR(50)) + CAST( Isnull(ZPCPD.LocaleId,'') AS VARCHAR(50)) IndexId  --2
				,CAST(ZPCPD.PublishProductId AS VARCHAR(50)) PublishProductId,CAST(ZPCPD.PublishCatalogId  AS VARCHAR(50)) PublishCatalogId  --3 
				,CAST(ISNULL(ZPCPD.SKU ,'') AS NVARCHAR(2000)) SKU,CAST( Isnull(ZPCPD.LocaleId,'') AS VARCHAR(50)) LocaleId -- 4 
				,CAST(isnull(ZPCPD.ProductName,'') AS NVARCHAR(2000) )  ProductName ,CAST(ISNULL(ZPCP.PublishCategoryId,'')  AS VARCHAR(50)) PublishCategoryId  -- 5 
				,CAST(ISNULL(ZPCPD.IsActive ,'0') AS VARCHAR(50)) IsActive ,ZPCPD.ProductXML,'[]' Brands,CAST(isnull(ZPCPD.CategoryName,'') AS NVARCHAR(2000)) CategoryName  --6
				,CAST(Isnull(ZPCPD.CatalogName,'')  AS NVARCHAR(2000)) CatalogName,CAST(ISNULL(ZPCP.DisplayOrder,'') AS VARCHAR(50)) DisplayOrder  -- 7 
				,@RevisionType as RevisionType, 0 AssociatedProductDisplayOrder,-- pending  -- 8 
				ZPCPD.ProductIndex,  -- 9
				Case When @IsAllowIndexing = 1 then  ISNULL(CAST(TBZP.SalesPrice  AS VARCHAr(500)),'') else '' end SalesPrice , 
				Case When @IsAllowIndexing = 1 then  ISNULL(CAST(TBZP.RetailPrice  AS VARCHAr(500)),'') else '' end RetailPrice , 
				Case When @IsAllowIndexing = 1 then  ISNULL(TBZP.CultureCode ,'') else '' end CultureCode , 
				Case When @IsAllowIndexing = 1 then  ISNULL(TBZP.CurrencySuffix ,'') else '' end CurrencySuffix , 
				Case When @IsAllowIndexing = 1 then  ISNULL(TBZP.CurrencyCode ,'') else '' end CurrencyCode , 
				Case When @IsAllowIndexing = 1 then  ISNULL(TBPS.SEODescription,'') else '' end SEODescriptionForIndex,
				Case When @IsAllowIndexing = 1 then  ISNULL(TBPS.SEOKeywords,'') else '' end SEOKeywords,
				Case When @IsAllowIndexing = 1 then  ISNULL(SEOTitle,'') else '' end SEOTitle,
				Case When @IsAllowIndexing = 1 then  ISNULL(TBPS.SEOUrl ,'') else '' end SEOUrl,
				Case When @IsAllowIndexing = 1 then  ISNULL(TBPI.ImageSmallPath,'') else '' end ImageSmallPath,
				CAST(ISNULL(LOWER(ZPCPD.SKU) ,'') AS NVARCHAR(2000)) Lower_SKU
			INTO #ProductEntity_LocaleWise
			FROM #ProductAttributeXML_LocaleWise ZPCPD
			INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPCPD.PublishCatalogId)
			--INNER JOIN #ProductAttributeXML_LocaleWise PAX ON PAX.PublishProductId = ZPCPD.PublishProductId  AND PAX.LocaleId = ZPCPD.LocaleId 
			LEFT JOIN  #ZnodePrice TBZP ON (TBZP.PublishProductId = ZPCPD.PublishProductId)
			LEFT JOIN  #ProductSKU TBPS ON (TBPS.PublishProductId = ZPCPD.PublishProductId)
			LEFT JOIN  #ProductImages TBPI ON (TBPI.PublishProductId = ZPCPD.PublishProductId)
			LEFT JOIN #TempPublishCategoryProduct ZPCP  ON (ZPCP.PublishProductId = ZPCPD.PublishProductId AND ZPCP.PublishCatalogId = ZPCPD.PublishCatalogId AND ZPCP.PimCategoryHierarchyId = ZPCPD.PimCategoryHierarchyId)
			WHERE ZPCPD.LocaleId = @LocaleId 
			AND EXISTS(SELECT * FROM #ZnodePublishCatalogProductDetail z WHERE ZPCPD.PimCatalogProductDetailId = z.PimCatalogProductDetailId AND z.Id BETWEEN @MinRow AND @MaxRow)
			AND ZPCPD.PublishCatalogId = @PublishCatalogId

			CREATE INDEX Idx_#ProductEntity_LocaleWise ON #ProductEntity (PublishProductId,PublishCatalogId,LocaleId,PublishCategoryId)

			UPDATE A SET a.SKU = b.SKU,A.SKULower = Lower_SKU,A.Name = B.ProductName, A.Brands = B.Brands, A.CategoryName = B.CategoryName, A.CatalogName = B.CatalogName,
				A.DisplayOrder = B.DisplayOrder, A.AssociatedProductDisplayOrder = B.AssociatedProductDisplayOrder,
				A.SalesPrice = B.SalesPrice, A.RetailPrice = B.RetailPrice, A.SeoDescription=B.SEODescriptionForIndex,
				A.SeoKeywords = B.SEOKeywords, A.SeoTitle = B.SEOTitle, A.SeoUrl = B.SEOUrl, A.ImageSmallPath = B.ImageSmallPath,
				A.ProductIndex = B.ProductIndex, A.IsActive = B.IsActive, A.IndexId = B.IndexId, A.Attributes = B.ProductXML,
				--A.ElasticSearchEvent = 1
				A.ElasticSearchEvent = Case When  B.IsActive =1 Then  1 else 2 End 
			FROM ZnodePublishProductEntity A
			INNER JOIN #ProductEntity_LocaleWise B ON A.ZnodeProductId = B.PublishProductId AND A.ZnodeCatalogId = B.PublishCatalogId AND A.LocaleId = B.LocaleId 
				AND ISNULL(A.ZnodeCategoryIds,0) = ISNULL(B.PublishCategoryId,0) AND A.VersionId = B.VersionId AND A.ElasticSearchEvent <> 2
			AND EXISTS(SELECT * FROM @TBL_LocaleId V WHERE A.VersionId = V.VersionId)

			INSERT INTO ZnodePublishProductEntity (
			VersionId, --1
			IndexId, --2 
			ZnodeProductId,ZnodeCatalogId, --3
			SKU,LocaleId, --4 
			Name,ZnodeCategoryIds, --5
			IsActive,Attributes,Brands,CategoryName, --6 
			CatalogName,DisplayOrder, --7 
			RevisionType,AssociatedProductDisplayOrder, --8
			ProductIndex,--9
			SalesPrice,RetailPrice,CultureCode,CurrencySuffix,CurrencyCode,SeoDescription,SeoKeywords,SeoTitle,SeoUrl,ImageSmallPath,SKULower,ElasticSearchEvent)
			SELECT VersionId --1 
				,IndexId  --2
				,PublishProductId,PublishCatalogId  --3 
				,SKU,LocaleId -- 4 
				,ProductName ,PublishCategoryId  -- 5 
				,IsActive ,ProductXML,Brands,CategoryName  --6
				,CatalogName,DisplayOrder  -- 7 
				,RevisionType, AssociatedProductDisplayOrder,-- pending  -- 8 
				ProductIndex,  -- 9
				SalesPrice , 
				RetailPrice , 
				CultureCode , 
				CurrencySuffix , 
				CurrencyCode , 
				SEODescriptionForIndex,
				SEOKeywords,
				SEOTitle,
				SEOUrl,
				ImageSmallPath,
				Lower_SKU,1 as ElasticSearchEvent
			FROM #ProductEntity_LocaleWise B
			WHERE NOT EXISTS(SELECT * FROM ZnodePublishProductEntity A WHERE A.ZnodeProductId = B.PublishProductId AND A.ZnodeCatalogId = B.PublishCatalogId AND A.LocaleId = B.LocaleId 
				AND ISNULL(A.ZnodeCategoryIds,0) = ISNULL(B.PublishCategoryId,0) AND A.VersionId = B.VersionId AND A.ElasticSearchEvent <> 2)
		
			--select * from #ProductEntity_LocaleWise where PublishProductId = 191 

		END

		SET @VersionId = 0  

		IF OBJECT_ID('tempdb..#ProductEntity') is not null
			DROP TABLE #ProductEntity

		IF OBJECT_ID('TEMPDB..#TempProductEntity_LocaleWise') IS NOT NULL
			DROP TABLE #TempProductEntity_LocaleWise

	FETCH NEXT FROM cur_BulkData INTO  @MinRow, @MaxRow,@LocaleId, @RevisionType ,@VersionId 
	END;  
	CLOSE cur_BulkData;  
	DEALLOCATE cur_BulkData;  
  
  
	IF @RevisionState = 'PREVIEW'   
		UPDATE ZnodePimProduct SET IsProductPublish = 1,PublishStateId =  DBO.Fn_GetPublishStateIdForPreview()    
		WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePublishProduct ZPP WHERE ZPP.PimProductId = ZnodePimProduct.PimProductId   
		AND ZPP.PublishCatalogId = @PublishCatalogId)  
	Else  
		UPDATE ZnodePimProduct SET IsProductPublish = 1,PublishStateId =  DBO.Fn_GetPublishStateIdForPublish()    
		WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePublishProduct ZPP WHERE ZPP.PimProductId = ZnodePimProduct.PimProductId   
		AND ZPP.PublishCatalogId = @PublishCatalogId) 
		

	UPDATE ZnodePublishCatalogLog   
	SET IsProductPublished = 1   
	,PublishProductId = (SELECT  COUNT(DISTINCT PublishProductId) FROM ZnodePublishCategoryProduct ZPP   
		WHERE ZPP.PublishCatalogId = ZnodePublishCatalogLog.PublishCatalogId AND ZPP.PublishCategoryId IS NOT NULL)   
	WHERE ZnodePublishCatalogLog.PublishCatalogId = @PublishCatalogId  
	and PublishStateId = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE PublishStateCode = 'Processing' ) 
    
	--UPDATE ZnodePublishCatalogLog   
	--SET IsProductPublished = 1   
	--,PublishProductId = (SELECT  COUNT(DISTINCT PublishProductId) FROM ZnodePublishCatalogProductDetail ZPP   
	--	WHERE ZPP.PublishCatalogId = ZnodePublishCatalogLog.PublishCatalogId AND ZPP.PimCategoryHierarchyId <> 0)   
	--WHERE ZnodePublishCatalogLog.PublishCatalogId = @PublishCatalogId  
	--AND PublishStateId = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE PublishStateCode = 'Processing' )  

	SET @Status = 1   

	IF OBJECT_ID('tempdb..##AttributeValueLocale') IS NOT NULL  
		DROP TABLE ##AttributeValueLocale; 

END TRY   
BEGIN CATCH   
	SELECT ERROR_MESSAGE()
	 SET @Status =0    
	 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),   
	  @ErrorLine VARCHAR(100)= ERROR_LINE(),  
	  @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishProductJson   
		 @PublishCatalogId = '+CAST(@PublishCatalogId AS VARCHAR (max))+',@UserId='+CAST(@UserId AS VARCHAR(50))  
	  +',@PimCatalogId = ' + CAST(@PimCatalogId AS varchar(20))  
	  +',@VersionIdString= ' + CAST(@VersionIdString AS varchar(20))  
       
	 EXEC Znode_InsertProcedureErrorLog  
	  @ProcedureName = 'Znode_GetPublishProductJson',  
	  @ErrorInProcedure = @Error_procedure,  
	  @ErrorMessage = @ErrorMessage,  
	  @ErrorLine = @ErrorLine,  
	  @ErrorCall = @ErrorCall;  
END CATCH  
  
END