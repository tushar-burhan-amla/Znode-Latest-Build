
CREATE PROCEDURE [dbo].[Znode_GetPublishProductbulk]
(
@PublishCatalogId INT = 0 
,@VersionId       VARCHAR(50) = 0 
,@PimProductId    TransferId Readonly
,@UserId		  INT = 0 
,@PimCategoryHierarchyId  INT = 0 
,@PimCatalogId INT = 0 
,@LocaleIds TransferId READONLY
,@PublishStateId INT = 0 
)
With RECOMPILE
AS
-- 
/*
DECLARE @rrte transferId 
INSERT INTO @rrte
SELECT 1 

EXEC Znode_GetPublishProductbulk @PublishCatalogId=3,@UserId= 2 ,@localeIDs = @rrte,@PublishStateId = 3 

*/
BEGIN 
  BEGIN TRY 
 SET NOCOUNT ON 

EXEC Znode_InsertUpdatePimAttributeXML 1 
EXEC Znode_InsertUpdateCustomeFieldXML 1
EXEC Znode_InsertUpdateAttributeDefaultValue 1 

create table #ZnodePrice (RetailPrice numeric(28,13),SalesPrice numeric(28,13),CurrencyCode varchar(100), CultureCode varchar(100), CurrencySuffix varchar(100), PublishProductId int)
	
	create table #ProductSKU (SEOUrl nvarchar(max), SEODescription  nvarchar(max),SEOKeywords  nvarchar(max),SEOTitle  nvarchar(max), PublishProductId int)

	create table #ProductImages (PublishProductId int, ImageSmallPath  varchar(max))

	declare @DefaultPortal int, @IsAllowIndexing int
	select @DefaultPortal = PortalId, @IsAllowIndexing = 1 from ZnodePimCatalog ZPC Inner Join ZnodePublishCatalog ZPC1 ON ZPC.PimCatalogId = ZPC1.PimCatalogId where ZPC1.PublishCatalogId =  @PublishCatalogId and isnull(ZPC.IsAllowIndexing,0) = 1 


