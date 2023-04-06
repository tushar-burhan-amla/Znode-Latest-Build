CREATE procedure [dbo].[Znode_GetKeywordSearchNoResultsFoundReport] 
(
	@WhereClause    VARCHAR(max) = '',
	@Rows			INT           = 100,
	@PageNo			INT           = 1,
	@Order_By		VARCHAR(1000) = '',
	@RowCount		INT        = 0 OUT,
	@PortalId		VARCHAR(1000) = 0
)
/*
	exec [Znode_GetKeywordSearchNoResultsFoundReport] @PortalId = 1, @Rows = 1, @PageNo = 1, @Order_By = ''
	,@WhereClause='SearchKeyword=''Test'''
*/
as 
begin
	BEGIN TRY
	SET NOCOUNT ON;

	DECLARE @SQL NVARCHAR(MAX) = '',
	        @PaginationWhereClause VARCHAR(300)= dbo.Fn_GetRowsForPagination(@PageNo, @Rows, ' WHERE RowId');

	SET @SQL = '
	;with cte_SearchActivityData as
	(
		select ZSA.PortalId, ZSA.SearchKeyword, ZP.StoreName as PortalName, 0 AS ResultCount, 
		cast(cast(convert(date,ZSA.CreatedDate) as varchar(10))+'' ''+cast(DATEPART(hour, ZSA.CreatedDate) as varchar) + '':'' + cast(DATEPART(minute, ZSA.CreatedDate) as varchar) as datetime) as CreatedDate
		from ZnodeSearchActivity ZSA
		INNER JOIN ZnodePortal ZP ON ZSA.PortalId = ZP.PortalId
		WHERE ZSA.ResultCount = 0 AND ZSA.PortalId = '+CAST(@PortalId AS VARCHAR(10))+ ' 
		
	)
	,cte_SearchActivity_WhereClause as
	(
		select PortalId, SearchKeyword, PortalName, ResultCount, COUNT(1) AS  NumberOfSearches
		from cte_SearchActivityData
		where 1 = 1 '+dbo.Fn_GetWhereClause(@WhereClause, ' AND ')+' 
		GROUP By PortalId, SearchKeyword, PortalName, ResultCount
	)
	select PortalId, SearchKeyword, PortalName, NumberOfSearches, ResultCount
	into #SearchActivity_Filter
	from cte_SearchActivity_WhereClause
	
	select PortalId, SearchKeyword, PortalName, NumberOfSearches, ResultCount,
	       Row_Number()Over('+dbo.Fn_GetOrderByClause(@Order_By, 'NumberOfSearches DESC, ResultCount DESC')+',NumberOfSearches DESC, ResultCount DESC) RowId
	into #SearchActivity_RowNumber
	from #SearchActivity_Filter

	SET @Count= ISNULL((SELECT  Count(1) FROM #SearchActivity_RowNumber ),0)
	
	select PortalId, SearchKeyword, PortalName, NumberOfSearches, ResultCount
	from #SearchActivity_RowNumber 
	'+@PaginationWhereClause+' '+dbo.Fn_GetOrderByClause(@Order_By, 'NumberOfSearches DESC, ResultCount DESC');
	print @SQL
	EXEC SP_executesql
				@SQL,
				N'@Count INT OUT',
				@Count = @RowCount OUT;

	END TRY
    BEGIN CATCH
		DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetKeywordSearchNoResultsFoundReport @WhereClause='+cast(@WhereClause as varchar(max))+' ,@Rows= '+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_By='+@Order_By+',@RowCount='+CAST(@RowCount AS VARCHAR(50))+'
		@PortalId='+CAST(@PortalId AS VARCHAR(50));
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName    = 'Znode_GetKeywordSearchNoResultsFoundReport',
		@ErrorInProcedure = @ERROR_PROCEDURE,
		@ErrorMessage     = @ErrorMessage,
		@ErrorLine        = @ErrorLine,
		@ErrorCall        = @ErrorCall;
    END CATCH;

	

END