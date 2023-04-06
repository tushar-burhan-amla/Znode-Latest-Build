CREATE PROCEDURE [dbo].[Znode_GetParentAccountList]
(
    @WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT
)
AS
--declare @p7 int
--set @p7=4
--exec sp_executesql N'Znode_GetParentAccountList @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT',N'@WhereClause nvarchar(30),@Rows int,@PageNo int,@Order_By nvarchar(4000),@RowCount int output',@WhereClause=N'(PortalId in(''1'',''4'',''5'',''6''))',@Rows=50,@PageNo=1,@Order_By=N'',@RowCount=@p7 output
--select @p7
BEGIN
   BEGIN TRY
   SET NOCOUNT ON;

		DECLARE @SQL nvarchar(max)

		SET @SQL= '
		CREATE TABLE #TBL_ParentAccount (AccountId INT, Name NVARCHAR(max), ParentAccountId INT, AccountCode NVARCHAR(max), RowId INT)

		INSERT INTO #TBL_ParentAccount(AccountId, Name, ParentAccountId, AccountCode, RowId)
		SELECT  ZPA.AccountId, ZA.Name, ZA.ParentAccountId, ZA.AccountCode, '+dbo.Fn_GetPagingRowId(@Order_BY,'ZPA.AccountId ASC')+'
		FROM ZnodePortalAccount ZPA
		INNER JOIN ZnodeAccount ZA ON ZPA.AccountId = ZA.AccountId
		'+dbo.Fn_GetWhereClause(@WhereClause, ' WHERE ')+' '+'AND ParentAccountId IS null'+
		' '+' ORDER BY ZA.Name

		SET @Count = (SELECT COUNT(1) FROM #TBL_ParentAccount)

		SELECT TBLPA.AccountId, TBLPA.Name, TBLPA.ParentAccountId, TBLPA.AccountCode from #TBL_ParentAccount TBLPA
		'+ [dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)+'
		'
		--print @SQL

		EXEC SP_executesql
		@SQL,N'@Count INT OUT',@Count = @RowsCount OUT;

    END TRY
    BEGIN CATCH

		 DECLARE @Status BIT ;
		 SET @Status = 0;
		 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetParentAccountList @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
		 VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')
             
		 SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
   
		 EXEC Znode_InsertProcedureErrorLog
		 @ProcedureName = 'Znode_GetParentAccountList',
		 @ErrorInProcedure = @Error_procedure,
		 @ErrorMessage = @ErrorMessage,
		 @ErrorLine = @ErrorLine,
		 @ErrorCall = @ErrorCall;

	END CATCH;
END

