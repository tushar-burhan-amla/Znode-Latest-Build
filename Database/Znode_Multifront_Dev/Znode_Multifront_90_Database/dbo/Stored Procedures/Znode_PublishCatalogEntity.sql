CREATE PROCEDURE [dbo].[Znode_PublishCatalogEntity]  
(  
	@PimCatalogId  INT = 0   
	,@RevisionState VARCHAR(50) = ''   
	,@UserId INT = 0  
	,@NewGUID NVARCHAR(500)   
	,@IsDraftProductsOnly BIT = 1  
	,@PimProductId  INT = 0  
	,@PimCategoryHierarchyId INT = 0 
)  
AS  
/*  
To publish all catalog product and their details  
Unit Testing :   
Exec [dbo].[Znode_PublishCatalogEntity]  
@PimCatalogId  = 5  
,@RevisionState = 'PRODUCTION'   
,@UserId = 2  
,@NewGUID = '123'  
   
EXEC Znode_DeletePublishCatalogEntity @PublishCatalogId = 3,@UserId = 2 , @IsRevertPublish = 0 ,  
@NewGUID ='123'   
  
*/  
BEGIN  
BEGIN TRY   
	SET NOCOUNT ON  

	
	DECLARE @Tbl_versions  TABLE (RevisionState VARCHAr(300))
	DECLARE @DefaultLocaleId Int = dbo.fn_getDefaultLocaleId() , @messagestring varchar(300) =''  ,@PublishType varchar(2000)= 'Catalog'
			,@Getdate DATETIME = dbo.fn_getdate(), @CultureCode varchar(200) = '', @CurrencySuffix varchar(200) = '',@CurrencyCode varchar(200) = ''
			,@CatalogName VARCHAR(600), @IsAllowIndexing bit ,@Status bit, @email nvarchar(200) = (SELECT TOP 1 Username FROM znodeUSer WHERE UserId = @UserId) 
	DECLARE @Versions_new TABLE (VersionId int , LocaleId int , PublishStateId int, RevisionState VARCHAR(100) )
	DECLARE @Versions_working TABLE (VersionId int , LocaleId int , PublishStateId int, RevisionState VARCHAR(100) )
	CREATE TABLE  #PimProduct_catalog  (PimProductId int , ParentPimProductId INT, PimCategoryId INT , PimCategoryHierarchyId INT 
			,ParentPimCategoryHierarchyId INT ,DisplayOrder INT  ,IsActive INT,ActivationDate DATETIME , ExpirationDate DATETIME  ,IsDefault bit,PimAddonGroupId INT 
			, BundleQuantity int , IsAssocitedProduct bit   )
    DECLARE @inserted_pimIds TABLE (PimProductId INT )

	SET @messagestring = CASE WHEN @PimProductId > 0  THEN ' Product-'+(SELECT TOP 1 SKU FROM ZnodePimProduct WIth (Nolock) WHERE PimProductId = @PimProductId )+' ' ELSE '' END 

	SELECT TOp 1 @CatalogName= CatalogName,  @IsAllowIndexing= ISNULL(IsAllowIndexing,0)
	FROM ZnodePimCatalog with(nolock)  WHERE PimCatalogId = @PimCatalogId   
	
	SELECT TOp 1  @CurrencyCode = FeatureValues
	FROM ZnodeGlobalSetting a 
	WHERE FeatureName='Currency'

	SELECT TOp 1  @CultureCode = FeatureValues
	FROM ZnodeGlobalSetting a 
	WHERE FeatureName='Culture'

	SELECT TOP 1 @CurrencySuffix = Symbol
	FROM ZnodeCulture
	WHERE CultureCode = @CultureCode

	INSERT INTO ZnodePublishProgressNotifierEntity (VersionId,JobId,JobName,ProgressMark,IsCompleted,IsFailed,ExceptionMessage,StartedBy,StartedByFriendlyName)
	SELECT 0 , @NewGUID, 'Catalog-'+@CatalogName+'-Publish Started ', 10 , 0 , 0 , '',@UserId ,  @email

	INSERT INTO @Tbl_versions 
	SELECT 'PREVIEW'
	WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePublishStateApplicationTypeMapping with(nolock) WHERE ApplicationType = 'WebstorePreview' AND IsActive =1 AND IsEnabled = 1 )
	UNION ALL 
	SELECT 'PRODUCTION'
	WHERE @RevisionState = 'Production' OR  @RevisionState = 'None'
	
    SET @PublishType = CASE WHEN @PimProductId > 0 THEN 'Product'
	  WHEN @PimCategoryHierarchyId > 0 THEN 'Category'
	ELSE 'Catalog' END 

	INSERT INTO ZnodePublishCatalogLog(PublishCatalogId,PimCatalogId,IsCatalogPublished,PublishCategoryId,  
			IsCategoryPublished,PublishProductId,  
			IsProductPublished,UserId,LogDateTime,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Token,LocaleId,PublishStateId,PublishType)  
	OUTPUT inserted.PublishCatalogLogId , inserted.LocaleId, inserted.PublishStateId, inserted.Token INTO @Versions_new
	SELECT DISTINCT @PimCatalogId,  @PimCatalogId,NULL,0,  NULL,@PimProductId,  NULL,@UserId, @Getdate, @UserId, @Getdate, @UserId ,@Getdate,  r.RevisionState
				,a.LocaleId ,DBO.Fn_GetPublishStateIdForProcessing() ,@PublishType 
	FROM ZnodePortalLocale a with(nolock) 
	INNER JOIN ZnodePortalCatalog b with(nolock)  ON (b.PortalId = a.PortalId)
	CROSS APPLY @Tbl_versions  r
	WHERE  b.PublishCatalogId = @PimCatalogId 
	AND a.LocaleId IN (SELECT LocaleId FROM ZnodeLocale p with(nolock)  WHERE p.IsActive = 1 )
	UNION ALL 
	SELECT DISTINCT @PimCatalogId,  @PimCatalogId,NULL,0,  NULL,0,  NULL,@UserId, @Getdate, @UserId, @Getdate, @UserId ,@Getdate,  r.RevisionState
				,@DefaultLocaleId ,DBO.Fn_GetPublishStateIdForProcessing() ,@PublishType
	FROM  @Tbl_versions  r
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePortalCatalog t WHERE t.PublishCatalogId = @PimCatalogId)
	

	INSERT INTO ZnodePublishCatalogEntity (VersionId,ZnodeCatalogId,CatalogName,RevisionType,LocaleId,IsAllowIndexing) 
	SELECT VersionId, @PimCatalogId, @CatalogName, RevisionState, LocaleId, @IsAllowIndexing
	FROM @Versions_new p
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishCatalogEntity y with(nolock)  WHERE y.ZnodeCatalogId = @PimCatalogId AND y.RevisionType = p.RevisionState
		 AND y.LocaleId = p.LocaleId
	)
	INSERT INTO ZnodePublishVersionEntity (VersionId,ZnodeCatalogId,RevisionType,LocaleId,IsPublishSuccess)  
	SELECT VersionId, @PimCatalogId,  RevisionState, LocaleId, 0
	FROM @Versions_new p
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishVersionEntity y with(nolock)  WHERE y.ZnodeCatalogId = @PimCatalogId AND y.RevisionType = p.RevisionState
		AND y.LocaleId = p.LocaleId
	)

	INSERT INTO @Versions_working 
	SELECT VersionId , a.LocaleId, g.PublishStateId,UPPER( a.RevisionType)RevisionType
	FROM ZnodePublishVersionEntity a with(nolock)
	INNER JOIN ZnodePublishState g with(nolock) ON (g.PublishStateCode = a.RevisionType)
	WHERE a.ZnodeCatalogId = @PimCatalogId
	AND RevisionType IN (SELECT RevisionState FROM @Tbl_versions)
	AND a.LocaleId IN (SELECT p.LocaleId FROM @Versions_new p )
	

	UPDATE ZnodePublishProgressNotifierEntity
	SET JobName =  @messagestring+'Catalog-'+@CatalogName+'-'+'Collecting Products'
		,ProgressMark = 20
	WHERE Jobid = @NewGUID


    INSERT INTO #PimProduct_catalog 
    SELECT DISTINCT PimProductId , 0  , b.PimCategoryId, b.PimCategoryHierarchyId , b.ParentPimCategoryHierarchyId
        ,b.DisplayOrder ,b.IsActive,b.ActivationDate,b.ExpirationDate, 0 IsDefault , 0 PimAddonGroupId , 0, 0
    FROM ZnodePimCategoryProduct a with(nolock)
    INNER JOIN ZnodePimCategoryHierarchy b with(nolock) ON (b.PimCategoryId = a.PimCategoryId)
    WHERE b.PimCatalogId = @PimCatalogId
    AND ( a.PimProductId =  @PimProductId   OR @PimProductId =  0 ) 
    AND ( B.PimCategoryHierarchyId =  @PimCategoryHierarchyId   OR @PimCategoryHierarchyId =  0 )
	

