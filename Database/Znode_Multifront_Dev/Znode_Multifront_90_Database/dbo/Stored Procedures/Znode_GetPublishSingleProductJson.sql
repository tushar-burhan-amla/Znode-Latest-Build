CREATE PROCEDURE [dbo].[Znode_GetPublishSingleProductJson]
(
	 @PublishCatalogId INT = 0 
	,@VersionId       VARCHAR(50) = 0 
	,@PimProductId    TransferId Readonly 
	,@UserId		  INT = 0 
	,@TokenId nvarchar(max)= ''	
	,@LocaleIds TransferId READONLY
	,@PublishStateId INT = 0  
	,@RevisionType varchar(50)
	,@Status bit = 0 OutPut
	
)
AS


--Declare @PimProductId TransferId 
--insert into @PimProductId  select 230147
-- EXEC Znode_GetPublishSingleProductJson  @PublishCatalogId = 0 ,@VersionId= 0 ,@PimProductId =@PimProductId, @UserId=2 ,@RevisionType ='Production'


BEGIN 
BEGIN TRY 
 SET NOCOUNT ON 

EXEC Znode_InsertUpdatePimAttributeJson 1 
EXEC Znode_InsertUpdateCustomeFieldJson 1
EXEC Znode_InsertUpdateAttributeDefaultValueJson 1 

DECLARE @PimMediaAttributeId INT = dbo.Fn_GetProductImageAttributeId()
				
Select ZPLPD.PimParentProductId, ZPLPD.PimProductId, ZPLPD.PimAttributeId, ZPAVL.AttributeValue as SKU
into #LinkProduct
FROM ZnodePimLinkProductDetail ZPLPD 
INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId = ZPLPD.PimProductId)
INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
WHERE exists(select * from ZnodePimAttribute ZPA where ZPA.PimAttributeId = ZPAV.PimAttributeId and ZPA.AttributeCode = 'SKU')
and exists(select * from @PimProductId pp where ZPLPD.PimParentProductId = pp.Id)

select * into #PimProductId from @PimProductId

