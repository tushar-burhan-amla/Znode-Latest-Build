CREATE PROCEDURE [dbo].[Znode_GetPublishCategoryForSiteMap]
(
	@Rows INT,
	@PageNo INT,
	@CatalogId INT,
	@VersionId INT,
	@LocaleId INT = 1,
	@IncludeAssociatedCategories BIT =1,
	@RowsCount INT = 0 OUT
)
AS
---- Execute [Znode_GetPublishCategoryForSiteMap] @Rows = 20,@PageNo=1,@CatalogId=3,@VersionId=53,@LocaleId=1,@IncludeAssociatedCategories=1
BEGIN
BEGIN TRY 
SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	DECLARE @SQL NVARCHAR(MAX),@Rows1 BIGINT

	SET @Rows1 = @Rows * @PageNo 

	IF Object_id('tempdb..#PublishCategoryEntity') <>0
		DROP TABLE #PublishCategoryEntity

	IF Object_id('tempdb..#PublishCategoryEntityPagination') <>0
		DROP TABLE #PublishCategoryEntityPagination

	CREATE TABLE #PublishCategoryEntity
	(
		RowId INT IDENTITY(1,1),
		ZnodeCategoryId INT, 
		CategoryName VARCHAR(300),
		CategoryCode VARCHAR(100),
		ZnodeCatalogId INT,
		CatalogName VARCHAR(300),
		ZnodeParentCategoryIds VARCHAR(200),
		SEOUrl NVARCHAR(MAX)
	)

	CREATE TABLE #PublishCategoryEntityPagination
	(
		Id INT IDENTITY(1,1),
		ZnodeCategoryId INT, 
		CategoryName VARCHAR(300),
		CategoryCode VARCHAR(100),
		ZnodeCatalogId INT,
		CatalogName VARCHAR(300),
		ZnodeParentCategoryIds VARCHAR(200),
		SEOUrl NVARCHAR(MAX),
		DisplayOrder INT
	)

	
	SELECT @RowsCount = COUNT(*)
	FROM dbo.ZnodePublishCategoryEntity AS ZPCE
	WHERE ZPCE.ZnodeCatalogId = @CatalogId AND ZPCE.LocaleId = @LocaleId  
	AND ZPCE.IsActive = 1 AND ZPCE.VersionId = @VersionId 
	AND ZPCE.ZnodeParentCategoryIds IS NULL

	--Getting category entity data for site map
	SET @SQL = '	
	INSERT INTO #PublishCategoryEntity(ZnodeCategoryId ,CategoryName,CategoryCode,ZnodeCatalogId,CatalogName, ZnodeParentCategoryIds, SEOUrl)
	SELECT TOP '+CAST(@Rows1 AS VARCHAR(10))+' ZPCE.ZnodeCategoryId , ZPCE.Name as CategoryName,ZPCE.CategoryCode,ZPCE.ZnodeCatalogId,ZPCE.CatalogName, ZPCE.ZnodeParentCategoryIds, ZPSE.SEOUrl
	FROM dbo.ZnodePublishCategoryEntity AS ZPCE
	LEFT JOIN ZnodePublishSeoEntity ZPSE ON ZPCE.CategoryCode = ZPSE.SEOCode AND ItemName = ''Category''
		AND ZPCE.VersionId = ZPSE.VersionId 
	WHERE ZPCE.ZnodeCatalogId = '+CAST(@CatalogId AS VARCHAR(10))+' AND ZPCE.LocaleId = '+CAST(@LocaleId AS VARCHAR(10))+' 
	AND ZPCE.IsActive = 1 AND ZPCE.VersionId = '+CAST(@VersionId AS VARCHAR(10))+' 
	AND ZPCE.ZnodeParentCategoryIds IS NULL 
	ORDER BY ZPCE.DisplayOrder ASC'

	EXEC sp_executesql @SQL


	--Applying pagination
	SET @SQL = 'SELECT ZnodeCategoryId ,CategoryName,CategoryCode,ZnodeCatalogId,CatalogName, ZnodeParentCategoryIds, SEOUrl 
	FROM #PublishCategoryEntity '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)+' ORDER BY RowId'
	--PRINT @SQL
	INSERT INTO #PublishCategoryEntityPagination(ZnodeCategoryId ,CategoryName,CategoryCode,ZnodeCatalogId,CatalogName, ZnodeParentCategoryIds, SEOUrl)
	EXEC sp_executesql @SQL

	IF @IncludeAssociatedCategories = 1
	BEGIN
		----Getting child publish category data
		;WITH CategoryDetails AS 
		(
			SELECT  ZPCE.ZnodeCategoryId , ZPCE.Name as CategoryName,ZPCE.CategoryCode,ZPCE.ZnodeCatalogId,ZPCE.CatalogName, 
					ZPCE.ZnodeParentCategoryIds, ZPCE.VersionId, ZPCE.DisplayOrder
			FROM dbo.ZnodePublishCategoryEntity AS ZPCE
			WHERE ZPCE.ZnodeCatalogId = @CatalogId  AND ZPCE.LocaleId = @LocaleId
			AND ZPCE.IsActive = 1 AND ZPCE.VersionId = @VersionId 
			AND ZPCE.ZnodeParentCategoryIds IS NOT NULL
			AND EXISTS(SELECT * FROM #PublishCategoryEntityPagination P 
			WHERE '['+CAST(P.ZnodeCategoryId AS VARCHAR(10))+']' = ZPCE.ZnodeParentCategoryIds )
			UNION ALL
			SELECT  ZPCE1.ZnodeCategoryId , ZPCE1.Name as CategoryName,ZPCE1.CategoryCode,ZPCE1.ZnodeCatalogId,ZPCE1.CatalogName, 
					ZPCE1.ZnodeParentCategoryIds, ZPCE1.VersionId, ZPCE1.DisplayOrder
			FROM dbo.ZnodePublishCategoryEntity AS ZPCE1
			INNER JOIN CategoryDetails CD ON '['+CAST(CD.ZnodeCategoryId AS VARCHAR(10))+']' = ZPCE1.ZnodeParentCategoryIds
			WHERE ZPCE1.ZnodeCatalogId = @CatalogId  AND ZPCE1.LocaleId = @LocaleId
			AND ZPCE1.IsActive = 1 AND ZPCE1.VersionId = @VersionId 
			AND ZPCE1.ZnodeParentCategoryIds IS NOT NULL
		)
		INSERT INTO #PublishCategoryEntityPagination(ZnodeCategoryId ,CategoryName,CategoryCode,ZnodeCatalogId,CatalogName, ZnodeParentCategoryIds, SEOUrl, DisplayOrder)
		SELECT DISTINCT CD.ZnodeCategoryId,	CD.CategoryName, CD.CategoryCode, CD.ZnodeCatalogId, CD.CatalogName, CD.ZnodeParentCategoryIds, ZPSE.SEOUrl, CD.DisplayOrder
		FROM CategoryDetails CD
		LEFT JOIN ZnodePublishSeoEntity ZPSE ON CD.CategoryCode = ZPSE.SEOCode AND ZPSE.ItemName = 'Category' AND CD.VersionId = ZPSE.VersionId 
		WHERE NOT EXISTS(SELECT * FROM #PublishCategoryEntityPagination PCEP WHERE CD.ZnodeCategoryId = PCEP.ZnodeCategoryId AND ISNULL(CD.ZnodeParentCategoryIds,'') = ISNULL(PCEP.ZnodeParentCategoryIds,''))
		ORDER BY CD.DisplayOrder ASC
	END

	SELECT DISTINCT Id, ZnodeCategoryId,	CategoryName,	CategoryCode,	ZnodeCatalogId,	CatalogName,	ZnodeParentCategoryIds,	SEOUrl
	FROM #PublishCategoryEntityPagination ORDER BY Id 
	
END TRY
BEGIN CATCH
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishCategoryForSiteMap @Rows = '+CAST(@Rows AS VARCHAR(200))+',@PageNo='+CAST(@PageNo AS VARCHAR(200))
	+',@CatalogId='+CAST(@CatalogId AS VARCHAR(200))+',@VersionId='+CAST(@VersionId AS VARCHAR(200))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(200));

    EXEC Znode_InsertProcedureErrorLog
        @ProcedureName = 'Znode_GetPublishCategoryForSiteMap',
        @ErrorInProcedure = @Error_procedure,
        @ErrorMessage = @ErrorMessage,
        @ErrorLine = @ErrorLine,
        @ErrorCall = @ErrorCall;

END CATCH;
END