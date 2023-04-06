

CREATE PROCEDURE [dbo].[Znode_ECertificateWalletDetails] 
(
	
	@WhereClause NVARCHAR(max)= '',
	@Rows        INT            = 100,
	@PageNo      INT            = 1,
	@Order_BY    VARCHAR(1000)  = '',
	@RowsCount   INT  out,
	@UserId INT
)
AS
BEGIN
/*
 This procedure is use to get the ecertificate data 
 EXEC Znode_ECertificateWalletDetails 'portalid = 8 and userid = 228 ',50,1,'',0,228

*/
BEGIN TRY

	SET NOCOUNT ON;
	-- last updated date is to be fetched from order redemption flow.
		DECLARE @SQL NVARCHAR(max) = '
		
		;with Cte_getData AS (

		SELECT ZEC.ECertificateId,
			   ZEC.CertificateKey,
			   ZEC.CertificateType,
			   ZEC.IssuedDate, 
			   ZEC.IssuedAmount,
			   ZECW.BalanceAmount,
			   ZECW.ModifiedDate LastUsedCYMD, 
			   ABS(ZECW.BalanceAmount - ZEC.IssuedAmount) RedemptionApplied, 
			   ZEC.Custom1, 
			   ZEC.Custom2, 
			   ZEC.Custom3, 
			   ZEC.Custom4, 
			   ZECW.IssuedByUserId,
			   ZECW.IssuedToUserId as userid,
			   ZD.PortalId  PortalId,
			   ISNULL(ZUCert.FirstName+'' '','''') +ISNULL(ZUCert.LastName,'''') as IssuedByName
	FROM ZnodeEcertificate ZEC
	LEFT JOIN ZnodeEcertificateWallet ZECW ON ZEC.ECertificateId = ZECW.ECertificateId
	LEFT JOIN ZnodeUser ZUWallet ON ZECW.IssuedToUserId = ZUWallet.UserId
	LEFT JOIN ZnodeUser ZUCert ON ZECW.IssuedByUserId = ZUCert.UserId
	LEFT JOIN ZnodeUserPortal ZD ON (ZD.UserId = ZECW.IssuedToUserId )
	WHERE ZECW.IssuedToUserId = '+CAST(@UserId AS VARCHAR(max)) +'
	
	) 
	,Cte_Pagination AS 
	(
	SELECT * , '+[dbo].[Fn_GetPagingRowId](@Order_BY,'ECertificateId DESC')+' , Count(*)Over() CountId 
	FROM Cte_getData 
	WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+'
	)
	SELECT * 
	INTO #TempData
	FROM Cte_Pagination
	'+ dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)+ ' 
	
	SELECT * FROM #TempData
	 
	SET @RowsCount =( SELECT TOP 1 CountId FROM #TempData)
	'

 EXEC SP_EXECUTESQL  @SQL,N'@RowsCount INT OUT ', @RowsCount = @RowsCount OUT

END TRY
BEGIN CATCH 
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePimCatalog 
												@WhereClause = '+@WhereClause+',
												@Rows='+CAST(@Rows AS VARCHAR(10))+',
												@PageNo='+CAST(@PageNo AS VARCHAR(10))+',
												@Order_BY='+@Order_BY+',
												@RowsCount='+CAST(@RowsCount AS VARCHAR(10))+',
												@UserId='+CAST(@UserId AS VARCHAR(10));
              			  
        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_ECertificateWalletDetails',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END