create index Idx_#PimProductId_Id on #PimProductId(Id)
 IF OBJECT_ID('tempdb..#Cte_BrandData') is not null
 BEGIN 
	DROP TABLE #Cte_BrandData
 END 
 

 IF OBJECT_ID('tempdb..#ProductIds') is not null
 BEGIN 
	DROP TABLE #ProductIds
 END 

			Create Table #ProductIds (PimProductId int, PublishProductId  int )
			
			--DECLARE @PimProductAttributeJson TABLE(PimAttributeJsonId INT  PRIMARY KEY ,PimAttributeId INT,LocaleId INT  )
			CREATE TABLE #PimProductAttributeJson (PimAttributeJsonId INT  PRIMARY KEY ,PimAttributeId INT,LocaleId INT  )
			DECLARE @PimDefaultValueLocale  TABLE (PimAttributeDefaultJsonId INT  PRIMARY KEY ,PimAttributeDefaultValueId INT ,LocaleId INT ) 
			DECLARE @ProductNamePimAttributeId INT = dbo.Fn_GetProductNameAttributeId(),@DefaultLocaleId INT= Dbo.Fn_GetDefaultLocaleId(),@LocaleId INT = 0 
			,@SkuPimAttributeId  INT =  dbo.Fn_GetProductSKUAttributeId() , @IsActivePimAttributeId INT =  dbo.Fn_GetProductIsActiveAttributeId()
			DECLARE @GetDate DATETIME =dbo.Fn_GetDate()
			DECLARE @TBL_LocaleId  TABLE (RowId INT IDENTITY(1,1) PRIMARY KEY  , LocaleId INT )

			DECLARE @DomainUrl varchar(max) = (select TOp 1 URL FROM ZnodeMediaConfiguration WHERE IsActive =1)

			INSERT INTO @TBL_LocaleId (LocaleId)
			SELECT  LocaleId
			FROM ZnodeLocale MT
			WHERE IsActive = 1
			AND (EXISTS (SELECT TOP 1 1  FROM @LocaleIds RT WHERE RT.Id = MT.LocaleId )
			OR NOT EXISTS (SELECT TOP 1 1 FROM @LocaleIds )) 
	
			-----to update link products newly addded and deleted from PIM
			delete ZPAP
			from ZnodePublishAssociatedProduct ZPAP
			where ZPAP.IsLink = 1
			AND not exists(select * from ZnodePimLinkProductDetail ZPPD where ZPAP.ParentPimProductId = ZPPD.PimParentProductId AND ZPAP.PimProductId = ZPPD.PimProductId)
			and exists(select * from #PimProductId PP where PP.Id = ZPAP.ParentPimProductId )

			UPDATE ZPACP SET ZPACP.DisplayOrder = ZPLPD.DisplayOrder
			from ZnodePimLinkProductDetail ZPLPD
			INNER JOIN ZnodePimCategoryProduct ZPCP ON ZPLPD.PimParentProductId = ZPCP.PimProductId
			INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId
			INNER JOIN ZnodePublishAssociatedProduct ZPACP ON ZPCH.PimCatalogId = ZPACP.PimCatalogId and ZPLPD.PimParentProductId = ZPACP.ParentPimProductId AND ZPLPD.PimProductId = ZPACP.PimProductId 
			where exists(select * from #PimProductId PP where PP.Id = ZPLPD.PimParentProductId )
			AND ZPACP.IsLink = 1

			insert into ZnodePublishAssociatedProduct(PimCatalogId,ParentPimProductId,PimProductId,PublishStateId,IsConfigurable,IsBundle,IsGroup,IsAddOn,IsLink,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			select distinct ZPCH.PimCatalogId, ZPLPD.PimParentProductId, ZPLPD.PimProductId, @PublishStateId, 0, 0, 0, 0, 1, ZPLPD.DisplayOrder, @UserId,@GetDate ,@UserId , @GetDate
			from ZnodePimLinkProductDetail ZPLPD
			INNER JOIN ZnodePimCategoryProduct ZPCP ON ZPLPD.PimParentProductId = ZPCP.PimProductId
			INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId
			where exists(select * from #PimProductId PP where PP.Id = ZPLPD.PimParentProductId )
			and not exists(select * from ZnodePublishAssociatedProduct ZPACP where ZPCH.PimCatalogId = ZPACP.PimCatalogId and ZPLPD.PimParentProductId = ZPACP.ParentPimProductId AND ZPLPD.PimProductId = ZPACP.PimProductId  )
		
			-----to update config products newly addded and deleted from PIM
			delete ZPAP
			from ZnodePublishAssociatedProduct ZPAP
			where ZPAP.IsConfigurable = 1
			AND exists(select * from ZnodePimProductTypeAssociation ZPPD where ZPAP.ParentPimProductId = ZPPD.PimParentProductId )
			and exists(select * from #PimProductId PP where PP.Id = ZPAP.ParentPimProductId )

			Update ZPACP SET ZPACP.DisplayOrder = ZPLPD.DisplayOrder
			from ZnodePimProductTypeAssociation ZPLPD
			INNER JOIN ZnodePimCategoryProduct ZPCP ON ZPLPD.PimParentProductId = ZPCP.PimProductId
			INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId
			INNER JOIN ZnodePublishAssociatedProduct ZPACP ON ZPCH.PimCatalogId = ZPACP.PimCatalogId and ZPLPD.PimParentProductId = ZPACP.ParentPimProductId AND ZPLPD.PimProductId = ZPACP.PimProductId
			where exists(select * from #PimProductId PP where PP.Id = ZPLPD.PimParentProductId )
			AND ZPACP.IsConfigurable = 1

			insert into ZnodePublishAssociatedProduct(PimCatalogId,ParentPimProductId,PimProductId,PublishStateId,IsConfigurable,IsBundle,IsGroup,IsAddOn,IsLink,DisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, IsDefault)
			select distinct ZPCH.PimCatalogId, ZPLPD.PimParentProductId, ZPLPD.PimProductId, @PublishStateId, 1, 0, 0, 0, 0, ZPLPD.DisplayOrder, @UserId,@GetDate ,@UserId , @GetDate, ZPLPD.IsDefault
			from ZnodePimProductTypeAssociation ZPLPD
			INNER JOIN ZnodePimCategoryProduct ZPCP ON ZPLPD.PimParentProductId = ZPCP.PimProductId
			INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId
			where exists(select * from #PimProductId PP where PP.Id = ZPLPD.PimParentProductId )
			and not exists(select * from ZnodePublishAssociatedProduct ZPACP where ZPCH.PimCatalogId = ZPACP.PimCatalogId and ZPLPD.PimParentProductId = ZPACP.ParentPimProductId AND ZPLPD.PimProductId = ZPACP.PimProductId  )
			--group by ZPCH.PimCatalogId, ZPLPD.PimParentProductId, ZPLPD.PimProductId, ZPLPD.DisplayOrder, ZPLPD.IsDefault
			-------

			DECLARE @Counter INT =1 ,@maxCountId INT = (SELECT max(RowId) FROM @TBL_LocaleId ) 

			CREATE TABLE #TBL_PublishCatalogId (PublishCatalogId INT,PublishProductId INT,PimProductId  INT   , VersionId INT ,LocaleId INT, PriceListId INT , PortalId INT ,MaxSmallWidth NVARCHAr(max)  )
			CREATE INDEX idx_#TBL_PublishCatalogIdPimProductId on #TBL_PublishCatalogId(PimProductId)
			CREATE INDEX idx_#TBL_PublishCatalogIdPimPublishCatalogId on #TBL_PublishCatalogId(PublishCatalogId)

			INSERT INTO #TBL_PublishCatalogId 
			SELECT Distinct ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId, 0,0 ,
			(SELECT TOP 1 PriceListId FROM ZnodePriceListPortal NT 
			INNER JOIN ZnodePimCatalog ZPC on ZPC.PortalId=NT.PortalId  
			ORDER BY NT.Precedence ASC ) ,TY.PortalId,
			(SELECT TOP 1  MAX(MaxSmallWidth) FROM ZnodeGlobalMediaDisplaySetting)
			FROM ZnodePublishProduct ZPP 
			LEFT JOIN ZnodePortalCatalog TY ON (TY.PublishCatalogId = ZPP.PublishCatalogId)
			WHERE (EXISTS (SELECT TOP 1 1 FROM #PimProductId SP WHERE SP.Id = ZPP.PimProductId  
			AND  (@PublishCatalogId IS NULL OR @PublishCatalogId = 0 ))
			OR  (ZPP.PublishCatalogId = @PublishCatalogId ))
			And Exists 
			(Select TOP 1 1 from ZnodePublishVersionEntity ZPCP  where ZPCP.ZnodeCatalogId  = ZPP.PublishCatalogId AND ZPCP.IsPublishSuccess =1 )

			Insert into #ProductIds (PimProductId,PublishProductId) Select distinct PimProductId,PublishProductId from #TBL_PublishCatalogId  

             Create TABLE #TBL_ZnodeTempPublish (PimProductId INT , AttributeCode VARCHAR(300) ,AttributeValue NVARCHAR(max) ) 			
			 DECLARE @TBL_AttributeVAlueLocale TABLE(PimProductId INT,PimAttributeId INT,ZnodePimAttributeValueLocaleId INT,LocaleId INT ,AttributeValue Nvarchar(1000) )


			 INSERT INTO @TBL_AttributeValueLocale (PimProductId ,PimAttributeId ,ZnodePimAttributeValueLocaleId ,LocaleId ,AttributeValue )
			 SELECT VIR.PimProductId,PimAttributeId,ZnodePimAttributeValueLocaleId,VIR.LocaleId, ''
			 FROM View_LoadManageProductInternal VIR
			 INNER JOIN #ProductIds ZPP ON (ZPP.PimProductId = VIR.PimProductId)
			 UNION ALL 
			 SELECT VIR.PimProductId,PimAttributeId,PimProductAttributeMediaId,ZPDE.LocaleId , ''
			 FROM ZnodePimAttributeValue  VIR
			 INNER JOIN ZnodePimProductAttributeMedia ZPDE ON (ZPDE.PimAttributeValueId = VIR.PimAttributeValueId )
			 WHERE EXISTS (SELECT TOP 1 1 FROM #ProductIds ZPP WHERE (ZPP.PimProductId = VIR.PimProductId) )
			 Union All 
			 SELECT VIR.PimProductId,VIR.PimAttributeId,ZPDVL.PimAttributeDefaultValueLocaleId,ZPDVL.LocaleId ,ZPDVL.AttributeDefaultValue
			   FROM ZnodePimAttributeValue  VIR
			 INNER JOIN ZnodePimAttribute D ON ( D.PimAttributeId=VIR.PimAttributeId AND D.IsPersonalizable =1 )
			 INNER JOIN ZnodePimAttributeDefaultValue ZPADV ON ZPADV.PimAttributeId = D.PimAttributeId
			 INNER JOIN ZnodePimAttributeDefaultValueLocale ZPDVL   on (ZPADV.PimAttributeDefaultValueId = ZPDVL.PimAttributeDefaultValueId)
			 WHERE ( ZPDVL.LocaleId = @DefaultLocaleId OR ZPDVL.LocaleId = @LocaleId )
			 AND EXISTS(SELECT TOP 1 1 FROM #ProductIds ZPP WHERE (ZPP.PimProductId = VIR.PimProductId) )
			 Union All 
			 SELECT VIR.PimProductId,VIR.PimAttributeId,'','' ,''
			 FROM ZnodePimAttributeValue  VIR
			 INNER JOIN ZnodePimAttribute D ON ( D.PimAttributeId=VIR.PimAttributeId AND D.IsPersonalizable =1 )
			 WHERE  EXISTS(SELECT TOP 1 1 FROM #ProductIds ZPP WHERE (ZPP.PimProductId = VIR.PimProductId) )
		
				--insert INTO #ZnodePrice
				SELECT RetailPrice,SalesPrice,ZC.CurrencyCode,ZCC.CultureCode ,ZCC.Symbol CurrencySuffix,TYU.PublishProductId ,isnull(ZPC1.IsAllowIndexing,0) as IsAllowIndexing
				into #ZnodePrice
				FROM ZnodePrice ZP 
				INNER JOIN ZnodePriceList ZPL ON (ZPL.PriceListId = ZP.PriceListId)
				INNER JOIN ZnodeCurrency ZC oN (ZC.CurrencyId = ZPL.CurrencyId )
				INNER JOIN ZnodeCulture ZCC ON (ZCC.CultureId = ZPL.CultureId)
				INNER JOIN ZnodePublishProductDetail TY ON (TY.SKU = ZP.SKU ) 
				INNER JOIN ZnodePublishProduct TYU ON (TYU.PublishProductId = TY.PublishProductId)
				INNER JOIN ZnodePublishCatalog ZPC ON (TYU.PublishCatalogId = ZPC.PublishCatalogId)
				INNER JOIN ZnodePimCatalog ZPC1 ON (ZPC.PimCatalogId = ZPC1.PimCatalogId)
				WHERE EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId TYUR WHERE TYUR.PriceListId = ZPL.PriceListId AND TYUR.PublishCatalogId = TYU.PublishCatalogId
				AND TYU.PublishProductId = TYUR.PublishProductId)
				AND TY.LocaleId = dbo.Fn_GetDefaultLocaleId()
				AND EXISTS (SELECT TOP 1 1 FROM ZnodePriceListPortal ZPLP 
				INNER JOIN ZnodePimCatalog ZPC on ZPC.PortalId=ZPLP.PortalId WHERE ZPLP.PriceListId=ZP.PriceListId )
				
				--insert INTO #ProductSKU
				SELECT ZCSD.SEOUrl , ZCDL.SEODescription,ZCDL.SEOKeywords ,ZCDL.SEOTitle, TYU.PublishProductId ,isnull(ZPC1.IsAllowIndexing,0) as IsAllowIndexing
				INTO #ProductSKU
				FROM ZnodeCMSSEODetail ZCSD 
				INNER JOIN ZnodeCMSSEODetailLocale ZCDL ON (ZCDL.CMSSEODetailId = ZCSD.CMSSEODetailId)
				INNER JOIN ZnodePublishProductDetail TY ON (TY.SKU = ZCSD.SEOCode AND ZCDL.LocaleId = TY.LocaleId) 
				INNER JOIN ZnodePublishProduct TYU ON (TYU.PublishProductId = TY.PublishProductId)
				INNER JOIN ZnodePublishCatalog ZPC ON (TYU.PublishCatalogId = ZPC.PublishCatalogId)
				INNER JOIN ZnodePimCatalog ZPC1 ON (ZPC.PimCatalogId = ZPC1.PimCatalogId)
				WHERE CMSSEOTypeId = (SELECT TOP 1 CMSSEOTypeId FROM ZnodeCMSSEOType WHERE Name = 'Product') 
				AND EXISTS (SELECT TOP 1 1  FROM #TBL_PublishCatalogId TYUR WHERE  TYUR.PublishCatalogId = TYU.PublishCatalogId
				AND TYU.PublishProductId = TYUR.PublishProductId)
				AND ZCDL.LocaleId = dbo.Fn_GetDefaultLocaleId()
				and ZCSD.PortalId = isnull(ZPC1.PortalId,0)

			
				--insert INTO #ProductImages
				SELECT  TUI.PublishCatalogId, TYU.PublishProductId , @DomainUrl +'Catalog/'  + CAST(Max(ZPC1.PortalId) AS VARCHAr(100)) + '/'+ CAST(Isnull(Max(TUI.MaxSmallWidth),'') AS VARCHAR(100)) + '/' + Isnull(RT.MediaPath,'') AS ImageSmallPath    
				,isnull(ZPC1.IsAllowIndexing,0) as IsAllowIndexing
				INTO #ProductImages
				FROM ZnodePimAttributeValue ZPAV 
				INNER JOIN ZnodePublishProduct TYU ON (TYU.PimProductId  = ZPAV.PimProductId)
				INNER JOIN ZnodePimProductAttributeMedia  RT ON ( RT.PimAttributeValueId = ZPAV.PimAttributeValueId )
				INNER JOIN #TBL_PublishCatalogId TUI ON (TUI.PublishProductId = TYU.PublishProductId AND TUI.PublishCatalogId = TYU.PublishCatalogId
						 )--AND  TUI.LocaleId = dbo.Fn_GetDefaultLocaleId()
				INNER JOIN ZnodePublishCatalog ZPC ON (TYU.PublishCatalogId = ZPC.PublishCatalogId)
				INNER JOIN ZnodePimCatalog ZPC1 ON (ZPC.PimCatalogId = ZPC1.PimCatalogId)
				WHERE  RT.LocaleId = dbo.Fn_GetDefaultLocaleId()
				AND ZPAV.PimAttributeId = (SELECT TOp 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'ProductImage')
				group by TUI.PublishCatalogId, TYU.PublishProductId ,isnull(RT.MediaPath,''),isnull(ZPC1.IsAllowIndexing,0) 
		  -- end
	  
WHILE @Counter <= @maxCountId
BEGIN
 SET @LocaleId = (SELECT TOP 1 LocaleId FROM @TBL_LocaleId WHERE RowId = @Counter)

  INSERT INTO #PimProductAttributeJson 
  SELECT PimAttributeJsonId ,PimAttributeId,LocaleId
  FROM ZnodePimAttributeJSON
  WHERE LocaleId = @LocaleId
  
  INSERT INTO #PimProductAttributeJson 
  SELECT PimAttributeJsonId ,PimAttributeId,LocaleId
  FROM ZnodePimAttributeJSON ZPAX
  WHERE ZPAX.LocaleId = @DefaultLocaleId  
  AND NOT EXISTS (SELECT TOP 1 1 FROM #PimProductAttributeJson ZPAXI WHERE ZPAXI.PimAttributeId = ZPAX.PimAttributeId )

  INSERT INTO @PimDefaultValueLocale
  SELECT PimAttributeDefaultJsonId,PimAttributeDefaultValueId,LocaleId 
  FROM ZnodePimAttributeDefaultJson
  WHERE localeId = @LocaleId

  INSERT INTO @PimDefaultValueLocale 
   SELECT PimAttributeDefaultJsonId,PimAttributeDefaultValueId,LocaleId 
  FROM ZnodePimAttributeDefaultJson ZX
  WHERE localeId = @DefaultLocaleId
  AND NOT EXISTS (SELECT TOP 1 1 FROM @PimDefaultValueLocale TRTR WHERE TRTR.PimAttributeDefaultValueId = ZX.PimAttributeDefaultValueId)
  
 
  --DECLARE @TBL_AttributeVAlue TABLE(PimProductId INT,PimAttributeId INT,ZnodePimAttributeValueLocaleId INT  )
  --DECLARE @TBL_CustomeFiled TABLE (PimCustomeFieldJsonId INT ,CustomCode VARCHAR(300),PimProductId INT ,LocaleId INT )
  CREATE TABLE #TBL_CustomeFiled  (PimCustomeFieldJsonId INT ,CustomCode VARCHAR(300),PimProductId INT ,LocaleId INT )
  CREATE TABLE #TBL_AttributeVAlue (PimProductId INT,PimAttributeId INT,ZnodePimAttributeValueLocaleId INT  )



  INSERT INTO #TBL_CustomeFiled (PimCustomeFieldJsonId,PimProductId ,LocaleId,CustomCode)
  SELECT  PimCustomeFieldJsonId,RTR.PimProductId ,RTR.LocaleId,CustomCode
  FROM ZnodePimCustomeFieldJson RTR 
  INNER JOIN #ProductIds ZPP ON (ZPP.PimProductId = RTR.PimProductId)
  WHERE RTR.LocaleId = @LocaleId
 

  INSERT INTO #TBL_CustomeFiled (PimCustomeFieldJsonId,PimProductId ,LocaleId,CustomCode)
  SELECT  Distinct  PimCustomeFieldJsonId,ITR.PimProductId ,ITR.LocaleId,CustomCode
  FROM ZnodePimCustomeFieldJson ITR
  INNER JOIN #ProductIds ZPP ON (ZPP.PimProductId = ITR.PimProductId)
  WHERE ITR.LocaleId = @DefaultLocaleId
  AND NOT EXISTS (SELECT TOP 1 1 FROM #TBL_CustomeFiled TBL  WHERE ITR.CustomCode = TBL.CustomCode AND ITR.PimProductId = TBL.PimProductId)
  

    INSERT INTO #TBL_AttributeVAlue (PimProductId ,PimAttributeId ,ZnodePimAttributeValueLocaleId )
    SELECT Distinct  PimProductId,PimAttributeId,ZnodePimAttributeValueLocaleId
	FROM @TBL_AttributeVAlueLocale
    WHERE LocaleId = @LocaleId

    
	INSERT INTO #TBL_AttributeVAlue(PimProductId ,PimAttributeId ,ZnodePimAttributeValueLocaleId )
	SELECT VI.PimProductId,PimAttributeId,ZnodePimAttributeValueLocaleId
	FROM @TBL_AttributeVAlueLocale VI 
    WHERE VI.LocaleId = @DefaultLocaleId 
	AND NOT EXISTS (SELECT TOP 1 1 FROM #TBL_AttributeVAlue  CTE WHERE CTE.PimProductId = VI.PimProductId AND CTE.PimAttributeId = VI.PimAttributeId )
 
	------------Facet Merging Patch --------------
	IF OBJECT_ID('tempdb..#PimChildProductFacets') is not null
	BEGIN 
		DROP TABLE #PimChildProductFacets
	END 

	IF OBJECT_ID('tempdb..#PimAttributeDefaultXML') is not null
	BEGIN 
		DROP TABLE #PimAttributeDefaultXML
	END
	----Getting parent facets data
	Select  ZPPADV.PimAttributeDefaultValueId, ZPAV_Parent.PimAttributeValueId, ZPPADV.LocaleId
	Into #PimChildProductFacets
	from ZnodePimAttributeValue ZPAV_Parent
	inner join ZnodePimProductAttributeDefaultValue ZPPADV ON ZPAV_Parent.PimAttributeValueId = ZPPADV.PimAttributeValueId 
	where exists(select * from #ProductIds ZPPC where ZPAV_Parent.PimProductId = ZPPC.PimProductId )

	----Getting child facets for merging	
	insert into #PimChildProductFacets	  
	Select distinct ZPPADV.PimAttributeDefaultValueId, ZPAV_Parent.PimAttributeValueId, ZPPADV.LocaleId
	from ZnodePimAttributeValue ZPAV_Parent
	inner join ZnodePimProductTypeAssociation ZPPTA ON ZPAV_Parent.PimProductId = ZPPTA.PimParentProductId
	inner join ZnodePimAttributeValue ZPAV_Child ON ZPPTA.PimProductId = ZPAV_Child.PimProductId AND ZPAV_Parent.PimAttributeId = ZPAV_Child.PimAttributeId
	inner join ZnodePimProductAttributeDefaultValue ZPPADV ON ZPAV_Child.PimAttributeValueId = ZPPADV.PimAttributeValueId 
	where exists(select * from ZnodePimFrontendProperties ZPFP where ZPAV_Parent.PimAttributeId = ZPFP.PimAttributeId and ZPFP.IsFacets = 1)
	and exists(select * from #ProductIds ZPPC where ZPAV_Parent.PimProductId = ZPPC.PimProductId )
	and not exists(select * from ZnodePimProductAttributeDefaultValue ZPPADV1 where ZPAV_Parent.PimAttributeValueId = ZPPADV1.PimAttributeValueId 
		            and ZPPADV1.PimAttributeDefaultValueId = ZPPADV.PimAttributeDefaultValueId )

	----Merging childs facet attribute Default value XML for parent
	Create table #PimAttributeDefaultXML (DefaultValueJson nvarchar(max), PimAttributeValueId int, LocaleId int )

    Insert into #PimAttributeDefaultXML (DefaultValueJson , PimAttributeValueId , LocaleId )
	select  ZPADX.DefaultValueJson, ZPPADV.PimAttributeValueId, ZPPADV.LocaleId
	from #PimChildProductFacets ZPPADV		  
	inner join ZnodePimAttributeDefaultJson ZPADX ON ( ZPPADV.PimAttributeDefaultValueId = ZPADX.PimAttributeDefaultValueId )--AND ZPPADV.LocaleId = ZPADX.LocaleId)
	INNER JOIN @PimDefaultValueLocale GH ON (GH.PimAttributeDefaultJsonId = ZPADX.PimAttributeDefaultJsonId)
	
	IF ( @LocaleId <> @DefaultLocaleId )
	BEGIN
		Insert into #PimAttributeDefaultXML (DefaultValueJson , PimAttributeValueId , LocaleId )
		Select  ZPADX.DefaultValueJson, ZPPADV.PimAttributeValueId, @LocaleId
		from #PimChildProductFacets ZPPADV		  
		inner join ZnodePimAttributeDefaultJson ZPADX ON ( ZPPADV.PimAttributeDefaultValueId = ZPADX.PimAttributeDefaultValueId )--AND ZPPADV.LocaleId = ZPADX.LocaleId)
		INNER JOIN @PimDefaultValueLocale GH ON (GH.PimAttributeDefaultJsonId = ZPADX.PimAttributeDefaultJsonId)
		Where Not exists (select TOP 1 1 From #PimAttributeDefaultXML X Where X.LocaleId = @LocaleId and 
		 ZPPADV.PimAttributeValueId = X.PimAttributeValueId) and GH.LocaleId = @DefaultLocaleId
	END
	------------Facet Merging Patch --------------   

	 IF OBJECT_ID('tempdb..#View_LoadManageProductInternal') is not null
	 BEGIN 
		DROP TABLE #View_LoadManageProductInternal
	 END 

	SELECT a.PimProductId ,b.AttributeValue as AttributeValue , b.LocaleId  ,a.PimAttributeId,c.AttributeCode ,b.ZnodePimAttributeValueLocaleId
	into #View_LoadManageProductInternal
	FROM ZnodePimAttributeValue a 
	INNER JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId )
	INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
	INNER JOIN ZnodePimAttributeJSON c1   ON (c1.PimAttributeId = a.PimAttributeId )
	INNER JOIN #PimProductAttributeJson b1 ON (b1.PimAttributeJsonId = c1.PimAttributeJsonId )
	INNER JOIN #TBL_AttributeVAlue CTE ON (Cte.PimAttributeId = a.PimAttributeId AND Cte.ZnodePimAttributeValueLocaleId = b.ZnodePimAttributeValueLocaleId)
	UNION ALL
	SELECT a.PimProductId,ZPPATAV.AttributeValue AS AttributeValue  
	,ZPPATAV.LocaleId,a.PimAttributeId,c.AttributeCode  ,ZPPATAV.PimProductAttributeTextAreaValueId
	FROM ZnodePimAttributeValue a 
	INNER JOIN ZnodePimProductAttributeTextAreaValue ZPPATAV ON (ZPPATAV.PimAttributeValueId = a.PimAttributeValueId )
	INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
	INNER JOIN ZnodePimAttributeJSON c1   ON (c1.PimAttributeId = a.PimAttributeId )
	INNER JOIN #PimProductAttributeJson b1 ON (b1.PimAttributeJsonId = c1.PimAttributeJsonId )
	INNER JOIN #TBL_AttributeVAlue CTE ON (Cte.PimAttributeId = a.PimAttributeId AND Cte.ZnodePimAttributeValueLocaleId = ZPPATAV.PimProductAttributeTextAreaValueId)
	
	INSERT INTO #TBL_ZnodeTempPublish  
		SELECT  a.PimProductId,a.AttributeCode , 
			JSON_MODIFY (JSON_MODIFY (Json_Query( c.AttributeJSON  ) , '$.AttributeValues' ,  
			ISNULL(a.AttributeValue,'') ) ,'$.SelectValues',Json_Query('[]'))
			AS 'AttributeValue'
		FROM #View_LoadManageProductInternal a 
		INNER JOIN ZnodePimAttributeJSON c   ON (c.PimAttributeId = a.PimAttributeId )
		INNER JOIN #PimProductAttributeJson b ON (b.PimAttributeJsonId = c.PimAttributeJsonId )
		INNER JOIN #TBL_AttributeVAlue CTE ON (Cte.PimAttributeId = a.PimAttributeId AND Cte.ZnodePimAttributeValueLocaleId = a.ZnodePimAttributeValueLocaleId)
	UNION ALL 
			SELECT  a.PimProductId,c.AttributeCode , 
			JSON_MODIFY (JSON_MODIFY (Json_Query( c.AttributeJSON  ) , '$.AttributeValues' ,  
			ISNULL(TAVL.AttributeValue,'') ) ,'$.SelectValues',Json_Query('[]'))
			AS 'AttributeValue'
		FROM ZnodePimAttributeValue  a 
		INNER JOIN ZnodePimAttributeJSON c   ON (c.PimAttributeId = a.PimAttributeId )
		INNER JOIN #PimProductAttributeJson b ON (b.PimAttributeJsonId = c.PimAttributeJsonId )
		INNER JOIN ZnodePImAttribute ZPA  ON (ZPA.PimAttributeId = a.PimAttributeId)
		INNER JOIN #ProductIds ZPP ON (ZPP.PimProductId = a.PimProductId)
		Inner JOIN @TBL_AttributeVAlueLocale TAVL ON  (c.PimAttributeId = TAVL.PimAttributeId  and ZPP.PimProductId = TAVL.PimProductId )
		WHERE ZPA.IsPersonalizable = 1 
		AND NOT EXISTS ( SELECT TOP 1 1 FROM ZnodePimAttributeValueLocale q WHERE q.PimAttributeValueId = a.PimAttributeValueId) 
	UNION ALL 
		SELECT THB.PimProductId,THB.CustomCode,
		--'<Attributes><AttributeEntity>'+CustomeFiledJson +'</AttributeEntity></Attributes>' 
		JSON_MODIFY (Json_Query( CustomeFiledJson ) ,'$.SelectValues',Json_Query('[]')) 
		FROM ZnodePimCustomeFieldJson THB 
		INNER JOIN #TBL_CustomeFiled TRTE ON (TRTE.PimCustomeFieldJsonId = THB.PimCustomeFieldJsonId)
		UNION ALL 
		SELECT ZPAV.PimProductId,c.AttributeCode,
			JSON_MODIFY (JSON_MODIFY (c.AttributeJson,'$.AttributeValues',''), '$.SelectValues',
			Isnull((SELECT 
			Isnull(JSON_VALUE(DefaultValueJson, '$.Code'),'') Code 
			,Isnull(JSON_VALUE(DefaultValueJson, '$.LocaleId'),0) LocaleId
			,IsNull(JSON_VALUE(DefaultValueJson, '$.Value'),'') Value
			,IsNull(JSON_VALUE(DefaultValueJson, '$.AttributeDefaultValue'),'') AttributeDefaultValue
			,Isnull(JSON_VALUE(DefaultValueJson, '$.DisplayOrder'),0) DisplayOrder
			,Isnull(JSON_VALUE(DefaultValueJson, '$.IsEditable'),'false') IsEditable
			,Isnull(JSON_VALUE(DefaultValueJson, '$.SwatchText'),'') SwatchText
			,Isnull(JSON_VALUE(DefaultValueJson, '$.Path'),'') Path
			FROM #PimAttributeDefaultXML ZPADV
			WHERE (ZPADV.PimAttributeValueId = ZPAV.PimAttributeValueId) For JSON Auto 
			),'[]') 
		)  AttributeValue
		FROM ZnodePimAttributeValue ZPAV  With (NoLock)
		INNER JOIN ZnodePimAttributeJSON c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
		INNER JOIN #PimProductAttributeJson b ON (b.PimAttributeJsonId = c.PimAttributeJsonId )
		INNER JOIN #ProductIds ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
		WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue ZPADVL 
		WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
	UNION ALL 
		SELECT DISTINCT  ZPAV.PimProductId,c.AttributeCode,
			JSON_MODIFY (JSON_MODIFY (Json_Query( c.AttributeJson  ) , '$.AttributeValues',  
			ISNULL((Select stuff( 
			(SELECT ','+ZPPG.MediaPath 
			FROM ZnodePimProductAttributeMedia ZPPG INNER JOIN  #TBL_AttributeVAlue TBLV ON 
			(	TBLV.PimProductId=  ZPAV.PimProductId AND TBLV.PimAttributeId = ZPAV.PimAttributeId )
			WHERE ZPPG.PimProductAttributeMediaId = TBLV.ZnodePimAttributeValueLocaleId
			FOR XML PATH(''),Type).value('.', 'varchar(max)'), 1, 1, '')),'') ) ,'$.SelectValues',Json_Query('[]'))   
			AS 'AttributeEntity'
		FROM ZnodePimAttributeValue ZPAV 
		INNER JOIN ZnodePimAttributeJSON c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
		INNER JOIN #PimProductAttributeJson b ON (b.PimAttributeJsonId = c.PimAttributeJsonId )
		INNER JOIN #ProductIds ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
		WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeMedia ZPADVL WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
	UNION ALL 
		SELECT ZPLP.PimParentProductId ,c.AttributeCode, 
			JSON_MODIFY( JSON_Modify(c.AttributeJson , '$.AttributeValues' , 
			ISNULL(SUBSTRING((SELECT ','+cast( LP.SKU as varchar(600)) 
							 FROM #LinkProduct LP
							 WHERE LP.PimParentProductId = ZPLP.PimParentProductId 
							 AND LP.PimAttributeId = ZPLP.PimAttributeId
		FOR XML PATH ('') ),2,4000),'')),'$.SelectValues',Json_Query('[]'))   
	
		FROM ZnodePimLinkProductDetail ZPLP 
		INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPLP.PimParentProductId)
		INNER JOIN ZnodePimAttributeJSON c   ON (c.PimAttributeId = ZPLP.PimAttributeId )
		INNER JOIN #PimProductAttributeJson b ON (b.PimAttributeJsonId = c.PimAttributeJsonId )
		GROUP BY ZPLP.PimParentProductId , ZPP.PublishProductId  ,ZPLP.PimAttributeId,c.AttributeCode,c.AttributeJson,ZPP.PublishCatalogId
	UNION ALL 
		SELECT ZPAV.PimProductId,'DefaultSkuForConfigurable' ,
			JSON_MODIFY( JSON_Modify(
			REPLACE(REPLACE (c.AttributeJson,'ProductType','DefaultSkuForConfigurable'),'Product Type','Default Sku For Configurable'),
			'$.AttributeValues' , 
			ISNULL(SUBSTRING((SELECT ','+CAST(adl.AttributeValue AS VARCHAR(50)) 
		FROM ZnodePimAttributeValue ad 
		inner join ZnodePimAttributeValueLocale adl on ad.PimattributeValueId = adl.PimAttributeValueId
		INNER JOIN ZnodePimProductTypeAssociation yt ON (yt.PimProductId = ad.PimProductId)
		WHERE EXISTS (select * from #ProductIds p where yt.PimParentProductId = p.PimProductId)
		AND Ad.PimAttributeId =(select top 1 PimAttributeId from ZnodePimAttribute zpa where zpa.AttributeCode = 'SKU')
		AND yt.PimParentProductId = ZPAV.PimProductId 
		ORDER BY yt.DisplayOrder , yt.PimProductTypeAssociationId ASC FOR XML PATH ('') ),2,4000),'')),'$.SelectValues',Json_Query('[]'))   
		FROM ZnodePimAttributeValue ZPAV  
		INNER JOIN ZnodePimAttributeJSON c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
		INNER JOIN #ProductIds ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
		WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue ZPADVL 
		INNER JOIN ZnodePimAttributeDefaultValue dr ON (dr.PimAttributeDefaultValueId = ZPADVL.PimAttributeDefaultValueId)
		WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId
		AND dr.AttributeDefaultValueCode= 'ConfigurableProduct' 
		)
		AND EXISTS (select * from #PimProductAttributeJson b where b.PimAttributeJsonId = c.PimAttributeJsonId)
		AND c.AttributeCode = 'ProductType' 
	UNION ALL
		SELECT DISTINCT  UOP.PimProductId,c.AttributeCode,
			JSON_MODIFY (JSON_MODIFY (c.AttributeJson,'$.AttributeValues',''), '$.SelectValues',
			Isnull((SELECT  DISTINCT 
			Isnull(JSON_VALUE(AA.DefaultValueJson, '$.Code'),'') Code 
			,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.LocaleId'),0) LocaleId
			,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.Value'),'') Value
			,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.AttributeDefaultValue'),'') AttributeDefaultValue
			,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.DisplayOrder'),0) DisplayOrder
			,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.IsEditable'),'false') IsEditable
			,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.SwatchText'),'') SwatchText
			,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.Path'),'') Path 
			,ISNULL(ZPA.DisplayOrder,0)  AS VariantDisplayOrder 
			,ISNULL(ZPAVL_SKU.AttributeValue,'')   AS VariantSKU 
			--,Isnull(ZM.Path,'') 
		,ZM.Path AS VariantImagePath 
		FROM ZnodePimAttributeDefaultJson AA 
		INNER JOIN ZnodePimProductAttributeDefaultValue ZPADV ON ( ZPADV.PimAttributeDefaultValueId = AA.PimAttributeDefaultValueId )
		INNER JOIN ZnodePimAttributeValue ZPAV1 ON (ZPAV1.PimAttributeValueId= ZPADV.PimAttributeValueId )
		-- check/join for active variants 
		INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId =ZPAV1.PimProductId)
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (ZPAV.PimAttributevalueid = ZPAVL.PimAttributeValueId AND ZPAVL.AttributeValue = 'True')
		INNER JOIN ZnodePimProductTypeAssociation YUP ON (YUP.PimProductId = ZPAV1.PimProductId)
		-- SKU
		INNER JOIN ZnodePimAttributeValue ZPAV_SKU ON(YUP.PimProductId = ZPAV_SKU.PimProductId)
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL_SKU ON (ZPAVL_SKU.PimAttributeValueId = ZPAV_SKU.PimAttributeValueId)
		LEFT JOIN ZnodePimAttribute ZPA ON (ZPA.PimattributeId = ZPAV1.PimAttributeId)
		--VariantImagePath
		LEFT  JOIN ZnodePimAttributeValue ZPAV12 ON (ZPAV12.PimProductId= YUP.PimProductId  AND ZPAV12.PimAttributeId = @PimMediaAttributeId ) 
		LEFT JOIN ZnodePimProductAttributeMedia ZPAVM ON (ZPAVM.PimAttributeValueId= ZPAV12.PimAttributeValueId ) 
		LEFT JOIN ZnodeMedia ZM ON (ZM.MediaId = ZPAVM.MediaId)
		WHERE (YUP.PimParentProductId  = UOP.PimProductId AND ZPAV1.pimAttributeId = UOP.PimAttributeId )
		-- Active Variants
		AND ZPAV.PimAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'IsActive')
		-- VariantSKU
		AND ZPAV_SKU.PimAttributeId = (SELECT PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'SKU')
		For JSON Auto 
		),'[]')) 
				
		--</AttributeEntity></Attributes>' 
		FROM ZnodePimConfigureProductAttribute UOP 
		INNER JOIN ZnodePimAttributeJSON c   ON (c.PimAttributeId = UOP.PimAttributeId )
		WHERE  exists(select * from #TBL_PublishCatalogId PPCP1 where UOP.PimProductId = PPCP1.PimProductId )
		AND EXISTS (select * from #PimProductAttributeJson b where b.PimAttributeJsonId = c.PimAttributeJsonId)

			-------------configurable attribute 
			---------------------------------------------------------------------
			
			If (@RevisionType like '%Preview%'  OR @RevisionType like '%Production%'  ) 
				Delete from ZnodePublishProductEntity where ZnodeProductId  in (select a.PublishProductId from #TBL_PublishCatalogId
				A inner join ZnodePublishProductDetail B on A.PublishProductId   =B.PublishProductId   )
				AND LocaleId = @LocaleId
				AND VersionId in (SELECT VersionId FROM ZnodePublishVersionEntity where RevisionType = 'PREVIEW')
			If (@RevisionType like '%Production%' OR @RevisionType = 'None')
				Delete from ZnodePublishProductEntity where ZnodeProductId  in (select A.PublishProductId from #TBL_PublishCatalogId
				A inner join ZnodePublishProductDetail B on A.PublishProductId   =B.PublishProductId   )
				AND LocaleId = @LocaleId
				AND VersionId in (SELECT VersionId FROM ZnodePublishVersionEntity where RevisionType = 'PRODUCTION')

			Insert into ZnodePublishProductEntity (
					VersionId, --1
					IndexId, --2 
					ZnodeProductId,ZnodeCatalogId, --3
					SKU,LocaleId, --4 
					Name,ZnodeCategoryIds, --5
					IsActive, -- 6 
					Attributes, -- 7 
					Brands, -- 9
					CategoryName, --9
					CatalogName,DisplayOrder, --10 
					RevisionType,AssociatedProductDisplayOrder, --11
					ProductIndex,--12
					SalesPrice,RetailPrice,CultureCode,CurrencySuffix,CurrencyCode,SeoDescription,SeoKeywords,SeoTitle,SeoUrl,ImageSmallPath,SKULower --13 
					)
 			SELECT distinct ZPVE.VersionId, --1 
			CAST(ISNULL(ZPCP.ProductIndex,1) AS VARCHAr(100)) + CAST(ISNULL(ZPC.PublishCategoryId,'')  AS VARCHAR(50))  + 
			CAST(Isnull(ZPP.PublishCatalogId ,'')  AS VARCHAR(50)) + CAST( @LocaleId AS VARCHAR(50)) IndexId, --2 
			CAST(ZPP.PublishProductId AS VARCHAR(50)) PublishProductId,CAST(ZPP.PublishCatalogId  AS VARCHAR(50)) PublishCatalogId,  --3 
			CAST(ISNULL(ZPPDFG.SKU ,'') AS NVARCHAR(2000)) SKU,CAST( Isnull(@LocaleId ,'') AS VARCHAR(50)) LocaleId, -- 4 
			CAST(isnull(ZPPDFG.ProductName,'') AS NVARCHAR(2000) )  ProductName ,CAST(ISNULL(ZPCD.PublishCategoryId,'')  AS VARCHAR(50)) PublishCategoryId  -- 5 
			,CAST(ISNULL(ZPPDFG.IsActive ,'0') AS VARCHAR(50)) IsActive , --6 
			'[' +
				(Select STUFF((SELECT distinct ','+ AttributeValue from #TBL_ZnodeTempPublish TY WHERE TY.PimProductId = ZPP.PimProductId   
				FOR XML Path ('')) ,1,1,'')  ) 
			+ ']' xmlvalue,  -- 7 
			'[]' Brands  --8 
			,CAST(isnull(PublishCategoryName,'') AS NVARCHAR(2000)) CategoryName  --9
			,CAST(Isnull(CatalogName,'')  AS NVARCHAR(2000)) CatalogName,CAST(ISNULL(ZPCCF.DisplayOrder,'') AS VARCHAR(50)) DisplayOrder  -- 10  
			,ZPVE.RevisionType RevisionType , 0 AssociatedProductDisplayOrder,-- pending  -- 11 
			Isnull(ZPCP.ProductIndex,1),  -- 12 

			Case When TBZP.IsAllowIndexing = 1 then  ISNULL(CAST(SalesPrice  AS varchar(500)),'') else '' end SalesPrice , 
			Case When TBZP.IsAllowIndexing = 1 then  ISNULL(CAST(RetailPrice  AS varchar(500)),'') else '' end RetailPrice , 
			Case When TBZP.IsAllowIndexing = 1 then  ISNULL(CultureCode ,'') else '' end CultureCode , 
			Case When TBZP.IsAllowIndexing = 1 then  ISNULL(CurrencySuffix ,'') else '' end CurrencySuffix , 
			Case When TBZP.IsAllowIndexing = 1 then  ISNULL(CurrencyCode ,'') else '' end CurrencyCode , 
			Case When TBZP.IsAllowIndexing = 1 then  ISNULL(SEODescription,'') else '' end SEODescriptionForIndex,
			Case When TBZP.IsAllowIndexing = 1 then  ISNULL(SEOKeywords,'') else '' end SEOKeywords,
			Case When TBZP.IsAllowIndexing = 1 then  ISNULL(SEOTitle,'') else '' end SEOTitle,
			Case When TBZP.IsAllowIndexing = 1 then  ISNULL(SEOUrl ,'') else '' end SEOUrl,
			Case When TBZP.IsAllowIndexing = 1 then  ISNULL(ImageSmallPath,'') else '' end ImageSmallPath,
			CAST(ISNULL(LOWER(ZPPDFG.SKU) ,'') AS NVARCHAR(100)) Lower_SKU -- 13
	FROM  #TBL_PublishCatalogId zpp
	INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPP.PublishCatalogId)
	INNER JOIN ZnodePublishProductDetail ZPPDFG ON (ZPPDFG.PublishProductId =  ZPP.PublishProductId)
	INNER JOIN ZnodePublishVersionEntity ZPVE ON (ZPVE.ZnodeCatalogId  = ZPP.PublishCatalogId AND ZPVE.IsPublishSuccess =1 AND ZPVE.LocaleId = @LocaleId )
	LEFT JOIN #ZnodePrice TBZP ON (TBZP.PublishProductId = ZPP.PublishProductId)
	LEFT JOIN #ProductSKU TBPS ON (TBPS.PublishProductId = ZPP.PublishProductId)
	LEFT JOIN #ProductImages TBPI ON (TBPI.PublishProductId = ZPP.PublishProductId  )
	LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishProductId = ZPP.PublishProductId AND ZPCP.PublishCatalogId = ZPP.PublishCatalogId)
	LEFT JOIN ZnodePublishCategory ZPC ON (ZPC.PublishCatalogId = ZPCP.PublishCatalogId AND   ZPC.PublishCategoryId = ZPCP.PublishCategoryId)
	LEFT JOIN ZnodePimCategoryProduct ZPCCF ON (ZPCCF.PimCategoryId = ZPC.PimCategoryId  AND ZPCCF.PimProductId = ZPP.PimProductId )
	LEFT JOIN ZnodePimCategoryHierarchy ZPCH ON (ZPCH.PimCatalogId = ZPCV.PimCatalogId AND  ZPCH.PimCategoryHierarchyId =  ZPC.PimCategoryHierarchyId) 
	LEFT JOIN ZnodePublishCategoryDetail ZPCD ON (ZPCD.PublishCategoryId = ZPCP.PublishCategoryId AND ZPCD.LocaleId = @LocaleId )
	WHERE ZPPDFG.LocaleId = @LocaleId
		--AND zpp.LocaleId = @LocaleId
	AND 
		(
			(ZPVE.RevisionType =  Case when  (@RevisionType like '%Preview%'  OR @RevisionType like '%Production%' ) then 'Preview' End ) 
			OR 
			(ZPVE.RevisionType =  Case when (@RevisionType like '%Production%' OR @RevisionType = 'None') then  'Production'  end )
		)


	DELETE FROM #TBL_ZnodeTempPublish
	IF OBJECT_ID('tempdb..#PimProductAttributeJson') is not null
	 BEGIN 
		DELETE FROM #PimProductAttributeJson
	 END
	 IF OBJECT_ID('tempdb..#TBL_CustomeFiled') is not null
	 BEGIN 
	 DROP TABLE #TBL_CustomeFiled
	 END
	 IF OBJECT_ID('tempdb..#TBL_AttributeVAlue') is not null
	 BEGIN 
	 DROP TABLE #TBL_AttributeVAlue
	 END
 
	DELETE FROM @PimDefaultValueLocale
SET @Counter = @counter + 1 
END

SET @Status =1 

END TRY 
BEGIN CATCH 
	SET @Status =0  
	 SELECT 1 AS ID,@Status AS Status;   
	 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC [Znode_GetPublishSingleProductJson] 
		@PublishCatalogId = '+CAST(@PublishCatalogId  AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
				
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetPublishSingleProductJson',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END