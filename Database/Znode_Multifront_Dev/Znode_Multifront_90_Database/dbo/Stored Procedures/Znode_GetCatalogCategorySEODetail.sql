CREATE PROCEDURE [dbo].[Znode_GetCatalogCategorySEODetail]
(
	  @WhereClause      VARCHAR(MAX),
	  @Rows             INT           = 100,
	  @PageNo           INT           = 1,
	  @Order_BY         VARCHAR(1000) = '',
	  @RowsCount        INT OUT,
	  @LocaleId         INT           = 1,
	  @PortalId         INT
)
AS
BEGIN
	SET NOCOUNT ON;

	Declare @PimCatalogId INT, @SQL VARCHAR(MAX), @DefaultLocaleId VARCHAR(20)= dbo.Fn_GetDefaultLocaleId()

	SELECT @PimCatalogId = ZPC.PublishCatalogId 
	FROM ZnodePortalCatalog ZPC WHERE PortalId = @PortalId


	IF OBJECT_ID('TEMPDB..#CategoryDetail') IS NOT NULL
		DROP TABLE #CategoryDetail

	IF OBJECT_ID('TEMPDB..##TempCategoryDetail') IS NOT NULL
		DROP TABLE ##TempCategoryDetail

		IF OBJECT_ID('TEMPDB..#znodeCatalogCategory') IS NOT NULL
		DROP TABLE #znodeCatalogCategory

	SELECT PimCategoryId, CategoryName, CategoryCode, LocaleId
	INTO #CategoryDetail
	FROM
	(
		SELECT ZPCAV.PimCategoryId,ZPA.AttributeCode,ZPCAVL.CategoryValue, ZPCAVL.LocaleId 
		FROM ZnodePimCategoryAttributeValue ZPCAV
		INNER JOIN ZnodePimCategoryAttributeValueLocale ZPCAVL on ZPCAV.PimCategoryAttributeValueId = ZPCAVL.PimCategoryAttributeValueId
		INNER JOIN ZnodePimAttribute ZPA on ZPCAV.PimAttributeId = ZPA.PimAttributeId
		where ZPA.AttributeCode in ( 'CategoryName', 'CategoryCode')
	)TB PIVOT(MAX(CategoryValue) FOR AttributeCode in ( CategoryName, CategoryCode))AS PVT
	
	

	SET @SQL = '

	select distinct ZPCH.PimCatalogId,ZPCP.PimCategoryId into #znodeCatalogCategory
	FROm ZnodePimCategoryProduct ZPCP
	Inner Join ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId AND ZPCH.PimCatalogId = '+CAST(@PimCatalogId AS VARCHAR(10))+'

	;With CTE_CategoryDetail AS
	(
		SELECT DISTINCT PC.PimCatalogId, PC.CatalogName, CD.PimCategoryId, CD.CategoryCode, CD.CategoryName , ISNULL(CSD.SEOCode,CD.CategoryCode) as SEOCode , CSD.SEOUrl, CSDL.SEOTitle, CSDL.SEODescription, CSDL.SEOKeywords,
		case when  ZPPO.DisplayName IS NULL THEN '''' ELSE ZPPO.DisplayName END IsPublish , ActivationDate , ExpirationDate, 
		CH.IsActive, CD.LocaleId, CSDL.CanonicalURL, CSDL.RobotTag
		FROM #CategoryDetail CD
		INNER JOIN ZnodePimCategory ZPC ON (ZPC.PimCategoryId = CD.PimCategoryId)
		INNER JOIN  #znodeCatalogCategory PCC on CD.PimCategoryId = PCC.PimCategoryId
		INNER JOIN ZnodePimCatalog PC on PCC.PimCatalogId = PC.PimCatalogId
		LEFT JOIN ZnodePimCategoryHierarchy CH ON (CH.PimCategoryId = CD.PimCategoryId)
		LEFT JOIN ZnodeCMSSEOType CST ON CST.Name = ''Category''
		LEFT JOIN ZnodeCMSSEODetail CSD on CD.CategoryCode = CSD.SEOCode and CSD.CMSSEOTypeId = CST.CMSSEOTypeId AND CSD.PortalId = '+CAST(@PortalId AS VARCHAR(10))+'
		LEFT JOIN ZnodeCMSSEODetailLocale CSDL ON  CSD.CMSSEODetailId = CSDL.CMSSEODetailId AND CSDL.LocaleId =  '+CAST(@LocaleId AS VARCHAR(10))+'
		LEFT JOIN ZnodePublishState ZPPO ON (ZPPO.PublishStateId = CSD.PublishStateId)
		WHERE PCC.PimCatalogId = '+CAST(@PimCatalogId AS VARCHAR(10))+' AND CD.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+', '+CAST(@DefaultLocaleId AS VARCHAR(50))+') 
			AND PC.PortalId = '+CAST(@PortalId AS VARCHAR(10))+'
	)
	,CTE_CategoryDetail_SEO AS
	(
		SELECT DISTINCT null as PimCatalogId, '''' CatalogName, CD.PimCategoryId, CD.CategoryCode, CD.CategoryName , ISNULL(CSD.SEOCode,CD.CategoryCode) as SEOCode , CSD.SEOUrl, CSDL.SEOTitle, CSDL.SEODescription, CSDL.SEOKeywords,
		case when  ZPPO.DisplayName IS NULL THEN '''' ELSE ZPPO.DisplayName END IsPublish , ActivationDate , ExpirationDate, 
		CH.IsActive, CD.LocaleId, CSDL.CanonicalURL, CSDL.RobotTag
		FROM #CategoryDetail CD
		INNER JOIN ZnodePimCategory ZPC ON (ZPC.PimCategoryId = CD.PimCategoryId)
		LEFT JOIN ZnodePimCategoryHierarchy CH ON (CH.PimCategoryId = CD.PimCategoryId)
		LEFT JOIN ZnodeCMSSEOType CST ON CST.Name = ''Category''
		LEFT JOIN ZnodeCMSSEODetail CSD on CD.CategoryCode = CSD.SEOCode and CSD.CMSSEOTypeId = CST.CMSSEOTypeId AND CSD.PortalId = '+CAST(@PortalId AS VARCHAR(10))+'
		LEFT JOIN ZnodeCMSSEODetailLocale CSDL ON  CSD.CMSSEODetailId = CSDL.CMSSEODetailId AND CSDL.LocaleId =  '+CAST(@LocaleId AS VARCHAR(10))+'
		LEFT JOIN ZnodePublishState ZPPO ON (ZPPO.PublishStateId = CSD.PublishStateId)
		WHERE CD.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+', '+CAST(@DefaultLocaleId AS VARCHAR(50))+') AND CH.PimCatalogId = '+CAST(@PimCatalogId AS VARCHAR(10))+'
		AND NOT EXISTS(select * from CTE_CategoryDetail CDC where CD.PimCategoryId = CDC.PimCategoryId )
	)
	,CTE_CategoryDetail_Locale as
	(
		SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive, LocaleId, CanonicalURL, RobotTag
		FROM CTE_CategoryDetail CD
		WHERE CD.LocaleId ='+CAST(@LocaleId AS VARCHAR(50))+'
		union ALL
		SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive, LocaleId, CanonicalURL, RobotTag
		FROM CTE_CategoryDetail_SEO CD
		WHERE CD.LocaleId ='+CAST(@LocaleId AS VARCHAR(50))+'
	)
	,CTE_CategoryDetail_BothLocale as
	(
		SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive, CanonicalURL, RobotTag
		FROM CTE_CategoryDetail_Locale
		Union All
		SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive, CanonicalURL, RobotTag
		FROM CTE_CategoryDetail CDS
		WHERE LocaleId ='+CAST(@DefaultLocaleId AS VARCHAR(50))+' AND
			NOT EXISTS( SELECT TOP 1 1 FROM CTE_CategoryDetail_Locale CSD1
						WHERE CDS.PimCategoryId = CSD1.PimCategoryId AND CDS.CatalogName = CSD1.CatalogName )
		Union All
		SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive, CanonicalURL, RobotTag
		FROM CTE_CategoryDetail_SEO CDS2
		WHERE LocaleId ='+CAST(@DefaultLocaleId AS VARCHAR(50))+' AND
			NOT EXISTS( SELECT TOP 1 1 FROM CTE_CategoryDetail_Locale CSD1
						WHERE CDS2.PimCategoryId = CSD1.PimCategoryId AND CDS2.CatalogName = CSD1.CatalogName )
	)
	,CTE_CategoryDetail_WhereClause AS
	(
		SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive, CanonicalURL, RobotTag, '+[dbo].[Fn_GetPagingRowId](@Order_BY, 'PimCategoryId')+',Count(*)Over() CountId
		FROM CTE_CategoryDetail_BothLocale CD
		WHERE 1 = 1 '+CASE WHEN @WhereClause = '' THEN '' ELSE ' AND '+@WhereClause END +'
	)
	SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive, CanonicalURL, RobotTag, CountId
	INTO ##TempCategoryDetail
	FROM CTE_CategoryDetail_WhereClause
	'+[dbo].[Fn_GetPaginationWhereClause](@PageNo, @Rows);
	print @SQL
	EXEC (@SQL)

	SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM ##TempCategoryDetail ),0)

	SELECT  PimCategoryId, CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish, ActivationDate,ExpirationDate,IsActive, CanonicalURL, RobotTag 
	FROM ##TempCategoryDetail

	IF OBJECT_ID('TEMPDB..#CategoryDetail') IS NOT NULL
		DROP TABLE #CategoryDetail

	IF OBJECT_ID('TEMPDB..##TempCategoryDetail') IS NOT NULL
		DROP TABLE ##TempCategoryDetail

	IF OBJECT_ID('TEMPDB..#znodeCatalogCategory') IS NOT NULL
	DROP TABLE #znodeCatalogCategory

END