--DECLARE @PortalId INT = (SELECT TOP 1 POrtalId FROM ZnodePortalCatalog WHERE PublishCatalogId = @PublishCatalogId)
DECLARE @PriceListId INT = (SELECT TOP 1 PriceListId FROM ZnodePriceListPortal WHERE PortalId = @DefaultPortal
ORDER BY Precedence ASC )
DECLARE @DomainUrl varchar(max) = (select TOp 1 URL FROM ZnodeMediaConfiguration WHERE IsActive =1)
DECLARE @MaxSmallWidth INT  = (SELECT TOP 1  MAX(MaxSmallWidth) FROM ZnodePortalDisplaySetting WHERE PortalId = @DefaultPortal)


   IF OBJECT_ID('tempdb..#PimProductAttributeXML') is not null
   BEGIN 
	 DROP TABLE #PimProductAttributeXML
   END
   IF OBJECT_ID('tempdb..#PimDefaultValueLocale') is not null
   BEGIN 
    DROP TABLE #PimDefaultValueLocale
   END
   IF OBJECT_ID('tempdb..#TBL_CategoryCategoryHierarchyIds') is not null
   BEGIN 
    DROP TABLE #TBL_CategoryCategoryHierarchyIds
   END

   DECLARE @PimMediaAttributeId INT = dbo.Fn_GetProductImageAttributeId()
   
   CREATE TABLE #PimProductAttributeXML (PimAttributeXMLId INT  PRIMARY KEY ,PimAttributeId INT,LocaleId INT  )
  	
   CREATE TABLE #TBL_CategoryCategoryHierarchyIds  (CategoryId int , ParentCategoryId int ) 
	
   If (@PimCategoryHierarchyId <> 0 AND @PimCatalogId <> 0 )
		INSERT INTO #TBL_CategoryCategoryHierarchyIds(CategoryId , ParentCategoryId )
			Select Distinct PimCategoryId , Null FROM (
				SELECT PimCategoryId,ParentPimCategoryId from DBO.[Fn_GetRecurciveCategoryIds](@PimCategoryHierarchyId,@PimCatalogId)
				Union 
				Select PimCategoryId , null  from ZnodePimCategoryHierarchy where PimCategoryHierarchyId = @PimCategoryHierarchyId 
				Union 
				Select PimCategoryId , null  from [Fn_GetRecurciveCategoryIds_new] (@PimCategoryHierarchyId,@PimCatalogId) ) Category  


   CREATE TABLE #PimDefaultValueLocale  (PimAttributeDefaultXMLId INT  PRIMARY KEY ,PimAttributeDefaultValueId INT ,LocaleId INT ) 
   DECLARE @ProductNamePimAttributeId INT = dbo.Fn_GetProductNameAttributeId(),@DefaultLocaleId INT= Dbo.Fn_GetDefaultLocaleId(),@LocaleId INT = 0 
		,@SkuPimAttributeId  INT =  dbo.Fn_GetProductSKUAttributeId() , @IsActivePimAttributeId INT =  dbo.Fn_GetProductIsActiveAttributeId()
   DECLARE @GetDate DATETIME =dbo.Fn_GetDate()
   DECLARE @TBL_LocaleId  TABLE (RowId INT IDENTITY(1,1) PRIMARY KEY  , LocaleId INT )
			
			INSERT INTO @TBL_LocaleId (LocaleId)
			SELECT  LocaleId
			FROM ZnodeLocale MT 
			WHERE IsActive = 1
			AND (EXISTS (SELECT TOP 1 1  FROM @LocaleIds RT WHERE RT.Id = MT.LocaleId )
			OR NOT EXISTS (SELECT TOP 1 1 FROM @LocaleIds )) 

  DECLARE @Counter INT =1 ,@maxCountId INT = (SELECT max(RowId) FROM @TBL_LocaleId ) 
 

 CREATE TABLE #TBL_PublishCatalogId (PublishCatalogId INT,PublishProductId INT,PimProductId  INT   , VersionId INT, PublishCategoryId int ,LocaleId INT )

 CREATE INDEX idx_#TBL_PublishCatalogIdPimProductId on #TBL_PublishCatalogId(PimProductId)

  CREATE INDEX idx_#TBL_PublishCatalogIdPimPublishCatalogId on #TBL_PublishCatalogId(PublishCatalogId)

  If (@PimCategoryHierarchyId <> 0 AND @PimCatalogId <> 0 )
  BEGIN
			 INSERT INTO #TBL_PublishCatalogId(PublishCatalogId ,PublishProductId ,PimProductId  , VersionId ,PublishCategoryId ,LocaleId)  
			 SELECT distinct ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId,MAX(ZPCP.PublishCataloglogId ) VersionId ,ZPC.PublishCategoryId,ZPCP.LocaleId
				 FROM ZnodePublishProduct ZPP INNER JOIN ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)
				 INNER JOIN ZnodePublishCategoryProduct ZPPP ON ZPP.PublishProductId  = ZPPP.PublishProductId  
				 AND ZPCP.PublishCatalogId = ZPPP.PublishCatalogId
				 INNER JOIN ZnodePublishCategory ZPC ON ZPC.PublishCatalogId = ZPPP.PublishCatalogId AND ZPPP.PublishCategoryId = ZPC.PublishCategoryId 
				 WHERE ZPP.PublishCatalogId = @PublishCatalogId  and ZPCP.PublishStateId =  @PublishStateId
				 AND ZPC.PimCategoryId in (Select CategoryId from #TBL_CategoryCategoryHierarchyIds )
				 AND EXISTS(SELECT * FROM ZnodePimProduct ZPP1 where zpp.PimProductId = ZPP1.PimProductId )
				 GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId ,ZPC.PublishCategoryId,ZPCP.LocaleId	

			INSERT INTO #TBL_PublishCatalogId(PublishCatalogId ,PublishProductId ,PimProductId  , VersionId ,PublishCategoryId ,LocaleId)
			SELECT DISTINCT ZPP.PublishCatalogId,ZPP.PublishProductId,PimProductId,MAX(ZPCP.PublishCatalogLogId) VersionId,NULL, ZPCP.LocaleId
			FROM ZnodePublishProduct ZPP 
			INNER JOIN ZnodePublishCatalogLog ZPCP ON 
			(ZPCP.PublishCatalogId = ZPP.PublishCatalogId) 
			WHERE (EXISTS (SELECT TOP 1 1 FROM @pimProductId SP WHERE SP.Id = ZPP.PimProductId ))
			AND (ZPP.PublishCatalogId = @publishCatalogId )
			AND NOT Exists (Select TOP 1 1 from #TBL_PublishCatalogId TPL where TPL.PublishProductId = ZPP.PublishProductId)
			AND ZPCP.PublishStateId =  @PublishStateId
			AND EXISTS(select * from ZnodePimProduct ZPP1 where zpp.PimProductId = ZPP1.PimProductId )
			GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId , ZPCP.LocaleId



  END
  ELSE 
  BEGIN
			INSERT INTO #TBL_PublishCatalogId(PublishCatalogId ,PublishProductId,PimProductId ,VersionId,LocaleId  ) 
			 SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId, MAX(PublishCatalogLogId) ,ZPCP.LocaleId
				FROM ZnodePublishProduct ZPP 
				INNER JOIN ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)
				WHERE (EXISTS (SELECT TOP 1 1 FROM @PimProductId SP 
				WHERE SP.Id = ZPP.PimProductId  AND  (@PublishCatalogId IS NULL OR @PublishCatalogId = 0 ))
				OR  (ZPP.PublishCatalogId = @PublishCatalogId ))
				--AND  ZPCP.PublishStateId =  @PublishStateId
				AND EXISTS(select * from ZnodePimProduct ZPP1 where zpp.PimProductId = ZPP1.PimProductId )
				GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId ,ZPCP.LocaleId
  END
           
		     DECLARE   @TBL_ZnodeTempPublish TABLE (PimProductId INT , AttributeCode VARCHAR(300) ,AttributeValue NVARCHAR(max) ) 			
			 DECLARE @TBL_AttributeVAlueLocale TABLE(PimProductId INT,PimAttributeId INT,ZnodePimAttributeValueLocaleId INT,LocaleId INT 
			 )

	
	
	if (@IsAllowIndexing=1)
	begin 
		insert into #ZnodePrice
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
	
		insert INTO #ProductImages
		SELECT  TYU.PublishProductId , @DomainUrl +'Catalog/'  + CAST(@DefaultPortal AS VARCHAr(100)) + '/'+ CAST(@MaxSmallWidth AS VARCHAR(100)) + '/' + RT.MediaPath AS ImageSmallPath   
		FROM ZnodePimAttributeValue ZPAV 
		INNER JOIN ZnodePublishProduct TYU ON (TYU.PimProductId  = ZPAV.PimProductId)
		INNER JOIN ZnodePimProductAttributeMedia  RT ON ( RT.PimAttributeValueId = ZPAV.PimAttributeValueId )
		WHERE  TYU.PublishCatalogId = @PublishCatalogId
		AND RT.LocaleId = @DefaultLocaleId
		AND ZPAV.PimAttributeId = (SELECT TOp 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'ProductImage')
	
		insert INTO #ProductSKU 
		SELECT ZCSD.SEOUrl , ZCDL.SEODescription,ZCDL.SEOKeywords ,ZCDL.SEOTitle, TYU.PublishProductId
		FROM ZnodeCMSSEODetail ZCSD 
		INNER JOIN ZnodeCMSSEODetailLocale ZCDL ON (ZCDL.CMSSEODetailId = ZCSD.CMSSEODetailId)
		INNER JOIN ZnodePublishProductDetail TY ON (TY.SKU = ZCSD.SEOCode AND ZCDL.LocaleId = TY.LocaleId) 
		INNER JOIN ZnodePublishProduct TYU ON (TYU.PublishProductId = TY.PublishProductId)
		WHERE CMSSEOTypeId = (SELECT TOP 1 CMSSEOTypeId FROM ZnodeCMSSEOType WHERE Name = 'Product') 
		AND ZCDL.LocaleId = @DefaultLocaleId
		AND TYU.PublishCatalogId = @PublishCatalogId
		AND ZCSD.PublishStateId = @PublishStateId
		AND ZCSD.PortalId = @DefaultPortal

	end
		
		
WHILE @Counter <= @maxCountId
BEGIN
 
 SET @LocaleId = (SELECT TOP 1 LocaleId FROM @TBL_LocaleId MT 
  WHERE  RowId = @Counter)
 
  INSERT INTO #PimProductAttributeXML 
  SELECT PimAttributeXMLId ,PimAttributeId,LocaleId
  FROM ZnodePimAttributeXML
  WHERE LocaleId = @LocaleId
  
  IF( @LocaleId <> @DefaultLocaleId )
  BEGIN
	INSERT INTO #PimProductAttributeXML 
	SELECT PimAttributeXMLId ,PimAttributeId,LocaleId
	FROM ZnodePimAttributeXML ZPAX
	WHERE ZPAX.LocaleId = @DefaultLocaleId  
	AND NOT EXISTS (SELECT TOP 1 1 FROM #PimProductAttributeXML ZPAXI WHERE ZPAXI.PimAttributeId = ZPAX.PimAttributeId )
  END

  INSERT INTO #PimDefaultValueLocale
  SELECT PimAttributeDefaultXMLId,PimAttributeDefaultValueId,LocaleId 
  FROM ZnodePimAttributeDefaultXML
  WHERE localeId = @LocaleId

  IF( @LocaleId <> @DefaultLocaleId )
  BEGIN
	INSERT INTO #PimDefaultValueLocale 
	SELECT PimAttributeDefaultXMLId,PimAttributeDefaultValueId,LocaleId 
	FROM ZnodePimAttributeDefaultXML ZX
	WHERE localeId = @DefaultLocaleId
	AND NOT EXISTS (SELECT TOP 1 1 FROM #PimDefaultValueLocale TRTR WHERE TRTR.PimAttributeDefaultValueId = ZX.PimAttributeDefaultValueId)
  END
  	 
  CREATE TABLE #TBL_CustomeFiled  (PimCustomeFieldXMLId INT ,CustomCode VARCHAR(300),PimProductId INT ,LocaleId INT )

  INSERT INTO #TBL_CustomeFiled (PimCustomeFieldXMLId,PimProductId ,LocaleId,CustomCode)
  SELECT  PimCustomeFieldXMLId,RTR.PimProductId ,RTR.LocaleId,CustomCode
  FROM ZnodePimCustomeFieldXML RTR 
  INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = RTR.PimProductId AND RTR.LocaleID = ZPP.LocaleId)
  WHERE RTR.LocaleId = @LocaleId
 
 
  INSERT INTO #TBL_CustomeFiled (PimCustomeFieldXMLId,PimProductId ,LocaleId,CustomCode)
  SELECT  PimCustomeFieldXMLId,ITR.PimProductId ,ITR.LocaleId,CustomCode
  FROM ZnodePimCustomeFieldXML ITR
  INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ITR.PimProductId AND ITR.LocaleID = ZPP.LocaleId)
  WHERE ITR.LocaleId = @DefaultLocaleId
  AND NOT EXISTS (SELECT TOP 1 1 FROM #TBL_CustomeFiled TBL  WHERE ITR.CustomCode = TBL.CustomCode AND ITR.PimProductId = TBL.PimProductId)
       
       
	 SELECT VIR.PimProductId,PimAttributeId,ZnodePimAttributeValueLocaleId,VIR.LocaleId , VIR.AttributeValue, VIR.AttributeCode ,ROW_NUMBER() Over(Partition By VIR.PimProductId,PimAttributeId ORDER BY VIR.PimProductId,PimAttributeId  ) RowId
	 INTO #TBL_AttributeVAlue
	 FROM View_LoadManageProductInternal VIR
	 WHERE ( LocaleId = @DefaultLocaleId OR LocaleId = @LocaleId )
	 AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId ZPP WHERE (ZPP.PimProductId = VIR.PimProductId) )
	  UNION ALL 
	 SELECT VIR.PimProductId,VIR.PimAttributeId,ZPDE.PimProductAttributeMediaId,ZPDE.LocaleId ,ZPDE.MediaPath AS AttributeValue, d.AttributeCode ,ROW_NUMBER() Over(Partition By VIR.PimProductId,VIR.PimAttributeId ORDER BY VIR.PimProductId,VIR.PimAttributeId  ) RowId
	 FROM ZnodePimAttributeValue  VIR
	 INNER JOIN ZnodePimProductAttributeMedia ZPDE ON (ZPDE.PimAttributeValueId = VIR.PimAttributeValueId )
	 INNER JOIN ZnodePimAttribute d ON ( d.PimAttributeId=VIR.PimAttributeId )
	 WHERE ( ZPDE.LocaleId = @DefaultLocaleId OR ZPDE.LocaleId = @LocaleId )
	 AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId ZPP WHERE (ZPP.PimProductId = VIR.PimProductId) )
	  Union All
	 SELECT VIR.PimProductId,VIR.PimAttributeId,ZPDVL.PimAttributeDefaultValueLocaleId,ZPDVL.LocaleId ,ZPDVL.AttributeDefaultValue AS AttributeValue, d.AttributeCode ,ROW_NUMBER() Over(Partition By VIR.PimProductId,VIR.PimAttributeId ORDER BY VIR.PimProductId,VIR.PimAttributeId  ) RowId
	 FROM ZnodePimAttributeValue  VIR
	 INNER JOIN ZnodePimAttribute D ON ( D.PimAttributeId=VIR.PimAttributeId AND D.IsPersonalizable =1  )
	 INNER JOIN ZnodePimAttributeDefaultValue ZPADV ON ZPADV.PimAttributeId = D.PimAttributeId
	 INNER JOIN ZnodePimAttributeDefaultValueLocale ZPDVL   on (ZPADV.PimAttributeDefaultValueId = ZPDVL.PimAttributeDefaultValueId)
	 --INNER JOIN ZnodePimProductAttributeDefaultValue ZPDVP ON (ZPDVP.PimAttributeValueId = VIR.PimAttributeValueId AND ZPADV.PimAttributeDefaultValueId = ZPDVP.PimAttributeDefaultValueId )
	 WHERE ( ZPDVL.LocaleId = @DefaultLocaleId OR ZPDVL.LocaleId = @LocaleId )
	 AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId ZPP WHERE (ZPP.PimProductId = VIR.PimProductId) )
	 Union All 
	SELECT VIR.PimProductId,VIR.PimAttributeId,'','','',D.AttributeCode,ROW_NUMBER() Over(Partition By VIR.PimProductId,VIR.PimAttributeId ORDER BY VIR.PimProductId,VIR.PimAttributeId  ) RowId
	FROM ZnodePimAttributeValue  VIR
	INNER JOIN ZnodePimAttribute D ON ( D.PimAttributeId=VIR.PimAttributeId AND D.IsPersonalizable =1 )
	WHERE  EXISTS(SELECT TOP 1 1 FROM #TBL_PublishCatalogId ZPP WHERE (ZPP.PimProductId = VIR.PimProductId) )

	 
	 UPDATE #TBL_AttributeVAlue SET rowid = (SELECT MAX(rowid) FROM #TBL_AttributeVAlue b WHERE a.PimProductId=b.PimProductId AND a.PimAttributeId = b.PimAttributeId )
	 FROM #TBL_AttributeVAlue a
	
  --SET @versionId = (SELECT TOP 1 VersionId FROM #TBL_PublishCatalogId) 
  

 IF OBJECT_ID('tempdb..#Cte_GetData') IS NOT NULL
 BEGIN 
 DROP TABLE #Cte_GetData
 END 

 CREATE TABLE #Cte_GetData (PimProductId INT,AttributeCode VARCHAR(600),AttributeValue NVARCHAR(max),VersionId INT)

 CREATE INDEX idx_#Cte_GetDataPimProductId ON #Cte_GetData(PimProductId)


INSERT INTO #Cte_GetData(PimProductId ,AttributeCode,AttributeValue,VersionId)
SELECT  a.PimProductId,a.AttributeCode , '<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+ISNULL(a.AttributeValue,'')+'</AttributeValues> </AttributeEntity>  </Attributes>'  AttributeValue,VersionId
FROM #TBL_AttributeVAlue a 
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = a.PimAttributeId )
INNER JOIN #PimProductAttributeXML b ON (b.PimAttributeXMLId = c.PimAttributeXMLId)
INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = a.PimProductId)
WHERE a.LocaleId  = CASE WHEN a.RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END
AND NOT EXISTS (SELECT TOP 1 1 FROM Fn_GetProductMediaAttributeId() TY WHERE TY.PimAttributeId = c.PimAttributeId)
 


