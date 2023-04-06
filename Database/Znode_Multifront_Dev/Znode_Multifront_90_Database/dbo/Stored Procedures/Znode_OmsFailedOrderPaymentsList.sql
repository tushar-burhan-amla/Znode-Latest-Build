CREATE PROCEDURE [dbo].[Znode_OmsFailedOrderPaymentsList]
( 
	@WhereClause NVARCHAR(MAX),
	@Rows        INT            = 100,
	@PageNo      INT            = 1,
	@Order_BY    VARCHAR(1000)  = '',
	@RowsCount   INT OUT			
 )
AS
/*
	Summary : This procedure is used to get the oms failed order payment details

	Unit Testing:

	EXEC [Znode_OmsFailedOrderPaymentsList] 'PortalId =1',@Order_BY = '',@RowsCount= 0, @UserId = 0 ,@Rows = 50, @PageNo = 1

	declare @p7 int
	set @p7=4
	exec sp_executesql N'Znode_OmsFailedOrderPaymentsList @WhereClause, @Rows,@PageNo,@Order_By,@RowCount OUT',N'@WhereClause nvarchar(30),
		@Rows int,@PageNo int,@Order_By nvarchar(14),@RowCount int output',@WhereClause=N'',@Rows=50,@PageNo=1,@Order_By=N'orderdate desc',
		@RowCount=@p7 output
	select @p7
*/
BEGIN
    BEGIN TRY
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
		DECLARE @SQL NVARCHAR(MAX)

		DECLARE @Fn_GetPaginationWhereClause VARCHAR(500) = dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows),
			@Fn_GetFilterWhereClause NVARCHAR(MAX) = ''
		SET @Fn_GetFilterWhereClause=dbo.Fn_GetFilterWhereClause(@WhereClause)
			
		IF @Order_BY = ''
			SET @Order_BY = 'OrderDate desc'

			SET @Order_BY = REPLACE(@Order_BY,'PaymentName','ZOFOP.PaymentDisplayName')
			SET @Order_BY = REPLACE(@Order_BY,'CustomerName','ZOFOP.UserName')
			SET @Order_BY = REPLACE(@Order_BY,'EmailAddress','ZOFOP.Email')
			SET @Order_BY = REPLACE(@Order_BY,'PaymentStatus','ZOPS.Name')
			SET @Order_BY = REPLACE(@Order_BY,'failedordertotalwithcurrency','ZOFOP.TotalAmount')
			
			DECLARE @Fn_GetPagingRowId NVARCHAR(MAX) = ' DENSE_RANK()Over('+ ' Order By '+CASE WHEN ISNULL(@Order_BY,'') = '' THEN 'OmsFailedOrderPaymentId DESC' ELSE @Order_BY + ',OmsFailedOrderPaymentId DESC' END  + ') RowId '
						
			IF OBJECT_ID('tempdb..#TBL_RowCount') IS NOT NULL
				DROP TABLE #TBL_RowCount
			
			CREATE TABLE #TBL_RowCount(RowsCount INT)
			 
			SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'PaymentName','ZOFOP.PaymentDisplayName')
			SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'CustomerName','ZOFOP.UserName')
			SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'EmailAddress','ZOFOP.Email')
			SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'PaymentStatus','ZOPS.Name')
			SET @Fn_GetFilterWhereClause = REPLACE(@Fn_GetFilterWhereClause,'failedordertotalwithcurrency','ZOFOP.TotalAmount')

			SET @Fn_GetPagingRowId = REPLACE(@Fn_GetPagingRowId,'OmsFailedOrderPaymentId','OmsFailedOrderPaymentId')

			SET @Rows = @PageNo * @Rows

		SET @SQL = '
		SELECT DISTINCT TOP '+CAST(@Rows as VARCHAR(10))+' ZOFOP.OrderNumber,ZOFOP.TotalAmount,ZOFOP.OrderDate,
			ZOFOP.PaymentDisplayName As PaymentName,ZOFOP.PaymentStatusId,ZOPS.Name PaymentStatus,
			ZOFOP.TransactionToken,ZOFOP.UserName As CustomerName,ZOFOP.Email As EmailAddress, '+@Fn_GetPagingRowId+' ,
			ZOFOP.OmsFailedOrderPaymentId
		INTO #Cte_OrderLineDescribe
		FROM ZnodeOmsFailedOrderPayments ZOFOP WITH (NOLOCK)
		LEFT JOIN ZnodeOmsPaymentState ZOPS WITH (NOLOCK) ON (ZOPS.OmsPaymentStateId = ZOFOP.PaymentStatusId)
		WHERE 1=1 '+ @Fn_GetFilterWhereClause+' 
		ORDER BY '+CASE WHEN ISNULL(@Order_BY,'') = '' THEN 'ZOFOP.OmsFailedOrderPaymentId DESC' ELSE @Order_BY + ',ZOFOP.OmsFailedOrderPaymentId DESC' END +' 

		INSERT INTO #TBL_RowCount
		SELECT COUNT(*)
		FROM ZnodeOmsFailedOrderPayments ZOFOP WITH (NOLOCK)
		LEFT JOIN ZnodeOmsPaymentState ZOPS WITH (NOLOCK) ON (ZOPS.OmsPaymentStateId = ZOFOP.PaymentStatusId)
		WHERE 1=1 '+ @Fn_GetFilterWhereClause+' 
			
		Create index Ind_OrderLineDescribe_RowId on #Cte_OrderLineDescribe(RowId )

		SELECT OrderNumber,TotalAmount,OrderDate,PaymentName AS PaymentDisplayName,PaymentStatusId,PaymentStatus,TransactionToken
			,CustomerName AS UserName,EmailAddress AS Email
		FROM #Cte_OrderLineDescribe
		' + @Fn_GetPaginationWhereClause +' order by RowId '

		PRINT @SQL
		EXEC(@SQL)
		Select @RowsCount= isnull(RowsCount  ,0) from #TBL_RowCount
		
		IF OBJECT_ID('tempdb..#TBL_RowCount') is not null
				DROP TABLE #TBL_RowCount
		
    END TRY
    BEGIN CATCH
DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_OmsFailedOrderPaymentsList @WhereClause = '''+ISNULL(CAST(@WhereClause AS VARCHAR(max)),'''''')+''',@Rows='''+ISNULL(CAST(@Rows AS VARCHAR(50)),'''''')+''',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',
		@Order_BY='+ISNULL(@Order_BY,'''''')+',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',''';
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_OmsFailedOrderPaymentsList',
		@ErrorInProcedure = 'Znode_OmsFailedOrderPaymentsList',
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
    END CATCH;
END;