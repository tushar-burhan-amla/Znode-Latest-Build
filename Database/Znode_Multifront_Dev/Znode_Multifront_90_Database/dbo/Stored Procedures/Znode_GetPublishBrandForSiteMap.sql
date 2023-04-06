CREATE PROCEDURE [dbo].[Znode_GetPublishBrandForSiteMap]
(
	@Rows INT,
	@PageNo INT,
	@PortalId INT,
	@VersionId INT,
	@LocaleId INT = 1,
	@RowsCount INT = 0 OUT
)
AS
---- Execute [Znode_GetPublishBrandForSiteMap] @Rows = 20,@PageNo=1,@PortalId=1,@VersionId=39,@LocaleId=1
BEGIN
BEGIN TRY 
SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	DECLARE @SQL NVARCHAR(MAX),@Rows1 BIGINT

	IF Object_id('tempdb..#PublishBrandEntity') <>0
		DROP TABLE #PublishBrandEntity

	SET @Rows1 = @Rows * @PageNo

	CREATE TABLE #PublishBrandEntity
	(
		RowId BIGINT PRIMARY KEY IDENTITY(1,1), 
		PortalId INT,
		LocaleId INT,
		BrandId INT,
		BrandCode VARCHAR(50),
		BrandName VARCHAR(500),
		SEOFriendlyPageName NVARCHAR(MAX)
	)

	INSERT INTO #PublishBrandEntity(PortalId ,LocaleId ,BrandId ,BrandCode,BrandName,SEOFriendlyPageName )
	SELECT
		ZPPBE.PortalId AS PortalId,
		ZPPBE.LocaleId AS LocaleId,
		ZPPBE.BrandId AS BrandId,
		ZPPBE.BrandCode AS BrandCode,
		ZPPBE.BrandName AS BrandName,
		ZPPBE.SEOFriendlyPageName AS SEOFriendlyPageName
	FROM dbo.ZnodePublishPortalBrandEntity AS ZPPBE
	--LEFT JOIN ZnodePublishSeoEntity ZPSE ON ZPPBE.BrandCode = ZPSE.SEOCode AND ItemName = 'Brand'
	--	AND ZPPBE.VersionId = ZPSE.VersionId
	WHERE ZPPBE.LocaleId = @LocaleId AND ZPPBE.PortalId = @PortalId AND ZPPBE.VersionId = @VersionId 
	AND (1 = ZPPBE.IsActive)
	ORDER BY ZPPBE.DisplayOrder ASC

	--Applying pagination
	SET @SQL = 'SELECT TOP '+CAST(@Rows1 AS VARCHAR(10))+' PortalId ,LocaleId ,BrandId ,BrandCode,BrandName,SEOFriendlyPageName 
	FROM #PublishBrandEntity
	'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
	
	EXEC sp_executesql @SQL
	SET @RowsCount = (SELECT COUNT(*) FROM #PublishBrandEntity)

END TRY
BEGIN CATCH
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishBrandForSiteMap @Rows = '+CAST(@Rows AS VARCHAR(200))+',@PageNo='+CAST(@PageNo AS VARCHAR(200))
	+',@PortalId ='+CAST(@PortalId AS VARCHAR(200))+',@VersionId='+CAST(@VersionId AS VARCHAR(200))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(200))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(200));

    EXEC Znode_InsertProcedureErrorLog
        @ProcedureName = 'Znode_GetPublishBrandForSiteMap',
        @ErrorInProcedure = @Error_procedure,
        @ErrorMessage = @ErrorMessage,
        @ErrorLine = @ErrorLine,
        @ErrorCall = @ErrorCall;

END CATCH;
END