INSERT INTO #Cte_GetData(PimProductId ,AttributeCode,AttributeValue,VersionId)
SELECT THB.PimProductId,THB.CustomCode,'<Attributes><AttributeEntity>'+CustomeFiledXML+'</AttributeEntity></Attributes>' ,VersionId
FROM ZnodePimCustomeFieldXML THB 
INNER JOIN #TBL_CustomeFiled TRTE ON (TRTE.PimCustomeFieldXMLId = THB.PimCustomeFieldXMLId)
INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = THB.PimProductId)
UNION ALL 
SELECT ZPAV.PimProductId,c.AttributeCode,'<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues></AttributeValues>'+'<SelectValues>'+
			   STUFF((
                    SELECT '  '+ DefaultValueXML  FROM ZnodePimAttributeDefaultXML AA 
				 INNER JOIN #PimDefaultValueLocale GH ON (GH.PimAttributeDefaultXMLId = AA.PimAttributeDefaultXMLId)
				 INNER JOIN ZnodePimProductAttributeDefaultValue ZPADV ON ( ZPADV.PimAttributeDefaultValueId = AA.PimAttributeDefaultValueId )
				 WHERE (ZPADV.PimAttributeValueId = ZPAV.PimAttributeValueId)
    FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</SelectValues> </AttributeEntity></Attributes>' AttributeValue ,VersionId
FROM ZnodePimAttributeValue ZPAV  
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue ZPADVL WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
AND EXISTS (select * from #PimProductAttributeXML b where b.PimAttributeXMLId = c.PimAttributeXMLId)

---for PLP

INSERT INTO #Cte_GetData(PimProductId ,AttributeCode,AttributeValue,VersionId)
SELECT DISTINCT  UOP.PimProductId,c.AttributeCode,'<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues></AttributeValues>'+'<SelectValues>'+
			   STUFF((
                    SELECT DISTINCT '  '+REPLACE(DefaultValueXML,'</SelectValuesEntity>','<VariantDisplayOrder>'+CAST(ISNULL(ZPA.DisplayOrder,0) AS VARCHAR(200))+'</VariantDisplayOrder>
					<VariantSKU>'+ISNULL(ZPAVL_SKU.AttributeValue,'')+'</VariantSKU>
					<VariantImagePath>'+ISNULL((SELECT ''+ZM.Path FOR XML Path ('')),'')+'</VariantImagePath></SelectValuesEntity>')   
				 FROM ZnodePimAttributeDefaultXML AA 
				 INNER JOIN #PimDefaultValueLocale GH ON (GH.PimAttributeDefaultXMLId = AA.PimAttributeDefaultXMLId)
				 INNER JOIN ZnodePimProductAttributeDefaultValue ZPADV ON ( ZPADV.PimAttributeDefaultValueId = AA.PimAttributeDefaultValueId )
				 INNER JOIN ZnodePimAttributeValue ZPAV1 ON (ZPAV1.PimAttributeValueId= ZPADV.PimAttributeValueId )
				 -- check/join for active variants 
				 INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId =ZPAV1.PimProductId)
				 INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (ZPAV.PimAttributevalueid = ZPAVL.PimAttributeValueId AND ZPAVL.AttributeValue = 'True')
				 INNER JOIN ZnodePimProductTypeAssociation YUP ON (YUP.PimProductId = ZPAV1.PimProductId)
				 -- SKU
				 INNER JOIN ZnodePimAttributeValue ZPAV_SKU ON(YUP.PimProductId = ZPAV_SKU.PimProductId)
				 INNER JOIN ZnodePimAttributeValueLocale ZPAVL_SKU ON (ZPAVL_SKU.PimAttributeValueId = ZPAV_SKU.PimAttributeValueId)
				 LEFT  JOIN ZnodePimAttributeValue ZPAV12 ON (ZPAV12.PimProductId= YUP.PimProductId  AND ZPAV12.PimAttributeId = @PimMediaAttributeId ) 
				 LEFT JOIN ZnodePimProductAttributeMedia ZPAVM ON (ZPAVM.PimAttributeValueId= ZPAV12.PimAttributeValueId ) 
				 LEFT JOIN ZnodeMedia ZM ON (ZM.MediaId = ZPAVM.MediaId)
				 LEFT JOIN ZnodePimAttribute ZPA ON (ZPA.PimattributeId = ZPAV1.PimAttributeId)
				 WHERE (YUP.PimParentProductId  = UOP.PimProductId AND ZPAV1.pimAttributeId = UOP.PimAttributeId )
				 -- Active Variants
				 AND ZPAV.PimAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'IsActive')
				 -- VariantSKU
				 AND ZPAV_SKU.PimAttributeId = (SELECT PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'SKU')
FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</SelectValues> </AttributeEntity></Attributes>' AttributeValue ,VersionId
FROM ZnodePimConfigureProductAttribute UOP 
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = UOP.PimAttributeId )
INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = UOP.PimProductId)
WHERE  EXISTS (SELECT * FROM #PimProductAttributeXML b WHERE b.PimAttributeXMLId = c.PimAttributeXMLId)


INSERT INTO #Cte_GetData(PimProductId ,AttributeCode,AttributeValue,VersionId)
SELECT DISTINCT  ZPAV.PimProductId,c.AttributeCode,'<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+SUBSTRING((SELECT DISTINCT  ',' +ZPPG.MediaPath FROM ZnodePimProductAttributeMedia ZPPG
     INNER JOIN #TBL_AttributeVAlue FTRE ON (FTRE.PimProductId = ZPAV.PimProductId AND FTRE.PimAttributeId = ZPAV.PimAttributeId  AND FTRE.LocaleId  = CASE WHEN FTRE.RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END )
	 WHERE ZPPG.PimProductAttributeMediaId = FTRE.ZnodePimAttributeValueLocaleId
	 FOR XML PATH ('')
 ),2,4000)+'</AttributeValues></AttributeEntity></Attributes>' AttributeValue ,VersionId	 
FROM ZnodePimAttributeValue ZPAV 
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeMedia ZPADVL WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
AND EXISTS (SELECT * FROM #PimProductAttributeXML b WHERE b.PimAttributeXMLId = c.PimAttributeXMLId)

insert into #Cte_GetData(PimProductId ,AttributeCode,AttributeValue,VersionId)
SELECT ZPLP.PimParentProductId ,c.AttributeCode, '<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+ISNULL(SUBSTRING((SELECT DISTINCT ','+CAST(PublishProductId AS VARCHAR(50)) 
							 FROM #TBL_PublishCatalogId ZPPI 
							 INNER JOIN ZnodePimLinkProductDetail ZPLPI ON (ZPLPI.PimProductId = ZPPI.PimProductId)
							 WHERE ZPLPI.PimParentProductId = ZPLP.PimParentProductId
							 AND ZPLPI.PimAttributeId   = ZPLP.PimAttributeId
							 FOR XML PATH ('') ),2,4000),'')+'</AttributeValues></AttributeEntity></Attributes>'   AttributeValue ,ZPP.VersionId
							
