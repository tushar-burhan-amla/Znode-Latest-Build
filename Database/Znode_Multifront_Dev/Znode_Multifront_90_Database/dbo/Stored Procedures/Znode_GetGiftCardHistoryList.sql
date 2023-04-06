CREATE PROCEDURE [dbo].[Znode_GetGiftCardHistoryList]  
(  
	@WhereClause NVARCHAR(max),  
    @Rows        INT            = 100,  
    @PageNo      INT            = 1,  
    @Order_BY    VARCHAR(1000)  = '',  
    @RowsCount   INT  out
  )    
AS  
/*  
    Summary: This procedure is used to find the GiftCardhistoryList

     EXEC [Znode_GetGiftCardHistoryList] @WhereClause='' , @RowsCount= 0,@ExpirationDate = ''  
*/  
 
BEGIN  
BEGIN TRY  
SET NOCOUNT ON;  
             
	DECLARE @SQL NVARCHAR(MAX);

	CREATE TABLE #TBL_GiftCardHistoryList (OmsOrderDetailsId int,OmsOrderId INT,OrderNumber NVARCHAR(600),TransactionAmount NUMERIC(28,6),TransactionDate DATETIME,UserName NVARCHAR(512),CustomerName NVARCHAR(512),PortalId INT,UserId INT,OmsUserId INT,GiftCardId INT,Notes VARCHAR(MAX), CultureCode VARCHAR(100), RowId INT, CountNo INT )  
 
	SET @SQL ='  
 
	;WITH CTE_GetGiftHistoryCard AS  
	(  
		SELECT ZOD.OmsOrderDetailsId,ZGH.GiftCardHistoryId, CASE WHEN ZOD.OmsOrderId IS NULL THEN 0 ELSE ZOD.OmsOrderId END  AS OmsOrderId ,ZO.OrderNumber,ZGH.TransactionAmount,ZGH.TransactionDate,ZU.Email As UserName,
		CASE WHEN ZU.FirstName IS NULL THEN '''' ELSE ZU.FirstName END + CASE WHEN ZU.LastName IS NULL  THEN '''' ELSE '' ''+ZU.LastName END as CustomerName,ZGC.PortalId,ZGC.UserId,ZOD.UserId as OmsUserId,ZGC.GiftCardId,ZGH.Notes,zc.CultureCode,ZGC.IsActive,ZGH.RemainingAmount
		FROM ZnodeGiftCard ZGC  
		INNER JOIN ZnodeGiftCardHistory ZGH ON (ZGC.GiftCardId = ZGH.GiftCardId)  
		LEFT JOIN ZnodeOmsOrderDetails ZOD on (ZGH.OmsOrderDetailsId = ZOD.OmsOrderDetailsId )
		INNER JOIN ZnodePortal ZP ON (ZGC.PortalId = ZP.PortalId)  
		INNER JOIN ZnodePortalUnit zpu on (zp.PortalId = zpu.PortalId)  
		LEFT JOIN ZnodeCulture zc on (zc.CultureId = zpu.CultureId)    
		LEFT JOIN ZnodeOmsOrder ZO on (ZOD.OmsOrderId = ZO.OmsOrderId)  
		LEFT JOIN ZnodeUser ZU ON (ZU.UserId = CASE WHEN ZOD.UserId IS NULL then ZGC.UserId ELSE ZOD.UserId END)
	)  
	, CTE_GetGiftCardHistoryList AS  
	(  
		SELECT OmsOrderDetailsId,GiftCardHistoryId, OmsOrderId ,OrderNumber,case when isnull(OmsOrderDetailsId,0)=0 then RemainingAmount Else TransactionAmount End as TransactionAmount,TransactionDate,UserName,CustomerName,PortalId,UserId,OmsUserId,GiftCardId,Notes,CultureCode,IsActive,RemainingAmount,
		'+dbo.Fn_GetPagingRowId(@Order_BY,'GiftCardHistoryId DESC')+',Count(*)Over() CountNo  
		FROM CTE_GetGiftHistoryCard  
		WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'      
	)  
	SELECT OmsOrderDetailsId,OmsOrderId ,OrderNumber,ISNull(TransactionAmount,0) AS TransactionAmount,TransactionDate,UserName,CustomerName,PortalId,UserId,OmsUserId,GiftCardId,Notes,CultureCode,RowId,CountNo
	FROM CTE_GetGiftCardHistoryList  
	'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
 
	 INSERT INTO #TBL_GiftCardHistoryList  
	 EXEC(@SQL)  

	 SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM #TBL_GiftCardHistoryList ),0)  

	 ;With CTE_GiftCardHistory as
	  (
		  SELECT Distinct ZOD.OmsOrderId,GiftHistory.GiftCardId,Gift.RemainingAmount, min(GiftHistory.OmsOrderDetailsId) as OmsOrderDetailsId
		  FROM ZnodeGiftCardHistory GiftHistory
		  inner join ZnodeGiftCard Gift ON GiftHistory.GiftCardId = Gift.GiftCardId
		  LEFT JOIN ZnodeOmsOrderDetails ZOD on (GiftHistory.OmsOrderDetailsId = ZOD.OmsOrderDetailsId)
		  LEFT JOIN ZnodeOmsOrder ZO on (ZOD.OmsOrderId = ZO.OmsOrderId)  
		  WHERE EXISTS(select * from  #TBL_GiftCardHistoryList Gift where GiftHistory.GiftCardId = Gift.GiftCardId)
		  GROUP BY ZOD.OmsOrderId,GiftHistory.GiftCardId,Gift.RemainingAmount
	  )
	  ,CTE_OrderVoucherAmount AS
	  (
		  SELECT Distinct ISNULL(cte.OmsOrderId,0) AS OmsOrderId,cte.OmsOrderDetailsId,Hist.GiftCardId,
		  CASE WHEN cte.OmsOrderId IS NOT NULL THEN Hist.TransactionAmount ELSE Cte.RemainingAmount END as OrderVoucherAmount
		  FROM ZnodeGiftCardHistory Hist
		  LEFT join CTE_GiftCardHistory Cte on Hist.GiftCardId = Cte.GiftCardId and ISNULL (Hist.OmsOrderDetailsId,0) = ISNULL(Cte.OmsOrderDetailsId,0)
		  WHERE EXISTS(select * from  #TBL_GiftCardHistoryList Gift where Hist.GiftCardId = Gift.GiftCardId)
		  AND CASE WHEN cte.OmsOrderId IS not NULL THEN Hist.TransactionAmount ELSE Cte.RemainingAmount END IS NOT NULL
	)
	SELECT Hist.OmsOrderId ,Hist.OrderNumber,Hist.TransactionDate,Hist.UserName,Hist.CustomerName,
	Hist.PortalId,Hist.UserId,Hist.OmsUserId,Hist.GiftCardId,Hist.Notes,Hist.CultureCode, Hist.TransactionAmount as TransactionAmount
	FROM #TBL_GiftCardHistoryList Hist
	INNER JOIN CTE_OrderVoucherAmount Voucher ON Hist.GiftCardId = Voucher.GiftCardId AND Voucher.OmsOrderId = Hist.OmsOrderId
	and ISNULL (Hist.OmsOrderDetailsId,0) = ISNULL(Voucher.OmsOrderDetailsId,0)


END TRY  
BEGIN CATCH  
	DECLARE @Status BIT ;  
	SET @Status = 0;  
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetGiftCardHIstoryList @WhereClause = '+CAST(@WhereClause AS VARCHAR(MAX))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+'@Status='+CAST(@Status AS VARCHAR(10));  
                   
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
     
	EXEC Znode_InsertProcedureErrorLog  
	@ProcedureName = 'Znode_GetGiftCardHistoryList',  
	@ErrorInProcedure = @Error_procedure,  
	@ErrorMessage = @ErrorMessage,  
	@ErrorLine = @ErrorLine,  
	@ErrorCall = @ErrorCall;  
END CATCH  
END