INSERT INTO #PimProduct_catalog 
SELECT DISTINCT a.PimProductId , b.PimProductId ,0 As PimCategoryId, 0 As PimCategoryHierarchyId, 0 As ParentPimCategoryHierarchyId
,b.DisplayOrder ,b.IsActive,b.ActivationDate,b.ExpirationDate, 0 IsDefault, 0 PimAddonGroupId , 0,0
FROM ZnodePimLinkProductDetail a with(nolock)
INNER JOIN #PimProduct_catalog b ON (b.PimProductId = a.PimParentProductId)
WHERE NOT EXISTS (SELECT TOP 1 1 FROM #PimProduct_catalog rt WHERE rt.PimProductId = a.PimProductId )

SELECT DISTINCT ZPAPD.PimChildProductId PimProductId,ZPAP.PimProductId ParentPimProductId, 0 PimCategoryId , 0 PimCategoryHierarchyId,0 ParentPimCategoryHierarchyId , ZPAPD.DisplayOrder ,1 IsActive
,NULL ActivationDate,NULL ExpirationDate ,IsDefault
, ZPAP.PimAddonGroupId PimAddonGroupId , 0 BundleQuantity,RequiredType
INTO #TBL_AddOnProduct
FROM ZnodePimAddOnProductDetail AS ZPAPD with(nolock)
INNER JOIN ZnodePimAddOnProduct AS ZPAP with(nolock) ON (ZPAP.PimAddOnProductId = ZPAPD.PimAddOnProductId)
WHERE EXISTS (SELECT TOP 1 1 FROM #PimProduct_catalog ty WHERE ty.PimProductId = ZPAP.PimProductId )

-- SELECT * FROM ZnodePimAddOnProduct
INSERT INTO #PimProduct_catalog 
SELECT DISTINCT ZPAPD.PimProductId,ZPAPD.ParentPimProductId, 0 PimCategoryId , 0 PimCategoryHierarchyId , 0 ,DisplayOrder ,0 ,NULL,NULL,IsDefault
,PimAddonGroupId  , 0 ,0
FROM #TBL_AddOnProduct ZPAPD
WHERE NOT EXISTS (SELECT TOP 1 1 FROM #PimProduct_catalog rt WHERE rt.PimProductId = ZPAPD.PimProductId )

INSERT INTO #PimProduct_catalog 
SELECT DISTINCT a.PimProductId , a.PimParentProductId , 0 PimCategoryId , 0 PimCategoryHierarchyId,a.DisplayOrder,0,0,NULL,NULL,IsDefault
, 0 PimAddonGroupId , BundleQuantity, 1
FROM ZnodePimProductTypeAssociation a with(nolock)
WHERE EXISTS (SELECT TOP 1 1 FROM #PimProduct_catalog ty WHERE ty.PimProductId = a.PimParentProductId )
--AND NOT EXISTS (SELECT TOP 1 1 FROM #PimProduct_catalog rt WHERE rt.PimProductId = a.PimProductId )

CREATE INDEX IDX_#PimProduct_catalog ON #PimProduct_catalog(PimProductId)

	IF @IsDraftProductsOnly = 0  
    BEGIN 
      --  EXEC Znode_CatalogProductDraftForPublish @PublishCatalogId=@PimCatalogId   
	  ---To draft all catalog products AND associated products for full catalog publish  

	  UPDATE ZnodePimProduct 
	  SET PublishStateId = 2 
	  WHERE EXISTS (SELECT TOP 1 1 FROM #PimProduct_catalog y WHERE y.PimProductId = ZnodePimProduct.PimProductId )
    END 
    


DROP TABLE IF EXISTS #PimProduct_distinct

SELECT DISTINCT a.PimProductId, PimCategoryHierarchyId PimCategoryId ,sku,ProductName, CAST(b.IsActive AS VARCHAR(50)) IsActive,b.PublishStateId, b.ProductType
INTO #PimProduct_distinct
FROM #PimProduct_catalog a
INNER JOIN ZnodePimProduct b with(nolock) ON (b.PimProductId = a.PimProductId)


CREATE INDEX IDX_#PimProduct_distinct ON #PimProduct_distinct(PimProductId,IsActive)

DELETE tt FROM #PimProduct_distinct tt 
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProduct t with(nolock) WHERE t.PimProductId = tt.PimProductId AND t.ProductType = 'SimpleProduct' ) 
AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductTypeAssociation ty with(nolock) WHERE ty.PimParentProductId = tt.PimProductId)

	

UPDATE ZnodePublishProgressNotifierEntity
SET JobName =  @messagestring+'Catalog-'+@CatalogName+'-'+'Create backup of version'
	,ProgressMark = 25
WHERE Jobid = @NewGUID

DECLARE @Deleted_Products TABLE (PimProductId INT )

SELECT p.PublishProductEntityId
INTO #PublishProductEntityId
FROM ZnodePublishProductEntity p With(nolock)  WHERE  NOT EXISTS (SELECT TOP 1 1 FROM #PimProduct_distinct ty WHERE ty.PimProductId = p.ZnodeProductId)
AND EXISTS (SELECT TOP 1 1 FROM @Versions_working n WHERE n.VersionId = p.VersionId ) 
AND  @PimProductId = 0 AND @PimCategoryHierarchyId = 0 
UNION ALL 
SELECT p.PublishProductEntityId
FROM ZnodePublishProductEntity p With(nolock)  WHERE EXISTS (SELECT TOP 1 1 FROM #PimProduct_distinct ty WHERE ty.PimProductId = p.ZnodeProductId)
AND EXISTS (SELECT TOP 1 1 FROM @Versions_working n WHERE n.VersionId = p.VersionId ) 
AND EXISTS (SELECT TOP 1 1 FROM ZnodePimProduct t with(nolock) WHERE t.PimProductId = p.ZnodeProductId AND t.IsActive = 'false' ) 
AND @PimCategoryHierarchyId = 0 


UPDATE ZnodePublishProductEntity 
SET ElasticSearchEvent = 2 
WHERE EXISTS (SELECT TOP 1 1  FROM #PublishProductEntityId t WHERE t.PublishProductEntityId = ZnodePublishProductEntity.PublishProductEntityId ) 


SELECT AttributeCode , AttributeName , AttributeTypeName ,ISNULL( IsUseInSearch,0) IsUseInSearch,ISNULL(IsHtmlTags,0)IsHtmlTags
,ISNULL(IsComparable,0)IsComparable, a.PimAttributeId ,ISNULL(d.IsFacets,0) IsFacets
, ISNULL(a.IsConfigurable,0)IsConfigurable, ISNULL( a.IsPersonalizable, 0)IsPersonalizable
		, a.DisplayOrder,IsCategory, b.LocaleId
INTO #temp_attributename 
FROM ZnodePimAttribute a with(nolock)
INNER JOIN ZnodePimAttributeLocale b with(nolock) ON (b.PimAttributeId = a.PimAttributeId AND b.LocaleId =@DefaultLocaleId)
INNER JOIN ZnodeAttributeType c with(nolock) ON (c.AttributeTypeId = a.AttributeTypeId)
LEFT JOIN ZnodePimFrontendProperties d with(nolock) ON (d.PimAttributeId = a.PimAttributeId )

	
	UPDATE ZnodePublishProgressNotifierEntity
	SET JobName =   @messagestring+'Catalog-'+@CatalogName+'-'+'Publishing category'
		,ProgressMark = 30
	WHERE Jobid = @NewGUID


SELECT DISTINCT  rt.PimCategoryHierarchyId, c.AttributeCode,b.CategoryValue AttributeValues, 0 DisplayOrder , 
rt.ParentPimCategoryHierarchyId,b.LocaleId,a.PimCategoryAttributeValueId, rt.PimCategoryId,rt.IsActive
INTO #Temp_Categoryvalue
FROM ZnodePimCategoryAttributeValue a with(nolock)
INNER JOIN ZnodePimCategoryAttributeValueLocale b with(nolock) ON (b.PimCategoryAttributeValueId = a.PimCategoryAttributeValueId AND b.LocaleId = @DefaultLocaleId)
INNER JOIN #temp_attributename c ON (c.PimAttributeId = a.PimAttributeId AND c.IsCategory =1 )
INNER JOIN  #PimProduct_catalog rt ON ( rt.PimCategoryId = a.PimCategoryId  )


INSERT INTO #Temp_Categoryvalue
SELECT DISTINCT  ty.PimCategoryHierarchyId, c.AttributeCode,b.CategoryValue AttributeValues, ty.DisplayOrder ,
ty.ParentPimCategoryHierarchyId,b.LocaleId,a.PimCategoryAttributeValueId,ty.PimCategoryId,ty.IsActive
FROM ZnodePimCategoryAttributeValue a with(nolock)
INNER JOIN ZnodePimCategoryAttributeValueLocale b with(nolock) ON (b.PimCategoryAttributeValueId = a.PimCategoryAttributeValueId AND b.LocaleId = @DefaultLocaleId)
INNER JOIN #temp_attributename c ON (c.PimAttributeId = a.PimAttributeId AND c.IsCategory =1 )
INNER JOIN ZnodePimCategoryHierarchy ty with(nolock) ON (ty.PimCategoryId = a.PimCategoryId)
WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Temp_Categoryvalue ut WHERE ut.PimCategoryHierarchyId = ty.PimCategoryHierarchyId)
AND ty.PimCatalogId =@PimCatalogId
AND @PimProductId = 0 


UPDATE a SET a.AttributeValues = ZM.Path
FROM #Temp_Categoryvalue a
INNER  JOIN ZnodeMedia ZM ON (CAST(zm.MediaId AS VARCHAR(200)) = a.AttributeValues)
WHERE a.AttributeCode IN (SELECT n.AttributeCode FROM #temp_attributename n WHERE n.AttributeTypeName = 'Image' )

DELETE p 
FROM ZnodePublishCategoryEntity p 
WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Temp_Categoryvalue ty WHERE ty.PimCategoryHierarchyId = p.ZnodeCategoryId and ty.IsActive =1)
AND EXISTS (SELECT TOP 1 1 FROM @Versions_working n WHERE n.VersionId = p.VersionId ) 
AND @PimProductId = 0 AND @PimCategoryHierarchyId = 0 

IF EXISTS (SELECT TOP 1 1 FROM @Versions_working r WHERE r.LocaleId <> @DefaultLocaleId)
BEGIN 
    
	INSERT INTO #Temp_Categoryvalue(PimCategoryHierarchyId, AttributeCode, AttributeValues, DisplayOrder , ParentPimCategoryHierarchyId, t.LocaleId,PimCategoryAttributeValueId,PimCategoryId,IsActive)
	SELECT PimCategoryHierarchyId, AttributeCode, ISNULL(b.CategoryValue,a.AttributeValues)AttributeValues, DisplayOrder , ParentPimCategoryHierarchyId, t.LocaleId,a.PimCategoryAttributeValueId,a.PimCategoryId,a.IsActive
	FROM #Temp_Categoryvalue a 
	CROSS APPLY @Versions_working t 
	LEFT JOIN ZnodePimCategoryAttributeValueLocale b ON (b.PimCategoryAttributeValueId = a.PimCategoryAttributeValueId AND b.LocaleId = t.LocaleId)
	WHERE t.LocaleId <> @DefaultLocaleId

	INSERT INTO #temp_attributename
	SELECT AttributeCode , IIF(b.AttributeName IS NULL, a.AttributeName ,b.AttributeName) , AttributeTypeName ,ISNULL( IsUseInSearch,0) IsUseInSearch,ISNULL(IsHtmlTags,0)IsHtmlTags
		,ISNULL(IsComparable,0)IsComparable, a.PimAttributeId ,ISNULL(a.IsFacets,0) IsFacets
		, ISNULL(a.IsConfigurable,0)IsConfigurable, ISNULL( a.IsPersonalizable, 0)IsPersonalizable
		, a.DisplayOrder,IsCategory, t.LocaleId
	FROM #temp_attributename a
	CROSS APPLY @Versions_working t 
	LEFT JOIN ZnodePimAttributeLocale b ON (b.PimAttributeId = a.PimAttributeId AND b.LocaleId = t.LocaleId)
	WHERE t.LocaleId <> @DefaultLocaleId

END 

SELECT DISTINCT PimCategoryHierarchyId, DisplayOrder ,ParentPimCategoryHierarchyId ,LocaleId,a.PimCategoryId,a.IsActive
INTO #Temp_categoryDetails
FROM #Temp_Categoryvalue a 


ALTER TABLE #Temp_categoryDetails ADD  ActivationDate DATETIME ,ExpirationDate DATETIME  ,CategoryName NVARCHAr(max), CategoryCode NVARCHAr(600)

UPDATE a  
SET CategoryName = (SELECT TOP 1 AttributeValues FROM #Temp_Categoryvalue tu WHERE tu.PimCategoryHierarchyId = a.PimCategoryHierarchyId AND tu.LocaleId = a.LocaleId AND tu.AttributeCode = 'CategoryName')
,CategoryCode = (SELECT TOP 1 AttributeValues FROM #Temp_Categoryvalue tu WHERE tu.PimCategoryHierarchyId = a.PimCategoryHierarchyId AND tu.LocaleId = a.LocaleId AND tu.AttributeCode = 'CategoryCode')
,DisplayOrder = ISNULL(b.DisplayOrder,a.DisplayOrder) , ActivationDate = b.ActivationDate , ExpirationDate = b.ExpirationDate
FROM #Temp_categoryDetails  a
LEFT JOIN #PimProduct_catalog b ON (b.PimCategoryHierarchyId = a.PimCategoryHierarchyId)
--WHERE @PimProductId = 0 


UPDATE a
SET a.Name = t.CategoryName
,a.CategoryCode = t.CategoryCode
,a.Attributes = (SELECT r.AttributeCode, AttributeValues,jk.AttributeName, jk.AttributeTypeName  ,jk.IsUseInSearch,jk.IsHtmlTags,jk.IsComparable
					FROM #Temp_Categoryvalue r 
					INNER JOIN #temp_attributename jk ON (jk.AttributeCode = r.AttributeCode AND jk.LocaleId= r.LocaleId  AND jk.IsCategory = 1 )
					WHERE r.PimCategoryHierarchyId = t.PimCategoryHierarchyId AND r.LocaleId = t.LocaleId
					FOR JSON PATH ) 

,a.ZnodeParentCategoryIds  = '['+CAST(t.ParentPimCategoryHierarchyId AS VARCHAR(580))+']'
,a.ProductIds = '['+(SELECT  STRING_AGG(  CONVERT(VARCHAR(MAX),PimProductId) ,',') FROM #PimProduct_catalog ty WHERE ty.PimCategoryHierarchyId = a.ZnodeCategoryId  )+']' 
,a.DisplayOrder = t.DisplayOrder 
,a.IsActive = t.IsActive, a.ActivationDate = t.ActivationDate, a.ExpirationDate = t.ExpirationDate

FROM ZnodePublishCategoryEntity a with(nolock)
INNER JOIN @Versions_working q ON ( q.VersionId = a.VersionId)
INNER JOIN #Temp_categoryDetails t ON (t.PimCategoryHierarchyId = a.ZnodeCategoryId AND t.LocaleId = a.LocaleId )
WHERE a.ZnodeCatalogId = @PimCatalogId
--AND @PimProductId = 0 


INSERT INTO ZnodePublishCategoryEntity (VersionId,ZnodeCategoryId,Name,CategoryCode,ZnodeCatalogId,CatalogName,ZnodeParentCategoryIds
,ProductIds,LocaleId,IsActive,DisplayOrder,Attributes,ActivationDate,ExpirationDate,CategoryIndex,ElasticSearchEvent)

SELECT q.VersionId, a.PimCategoryHierarchyId, a.CategoryName, a.CategoryCode,@PimCatalogId,@CatalogName,'['+CAST(a.ParentPimCategoryHierarchyId AS VARCHAR(580))+']'
	,'['+(SELECT  STRING_AGG(   CONVERT(VARCHAR(MAX),PimProductId) ,',') FROM #PimProduct_catalog ty WHERE ty.PimCategoryHierarchyId = a.PimCategoryHierarchyId  )+']' 
	,q.LocaleId, a.IsActive,a.DisplayOrder,(SELECT r.AttributeCode , AttributeName, AttributeValues, AttributeTypeName,IsUseInSearch,IsHtmlTags,IsComparable
					FROM #Temp_Categoryvalue r 
					INNER JOIN #temp_attributename jk ON (jk.AttributeCode = r.AttributeCode AND jk.LocaleId= r.LocaleId AND jk.IsCategory =1 )
					WHERE r.PimCategoryHierarchyId = a.PimCategoryHierarchyId AND q.LocaleId = r.LocaleId					FOR JSON PATH ) , a.ActivationDate, a.ExpirationDate,1,1
FROM #Temp_categoryDetails a
CROSS APPLY @Versions_working q 
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishCategoryEntity Tot with(nolock) WHERE tot.VersionId = q.VersionId AND tot.ZnodeCategoryId = a.PimCategoryHierarchyId )
AND a.LocaleId = q.LocaleId AND a.IsActive=1
  


	UPDATE ZnodePublishProgressNotifierEntity
	SET JobName =   @messagestring+'Catalog-'+@CatalogName+'-'+'Publishing Product'
		,ProgressMark = 35
	WHERE Jobid = @NewGUID


SELECT  a.PimProductId,a.sku,a.ProductName,a.IsActive ,a.PublishStateId , max(a.PimCategoryId) ZnodeCategoryIds , CAST('' AS NVARCHAR(2000)) CategoryName
         ,STRING_AGG( CONVERT(VARCHAR(MAX),a.PimCategoryId),',') WITHIN GROUP (ORDER BY a.PimCategoryId) ZnodeParentCategoryIds
		 ,CAST('' AS VARCHAR(1000)) SeoDescription,CAST('' AS VARCHAR(1000))SeoKeywords
	   ,CAST('' AS VARCHAR(1000))SeoTitle,CAST('' AS VARCHAR(1000)) SeoUrl
	   , CAST('' AS VARCHAR(50)) SalesPrice,CAST('' AS VARCHAR(50)) RetailPrice
INTO #TBL_finalProducts
FROM #PimProduct_distinct a
GROUP BY  a.PimProductId,a.sku,a.ProductName,a.IsActive ,a.PublishStateId--, a.PimCategoryId




SELECT CAST(SKU AS VARCHAR(2000)) SeoCode 
INTO #filterSEOCOde
FROM #TBL_finalProducts
WHERE PublishStateId IN (2 ,1)
UNION ALL 
SELECT CAST(CategoryCode AS VARCHAR(2000))
FROM #Temp_categoryDetails

CREATE INDEX IDX_#filterSEOCOde_SEOCode ON #filterSEOCOde(SeoCode)

UPDATE ZnodeCMSSEODetail 
SET PublishStateId = 2 
WHERE EXISTS (SELECT TOP 1 1 FROM  #filterSEOCOde y WHERE y.SeoCode = ZnodeCMSSEODetail.SEOCode)
AND CMSSEOTypeId IN (1,2)
AND EXISTS (SELECT TOP 1 1 FROM ZnodePortalCatalog t with(nolock) WHERE t.PublishCatalogId =@PimCatalogId AND ZnodeCMSSEODetail.PortalId = t.PortalId  )
AND PublishStateId <> 2 
--CREATE INDEX IDX_ZnodeCMSSEODetail_SEOCode_CMSSEOTypeId_PortalId ON ZnodeCMSSEODetail(SEOCode,CMSSEOTypeId,PortalId)

DROP TABLE IF EXISTS  #Seo_entity



SELECT 0 VersionId
,@Getdate PublishStartTime,c.Name ItemName,a.CMSSEODetailId,b.CMSSEODetailLocaleId,c.CMSSEOTypeId,a.SEOId,c.name SEOTypeName,b.SEOTitle
,b.SEODescription,b.SEOKeywords,a.SEOUrl,a.IsRedirect,a.MetaInformation,b.LocaleId,'' OldSEOURL,0 CMSContentPagesId,a.PortalId,a.SEOCode,b.CanonicalURL
,b.RobotTag,1 ElasticSearchEvent, 0 PublishSeoEntityId
INTO #Seo_entity
FROM ZnodeCMSSEODetail a with (nolock)
INNER JOIN ZnodeCMSSEODetailLocale b with(nolock) ON (b.CMSSEODetailId = a.CMSSEODetailId)
INNER JOIN ZnodeCMSSEOType c with(nolock) ON (c.CMSSEOTypeId = a.CMSSeoTypeId ) 
WHERE c.CMSSEOTypeId IN (1,2) AND a.PublishStateId = 2
AND EXISTS (SELECT TOP 1 1 FROM ZnodePortalCatalog t with(nolock) WHERE t.PublishCatalogId =@PimCatalogId AND a.PortalId = t.PortalId  )




UPDATE ZnodeCMSSEODetail
SET PublishStateId = CASE WHEN @RevisionState = 'Preview' THEN 4 ELSE 3 END 
WHERE EXISTS (SELECT TOP 1 1 FROM #Seo_entity m WHERE m.CMSSEODetailId = ZnodeCMSSEODetail.CMSSEODetailId )

CREATE INDEX IDX_#Seo_entity_PublishSeoEntityId ON #Seo_entity(PublishSeoEntityId)


IF  @IsAllowIndexing = 1  
BEGIN 



SELECT SEoCode SKU ,MAX(SeoDescription)SeoDescription,Max(SeoKeywords) SeoKeywords,Max(SeoTitle)SeoTitle,Max(SeoUrl)SeoUrl
INTO #Distinct_seo
FROM #Seo_entity
WHERE CMSSEOTypeId = 1 
GROUP BY SEoCode


SELECT SKU , MIN(RetailPrice) RetailPrice, MIN(SalesPrice ) SalesPrice
INTO #price_data
FROM ZnodePrice a with (nolock)
WHERE EXISTS (SELECT TOP 1 1 FROM #PimProduct_distinct r WHERE r.SKU = a.SKU )
GROUP BY SKU

UPDATE a
SET SeoDescription = ISNULL(p.SeoDescription,'') ,SeoKeywords= ISNULL(p.SeoKeywords,'')
	   ,SeoTitle=ISNULL(p.SeoTitle,''),SeoUrl=ISNULL(p.SeoUrl,'')
FROM #TBL_finalProducts a
INNER JOIN #Distinct_seo p ON (p.SKU = a.SKU )
WHERE a.PublishStateId IN (2,1)

UPDATE a
SET SalesPrice =ISNULL(toe.SalesPrice,0) ,RetailPrice=ISNULL(toe.RetailPrice,0) 
FROM #TBL_finalProducts a
INNER JOIN #price_data toe ON (toe.SKU = a.SKU  )
 WHERE a.PublishStateId IN (2,1)
END
ELSE 
BEGIN 

 UPDATE ZnodePublishProductEntity 
SET SeoUrl = '' , SeoDescription = '', SeoKeywords = '', SeoTitle = '', SalesPrice = 0 
, RetailPrice =0 
WHERE @IsAllowIndexing = 0 AND ZnodeCatalogId =@PimCatalogId 
AND EXISTS (SELECT TOP 1 1 FROM @Versions_working t WHERE t.VersionId = ZnodePublishProductEntity.VersionId)

END 




DECLARE @VersionId_l INT ,@LocaleId_l INT ,@RevisionState_l VARCHAR(300)

DECLARE cur_localeId CURSOR FOR 
SELECT VersionId,LocaleId,RevisionState
FROM @Versions_working

OPEN cur_localeId

FETCH NEXT FROM cur_localeId INTO @VersionId_l,@LocaleId_l,@RevisionState_l
WHILE @@FETCH_STATUS = 0
BEGIN 


--AND  1=0 

UPDATE a
SET a.PublishSeoEntityId = n.PublishSeoEntityId
FROM #Seo_entity a
INNER  JOIN ZnodePublishSeoEntity n with(nolock) ON (n.VersionId = @VersionId_l AND n.SEOCode = a.SEOCode AND n.PortalId = a.PortalId AND n.CMSSEOTypeId = a.CMSSEOTypeId AND n.LocaleId =@LocaleId_l)
WHERE a.LocaleId = @LocaleId_l


UPDATE a 
SET PublishStateId = 1 
FROM #TBL_finalProducts a 
WHERE NOT EXISTS (
SELECT TOP 1 1 FROM ZnodePublishProductEntity y with(nolock) WHERE y.ZnodeProductId = a.PimProductId AND y.ZnodeCategoryIds = a.ZnodeCategoryIds
AND y.VersionId =@VersionId_l
)



UPDATE a SET CategoryName = uy.Name
FROM #TBL_finalProducts a 
INNER JOIN ZnodePublishCategoryEntity uy  with(nolock) ON (uy.ZnodeCategoryId = a.ZnodeCategoryIds)
WHERE uy.VersionId =@VersionId_l
AND a.PublishStateId IN (2,1)




DROP TABLE IF EXISTS  #insertedPublishEntity

SELECT  @VersionId_l VersionId , ROW_NUMBER()Over(ORDER BY a.PimProductId )IndexId, a.PimProductId ZnodeProductId , @PimCatalogId ZnodeCatalogId,a.sku,@LocaleId_l LocaleId,a.ProductName Name , ZnodeCategoryIds ,a.IsActive,'' Attributes,'[]' Brands
       , CategoryName, @CatalogName CatalogName, 0 DisplayOrder, @RevisionState_l  RevisionType, 0 AssociatedProductDisplayOrder, ROW_NUMBER()Over(partition By  a.PimProductId ORDER BY  a.PimProductId  )  ProductIndex,  SalesPrice, RetailPrice
	   ,@CultureCode CultureCode,@CurrencySuffix CurrencySuffix
	   ,@CurrencyCode CurrencyCode, SeoDescription,SeoKeywords
	   ,SeoTitle, SeoUrl,'' ImageSmallPath,LOWER(a.sku) SKULower,1 ElasticSearchEvent, ZnodeParentCategoryIds ,@Getdate ModifiedDate,0 IsSingleProductPublish
	   ,0 IsCacheClear, 0 PublishProductEntityId,a.PublishStateId
INTO #insertedPublishEntity 
FROM #TBL_finalProducts a 
WHERE a.IsActive = 'true'  AND a.PublishStateId IN (2,1)



UPDATE a
SET a.PublishProductEntityId=ty.PublishProductEntityId
FROM #insertedPublishEntity a
INNER JOIN ZnodePublishProductEntity ty with(nolock) ON ( ty.VersionId = @VersionId_l AND ty.ZnodeProductId  = a.ZnodeProductId AND ty.ZnodeCatalogId =@PimCatalogId )


UPDATE a
SET Name = b.name , a.SalesPrice = b.SalesPrice , a.RetailPrice = b.RetailPrice, CurrencyCode = b.CurrencyCode , CultureCode= b.CultureCode, CurrencySuffix = b.CurrencySuffix
, CategoryName = b.CategoryName, a.ElasticSearchEvent = 1 , ZnodeParentCategoryIds = b.ZnodeParentCategoryIds , a.SKU = b.SKU , a.SKULower = b.SKULower, a.SeoUrl = b.SeoUrl
, a.SeoTitle = b.SeoTitle , a.SeoKeywords = b.SeoKeywords , a.SeoDescription = b.SeoDescription
FROM ZnodePublishProductEntity a 
INNER JOIN #insertedPublishEntity b ON (b.PublishProductEntityId = a.PublishProductEntityId)
WHERE b.PublishProductEntityId <> 0 --AND b.PublishStateId = 2 



INSERT INTO ZnodePublishProductEntity(VersionId,IndexId,ZnodeProductId,ZnodeCatalogId,SKU,LocaleId,Name,ZnodeCategoryIds,IsActive,Attributes,Brands
,CategoryName,CatalogName,DisplayOrder,RevisionType,AssociatedProductDisplayOrder,ProductIndex,SalesPrice,RetailPrice,CultureCode,CurrencySuffix
,CurrencyCode,SeoDescription,SeoKeywords,SeoTitle,SeoUrl,ImageSmallPath,SKULower,ElasticSearchEvent,ZnodeParentCategoryIds,ModifiedDate,IsSingleProductPublish
,IsCacheClear)
SELECT VersionId,IndexId,ZnodeProductId,ZnodeCatalogId,SKU ,LocaleId,Name,ZnodeCategoryIds,IsActive,Attributes,Brands
,CategoryName,CatalogName,DisplayOrder,RevisionType,AssociatedProductDisplayOrder,ProductIndex,SalesPrice,RetailPrice,CultureCode,CurrencySuffix
,CurrencyCode,SeoDescription,SeoKeywords,SeoTitle,SeoUrl,ImageSmallPath,SKULower,ElasticSearchEvent,ZnodeParentCategoryIds,ModifiedDate,IsSingleProductPublish
,IsCacheClear
FROM #insertedPublishEntity
WHERE PublishProductEntityId = 0 



--UPDATE a 
--SET a.ItemName					=b.ItemName,a.CMSSEODetailId				=b.CMSSEODetailId		,a.CMSSEODetailLocaleId			=b.CMSSEODetailLocaleId	
--,a.CMSSEOTypeId					=b.CMSSEOTypeId	,a.SEOId						=b.SEOId,a.SEOTypeName					=b.SEOTypeName			
--,a.SEOTitle						=b.SEOTitle,a.SEODescription				=b.SEODescription	,a.SEOKeywords					=b.SEOKeywords			
--,a.SEOUrl						=b.SEOUrl	,a.IsRedirect					=b.IsRedirect	,a.MetaInformation				=b.MetaInformation		
--,a.LocaleId						=b.LocaleId	,a.OldSEOURL					=b.OldSEOURL	,a.CMSContentPagesId			=b.CMSContentPagesId	
--,a.PortalId						=b.PortalId	,a.SEOCode						=b.SEOCode	,a.CanonicalURL					=b.CanonicalURL			
--,a.RobotTag						=b.RobotTag	,a.ElasticSearchEvent			=b.ElasticSearchEvent	
DELETE a 
FROM  ZnodePublishSeoEntity a 
INNER JOIN #Seo_entity b ON (b.PublishSeoEntityId = a.PublishSeoEntityId)

INSERT INTO ZnodePublishSeoEntity (VersionId,PublishStartTime,ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,SEOTypeName
,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL,CMSContentPagesId,PortalId,SEOCode,CanonicalURL
,RobotTag,ElasticSearchEvent)
SELECT @VersionId_l,PublishStartTime,ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,SEOTypeName
,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL,CMSContentPagesId,PortalId,SEOCode,CanonicalURL
,RobotTag,ElasticSearchEvent
FROM #Seo_entity a 
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishSeoEntity r with(nolock) WHERE r.PublishSeoEntityId = a.PublishSeoEntityId )
AND a.LocaleId = @LocaleId_l

FETCH NEXT FROM cur_localeId INTO @VersionId_l,@LocaleId_l,@RevisionState_l


 END 

 CLOSE cur_localeId
 DEALLOCATE cur_localeId

 DROP TABLE IF EXISTS #TBL_finalProducts
 DROP TABLE IF EXISTS #filterSEOCOde
 
DECLARE @pimProductId_oi transferId 
DECLARE @Version_str VARCHAr(2000) 

SELECT @Version_str= STRING_AGG( CONVERT(VARCHAR(200), q.VersionId) ,',')
FROM @Versions_working q

INSERT INTO @pimProductId_oi 
SELECT DISTINCT  pimProductId 
FROM #PimProduct_distinct a
WHERE a.PublishStateId IN (1,2)
UNION  
SELECT ZnodeProductId
FROM #insertedPublishEntity
WHERE PublishProductEntityId = 0  


 DROP TABLE IF EXISTS  #insertedPublishEntity

 IF  EXISTS ( SELECT TOP 1 1 FROM @pimProductId_oi )
 BEGIN 

EXEC Znode_PublishUpdateProductJson @PimProductIds=@pimProductId_oi
,@VersionId = @Version_str
,@LocaleId = 1 
,@IsSingleProductPublish = 0 ,@NewGUID=@NewGUID, @CatalogName = @CatalogName,@messagestring=@messagestring

END 

	UPDATE ZnodePublishProgressNotifierEntity
	SET JobName =   @messagestring+'Catalog-'+@CatalogName+'-'+'Publishing Catalog Attribute'
		,ProgressMark = 60
	WHERE Jobid = @NewGUID


SELECT VersionId,@PimCatalogId PimCatalogId,AttributeCode,AttributeTypeName,0 IsPromoRuleCondition
,ISNULL(IsComparable,0)IsComparable,ISNULL(IsHtmlTags,0)IsHtmlTags,ISNULL(IsFacets,0)IsFacets,ISNULL(IsUseInSearch,0)IsUseInSearch,IsPersonalizable,IsConfigurable,AttributeName,a.LocaleId,DisplayOrder
	,ISNULL((
	SELECT  p.AttributeDefaultValue Value, m.DisplayOrder 
	FROM ZnodePimAttributeDefaultValue m
	INNER JOIN ZnodePimAttributeDefaultValueLocale p ON (p.PimAttributeDefaultValueId = m.PimAttributeDefaultValueId)
	WHERE m.PimAttributeId = a.PimAttributeId AND p.LocaleId = a.LocaleId
	FOR JSON PATH 
	),'[]')SelectValues
INTO #temp_updatecatalogAttribute 
FROM #temp_attributename a 
INNER JOIN @Versions_working h ON (h.LocaleId = a.LocaleId) 

UPDATE a 
SET AttributeTypeName=b.AttributeTypeName,IsPromoRuleCondition
=b.IsPromoRuleCondition,IsComparable=b.IsComparable,IsHtmlTags=b.IsHtmlTags,IsFacets=b.IsFacets
,IsUseInSearch=b.IsUseInSearch,IsPersonalizable=b.IsPersonalizable,IsConfigurable=b.IsConfigurable,AttributeName=b.AttributeName
,LocaleId=b.LocaleId,DisplayOrder=b.DisplayOrder,SelectValues=b.SelectValues
FROM ZnodePublishCatalogAttributeEntity a 
INNER JOIN #temp_updatecatalogAttribute b ON (b.VersionId = a.VersionId AND b.AttributeCode = a.AttributeCode)


INSERT INTO ZnodePublishCatalogAttributeEntity (VersionId,ZnodeCatalogId,AttributeCode,AttributeTypeName,IsPromoRuleCondition
,IsComparable,IsHtmlTags,IsFacets,IsUseInSearch,IsPersonalizable,IsConfigurable,AttributeName,LocaleId,DisplayOrder,SelectValues)
SELECT VersionId,PimCatalogId,AttributeCode,AttributeTypeName,IsPromoRuleCondition
,IsComparable,IsHtmlTags,IsFacets,IsUseInSearch,IsPersonalizable,IsConfigurable,AttributeName,LocaleId,DisplayOrder,SelectValues
FROM #temp_updatecatalogAttribute a
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishCatalogAttributeEntity r WHERE r.AttributeCode = a.AttributeCode AND r.VersionId = a.VersionId)



	UPDATE ZnodePublishProgressNotifierEntity
	SET JobName =   @messagestring+'Catalog-'+@CatalogName+'-'+'Publishing SEO data'
		,ProgressMark = 65
	WHERE Jobid = @NewGUID




SELECT q.VersionId,a.PimProductId, @PimCatalogId ZnodeCatalogId, b.PimProductId AssociatedZnodeProductId , b.DisplayOrder,'' SelectValues
, ISNULL((SELECT CONCAT('["',STRING_AGG([AttributeCode], '","'),'"]') FROM #temp_attributename n 
INNER JOIN ZnodePimConfigureProductAttribute g WITH (nolock) ON (g.PimProductId = a.PimProductId )
WHERE n.PimAttributeId = g.PimAttributeId AND n.LocaleId = q.LocaleId),'[]') ConfigurableAttributeCodes
, IsDefault,1 ElasticSearchEvent
INTO #temp_configProductids 
FROM ZnodePimConfigureProductAttribute a  WITH (nolock)
INNER JOIN #PimProduct_catalog b ON (b.ParentPimProductId = a.PimProductId AND b.IsAssocitedProduct =1 )
CROSS APPLY @Versions_working q 
GROUP BY q.VersionId,a.PimProductId, b.PimProductId  , b.DisplayOrder, IsDefault,q.LocaleId

DELETE a FROM ZnodePublishConfigurableProductEntity a 
WHERE EXISTS (SELECT TOP 1 1 FROM #temp_configProductids t WHERE t.PimProductId = a.ZnodeProductId AND t.VersionId = a.VersionId
AND t.AssociatedZnodeProductId <> a.AssociatedZnodeProductId
)
  
 INSERT INTO ZnodePublishConfigurableProductEntity (VersionId,ZnodeProductId,ZnodeCatalogId,AssociatedZnodeProductId,AssociatedProductDisplayOrder,SelectValues,ConfigurableAttributeCodes,IsDefault,ElasticSearchEvent)
 SELECT VersionId,PimProductId,ZnodeCatalogId,AssociatedZnodeProductId,DisplayOrder,SelectValues,ConfigurableAttributeCodes,IsDefault,ElasticSearchEvent
 FROM #temp_configProductids a
 WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishConfigurableProductEntity t WHERE t.VersionId = a.VersionId AND a.PimProductId = t.ZnodeProductId AND t.AssociatedZnodeProductId = a.AssociatedZnodeProductId )

SELECT q.VersionId, ParentPimProductId,@PimCatalogId ZnodeCatalogId, a.PimProductId, a.DisplayOrder, a.BundleQuantity ,1 ElasticSearchEvent
INTO #temp_bundleProduct 
FROM #PimProduct_catalog a 
CROSS APPLY @Versions_working q 
WHERE EXISTS (SELECT TOP 1 1 FROM #PimProduct_distinct tt WHERE tt.PimProductId = a.ParentPimProductId AND tt.ProductType = 'BundleProduct')
AND a.IsAssocitedProduct =1 

DELETE a FROM ZnodePublishBundleProductEntity a 
WHERE EXISTS (SELECT TOP 1 1 FROM #temp_bundleProduct t WHERE t.ParentPimProductId = a.ZnodeProductId AND t.VersionId = a.VersionId
AND  t.PimProductId <> a.ZnodeProductId   
)


UPDATE a
SET  a.AssociatedProductBundleQuantity = t.BundleQuantity, a.AssociatedProductDisplayOrder = t.DisplayOrder
FROM ZnodePublishBundleProductEntity a
INNER JOIN  #temp_bundleProduct t ON( t.ParentPimProductId = a.ZnodeProductId AND t.VersionId = a.VersionId
AND  t.PimProductId = a.ZnodeProductId   ) 

INSERT INTO ZnodePublishBundleProductEntity (VersionId,ZnodeProductId,ZnodeCatalogId,AssociatedZnodeProductId,AssociatedProductDisplayOrder,AssociatedProductBundleQuantity,ElasticSearchEvent)
SELECT VersionId, ParentPimProductId,@PimCatalogId ZnodeCatalogId, PimProductId, DisplayOrder, ISNULL(BundleQuantity,0) BundleQuantity , ElasticSearchEvent
FROM #temp_bundleProduct a
WHERE  NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishBundleProductEntity t WHERE t.VersionId = a.VersionId AND t.ZnodeProductId = a.ParentPimProductId AND t.AssociatedZnodeProductId = a.PimProductId)


SELECT q.VersionId, ParentPimProductId,@PimCatalogId ZnodeCatalogId, PimProductId, a.DisplayOrder ,1 ElasticSearchEvent
INTO #temp_groupProduct
FROM #PimProduct_catalog a 
CROSS APPLY @Versions_working q 
WHERE EXISTS (SELECT TOP 1 1 FROM #PimProduct_distinct tt WHERE tt.PimProductId = a.ParentPimProductId AND tt.ProductType = 'GroupedProduct')
AND a.IsAssocitedProduct =1 

DELETE a FROM ZnodePublishGroupProductEntity a 
WHERE EXISTS (SELECT TOP 1 1 FROM #temp_groupProduct t WHERE t.ParentPimProductId = a.ZnodeProductId AND t.VersionId = a.VersionId
AND t.PimProductId <> a.ZnodeProductId
)

INSERT INTO ZnodePublishGroupProductEntity (VersionId,ZnodeProductId,ZnodeCatalogId,AssociatedZnodeProductId,AssociatedProductDisplayOrder,ElasticSearchEvent)
SELECT VersionId, ParentPimProductId,ZnodeCatalogId, PimProductId, DisplayOrder ,ElasticSearchEvent
FROM #temp_groupProduct a
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishGroupProductEntity t WHERE t.VersionId = a.VersionId AND t.ZnodeProductId = a.ParentPimProductId AND t.AssociatedZnodeProductId = a.PimProductId)
 	
		--For full catalog publish
	IF @PimProductId =  0   
	BEGIN
		Delete from ZnodePublishAddonEntity 
		WHERE EXISTS (SELECT TOP 1 1 FROM @Versions_working q  
		where ZnodePublishAddonEntity.VersionId = q.VersionId)
	END 

	-- for single product publish
	ELSE 
	BEGIN 
		DELETE FROM ZnodePublishAddonEntity 
		WHERE EXISTS (	SELECT TOP 1 1 FROM   @Versions_working q  
						WHERE ZnodePublishAddonEntity.VersionId = q.VersionId)
			AND @PimProductId = ZnodePublishAddonEntity.ZnodeProductId   
	END 

INSERT INTO ZnodePublishAddonEntity (VersionId,ZnodeProductId,ZnodeCatalogId,AssociatedZnodeProductId,AssociatedProductDisplayOrder
,LocaleId,GroupName,DisplayType,DisplayOrder,IsRequired,RequiredType,IsDefault,ElasticSearchEvent)
SELECT VersionId,ParentPimProductId,@PimCatalogId,PimProductId,DisplayOrder
,q.LocaleId,[AddonGroupName] GroupName, DisplayType,DisplayOrder,IIF(RequiredType='requierd',1,0) IsRequired, RequiredType,IsDefault,1 ElasticSearchEvent
FROM  #TBL_AddOnProduct a
CROSS APPLY @Versions_working q 
Inner join ZnodePimAddonGroup AS ZPADG with(nolock) on ( ZPADG.PimAddonGroupId = a.PimAddonGroupId ) 
Inner join ZnodePimAddonGroupLocale AS ZPADGL  with(nolock) on ( ZPADG.PimAddonGroupId = ZPADGL.PimAddonGroupId  and ZPADGL.LocaleId = @DefaultLocaleId ) 
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishAddonEntity ty WHERE ty.VersionId = q.VersionId AND ty.ZnodeProductId = a.ParentPimProductId
AND ty.AssociatedZnodeProductId = a.PimProductId  )
AND a.PimAddonGroupId > 0 

		
	UPDATE ZnodePublishProgressNotifierEntity
	SET JobName =   @messagestring+'Catalog-'+@CatalogName+'-'+'Started Elastic update'
		,ProgressMark = 80
	WHERE Jobid = @NewGUID
	
	SET @Status = 1  
  
    UPDATE a 
	SET --PublishSTateId = c.PublishStateId, IsCatalogPublished = 1 	    ,
	PublishCategoryId = (SELECT COUNT(DISTINCT PimCategoryHierarchyId )  FROM #Temp_Categoryvalue n )
		, PublishProductId = CASE WHEN @PimProductId <> 0 THEN @PimProductId ELSE  (SELECT COUNT(DISTINCT PimProductId )  FROM #PimProduct_distinct np  ) END 
	FROM ZnodePublishCatalogLog a 
	INNER JOIN @Versions_new b ON (b.VersionId = a.PublishCatalogLogId)
	INNER JOIN ZnodePublishState c with(nolock) ON (c.PublishStateCode = b.RevisionState)
    
	UPDATE ZnodePublishVersionEntity SET IsPublishSuccess = 1 WHERE ZnodeCatalogId = @PimCatalogId

	UPDATE ZnodePimProduct 
	SET PublishStateId =  CASE WHEN EXISTS (SELECT TOP 1 1 FROM @Versions_working WHERE PublishStateId = 3 ) THEN 3 ELSE 4 END
	WHERE PublishStateId IN (1,2)
	AND PimProductId IN ( SELECT PimProductId FROM  #PimProduct_distinct ) 
	AND @PimProductId = 0 

	UPDATE ZnodePimCategory 
	SET PublishStateId = CASE WHEN EXISTS (SELECT TOP 1 1 FROM @Versions_working WHERE PublishStateId = 3 ) THEN 3 ELSE 4 END 
	WHERE PublishStateId IN (1,2)
	AND PimCategoryId IN ( SELECT t.PimCategoryId FROM  #Temp_categoryDetails t  ) 


	IF @PimProductId = 0 
	BEGIN 
		SELECT @PimCatalogId AS id,@Status AS Status;     
    END 
	ELSE 
	BEGIN 
	
	  SELECT PimProductId , 0 IsDeleted , @PimCatalogId ZnodecatalogId , n.VersionId
	  FROM #PimProduct_distinct a 
	  CROSS APPLY @Versions_working n 
	  UNION ALL 
	  SELECT PimProductId , 1 IsDeleted , @PimCatalogId , n.VersionId
	  FROM @Deleted_Products 
	  CROSS APPLY @Versions_working n 
	END 

END TRY   
BEGIN CATCH
	SET @Status =0    
	SELECT ERROR_MESSAGE(), ERROR_LINE(),ERROR_PROCEDURE()
	SELECT 1 AS ID,@Status AS Status;     
   
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),   
	@ErrorLine VARCHAR(100)= ERROR_LINE(),  
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PublishCatalogEntity   
	@PimCatalogId = '+CAST(@PimCatalogId  AS VARCHAR (max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))  
	+',@PreviewVersionId = ' + CAST(0  AS VARCHAR(20))  
	+',@ProductionVersionId = ' + CAST(0  AS VARCHAR(20))  
	+',@RevisionState = ''' + CAST(@RevisionState  AS VARCHAR(50))  
	+',@UserId = ' + CAST(@UserId AS VARCHAR(20)); SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
   
    	
	EXEC Znode_InsertProcedureErrorLog  
	@ProcedureName = 'Znode_PublishCatalogEntity',  
	@ErrorInProcedure = @Error_procedure,  
	@ErrorMessage = @ErrorMessage,  
	@ErrorLine = @ErrorLine,  
	@ErrorCall = @ErrorCall;  
END CATCH  
END

