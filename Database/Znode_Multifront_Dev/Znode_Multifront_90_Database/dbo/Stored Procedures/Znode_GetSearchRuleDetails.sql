CREATE PROCEDURE [dbo].[Znode_GetSearchRuleDetails]
(
	@WhereClause         VARCHAR(MAX)  = '',
    @Rows                INT           = 100,
    @PageNo              INT           = 1,
    @Order_BY            VARCHAR(1000) = '',
    @RowsCount           INT OUT
)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		Declare @TBL_SerchRuleDetail Table (SearchCatalogRuleId Int, RuleName varchar(600), UserName nvarchar(512), CreatedDate Datetime, StartDate Datetime, EndDate Datetime, IsPause Bit,RowId Int, CountId Int)
		Declare @SQL Varchar(max)

		SET @SQL = '
		;With Cte_SerchRuleDetail As'+
		+'('+
			+' SELECT ZSCR.publishcatalogid, ZSCR.SearchCatalogRuleId, ZSCR.RuleName, ANZU.UserName, ZSCR.CreatedDate, ZSCR.StartDate, ZSCR.EndDate, ZSCR.IsPause '+ 
			+' FROM ZnodeSearchCatalogRule ZSCR '+ 
			+' LEFT JOIN ZnodeUser ZU ON ZSCR.CreatedBy = ZU.UserId '+
			+' INNER JOIN AspNetUsers ANU ON ZU.AspNetUserId = ANU.Id '+
			+' INNER JOIN AspNetZnodeUser ANZU ON ANU.UserName = ANZU.AspNetZnodeUserId WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)
		 +')'+
		 +',Cte_SerchRuleDetail_OrderBy AS '+
		 +'('
			+' SELECT publishcatalogid,SearchCatalogRuleId, RuleName, UserName, CreatedDate, StartDate, EndDate, IsPause '+
			+' , '+[dbo].[Fn_GetPagingRowId](@Order_BY, ' SearchCatalogRuleId ASC')+
			+' FROM Cte_SerchRuleDetail '+
		+')'+
		 +' SELECT CSRD.SearchCatalogRuleId, CSRD.RuleName, CSRD.UserName, CSRD.CreatedDate, StartDate, EndDate, IsPause, Count(*)Over() As CountId '+
		 +' FROM Cte_SerchRuleDetail_OrderBy CSRD 
		 '+[dbo].[Fn_GetPaginationWhereClause](@PageNo, @Rows)+'
		 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+'
		 '+[dbo].[Fn_GetOrderByClause](@Order_BY, 'CSRD.SearchCatalogRuleId ASC')
		
		 print @SQL
		 INSERT INTO @TBL_SerchRuleDetail (SearchCatalogRuleId, RuleName, UserName, CreatedDate, StartDate, EndDate, IsPause, CountId)
		 EXEC (@SQL)

		 SELECT SearchCatalogRuleId, RuleName, UserName, CreatedDate, StartDate, EndDate,IsPause, 
		        CASE WHEN IsPause = 1 THEN 'Yes' ELSE 'No' END AS Paused, 
				CountId
		 FROM @TBL_SerchRuleDetail

		 SET @RowsCount = ISNULL((SELECT top 1 CountId FROM @TBL_SerchRuleDetail), 0);
	END TRY
         BEGIN CATCH
		
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSearchRuleDetails @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetSearchRuleDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
            
      END CATCH;
END