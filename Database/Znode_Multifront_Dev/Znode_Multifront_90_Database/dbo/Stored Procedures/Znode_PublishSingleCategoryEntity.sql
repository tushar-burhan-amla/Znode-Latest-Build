CREATE PROCEDURE [dbo].[Znode_PublishSingleCategoryEntity]
(
    @PimCategoryId  int 
   ,@RevisionType   Varchar(30) = ''
   ,@UserId int = 0
   ,@IsAssociate int  = 0 OUT
   ,@PimCatalogId int = 0
)
AS
/*
	To publish single category 
	Unit Testing : 
	*/
BEGIN
BEGIN TRY 
SET NOCOUNT ON
	DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
	Declare @Status Bit =0 
	Declare @Type varchar(50) = '',	@CMSSEOCode varchar(300),@DefaultLocaleId int = dbo.fn_getdefaultLocaleId()
	SET @Status = 1 
	Declare @IsPreviewEnable int 
	DECLARE @Tbl_versions  TABLE (RevisionState VARCHAr(300))

	INSERT INTO @Tbl_versions 
	SELECT 'PREVIEW'
	WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePublishStateApplicationTypeMapping with(nolock) WHERE ApplicationType = 'WebstorePreview' AND IsActive =1 AND IsEnabled = 1 )
	UNION ALL 
	SELECT 'PRODUCTION'
	WHERE @RevisionType = 'Production' OR  @RevisionType = 'None'

		
    SELECT AttributeCode , AttributeName , AttributeTypeName ,ISNULL( IsUseInSearch,0) IsUseInSearch,ISNULL(IsHtmlTags,0)IsHtmlTags
,ISNULL(IsComparable,0)IsComparable, a.PimAttributeId ,ISNULL(d.IsFacets,0) IsFacets
, ISNULL(a.IsConfigurable,0)IsConfigurable, ISNULL( a.IsPersonalizable, 0)IsPersonalizable
		, a.DisplayOrder,IsCategory, b.LocaleId
INTO #temp_attributename 
FROM ZnodePimAttribute a with(nolock)
INNER JOIN ZnodePimAttributeLocale b with(nolock) ON (b.PimAttributeId = a.PimAttributeId AND b.LocaleId =@DefaultLocaleId)
INNER JOIN ZnodeAttributeType c with(nolock) ON (c.AttributeTypeId = a.AttributeTypeId)
LEFT JOIN ZnodePimFrontendProperties d with(nolock) ON (d.PimAttributeId = a.PimAttributeId ) 
WHERE  a.IsCategory =1



SELECT DISTINCT yu.VersionId,yu.LocaleId, yu.CatalogName, yu.ZnodeCatalogId, a.PimCategoryId ,RT.ParentPimCategoryHierarchyId, rt.DisplayOrder, rt.PimCategoryHierarchyId
, c.AttributeCode,b.CategoryValue AttributeValues,rt.ActivationDate, rt.ExpirationDate
		,RT.IsActive, a.PimCategoryAttributeValueId
INTO #Temp_Categoryvalue
FROM ZnodePimCategoryAttributeValue a with(nolock)
INNER JOIN ZnodePimCategoryAttributeValueLocale b with(nolock) ON (b.PimCategoryAttributeValueId = a.PimCategoryAttributeValueId AND b.LocaleId = @DefaultLocaleId)
INNER JOIN #temp_attributename c ON (c.PimAttributeId = a.PimAttributeId AND c.IsCategory =1 )
INNER JOIN  ZnodePimCategoryHierarchy  rt with(nolock) ON ( rt.PimCategoryId = a.PimCategoryId  )
INNER JOIN  ZnodePublishCatalogEntity yu with(nolock) ON (yu.ZnodeCatalogId = rt.PimCatalogId)
WHERE rt.PimCategoryId =@PimCategoryId
AND yu.RevisionType IN (SELECT t.RevisionState FROM @Tbl_versions t)



