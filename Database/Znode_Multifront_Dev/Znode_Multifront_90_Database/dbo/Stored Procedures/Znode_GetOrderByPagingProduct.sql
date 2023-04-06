CREATE PROCEDURE [dbo].[Znode_GetOrderByPagingProduct]
(
 @Order_by  Nvarchar(max)
 ,@Rows     INT =10 
 ,@PageNo   INT =1 
 ,@PimProductId TransferId Readonly 
 ,@AttributeCode VARCHAR(max)= ''
 ,@localeId INT  
 ,@PimCategoryHierarchyId INT  = 0
 ,@PortalId INT = 0 
 ,@PimCategoryId INT=NULL
)
AS 
BEGIN 
 SET NOCOUNT ON 
 SET @AttributeCode = CASE WHEN @AttributeCode = '' OR  @AttributeCode IS NULL THEN REPLACE(REPLACE (@Order_by , ' DESC',''),' ASC','')

  ELSE @AttributeCode END 
 DECLARE @StartId INT =  CASE WHEN @PageNo = 1 OR @PageNo = 0 THEN 1 ELSE ((@PageNo-1)*@Rows)+1 END 
 DECLARE @EndId INT = CASE WHEN @PageNo = 0 THEN @Rows ELSE @PageNo*@Rows END
 ,@DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleID()   
 
 DECLARE @AttributeTypeName NVARCHAR(2000)= ''

 SELECT TOP 1 @AttributeTypeName = AttributeTypeName 
 FROM ZnodePimAttribute ZPA 
 INNER JOIN ZnodeAttributeType ZTY ON (ZTY.AttributeTypeId = ZPA.AttributeTypeId)
 WHERE ZPA.AttributeCode = @AttributeCode

 if object_id('tempdb..#PimProductId') is not null
	drop table #PimProductId

 SELECT * INTO #PimProductId FROM @PimProductId
 
 --CREATE INDEX Idx_#PimProductId_IND ON #PimProductId(id) 

 IF  @Order_by = '' 
 BEGIN 
  
  create table #Cte_getDataOrder( PimProductId INT, RowId INT Identity )

  INSERT INTO #Cte_getDataOrder (PimProductId)
  SELECT Id  
  FROM #PimProductId TBLP
  INNER JOIN ZnodePimProduct ZPP ON (TBLP.Id= ZPP.PimProductId)

  SELECT PimProductId ,RowId
  FROM #Cte_getDataOrder CTE
  WHERE RowId BETWEEN @StartId AND @EndId
  order by RowId

 END 

  IF @PimCategoryHierarchyId <> 0 AND  @Order_by LIKE 'DisplayOrder%'
 BEGIN 
	create table #Cte_getData( PimProductId INT, RowId INT Identity )
	if @Order_by LIKE  '% DESC'
		INSERT INTO #Cte_getData (PimProductId)
		SELECT TBLP.Id
	    FROM #PimProductId TBLP
	    LEFT JOIN ZnodePimCategoryProduct ZPP ON TBLP.Id= ZPP.PimProductId 
		INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ( ZPP.PimCategoryId = ZPCH.PimCategoryId AND ZPCH.PimCategoryHierarchyId= @PimCategoryHierarchyId )
	    group by TBLP.Id, ZPP.DisplayOrder
		ORDER BY ZPP.DisplayOrder DESC
	else
		INSERT INTO #Cte_getData (PimProductId)
		SELECT TBLP.Id
	    FROM #PimProductId TBLP
	    LEFT JOIN ZnodePimCategoryProduct ZPP ON TBLP.Id= ZPP.PimProductId 
		INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ( ZPP.PimCategoryId = ZPCH.PimCategoryId AND ZPCH.PimCategoryHierarchyId= @PimCategoryHierarchyId )
	    group by TBLP.Id, ZPP.DisplayOrder
		ORDER BY ZPP.DisplayOrder ASC

	SELECT PimProductId ,RowId
	FROM #Cte_getData CTE
	WHERE RowId BETWEEN @StartId AND @EndId
	order by RowId
	
 END 
 ELSE 
  IF @PimCategoryHierarchyId <> 0 AND  @Order_by LIKE 'Price%'
 BEGIN 
         DECLARE @tbl_ProductPricingSkuOrderBy TABLE (sku nvarchar(200),RetailPrice numeric(28,6),SalesPrice numeric(28,6),TierPrice numeric(28,6),
						TierQuantity numeric(28,6),CurrencyCode varchar(200),CurrencySuffix varchar(2000),CultureCode varchar(2000), ExternalId NVARCHAR(2000))	
	     DECLARE @SKUS VARCHAR(max) 
				,@userId INT = 0,@Date DATETIME  = dbo.FN_getDate() 

				SELECT @SKUS = COALESCE(@SKUS+',' ,'') + SKU
				FROM ZnodePublishProductDetail a 
				INNER JOIN ZnodePublishProduct b ON ( a.PublishProductId =b.PublishProductId ) 
				INNER JOIN ZnodePimCategoryProduct f ON (f.PimProductId = b.PimProductId )
				INNER JOIN ZnodePimCategoryHierarchy g ON (g.PimCategoryHierarchyId= @PimCategoryHierarchyId)
				INNER JOIN ZnodePortalCatalog c ON (c.PublishCatalogId = b.PublishCatalogId)
				WHERE c.PortalId = @PortalId 
				AND EXISTS (SELECT TOP 1  1 FROM #PimProductId R WHERE b.PimProductId = R.Id)
				AND a.LocaleId =dbo.Fn_GetDefaultLocaleId()

			DECLARE @Id TransferId 

			INSERT INTO @tbl_ProductPricingSkuOrderBy		
			SELECT * FROM [dbo].[FN_GetPublishProductPricingBySku]( @SKUS,  @PortalId ,@Date, @userid,@Id)
		
		CREATE TABLE #Cte_getDataPrice ( PimProductId INT, RowId INT Identity )

		IF @Order_by LIKE  '% DESC'
			INSERT INTO #Cte_getDataPrice ( PimProductId )
			SELECT Id 
		    FROM #PimProductId TBLP
		    LEFT JOIN View_LoadManageProductInternal ZPP ON (TBLP.Id= ZPP.PimProductId AND ZPP.AttributeCode= 'SKU' )
		    LEFT JOIN @tbl_ProductPricingSkuOrderBy b ON (b.SKU = ZPP.AttributeValue) 
			ORDER BY ISNULL(b.RetailPrice,0) DESC
		ELSE
			INSERT INTO #Cte_getDataPrice ( PimProductId )
			SELECT Id 
		    FROM #PimProductId TBLP
		    LEFT JOIN View_LoadManageProductInternal ZPP ON (TBLP.Id= ZPP.PimProductId AND ZPP.AttributeCode= 'SKU' )
		    LEFT JOIN @tbl_ProductPricingSkuOrderBy b ON (b.SKU = ZPP.AttributeValue) 
			ORDER BY ISNULL(b.RetailPrice,0) ASC

	  SELECT PimProductId ,RowId
	  FROM #Cte_getDataPrice CTE
	  WHERE RowId BETWEEN @StartId AND @EndId
	  order by RowId 

 END 
 ELSE  
 IF  ( @Order_by LIKE 'PimProductId%'  OR @Order_by LIKE 'DisplayOrder%' ) AND @PimCategoryHierarchyId = 0 
 BEGIN 
	CREATE TABLE #Cte_PimProductId ( PimProductId INT, RowId INT Identity )
	IF @Order_by LIKE  '% DESC'
		insert into #Cte_PimProductId ( PimProductId ) 
		SELECT Id
	    FROM #PimProductId TBLP
	    INNER JOIN ZnodePimProduct ZPP ON (TBLP.Id= ZPP.PimProductId)
		LEFT JOIN ZnodePimCategoryProduct ZPCP ON(ZPCP.PimProductId = Zpp.PimProductId) AND ZPCP.PimCategoryId=@PimCategoryId
		ORDER BY CASE WHEN @Order_by LIKE 'PimProductId%' THEN ZPP.PimProductId ELSE ZPCP.DisplayOrder END DESC
		--order by ZPP.PimProductId DESC
	else 
		insert into #Cte_PimProductId ( PimProductId ) 
		SELECT Id
	    FROM #PimProductId TBLP
	    INNER JOIN ZnodePimProduct ZPP ON (TBLP.Id= ZPP.PimProductId)
		LEFT JOIN ZnodePimCategoryProduct ZPCP ON(ZPCP.PimProductId = Zpp.PimProductId) AND ZPCP.PimCategoryId=@PimCategoryId
		ORDER BY CASE WHEN @Order_by LIKE 'PimProductId%' THEN ZPP.PimProductId ELSE ZPCP.DisplayOrder END ASC
		--order by ZPP.PimProductId ASC

	SELECT PimProductId ,RowId
    FROM #Cte_PimProductId CTE
    WHERE RowId BETWEEN @StartId AND @EndId

 END 
 ELSE IF  @Order_by LIKE  'ModifiedDate%' 
 BEGIN 
	CREATE TABLE #Cte_GetDataModifiedDate ( PimProductId INT, RowId INT Identity )
	IF @Order_by LIKE  '% DESC'
	    insert into #Cte_GetDataModifiedDate ( PimProductId )
		SELECT Id 
	    FROM  #PimProductId TBLP
	    INNER JOIN ZnodePimAttributeValue ZPAV ON (TBLP.Id = ZPAV.PimProductId)
	    INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId) 
	    WHERE ZPA.AttributeCode = CASE WHEN @AttributeCode = '' OR @AttributeCode = 'ModifiedDate'  THEN 'SKU' ELSE @AttributeCode END
	    ORDER BY ZPAV.ModifiedDate DESC,ZPAV.PimProductId
	else
		insert into #Cte_GetDataModifiedDate ( PimProductId )
		SELECT Id 
	    FROM  #PimProductId TBLP
	    INNER JOIN ZnodePimAttributeValue ZPAV ON (TBLP.Id = ZPAV.PimProductId)
	    INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId) 
	    WHERE ZPA.AttributeCode = CASE WHEN @AttributeCode = '' OR @AttributeCode = 'ModifiedDate'  THEN 'SKU' ELSE @AttributeCode END
	    ORDER BY ZPAV.ModifiedDate ASC,ZPAV.PimProductId

		SELECT PimProductId ,RowId
	   FROM #Cte_GetDataModifiedDate CTE
	   WHERE RowId BETWEEN @StartId AND @EndId
	   ORDER BY RowId

 END
  ELSE IF  @Order_by LIKE  'PublishStatus%' 
 BEGIN 
  
	SELECT TBLP.Id ,CASE WHEN ZPP.IsProductPublish  IS NULL THEN 'Not Published' 
				WHEN ZPP.IsProductPublish = 0 THEN 'Draft'
				ELSE  'Published' END PublishStatus 
    into #Cte_GetData3
    FROM  #PimProductId TBLP
    INNER JOIN ZnodePimProduct ZPP oN (ZPP.PimProductId = TBLP.Id)

	CREATE TABLE #Cte_OrderPublishStatus ( PimProductId INT, RowId INT Identity )

	IF @Order_by LIKE  '% DESC'
		INSERT INTO #Cte_OrderPublishStatus ( PimProductId )
		SELECT Id 
	    FROM  #Cte_GetData3
		Order by PublishStatus DESC , Id
	ELSE
		INSERT INTO #Cte_OrderPublishStatus ( PimProductId )
		SELECT Id 
	    FROM  #Cte_GetData3
		Order by PublishStatus ASC , Id
		

	SELECT PimProductId ,RowId
    FROM #Cte_OrderPublishStatus CTE
    WHERE RowId BETWEEN @StartId AND @EndId
    Order by RowId

 END
 ELSE IF  @Order_by LIKE  'AttributeFamily%' 
 BEGIN 
	;With Cte_attributeValue AS 
    (
		 SELECT ZPAF.PimAttributeFamilyId,FamilyCode,AttributeFamilyName ,ZPFL.LocaleId
		 FROM ZnodePimAttributeFamily ZPAF
		 INNER JOIN ZnodePimFamilyLocale ZPFL ON (ZPFL.PimAttributeFamilyId = ZPAF.PimAttributeFamilyId) 
		 WHERE ZPFL.LocaleId IN (@DefaultLocaleId,@LocaleId)
	 ) 
   , Cte_AttributeValueAttribute AS 
    (
	   SELECT PimAttributeFamilyId,FamilyCode,AttributeFamilyName
	   FROM Cte_attributeValue RTY 
	   WHERE LocaleId = @LocaleId
     )
   , Cte_AttributeValueTht AS (
      SELECT PimAttributeFamilyId,FamilyCode,AttributeFamilyName
	  FROM Cte_AttributeValueAttribute
	  UNION ALL 
	  SELECT PimAttributeFamilyId,FamilyCode,AttributeFamilyName
	  FROM Cte_attributeValue TYY  
	  WHERE NOT EXISTS (SELECT TOP 1 1 FROM Cte_AttributeValueAttribute THE WHERE THE.PimAttributeFamilyId = TYY.PimAttributeFamilyId )
	  AND TYY.LocaleId = @DefaultLocaleId
	  )
  
  SELECT PimAttributeFamilyId,FamilyCode,AttributeFamilyName
  INTO #TBL_FamilyLocale
  FROM Cte_AttributeValueTht 

  CREATE TABLE #Cte_AttributeFamily ( PimProductId INT, RowId INT Identity )

	IF @Order_by LIKE  '% DESC'
		INSERT INTO #Cte_AttributeFamily ( PimProductId )
		SELECT  TBLAV.PimProductId 
	    FROM ZnodePimProduct TBLAV 
	    INNER JOIN #TBL_FamilyLocale THY ON (THY.PimAttributeFamilyId = TBLAV.PimAttributeFamilyId )
	    Order by THY.AttributeFamilyName DESC ,TBLAV.PimProductId
	ELSE
		INSERT INTO #Cte_AttributeFamily ( PimProductId )
		SELECT  TBLAV.PimProductId 
	    FROM ZnodePimProduct TBLAV 
	    INNER JOIN #TBL_FamilyLocale THY ON (THY.PimAttributeFamilyId = TBLAV.PimAttributeFamilyId )
	    Order by THY.AttributeFamilyName ASC ,TBLAV.PimProductId

	SELECT PimProductId ,RowId
    FROM #Cte_AttributeFamily CTE
    WHERE RowId BETWEEN @StartId AND @EndId
    order by RowId
 
 END
 ELSE IF @AttributeTypeName IN ('Text','Number','Datetime','Yes/No')
 BEGIN 
  IF @DefaultLocaleId = @LocaleID 
  BEGIN 

  CREATE TABLE #Cte_AttributeTypeName ( PimProductId int, RowId Int Identity)
	IF  @Order_by LIKE  '% DESC'
	BEGIN
		INSERT INTO #Cte_AttributeTypeName(PimProductId)
		SELECT ZPAV.PimProductId 
		FROM dbo.ZnodePimAttribute ZPA
		INNER JOIN dbo.ZnodePimAttributeValue ZPAV ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)
		INNER JOIN dbo.ZnodePimAttributeValueLocale ZPAVL ON (ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
		INNER JOIN dbo.ZnodeAttributeType ZTA ON (ZTA.AttributeTypeId = ZPA.AttributeTypeId)
		inner join #PimProductId P ON P.Id = ZPAV.PimProductId
		WHERE ZPA.IsCategory =0 
		AND IsShowOnGrid =1 
		and ZTA.AttributeTypeName IN ('Text','Number','Datetime','Yes/No')
		--and exists(select * from #PimProductId P where P.Id = ZPAV.PimProductId )
		and AttributeCode = @AttributeCode AND LocaleId = @LocaleID
		Order by ZPAVL.AttributeValue DESC ,ZPAV.PimProductId
						
	END
	Else
	BEGIN

		INSERT INTO #Cte_AttributeTypeName(PimProductId)
		SELECT ZPAV.PimProductId 
		FROM dbo.ZnodePimAttribute ZPA
		INNER JOIN dbo.ZnodePimAttributeValue ZPAV ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)
		INNER JOIN dbo.ZnodePimAttributeValueLocale ZPAVL ON (ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
		INNER JOIN dbo.ZnodeAttributeType ZTA ON (ZTA.AttributeTypeId = ZPA.AttributeTypeId)
		inner join #PimProductId P ON P.Id = ZPAV.PimProductId
		WHERE ZPA.IsCategory =0 
		AND IsShowOnGrid =1 
		and ZTA.AttributeTypeName IN ('Text','Number','Datetime','Yes/No')
		--and exists(select * from #PimProductId P where P.Id = ZPAV.PimProductId )
		and AttributeCode = @AttributeCode AND LocaleId = @LocaleID
		Order by ZPAVL.AttributeValue ASC ,ZPAV.PimProductId

	END
	create index Idx_#Cte_AttributeTypeName_RowId ON #Cte_AttributeTypeName(RowId)
	SELECT PimProductId ,RowId
	FROM #Cte_AttributeTypeName CTE
	WHERE RowId >= @StartId AND RowId <= @EndId
	order by RowId


  END 
  ELSE 
  BEGIN 

	 SELECT TBLAV.ID PimProductId,ZPAVL.AttributeCode,ZPAVL.AttributeValue,ZPAVL.LocaleId,COUNT(*)Over(Partition By TBLAV.ID,ZPAVL.AttributeCode ORDER BY TBLAV.ID,ZPAVL.AttributeCode  ) RowIdIn
	 into #Cte_AttributeDetails
	 FROM #PimProductId   TBLAV 
	 INNER JOIN View_PimProducttextValue ZPAVL ON (ZPAVL.PimProductId = TBLAV.id )
	 WHERE (LocaleId = @DefaultLocaleId OR LocaleId = @LocaleId  )
	 AND AttributeCode = @AttributeCode
	 --) 
	 --,Cte_DataLocale AS 
	 --(
	 CREATE TABLE #Cte_DataLocale (PimProductId INT, RowId INT Identity )

	 IF @Order_by LIKE  '% DESC'
		 insert into #Cte_DataLocale (PimProductId)
		 SELECT  TBLAV.PimProductId 
		 FROM #Cte_AttributeDetails TBLAV 
		 WHERE LocaleId = CASE WHEN RowIdIn =2 THEN @localeId ELSE @DefaultLocaleId END
		 order by TBLAV.AttributeValue DESC ,TBLAV.PimProductId 
	else
	     insert into #Cte_DataLocale (PimProductId)
		 SELECT  TBLAV.PimProductId 
		 FROM #Cte_AttributeDetails TBLAV 
		 WHERE LocaleId = CASE WHEN RowIdIn =2 THEN @localeId ELSE @DefaultLocaleId END
		 order by TBLAV.AttributeValue ASC ,TBLAV.PimProductId

	 SELECT PimProductId ,RowId
	 FROM #Cte_DataLocale 
	 WHERE RowId BETWEEN @StartId AND @EndId
	 order by RowId

  END 
 END
 ELSE IF @AttributeTypeName IN ('Simple Select','Multi Select') 
  BEGIN 
 DECLARE @PimAttributeId TransferId 

 INSERT INTO @PimAttributeId 
 SELECT PimAttributeId
 FROM  ZnodePimAttribute 
 WHERE AttributeCode = @AttributeCode  
 CREATE TABLE #TBL_AttributeDefaultValue ( PimAttributeId INT ,
              AttributeDefaultValueCode VARCHAR(max),IsEditable INT,AttributeDefaultValue NVARCHAR(max),DisplayOrder INT,PimAttributeDefaultValueId INT  ) 
 
			 -- here collect the both locale data 
             SELECT   VIPDV.PimAttributeId,VIPDV.AttributeDefaultValueCode,VIPDV.IsEditable,VIPDVL.AttributeDefaultValue,VIPDVL.LocaleId,VIPDV.PimAttributeDefaultValueId,VIPDV.DisplayOrder
             
			 INTO #Cte_DefaultValueLocale
			 FROM [dbo].[ZnodePimAttributeDefaultValue] VIPDV
			 INNER JOIN [dbo].[ZnodePimAttributeDefaultValueLocale] VIPDVL ON (VIPDVL.PimAttributeDefaultValueId = VIPDV.PimAttributeDefaultValueId) 
             WHERE VIPDVL.LocaleId IN(@DefaultLocaleId, @LocaleId) 
             AND EXISTS
             (
                SELECT TOP 1 1
                FROM @PimAttributeId SP
                WHERE SP.id = VIPDV.PimAttributeId
             )

			 -- filter for first locale
             ;with Cte_DefaultValueFirstLocale
             AS (SELECT CTDVL.PimAttributeId,CTDVL.AttributeDefaultValueCode,CTDVL.IsEditable,CTDVL.AttributeDefaultValue,CTDVL.PimAttributeDefaultValueId,CTDVL.DisplayOrder
                 FROM #Cte_DefaultValueLocale CTDVL
                 WHERE LocaleId = @LocaleId	 
                ),

			 -- get data for second locale if not exists for firts locale 
             Cte_DefaultValueSecondLocale
             AS (SELECT CTDVFL.PimAttributeId,CTDVFL.AttributeDefaultValueCode,CTDVFL.IsEditable,CTDVFL.AttributeDefaultValue,CTDVFL.PimAttributeDefaultValueId,CTDVFL.DisplayOrder
                 FROM Cte_DefaultValueFirstLocale CTDVFL
                 UNION ALL
                 SELECT CTDVL.PimAttributeId,CTDVL.AttributeDefaultValueCode,CTDVL.IsEditable,CTDVL.AttributeDefaultValue,CTDVL.PimAttributeDefaultValueId,CTDVL.DisplayOrder
                 FROM #Cte_DefaultValueLocale CTDVL
                 WHERE LocaleId = @DefaultLocaleId 
                 AND NOT EXISTS
                  (
                      SELECT TOP 1 1
                      FROM Cte_DefaultValueFirstLocale CTDVFL
                      WHERE CTDVFL.PimAttributeDefaultValueId = CTDVL.PimAttributeDefaultValueId
                  ))

                 

    
 INSERT INTO #TBL_AttributeDefaultValue(PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder,PimAttributeDefaultValueId)
  SELECT PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder,PimAttributeDefaultValueId
                  FROM Cte_DefaultValueSecondLocale;


  IF @DefaultLocaleId = @LocaleID 
  BEGIN
	  SELECT  PimProductId ,AttributeDefaultValue as AttributeValue
	  INTO #Cte_AttributeValue
	  FROM #PimProductId TBLP  
	  INNER JOIN ZnodePimAttributeValue ZPAV  ON (TBLP.ID = ZPAV.PimProductId )
	  INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)
	  INNER JOIN ZnodePimProductAttributeDefaultValue ZPAVL ON ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId
	  inner join #TBL_AttributeDefaultValue TTR  ON (TTR.PimAttributeDefaultValueId = ZPAVL.PimAttributeDefaultValueId )
	  WHERE AttributeCode = @AttributeCode AND ZPAVL.LocaleId = @LocaleID

	  create table #CTe_GetDataIn (PimProductId int, RowId int identity)

	 IF @Order_by LIKE  '% DESC'
		 insert into #CTe_GetDataIn (PimProductId)
		 SELECT PimProductId
		 FROM  #Cte_AttributeValue  VPP
		 order by  VPP.AttributeValue DESC ,VPP.PimProductId
	else
		insert into #CTe_GetDataIn (PimProductId)
		SELECT PimProductId
		FROM  #Cte_AttributeValue  VPP
		order by  VPP.AttributeValue asc ,VPP.PimProductId

	 SELECT PimProductId ,RowId
	 FROM #CTe_GetDataIn 
	 WHERE RowId BETWEEN @StartId AND @EndId 
	 order by RowId

   END 
   ELSE 
   BEGIN
    SELECT ZPAV.PimAttributeValueId,ZPAVL.PimAttributeDefaultValueId , ZPAVL.LocaleId ,COUNT(*)Over(Partition By ZPAV.PimAttributeValueId ,ZPAV.PimProductId ORDER BY ZPAV.PimAttributeValueId ,ZPAV.PimProductId  ) RowId
			   INTO #temp_Table 
			   FROM #PimProductId TBLP  
	           INNER JOIN ZnodePimAttributeValue ZPAV  ON (TBLP.ID = ZPAV.PimProductId )
			   INNER JOIN ZnodePimProductAttributeDefaultValue ZPAVL ON (ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
			   WHERE (ZPAVL.LocaleId = @localeId  OR ZPAVL.LocaleId = @DefaultlocaleId )

	  SELECT  PimProductId ,SUBSTRING((SELECT ','+AttributeDefaultValue FROM #TBL_AttributeDefaultValue TTR 
				INNER JOIN #temp_Table  ZPAVL ON (TTR.PimAttributeDefaultValueId = ZPAVL.PimAttributeDefaultValueId )
				WHERE ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId  
				AND ZPAVL.LocaleId = CASE WHEN ZPAVL.RowId = 2 THEN @LocaleId  ELSE @DefaultLocaleId  END  
				FOR XML PATH('') ),2,4000) AttributeValue
	  INTO #Cte_AttributeValue1
	  FROM #PimProductId TBLP  
	  INNER JOIN ZnodePimAttributeValue ZPAV  ON (TBLP.ID = ZPAV.PimProductId )
	  INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)
	  WHERE AttributeCode = @AttributeCode

	  create table #CTe_GetDataIn1 (PimProductId int, RowId int identity)

	  IF  @Order_by LIKE  '% DESC'
		INSERT INTO #CTe_GetDataIn1 ( PimProductId )
		SELECT PimProductId  
	    FROM  #Cte_AttributeValue  VPP
		ORDER BY VPP.AttributeValue DESC ,VPP.PimProductId
	 ELSE
		INSERT INTO #CTe_GetDataIn1 ( PimProductId )
		SELECT PimProductId  
	    FROM  #Cte_AttributeValue  VPP
		ORDER BY VPP.AttributeValue ASC ,VPP.PimProductId

	 SELECT PimProductId ,RowId
	 FROM #CTe_GetDataIn1 
	 WHERE RowId BETWEEN @StartId AND @EndId 
	 order by RowId

   END 
    DROP TABLE #TBL_AttributeDefaultValue
  END 
  ELSE IF @AttributeTypeName IN ('Text Area') 
  BEGIN 
   IF @DefaultLocaleId = @LocaleID 
   BEGIN 
		create table #CTe_TextArea (PimProductId int, RowId int identity)

		IF @Order_by LIKE  '% DESC'
			INSERT INTO #CTe_TextArea ( PimProductId )
			SELECT VPP.PimProductId  
			FROM #PimProductId TBLP 
			INNER JOIN View_PimProductTextAreaValue VPP ON (TBLP.Id = VPP.PimProductId ) 
			WHERE AttributeCode = @AttributeCode  AND LocaleId = @LocaleID
			Order by VPP.AttributeValue DESC ,VPP.PimProductId
		ELSE
			INSERT INTO #CTe_TextArea ( PimProductId )
			SELECT VPP.PimProductId  
			FROM #PimProductId TBLP 
			INNER JOIN View_PimProductTextAreaValue VPP ON (TBLP.Id = VPP.PimProductId ) 
			WHERE AttributeCode = @AttributeCode  AND LocaleId = @LocaleID
			Order by VPP.AttributeValue ASC ,VPP.PimProductId

	  SELECT PimProductId ,RowId
	  FROM #CTe_TextArea CTE
	  WHERE RowId BETWEEN @StartId AND @EndId
	  order by RowId

   END 
   ELSE 
   BEGIN 

		SELECT TBLAV.ID PimProductId,ZPAVL.AttributeCode,ZPAVL.AttributeValue,ZPAVL.LocaleId,COUNT(*)Over(Partition By ZPAVL.PimProductId,ZPAVL.AttributeCode ORDER BY ZPAVL.PimProductId,ZPAVL.AttributeCode  ) RowIdIn
	    INTO #Cte_AttributeDetails1
		FROM #PimProductId   TBLAV 
	    INNER JOIN View_PimProductTextAreaValue ZPAVL ON (ZPAVL.PimProductId = TBLAV.id )
	    WHERE (LocaleId = @DefaultLocaleId OR LocaleId = @LocaleId  )
	    AND AttributeCode = @AttributeCode

		create table #CTe_TextArea1 (PimProductId int, RowId int identity)

		IF @Order_by LIKE  '% DESC'
			INSERT INTO #CTe_TextArea1 ( PimProductId )
			SELECT  TBLAV.PimProductId 
  			FROM #Cte_AttributeDetails1 TBLAV 
			WHERE LocaleId = CASE WHEN RowIdIn = 2 THEN @localeId ELSE @DefaultLocaleId END  
			Order by TBLAV.AttributeValue DESC ,TBLAV.PimProductId
		ELSE
			INSERT INTO #CTe_TextArea1 ( PimProductId )
			SELECT  TBLAV.PimProductId 
  			FROM #Cte_AttributeDetails1 TBLAV 
			WHERE LocaleId = CASE WHEN RowIdIn = 2 THEN @localeId ELSE @DefaultLocaleId END  
			Order by TBLAV.AttributeValue ASC ,TBLAV.PimProductId

		SELECT PimProductId ,RowId
		FROM #CTe_TextArea1 
		WHERE RowId BETWEEN @StartId AND @EndId
		order by RowId 

	 if object_id('tempdb..#PimProductId') is not null
		drop table #PimProductId
   END 
END 
END