FROM ZnodePimLinkProductDetail ZPLP  
INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPLP.PimParentProductId)
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPLP.PimAttributeId )
WHERE EXISTS (SELECT * FROM #PimProductAttributeXML b WHERE b.PimAttributeXMLId = c.PimAttributeXMLId)
GROUP BY ZPLP.PimParentProductId , ZPP.PublishProductId  ,ZPLP.PimAttributeId,c.AttributeCode,c.AttributeXML,ZPP.PublishCatalogId,ZPP.VersionId



SELECT a.PimProductId ,CAST((SELECT ''+dbo.FN_trim(b.AttributeValue) FOR XML PATH(''))  AS NVARCHAR(max)) AttributeValue , b.LocaleId  ,a.PimAttributeId,c.AttributeCode ,b.ZnodePimAttributeValueLocaleId
INTO #View_LoadManageProductInternal
FROM ZnodePimAttributeValue a 
INNER JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )
INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
WHERE c.AttributeCode = 'SKU'

INSERT INTO #Cte_GetData(PimProductId ,AttributeCode,AttributeValue,VersionId)
SELECT ZPAV.PimProductId,'DefaultSkuForConfigurable' ,'<Attributes><AttributeEntity>'+REPLACE(REPLACE (c.AttributeXML,'ProductType','DefaultSkuForConfigurable'),'Product Type','Default Sku For Configurable')+'<AttributeValues>'+
 (SELECT TOP 1 AttributeValue FROM #View_LoadManageProductInternal ad 
 INNER JOIN ZnodePimProductTypeAssociation yt ON (yt.PimProductId = ad.PimProductId)
 WHERE Ad.AttributeCode = 'SKU'
 AND yt.PimParentProductId = ZPAV.PimProductId
 ORDER BY yt.DisplayOrder , yt.PimProductTypeAssociationId ASC)
