
CREATE PROCEDURE [dbo].[Znode_GetSearchProfileList]
(
	@WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT
)
AS 
/*
	 Summary :- This Procedure is used to get the publish status of the catalog 
	 Unit Testig 
	 EXEC  Znode_GetSearchProfileList '',100,1,'',0
*/
BEGIN 
	BEGIN TRY 
	SET NOCOUNT ON 

		DECLARE @SQL  NVARCHAR(MAX);

		DECLARE @PublishStateIdForForNotPublishedState INT = dbo.Fn_GetPublishStateIdForForNotPublishedState()

		DECLARE @TBL_CatalogId TABLE 
		(PublishCatalogId int ,CatalogName NVARCHAR(2000), SearchProfileId INT , SearchQueryTypeId INT , ProfileName NVARCHAR(2000),
			QueryTypeName NVARCHAR(2000),QueryBuilderClassName  NVARCHAR(2000),SearchSubQueryTypeId INT,SubQueryTypeName  NVARCHAR(2000),
			SubQueryBuilderClassName  NVARCHAR(2000),--RowId INT ,
			PortalId INT,PortalName NVARCHAR(MAX),IsDefault BIT,CountId INT,CreatedDate Datetime,PublishStateId INT,CatalogCode VARCHAR(300), 
			PublishStatus VARCHAR(50)
		)
		
	 
		SET @SQL = '
		;With Cte_CatalogLog AS 
		(
		Select dd.PublishCatalogId,zpc.CatalogName,B.SearchProfileId,  b.SearchQueryTypeId ,b.ProfileName,c.QueryTypeName,c.QueryBuilderClassName,
			b.SearchSubQueryTypeId,d.QueryTypeName SubQueryTypeName,d.QueryBuilderClassName SubQueryBuilderClassName,ZPSP.PortalId,ZP.StoreName as PortalName,
			isnull(ZPSP.IsDefault,0) as Isdefault, B.CreatedDate,B.PublishStateId,ZPC.CatalogCode, PS.DisplayName As PublishStatus
		FROM ZnodeSearchProfile B 
		INNER JOIN ZnodePublishCatalogSearchProfile dd on  dd.SearchProfileId=b.SearchProfileId
		LEFT JOIN ZnodePimCatalog zpc on ZPC.PimCatalogId = dd.PublishCatalogId
		INNER JOIN ZnodeSearchQueryType C on b.SearchQueryTypeId=C.SearchQueryTypeId 
		LEFT JOIN ZnodeSearchQueryType d on  d.SearchQueryTypeId =b.SearchSubQueryTypeId
		LEFT JOIN ZnodePortalSearchProfile ZPSP ON (ZPSP.SearchProfileId = B.SearchProfileId AND ZPSP.PublishCatalogId = DD.PublishCatalogId)
		LEFT JOIN ZnodePortal ZP ON (ZP.PortalId = ZPSP.PortalId)
		LEFT JOIN ZnodePublishState PS ON ISNULL(B.PublishStateId,'+CAST(@PublishStateIdForForNotPublishedState AS VARCHAR(10))+') = PS.PublishStateId
		)	 
			,Cte_PublishStatus AS 
		(
		SELECT PublishCatalogId,CatalogName,SearchProfileId , SearchQueryTypeId ,ProfileName,QueryTypeName,QueryBuilderClassName,SearchSubQueryTypeId,
			SubQueryTypeName,SubQueryBuilderClassName,PortalId,PortalName,IsDefault,PublishStateId,
			'+[dbo].[Fn_GetPagingRowId](@Order_BY,'SearchProfileId DESC')+' , Count(*) Over () CountId ,CreatedDate,CatalogCode,PublishStatus
		FROM Cte_CatalogLog
		WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' )
	 
		SELECT PublishCatalogId,CatalogName,SearchProfileId , SearchQueryTypeId ,ProfileName,QueryTypeName,QueryBuilderClassName, SearchSubQueryTypeId,
			SubQueryTypeName,SubQueryBuilderClassName,PortalId,PortalName,IsDefault,CountId, CreatedDate,PublishStateId,CatalogCode,PublishStatus
		FROM Cte_PublishStatus 
		'+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)+' '
	
		PRINT @sql

		INSERT INTO @TBL_CatalogId 
		EXEC (@SQL)

		SELECT PublishCatalogId,CatalogName,SearchProfileId, SearchQueryTypeId,	ProfileName,QueryTypeName,QueryBuilderClassName,SearchSubQueryTypeId,
			SubQueryTypeName,SubQueryBuilderClassName,PortalId,PortalName,IsDefault,CountId, a.CreatedDate,a.PublishStatus,a.CatalogCode
		FROM @TBL_CatalogId a
		
		SET @RowsCount = ISNULL((SELECT TOP 1 COUNTID FROM @TBL_CatalogId),0)

	END TRY
	BEGIN CATCH
		DECLARE @Status BIT;
		SET @Status = 0;
		SELECT ERROR_MESSAGE()
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 
					'EXEC Znode_GetSearchProfileList 
						@WhereClause = '''+ISNULL(@WhereClause,'''''')+''',
						@Rows='+ISNULL(CAST(@Rows AS VARCHAR(50)),'''''')+',
						@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',
						@Order_BY='''+ISNULL(@Order_BY,'''''')+''',
						@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''');
              			 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetSearchProfileList',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH 
END