IF EXISTS (SELECT TOP 1 1 FROM #Temp_Categoryvalue r WHERE r.LocaleId <> @DefaultLocaleId)
BEGIN 
    UPDATE a 
	SET AttributeValues = b.CategoryValue
	FROM #Temp_Categoryvalue a 
	INNER JOIN ZnodePimCategoryAttributeValueLocale b ON (b.PimCategoryAttributeValueId = a.PimCategoryAttributeValueId AND b.LocaleId = a.LocaleId)
	WHERE a.LocaleId <> @DefaultLocaleId

	INSERT INTO #temp_attributename
	SELECT AttributeCode , IIF(b.AttributeName IS NULL, a.AttributeName ,b.AttributeName) , AttributeTypeName ,ISNULL( IsUseInSearch,0) IsUseInSearch,ISNULL(IsHtmlTags,0)IsHtmlTags
		,ISNULL(IsComparable,0)IsComparable, a.PimAttributeId ,ISNULL(a.IsFacets,0) IsFacets
		, ISNULL(a.IsConfigurable,0)IsConfigurable, ISNULL( a.IsPersonalizable, 0)IsPersonalizable
		, a.DisplayOrder,IsCategory, t.LocaleId
	FROM #temp_attributename a
	CROSS APPLY ZnodePublishCatalogEntity t 
	LEFT JOIN ZnodePimAttributeLocale b ON (b.PimAttributeId = a.PimAttributeId AND b.LocaleId = t.LocaleId)
	WHERE t.LocaleId <> @DefaultLocaleId

END 

UPDATE a SET a.AttributeValues = ZM.Path
FROM #Temp_Categoryvalue a
INNER  JOIN ZnodeMedia ZM ON (CAST(zm.MediaId AS VARCHAR(200)) = a.AttributeValues)
WHERE a.AttributeCode IN (SELECT n.AttributeCode FROM #temp_attributename n WHERE n.AttributeTypeName = 'Image' )


SELECT DISTINCT PimCategoryHierarchyId,LocaleId,ZnodeCatalogId, CatalogName,PimCategoryId
,ParentPimCategoryHierarchyId,DisplayOrder,VersionId,CAST('' AS NVARCHAr(300)) CategoryName,CAST('' AS NVARCHAr(100)) CategoryCode ,ActivationDate, ExpirationDate,IsActive
INTO #Temp_categoryDetails
FROM #Temp_Categoryvalue a 



UPDATE #Temp_categoryDetails
SET CategoryName = (SELECT TOP 1 ty.AttributeValues FROM #Temp_Categoryvalue ty 
WHERE ty.PimCategoryHierarchyId =#Temp_categoryDetails.PimCategoryHierarchyId AND ty.LocaleId = #Temp_categoryDetails.LocaleId
AND ty.AttributeCode ='CategoryName')
,CategoryCode = (SELECT TOP 1 ty.AttributeValues FROM #Temp_Categoryvalue ty 
WHERE ty.PimCategoryHierarchyId =#Temp_categoryDetails.PimCategoryHierarchyId AND ty.LocaleId = #Temp_categoryDetails.LocaleId
AND ty.AttributeCode ='CategoryCode')

-- SELECT * FROM #Temp_categoryDetails

UPDATE a
SET a.Name = t.CategoryName
,a.CategoryCode = t.CategoryCode
,a.Attributes = (SELECT   r.AttributeCode, AttributeValues,jk.AttributeName, jk.AttributeTypeName  ,jk.IsUseInSearch,jk.IsHtmlTags,jk.IsComparable
					FROM #Temp_Categoryvalue r 
					INNER JOIN #temp_attributename jk ON (jk.AttributeCode = r.AttributeCode AND jk.LocaleId = r.LocaleId AND jk.IsCategory = 1 )
					WHERE r.PimCategoryHierarchyId = t.PimCategoryHierarchyId AND r.LocaleId = t.LocaleId
					FOR JSON PATH ) 

,a.ZnodeParentCategoryIds  = '['+CAST(t.ParentPimCategoryHierarchyId AS VARCHAR(580))+']'
,a.ProductIds = '['+(SELECT  STRING_AGG(  CONVERT(VARCHAR(MAX),PimProductId) ,',') FROM ZnodePimCategoryProduct ty with(nolock) WHERE ty.PimCategoryId = t.PimCategoryId  )+']' 
,a.DisplayOrder = t.DisplayOrder 
,a.IsActive = t.IsActive, a.ActivationDate = t.ActivationDate, a.ExpirationDate = t.ExpirationDate

FROM ZnodePublishCategoryEntity a with(nolock)
INNER JOIN #Temp_categoryDetails t ON (t.PimCategoryHierarchyId = a.ZnodeCategoryId AND t.VersionId = a.VersionId)

INSERT INTO ZnodePublishCategoryEntity (VersionId,ZnodeCategoryId,Name,CategoryCode,ZnodeCatalogId,CatalogName,ZnodeParentCategoryIds
,ProductIds,LocaleId,IsActive,DisplayOrder,Attributes,ActivationDate,ExpirationDate,CategoryIndex,ElasticSearchEvent)

SELECT a.VersionId, a.PimCategoryHierarchyId, a.CategoryName, a.CategoryCode,@PimCatalogId,CatalogName,'['+CAST(a.ParentPimCategoryHierarchyId AS VARCHAR(580))+']'
	,'['+(SELECT  STRING_AGG(  CONVERT(VARCHAR(MAX),PimProductId) ,',') FROM ZnodePimCategoryProduct ty with(nolock) WHERE ty.PimCategoryId  = a.PimCategoryId  )+']' 
	,a.LocaleId, a.IsActive,a.DisplayOrder,(SELECT  r.AttributeCode , AttributeName, AttributeValues, AttributeTypeName,IsUseInSearch,IsHtmlTags,IsComparable
					FROM #Temp_Categoryvalue r 
					INNER JOIN #temp_attributename jk ON (jk.AttributeCode = r.AttributeCode AND jk.LocaleId= r.LocaleId AND jk.IsCategory =1 )
					WHERE r.PimCategoryHierarchyId = a.PimCategoryHierarchyId AND r.LocaleId = a.LocaleId	FOR JSON PATH ) , a.ActivationDate, a.ExpirationDate,1,1
FROM #Temp_categoryDetails a
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishCategoryEntity Tot with(nolock) WHERE tot.VersionId = a.VersionId AND tot.ZnodeCategoryId = a.PimCategoryHierarchyId )

DELETE t FROM ZnodePublishSeoEntity t WHERE EXISTS (SELECT TOP 1 1 FROM #Temp_categoryDetails yu WHERE yu.CategoryCode  = t.SEOCode 
AND t.CMSSEOTypeId = 2 
)

INSERT INTO ZnodePublishSeoEntity(VersionId,PublishStartTime,ItemName,CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,SEOTypeName,SEOTitle
,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,LocaleId,OldSEOURL,CMSContentPagesId,PortalId,SEOCode,CanonicalURL,RobotTag,ElasticSearchEvent) 

SELECT VersionId,@GetDate,a.SEOCode,a.CMSSEODetailId,CMSSEODetailLocaleId,CMSSEOTypeId,SEOId,'Category' SEOTypeName,SEOTitle
,SEODescription,SEOKeywords,SEOUrl,IsRedirect,MetaInformation,b.LocaleId,'' OldSEOURL,0 CMSContentPagesId,PortalId,SEOCode,CanonicalURL,RobotTag,1 ElasticSearchEvent
FROM ZnodeCMSSEODetail a 
INNER JOIN ZnodeCMSSEODetailLocale b ON (b.CMSSEODetailId = a.CMSSEODetailId)
INNER JOIN #Temp_categoryDetails c ON (c.CategoryCode = a.SEOCode )
WHERE a.CMSSEOTypeId =2 

     
	SELECT PimCategoryHierarchyId PublishCategoryId ,
			ZnodeCatalogId  PublishCatalogId  ,
           ''   CategoryXml       ,
              LocaleId          ,
			  VersionId		     
    FROM  #Temp_categoryDetails y 

	SELECT PortalId, y.ZnodeCatalogId PublishCatalogId, y.PimCategoryHierarchyId PublishCategoryId, y.CategoryCode, yu.SEOUrl,y. VersionId,y.LocaleId FROM ZnodePublishSeoEntity yu 
	INNER JOIN #Temp_categoryDetails y ON ( y.VersionId = yu.VersionId
	AND y.CategoryCode = yu.SEOCode)  
	
	SELECT 0 AS id,@Status AS Status;   


	UPDATE ZnodePimCategory 
	SET PublishStateId =  CASE WHEN @RevisionType= 'None' OR @RevisionType= 'Production'  THEN 3 ELSE 4 END 
	WHERE PimCategoryId =@PimCategoryId

END TRY 
BEGIN CATCH 
	SET @Status =0  
	
	 SELECT 1 AS ID,@Status AS Status;   
	 SELECT ERROR_MESSAGE()
	-- Rollback transaction
	 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PublishSingleCategoryEntity
		@PimCategoryId = '+CAST(@PimCategoryId  AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@RevisionType='+CAST(@RevisionType AS VARCHAR(10))
			
	INSERT INTO ZnodePublishSingleCategoryErrorLogEntity
	(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
	SELECT 'PublishSingleCategoryEntity', '' + isnull(@ErrorMessage,'') , 'Fail' , @GetDate, 
	@UserId ,''


	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_PublishSingleCategoryEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END