+'</AttributeValues></AttributeEntity></Attributes>' AttributeValue ,ZPP.VersionId
FROM ZnodePimAttributeValue ZPAV  
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue ZPADVL 
INNER JOIN ZnodePimAttributeDefaultValue dr ON (dr.PimAttributeDefaultValueId = ZPADVL.PimAttributeDefaultValueId)
 WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId
 AND dr.AttributeDefaultValueCode= 'ConfigurableProduct' 
)
AND EXISTS (SELECT * FROM #PimProductAttributeXML b WHERE b.PimAttributeXMLId = c.PimAttributeXMLId)
AND c.AttributeCode = 'ProductType' 


---------brand details 
CREATE TABLE #Cte_BrandData (PimProductId INT,BrandXML NVARCHAR(max))

INSERT INTO #Cte_BrandData ( PimProductId, BrandXML )
SELECT  DISTINCT ZBP.PimProductId,'<Brands><BrandEntity><BrandId>'+CAST(ZBD.BrandId AS VARCHAR(50))+'</BrandId><BrandCode>'+ZBD.BrandCode+'</BrandCode><BrandName>'+ZBDL.BrandName+'</BrandName></BrandEntity></Brands>' as BrandXML					   		   
FROM [ZnodeBrandDetails] AS ZBD
INNER JOIN ZnodeBrandDetaillocale ZBDL ON ZBD.BrandId = ZBDL.BrandId
INNER JOIN [ZnodeBrandProduct] AS ZBP ON ZBD.BrandId = ZBP.BrandId

--  --CREATE INDEX IND_Znode

  DELETE tu FROM ZnodePublishedXml tu  WHERE 
  EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId TY WHERE TY.VersionId = tu.PublishCatalogLogId AND Tu.PublishedId = ty.PublishProductId  )
  AND IsProductXML = 1   AND LocaleId = @LocaleId 

  
