CREATE PROCEDURE [dbo].[Znode_AdminUnassociatedUsers]
(
	@WhereClause    VARCHAR(max) = '',
	@Rows			INT           = 100,
	@PageNo			INT           = 1,
	@Order_By		VARCHAR(1000) = '',
	@RowCount		INT        = 0 OUT,
	@PortalId		INT = 0
)
/*
	exec Znode_AdminUnassociatedUsers @AccountId = 5, @Rows = 50, @PageNo = 1, @Order_By = 'UserId asc'
	,@WhereClause='Username like ''%abhilash.dadhe%'''
*/
AS
BEGIN

	BEGIN TRY
	SET NOCOUNT ON;

	--DECLARE @PortalId INT = isnull((select top 1 PortalId from ZnodePortalAccount where AccountId = @AccountId ),0)

	DECLARE @SQL NVARCHAR(MAX) = '',
	        @PaginationWhereClause VARCHAR(300)= dbo.Fn_GetRowsForPagination(@PageNo, @Rows, ' WHERE RowId');

	set @SQL = '
	;with cte_UserListPortalWise as
	(
		SELECT ZU.Userid, isnull(ZU.FirstName,'''')+case when ZU.FirstName is null then '''' else '' '' end +isnull(ZU.MiddleName,'''')+case when ZU.MiddleName is null then '''' else '' '' end +isnull(ZU.LastName, '''') as FullName, ANZU.UserName, ZU.Email, 
			   case when ANZU.PortalId is null then ''ALL'' Else ZP.StoreName end as StoreName, ZU.ModifiedDate, 
			   cast(case when ANU.LockoutEndDateUtc is null then 1 else 0 end as bit) as IsActive
		FROM AspNetZnodeUser ANZU
		inner join AspNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName
		inner join ZnodeUser ZU ON ANU.Id = ZU.AspNetUserId
		left join ZnodePortal ZP ON ANZU.PortalId = ZP.PortalId
		WHERE ZU.AccountId is null
		and ( ANZU.PortalId = '+Cast(@PortalId as varchar(10))+' OR ANZU.PortalId is null )
	)
	select Userid,FullName, UserName, Email, StoreName,ModifiedDate 
	into #UserListPortalWise
	from cte_UserListPortalWise
	where 1=1 '+dbo.Fn_GetWhereClause(@WhereClause, ' AND ')+'

		select Userid,FullName, UserName, Email, StoreName,ModifiedDate,
	       Row_Number()Over('+dbo.Fn_GetOrderByClause(@Order_By, 'ModifiedDate DESC, UserId DESC')+',ModifiedDate DESC, UserId DESC) RowId
		into #UserListPortalWise_RowNumber
		from #UserListPortalWise

		SET @Count= ISNULL((SELECT  Count(1) FROM #UserListPortalWise_RowNumber ),0)
	
		select Userid, FullName, UserName, Email, StoreName
		from #UserListPortalWise_RowNumber 
		'+@PaginationWhereClause+' '+dbo.Fn_GetOrderByClause(@Order_By, 'ModifiedDate DESC, UserId DESC');

		print @SQL
		EXEC SP_executesql
					@SQL,
					N'@Count INT OUT',
					@Count = @RowCount OUT;

		--select @RowCount AS [RowCount]
		
	END TRY
    BEGIN CATCH
		DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_AdminUnassociatedUsers @WhereClause='+cast(@WhereClause as varchar(max))+' ,@Rows= '+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_By='+@Order_By+',@RowCount='+CAST(@RowCount AS VARCHAR(50))+'
		@PortalId='+CAST(@PortalId AS VARCHAR(50));
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName    = 'Znode_AdminUnassociatedUsers',
		@ErrorInProcedure = @ERROR_PROCEDURE,
		@ErrorMessage     = @ErrorMessage,
		@ErrorLine        = @ErrorLine,
		@ErrorCall        = @ErrorCall;
    END CATCH;

END