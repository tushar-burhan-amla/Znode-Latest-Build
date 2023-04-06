CREATE PROCEDURE [dbo].[Znode_GetCatalogList]
(
	@WhereClause	NVARCHAR(MAX),
	@Rows			INT = 100,
	@PageNo			INT = 1,
	@Order_BY		VARCHAR(100) = '',
	@RowsCount		INT OUT
)
AS
/*
	Summary :- This Procedure is used to get the publish status of the catalog
	Unit Testig
	EXEC Znode_GetCatalogList '',100,1,'',0
	EXEC Znode_GetCatalogList null,100,1,'',0
*/
BEGIN 
	BEGIN TRY
	SET NOCOUNT ON

	DECLARE @SQL NVARCHAR(MAX)
	DECLARE @TBL_CatalogId TABLE (PimCatalogId INT, PublishCatalogLogId INT,CatalogName VARCHAR(MAX),PublishStatus VARCHAR(300),RowId INT, CountId INT,
		PublishCreatedDate DATETIME, PublishModifiedDate DATETIME, PublishCategoryCount INT, PublishProductCount INT, IsActive BIT, CatalogCode NVARCHAR(100));

	SET @SQL = '

		;With Cte_MaxPublish AS
		(
			SELECT max(PublishCatalogLogId) PublishCatalogLogId,PimCatalogId
			FROM ZnodePublishCatalogLog ZPCL 
			WHERE ZPCL.PublishType=''Catalog'' OR ZPCL.PublishType IS NULL
			GROUP BY PimCatalogId
		)
		,Cte_CatalogLog AS
		(
			SELECT ZPC.CatalogName CatalogName, PublishCatalogLogId PublishCatalogLogId, TYU.DisplayName PublishStatus ,ZPC.PimCatalogId
				,ZPCL.CreatedDate AS PublishCreatedDate,ZPCL.ModifiedDate AS PublishModifiedDate,
				ISNULL(ZPCL.PublishCategoryId,0)PublishCategoryCount,ISNULL(ZPCL.PublishProductId,0) PublishProductCount,ZPC.IsActive, ZPC.CatalogCode
			FROM ZnodePimCatalog ZPC 
			LEFT JOIN ZnodePublishCatalogLog ZPCL WITH (NOLOCK) ON ( EXISTS (SELECT TOP 1 1 FROM Cte_MaxPublish CTE 											
			WHERE CTE.PimCatalogId = ZPC.PimCatalogId AND CTE.PublishCatalogLogId = ZPCL.PublishCatalogLogId) )	
			LEFT JOIN ZnodePublishState TYU ON (TYU.PublishStateId = ZPCL.PublishStateId )
		)
		,Cte_PublishStatus AS
		(
			SELECT PimCatalogId, PublishCatalogLogId, CatalogName, PublishStatus,PublishCreatedDate,PublishModifiedDate,PublishCategoryCount,
				PublishProductCount,IsActive,CatalogCode,
				'+[dbo].[Fn_GetPagingRowId](@Order_BY,'PublishCatalogLogId DESC')+' , Count(*)Over() CountId FROM Cte_CatalogLog
			WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' 
		)

		SELECT PimCatalogId, PublishCatalogLogId,CatalogName,PublishStatus,RowId,CountId,PublishCreatedDate,PublishModifiedDate,
			PublishCategoryCount,PublishProductCount,IsActive,CatalogCode
		FROM Cte_PublishStatus 
		'+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)+' '

		INSERT INTO @TBL_CatalogId
		EXEC (@SQL);

		SELECT PimCatalogId,PublishCatalogLogId,CatalogName,PublishStatus,PublishCreatedDate,PublishModifiedDate,PublishCategoryCount,
			PublishProductCount,IsActive, CatalogCode
		FROM @TBL_CatalogId;

		SET @RowsCount = ISNULL((SELECT TOP 1 COUNTID FROM @TBL_CatalogId),0);

	END TRY
	BEGIN CATCH
		DECLARE @Status BIT ;
		SET @Status = 0;
			
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCatalogList 
					@WhereClause = '''+ISNULL(@WhereClause,'''''')+''',
					@Rows='+ISNULL(CAST(@Rows AS VARCHAR(50)),'''''')+',
					@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',
					@Order_BY='''+ISNULL(@Order_BY,'''''')+''',
					@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''');

	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

	EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetCatalogList',
			@ErrorInProcedure = 'Znode_GetCatalogList',
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END