--  --ALTER INDEX ALL ON ZnodePublishedXml  REBUILD WITH (FILLFACTOR = 80 ) 
  If (@PimCategoryHierarchyId <> 0 AND @PimCatalogId <> 0 )
  BEGIN
		
		--Collect index of other categorys
		IF OBJECT_ID('tempdb..#Index') IS NOT NULL
		BEGIN 
			DROP TABLE #Index
		END 
		CREATE TABLE #Index (RowIndex int ,PublishCategoryId int , PublishProductId  int )		
		INSERT INTO  #Index ( RowIndex ,PublishCategoryId , PublishProductId )
		SELECT CAST(ROW_NUMBER()Over(Partition BY ZPC.PublishProductId 
		Order BY ISNULL(ZPC.PublishCategoryId,'0') desc )   AS VARCHAR(100)),
		ZPC.PublishCategoryId , ZPC.PublishProductId
		FROM ZnodePublishCategoryProduct ZPC where ZPC.PublishCatalogId = @PublishCatalogId
	
	

		--Publish parent products with index number 
			INSERT INTO ZnodePublishedXml (PublishCatalogLogId,PublishedId,PublishedXML,IsProductXML,LocaleId
			,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PublishCategoryId)
			SELECT zpp.VersionId,zpp.PublishProductId,'<ProductEntity><VersionId>'+CAST(zpp.VersionId AS VARCHAR(50)) +'</VersionId><ZnodeProductId>'+CAST(zpp.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><ZnodeCategoryIds>'+CAST(ISNULL(ZPCP.PublishCategoryId,'')  AS VARCHAR(50))+'</ZnodeCategoryIds><Name>'+CAST(ISNULL((SELECT ''+ZPPDFG.ProductName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</Name>'+'<SKU>'+CAST(ISNULL((SELECT ''+ZPPDFG.SKU FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKU>'+'<SKULower>'+CAST(ISNULL((SELECT ''+LOWER(ZPPDFG.SKU) FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKULower>'+'<IsActive>'+CAST(ISNULL(ZPPDFG.IsActive ,'0') AS VARCHAR(50))+'</IsActive>' 
			+'<ZnodeCatalogId>'+CAST(ZPP.PublishCatalogId  AS VARCHAR(50))+'</ZnodeCatalogId><IsParentProducts>'+CASE WHEN ZPCD.PublishCategoryId IS NULL THEN '0' ELSE '1' END  +'</IsParentProducts><CategoryName>'+CAST(ISNULL((SELECT ''+PublishCategoryName FOR XML PATH ('')),'') AS NVARCHAR(2000)) +'</CategoryName><CatalogName>'+CAST(ISNULL((SELECT ''+CatalogName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</CatalogName><LocaleId>'+CAST( @LocaleId AS VARCHAR(50))+'</LocaleId>'
			+'<TempProfileIds>'+ISNULL(SUBSTRING( (SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
			FROM ZnodeProfileCatalog ZPFC 
			INNER JOIN ZnodeProfileCatalogCategory ZPCCH  ON ( ZPCCH.ProfileCatalogId = ZPFC.ProfileCatalogId )
			WHERE ZPCCH.PimCatalogCategoryId = ZPCCF.PimCatalogCategoryId  FOR XML PATH('')),2,8000),'')+
			'</TempProfileIds>
			<ProductIndex>'+ CAST(ISNULL(ZPCP.ProductIndex,1) AS VARCHAR(200))+
			'</ProductIndex>' +
			 
			Case When @IsAllowIndexing = 1 then 			
			+'<RetailPrice>'+ISNULL(CAST(RetailPrice  AS VARCHAr(500)),'')+'</RetailPrice>'
			+'<SalesPrice>'+ISNULL(CAST(SalesPrice AS VARCHAr(500)), '') +'</SalesPrice>'
			+'<CurrencyCode>'+ISNULL(CurrencyCode,'') +'</CurrencyCode>'
			+'<CultureCode>'+ISNULL(CultureCode,'') +'</CultureCode>'
			+'<CurrencySuffix>'+ISNULL(CurrencySuffix,'') +'</CurrencySuffix>'
			+'<SeoUrl>'+ISNULL(SEOUrl,'') +'</SeoUrl>'
			+'<SeoDescription>'+ISNULL(SEODescription,'') +'</SeoDescription>'
			+'<SeoKeywords>'+ISNULL(SEOKeywords,'') +'</SeoKeywords>'
			+'<SeoTitle>'+ISNULL(SEOTitle,'') +'</SeoTitle>'
			+'<ImageSmallPath>'+ISNULL(ImageSmallPath,'') +'</ImageSmallPath>'
			else '' End

			+'<IndexId>'
			+CAST( ISNULL(ZPCP.PublishCategoryProductId,'0') AS VARCHAr(100))+'</IndexId><IndexId>'+CAST( ISNULL(ZPCP.PublishCategoryProductId,'0') AS VARCHAr(100))+'</IndexId>'+
			'<DisplayOrder>'+CAST(ISNULL(ZPCCF.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder>'+
			ISNULL(STUFF(( SELECT '  '+ BrandXML  FROM #Cte_BrandData BD WHERE BD.PimProductId = ZPP.PimProductId   
			FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, ''),'')+
			STUFF(( SELECT '  '+ AttributeValue  FROM #Cte_GetData TY WHERE TY.PimProductId = ZPP.PimProductId AND TY.VersionId = ZPP.VersionId   
			FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</ProductEntity>' xmlvalue,1,@LocaleId,@UserId , @GetDate , @UserId,@GetDate
			,ZPCP.PublishCategoryId
			FROM  #TBL_PublishCatalogId zpp
			INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPP.PublishCatalogId)
			INNER JOIN ZnodePublishProductDetail ZPPDFG ON (ZPPDFG.PublishProductId =  ZPP.PublishProductId)
			LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishProductId = ZPP.PublishProductId AND ZPCP.PublishCatalogId = ZPP.PublishCatalogId)
			LEFT JOIN ZnodePublishCategory ZPC ON (ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND   ZPC.PublishCategoryId = ZPCP.PublishCategoryId 
			AND ZPP.PublishCategoryId = ZPC.PublishCategoryId 
			)
			LEFT JOIN ZnodePimCatalogCategory ZPCCF ON (ZPCCF.PimCatalogId = ZPCV.PimCatalogId AND ZPCCF.PimCategoryId = ZPC.PimCategoryId  AND ZPCCF.PimProductId = ZPP.PimProductId AND  ZPCCF.PimCategoryHierarchyId =  ZPC.PimCategoryHierarchyId
			)
			LEFT JOIN ZnodePublishCategoryDetail ZPCD ON (ZPCD.PublishCategoryId = ISNULL(ZPCP.PublishCategoryId,0) AND ZPCD.LocaleId = @LocaleId )
			LEFT JOIN #ZnodePrice TBZP ON (TBZP.PublishProductId = ZPP.PublishProductId)
			LEFT JOIN #ProductSKU TBPS ON (TBPS.PublishProductId = ZPP.PublishProductId)
			LEFT JOIN #ProductImages TBPI ON (TBPI.PublishProductId = ZPP.PublishProductId)
			WHERE ZPPDFG.LocaleId = @LocaleId AND ZPP.LocaleId = @LocaleId AND  ZPC.PimCategoryId in (Select CategoryId from #TBL_CategoryCategoryHierarchyIds ) 
			and zpp.PublishCategoryId IS NOT NULL

		
	
	 --Publish only associate product 
	 INSERT INTO ZnodePublishedXml (PublishCatalogLogId,PublishedId,PublishedXML,IsProductXML,LocaleId
		,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PublishCategoryId)
		SELECT zpp.VersionId,zpp.PublishProductId,'<ProductEntity><VersionId>'+CAST(zpp.VersionId AS VARCHAR(50)) +'</VersionId><ZnodeProductId>'+CAST(zpp.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><ZnodeCategoryIds>'+CAST(ISNULL(ZPCP.PublishCategoryId,'')  AS VARCHAR(50))+'</ZnodeCategoryIds><Name>'+CAST(ISNULL((SELECT ''+ZPPDFG.ProductName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</Name>'+'<SKU>'+CAST(ISNULL((SELECT ''+ZPPDFG.SKU FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKU>'+'<SKULower>'+CAST(ISNULL((SELECT ''+LOWER(ZPPDFG.SKU) FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKULower>'+'<IsActive>'+CAST(ISNULL(ZPPDFG.IsActive ,'0') AS VARCHAR(50))+'</IsActive>' 
		+'<ZnodeCatalogId>'+CAST(ZPP.PublishCatalogId  AS VARCHAR(50))+'</ZnodeCatalogId><IsParentProducts>'+CASE WHEN ZPCD.PublishCategoryId IS NULL THEN '0' ELSE '1' END  +'</IsParentProducts><CategoryName>'+CAST(ISNULL((SELECT ''+PublishCategoryName FOR XML PATH ('')),'') AS NVARCHAR(2000)) +'</CategoryName><CatalogName>'+CAST(ISNULL((SELECT ''+CatalogName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</CatalogName><LocaleId>'+CAST( @LocaleId AS VARCHAR(50))+'</LocaleId>'
		+'<TempProfileIds>'+ISNULL(SUBSTRING( (SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
						FROM ZnodeProfileCatalog ZPFC 
						INNER JOIN ZnodeProfileCatalogCategory ZPCCH  ON ( ZPCCH.ProfileCatalogId = ZPFC.ProfileCatalogId )
						WHERE ZPCCH.PimCatalogCategoryId = ZPCCF.PimCatalogCategoryId  FOR XML PATH('')),2,8000),'')+
						'</TempProfileIds>
						 <ProductIndex>'+ CAST(ISNULL(ZPCP.ProductIndex,1) AS VARCHAr(200))+
						'</ProductIndex>'+
		
		Case When @IsAllowIndexing = 1 then 
		+'<RetailPrice>'+ISNULL(CAST(RetailPrice  AS VARCHAr(500)),'')+'</RetailPrice>'
		+'<SalesPrice>'+ISNULL(CAST(SalesPrice AS VARCHAr(500)), '') +'</SalesPrice>'
		+'<CurrencyCode>'+ISNULL(CurrencyCode,'') +'</CurrencyCode>'
		+'<CultureCode>'+ISNULL(CultureCode,'') +'</CultureCode>'
		+'<CurrencySuffix>'+ISNULL(CurrencySuffix,'') +'</CurrencySuffix>'
		+'<SeoUrl>'+ISNULL(SEOUrl,'') +'</SeoUrl>'
		+'<SeoDescription>'+ISNULL(SEODescription,'') +'</SeoDescription>'
		+'<SeoKeywords>'+ISNULL(SEOKeywords,'') +'</SeoKeywords>'
		+'<SeoTitle>'+ISNULL(SEOTitle,'') +'</SeoTitle>'
		+'<ImageSmallPath>'+ISNULL(ImageSmallPath,'') +'</ImageSmallPath>'
		else '' End

		+'<IndexId>'
		+CAST( ISNULL(ZPCP.PublishCategoryProductId,'0') AS VARCHAr(100))+'</IndexId><IndexId>'+CAST( ISNULL(ZPCP.PublishCategoryProductId,'0') AS VARCHAr(100))+'</IndexId>'+
		'<DisplayOrder>'+CAST(ISNULL(ZPCCF.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder>'+
		STUFF(( SELECT '  '+ AttributeValue  FROM #Cte_GetData TY WHERE TY.PimProductId = ZPP.PimProductId   AND TY.VersionId= ZPP.VersionId
		FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</ProductEntity>' xmlvalue,1,@LocaleId,@UserId , @GetDate , @UserId,@GetDate
		,ZPCP.PublishCategoryId
		FROM  #TBL_PublishCatalogId zpp
		INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPP.PublishCatalogId)
		INNER JOIN ZnodePublishProductDetail ZPPDFG ON (ZPPDFG.PublishProductId =  ZPP.PublishProductId)
		LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishProductId = ZPP.PublishProductId AND ZPCP.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT JOIN ZnodePublishCategory ZPC ON (ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND   ZPC.PublishCategoryId = ZPCP.PublishCategoryId 
		AND ZPP.PublishCategoryId = ZPC.PublishCategoryId )
		AND ZPC.PimCategoryId in (Select CategoryId from #TBL_CategoryCategoryHierarchyIds )
		LEFT JOIN ZnodePimCatalogCategory ZPCCF ON (ZPCCF.PimCatalogId = ZPCV.PimCatalogId AND ZPCCF.PimCategoryId = ZPC.PimCategoryId  AND ZPCCF.PimProductId = ZPP.PimProductId AND  ZPCCF.PimCategoryHierarchyId =  ZPC.PimCategoryHierarchyId
		AND ZPCCF.PimCategoryId in (Select CategoryId from #TBL_CategoryCategoryHierarchyIds ))
		LEFT JOIN ZnodePublishCategoryDetail ZPCD ON (ZPCD.PublishCategoryId = ISNULL(ZPCP.PublishCategoryId,0) AND ZPCD.LocaleId = @LocaleId )
		LEFT JOIN #ZnodePrice TBZP ON (TBZP.PublishProductId = ZPP.PublishProductId)
		LEFT JOIN #ProductSKU TBPS ON (TBPS.PublishProductId = ZPP.PublishProductId)
		LEFT JOIN #ProductImages TBPI ON (TBPI.PublishProductId = ZPP.PublishProductId)

		WHERE ZPPDFG.LocaleId = @LocaleId AND ZPP.LocaleId = @LocaleId AND zpp.PublishCategoryId IS NULL
		
  END
  ELSE
  BEGIN

      INSERT INTO ZnodePublishedXml (PublishCatalogLogId
		,PublishedId
		,PublishedXML
		,IsProductXML
		,LocaleId
		,CreatedBy
		,CreatedDate
		,ModifiedBy
		,ModifiedDate
		,PublishCategoryId)
		SELECT zpp.VersionId,zpp.PublishProductId, '<ProductEntity><VersionId>'+CAST(zpp.VersionId AS VARCHAR(50)) +'</VersionId><ZnodeProductId>'+CAST(zpp.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><ZnodeCategoryIds>'+CAST(ISNULL(ZPCP.PublishCategoryId,'')  AS VARCHAR(50))+'</ZnodeCategoryIds><Name>'+CAST(ISNULL((SELECT ''+ZPPDFG.ProductName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</Name>'+'<SKU>'+CAST(ISNULL((SELECT ''+ZPPDFG.SKU FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKU>'+'<SKULower>'+CAST(ISNULL((SELECT ''+LOWER(ZPPDFG.SKU) FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKULower>'+'<IsActive>'+CAST(ISNULL(ZPPDFG.IsActive ,'0') AS VARCHAR(50))+'</IsActive>' 
		+'<ZnodeCatalogId>'+CAST(ZPP.PublishCatalogId  AS VARCHAR(50))+'</ZnodeCatalogId><IsParentProducts>'+CASE WHEN ZPCD.PublishCategoryId IS NULL THEN '0' ELSE '1' END  +'</IsParentProducts><CategoryName>'+CAST(ISNULL((SELECT ''+PublishCategoryName FOR XML PATH ('')),'') AS NVARCHAR(2000)) +'</CategoryName><CatalogName>'+CAST(ISNULL((SELECT ''+CatalogName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</CatalogName><LocaleId>'+CAST( @LocaleId AS VARCHAR(50))+'</LocaleId>'
		
		+Case When @IsAllowIndexing = 1 then 	
		+'<RetailPrice>'+ISNULL(CAST(RetailPrice  AS VARCHAr(500)),'')+'</RetailPrice>'
		+'<SalesPrice>'+ISNULL(CAST(SalesPrice AS VARCHAr(500)), '') +'</SalesPrice>'
		+'<CurrencyCode>'+ISNULL(CurrencyCode,'') +'</CurrencyCode>'
		+'<CultureCode>'+ISNULL(CultureCode,'') +'</CultureCode>'
		+'<CurrencySuffix>'+ISNULL(CurrencySuffix,'') +'</CurrencySuffix>'
		+'<SeoUrl>'+ISNULL(SEOUrl,'') +'</SeoUrl>'
		+'<SeoDescription>'+ISNULL((SELECT ''+SEODescription FOR XML PATH('') ),'') +'</SeoDescription>'
		+'<SeoKeywords>'+ISNULL((SELECT ''+SEOKeywords FOR XML PATH('')),'') +'</SeoKeywords>'
		+'<SeoTitle>'+ISNULL((SELECT ''+SEOTitle FOR XML PATH('')),'') +'</SeoTitle>'
		+'<ImageSmallPath>'+ISNULL(ImageSmallPath,'') +'</ImageSmallPath>'
		else '' End

		+'<TempProfileIds>'+ISNULL(SUBSTRING( (SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
						FROM ZnodeProfileCatalog ZPFC 
						INNER JOIN ZnodeProfileCatalogCategory ZPCCH  ON ( ZPCCH.ProfileCatalogId = ZPFC.ProfileCatalogId )
						WHERE ZPCCH.PimCatalogCategoryId = ZPCCF.PimCatalogCategoryId  FOR XML PATH('')),2,8000),'')+'</TempProfileIds><ProductIndex>'+CAST(ROW_NUMBER()Over(Partition BY zpp.PublishProductId Order BY ISNULL(ZPC.PublishCategoryId,'0') ) AS VARCHAr(100))+'</ProductIndex><IndexId>'+CAST( ISNULL(ZPCP.PublishCategoryProductId,'0') AS VARCHAr(100))+'</IndexId>'+
		'<DisplayOrder>'+CAST(ISNULL(ZPCCF.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder>'+
		STUFF(( SELECT '  '+ AttributeValue  FROM #Cte_GetData TY WHERE TY.PimProductId = ZPP.PimProductId   AND TY.VersionId = ZPP.VersionId
		FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</ProductEntity>'  xmlvalue,1,@LocaleId,@UserId , @GetDate , @UserId,@GetDate
		,ZPCP.PublishCategoryId
		
		FROM  #TBL_PublishCatalogId zpp
		INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPP.PublishCatalogId)
		INNER JOIN ZnodePublishProductDetail ZPPDFG ON (ZPPDFG.PublishProductId =  ZPP.PublishProductId)
		LEFT JOIN #ZnodePrice TBZP ON (TBZP.PublishProductId = ZPP.PublishProductId)
		LEFT JOIN #ProductSKU TBPS ON (TBPS.PublishProductId = ZPP.PublishProductId)
		LEFT JOIN #ProductImages TBPI ON (TBPI.PublishProductId = ZPP.PublishProductId)
		LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishProductId = ZPP.PublishProductId AND ZPCP.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT JOIN ZnodePublishCategory ZPC ON (ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND   ZPC.PublishCategoryId = ZPCP.PublishCategoryId )
		LEFT JOIN ZnodePimCatalogCategory ZPCCF ON (ZPCCF.PimCatalogId = ZPCV.PimCatalogId AND ZPCCF.PimCategoryId = ZPC.PimCategoryId  AND ZPCCF.PimProductId = ZPP.PimProductId AND  ZPCCF.PimCategoryHierarchyId =  ZPC.PimCategoryHierarchyId)
		LEFT JOIN ZnodePublishCategoryDetail ZPCD ON (ZPCD.PublishCategoryId = ISNULL(ZPCP.PublishCategoryId,0) AND ZPCD.LocaleId = @LocaleId )
		WHERE ZPPDFG.LocaleId = @LocaleId AND ZPP.LocaleId = @LocaleId
		ORDER BY zpp.PublishProductId ASC 

      
END 
 

DELETE FROM #TBL_CustomeFiled
DELETE FROM #PimDefaultValueLocale
 IF OBJECT_ID('tempdb..#PimProductAttributeXML') is not null
 BEGIN 
 DELETE FROM #PimProductAttributeXML
 END
 IF OBJECT_ID('tempdb..#Cte_GetData') is not null
 BEGIN 
 DROP TABLE #Cte_GetData
 END
   IF OBJECT_ID('tempdb..#Cte_BrandData') is not null
 BEGIN 
 DROP TABLE #Cte_BrandData
 END
  IF OBJECT_ID('tempdb..#TBL_AttributeVAlue') is not null
 BEGIN 
 DROP TABLE #TBL_AttributeVAlue
 END
 IF OBJECT_ID('tempdb..#View_LoadManageProductInternal') is not null
 BEGIN 
 DROP TABLE #View_LoadManageProductInternal
 END
 IF OBJECT_ID('tempdb..#TBL_CustomeFiled') is not null
 BEGIN 
 DROP TABLE #TBL_CustomeFiled
 END
SET @Counter = @counter + 1 
END 
END TRY 
BEGIN CATCH 

 SELECT ERROR_MESSAGE()
 UPDATE ZnodePublishCatalogLog 
	    SET IsCatalogPublished = 0 
		,IsCategoryPublished = 0,IsProductPublished= 0  
		WHERE EXISTS (SELECT TOP 1 1 from #TBL_PublishCatalogId TR WHERE TR.VersionId = ZnodePublishCatalogLog.PublishCatalogLogId)
END CATCH 
END