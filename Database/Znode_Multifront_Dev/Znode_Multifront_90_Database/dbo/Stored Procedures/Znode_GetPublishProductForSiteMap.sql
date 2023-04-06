CREATE PROCEDURE [dbo].[Znode_GetPublishProductForSiteMap]
(
	@Rows INT,
	@PageNo INT,
	@CatalogId INT,
	@VersionId INT,
	@LocaleId INT = 1,
	@RowsCount INT = 0 OUT
)
AS
---- Execute [Znode_GetPublishProductForSiteMap] @Rows = 5,@PageNo=1,@CatalogId=3,@VersionId=38,@LocaleId=1,@RowsCount=0
BEGIN
BEGIN TRY 
SET NOCOUNT ON;
	DECLARE @SQL NVARCHAR(MAX),@Rows1 BIGINT

	IF Object_id('tempdb..#PublishProductEntity') <>0
	DROP TABLE #PublishProductEntity

	CREATE TABLE #PublishProductEntity(RowId BIGINT PRIMARY KEY IDENTITY(1,1), ZnodeProductId INT,SKU NVARCHAR(500),Name NVARCHAR(500),CategoryName NVARCHAR(500),SeoUrl NVARCHAR(500))

	SET @Rows1 = @Rows * @PageNo

	INSERT INTO #PublishProductEntity(ZnodeProductId,SKU,Name,CategoryName,SeoUrl)
	SELECT
		ZPPE.[ZnodeProductId] AS [ZnodeProductId],
		ZPPE.[SKU] AS [SKU],
		ZPPE.[Name] AS [Name],
		ZPPE.[CategoryName] AS [CategoryName],
		ZPPE.[SeoUrl] AS [SeoUrl] 
	FROM [dbo].[ZnodePublishProductEntity] AS ZPPE
	LEFT JOIN ZnodePublishSeoEntity ZPSE ON ZPPE.SKU = ZPSE.SEOCode AND ZPSE.ItemName = 'Product'
		AND ZPPE.VersionId = ZPSE.VersionId 
	WHERE (ZPPE.[ZnodeCatalogId] = @CatalogId ) AND (ZPPE.[LocaleId] = @LocaleId ) AND (ZPPE.[VersionId] = @VersionId )
	and isnull([CategoryName],'') != ''
	ORDER BY ZPPE.[ZnodeCategoryIds] ASC
	print @SQL
	--Applying pagination
	SET @SQL = 'SELECT TOP '+CAST(@Rows1 AS VARCHAR(10))+' * FROM #PublishProductEntity
	'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
	
	EXEC sp_executesql @SQL
	SET @RowsCount = (SELECT COUNT(*) FROM #PublishProductEntity)

END TRY
BEGIN CATCH
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishProductForSiteMap @Rows = '+CAST(@Rows AS VARCHAR(200))+',@PageNo='+CAST(@PageNo AS VARCHAR(200))
	+',@CatalogId='+CAST(@CatalogId AS VARCHAR(200))+',@VersionId='+CAST(@VersionId AS VARCHAR(200))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(200));

    EXEC Znode_InsertProcedureErrorLog
        @ProcedureName = 'Znode_GetPublishProductForSiteMap',
        @ErrorInProcedure = @Error_procedure,
        @ErrorMessage = @ErrorMessage,
        @ErrorLine = @ErrorLine,
        @ErrorCall = @ErrorCall;

END CATCH;
END