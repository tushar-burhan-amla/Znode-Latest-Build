CREATE PROCEDURE [dbo].[Znode_GetOmsAccountOrderList]  
(
	@WhereClause NVARCHAR(MAX),  
	@Rows        INT           = 100,  
	@PageNo      INT           = 1,  
	@Order_BY    VARCHAR(100)  = '',  
	@RowsCount   INT OUT,  
	@AccountId   INT
)
AS
 /*  
  Summary :- This procedure is used to get the Quote list of account and Users   
    Fn_GetRecurciveAccounts is used to fetch AccountId and Its recursive ParentId   
    The result id fetched on the basis of AccountId of the User  
     Unit Testing    
     EXEC Znode_GetOmsAccountOrderList '' ,@RowsCount = 0 ,@AccountId = 1  
   
*/  
BEGIN  
    BEGIN TRY  
	SET NOCOUNT ON;  
    DECLARE @SQL NVARCHAR(MAX);  
  
    DECLARE @TBL_QuoteDetails TABLE (OmsOrderId INT,UserName NVARCHAR(300),OrderAmount NUMERIC(28, 6),[Status] VARCHAR(300),CreatedDate DATETIME,OmsPaymentStateId INT,OmsPaymentState NVARCHAR(MAX),  
            PaymentTypeId INT,PaymentType VARCHAR(50),OrderNumber VARCHAR(200),IsInRMA BIT ,RowId INT,CountNo INT,CurrencyCode VARCHAR(100),PaymentDisplayName Nvarchar(1200),CultureCode VARCHAR(100)
			,PortalId INT, UserId INT);
              
    SET @Order_BY = REPLACE(@Order_BY,'OmsOrderId','CTEQD.OmsOrderId')  
    SET @SQL = '  

	IF OBJECT_ID(''tempdb..#GetAccounts'') IS NOT NULL
    DROP TABLE #GetAccounts

	SELECT * INTO #GetAccounts FROM [dbo].[Fn_GetRecurciveAccounts]('+CAST(@AccountId AS VARCHAR(100))+')
  
   ;With Cte_GetQuoteDetail AS   
   (  
  
   SELECT Zu.UserId ,ZOO.OmsOrderId,APZU.UserName UserName ,Total  OrderTotal , ZOOS.OrderStateName OrderState  
    ,ZOOD.OrderDate ,ZOPS.OmsPaymentStateId,ZOPS.Name PaymentStatus  ,ZOOD.PaymentTypeId , ZPT.Name PaymentType,ZOO.OrderNumber,ZOOD.CurrencyCode  
    ,CASE WHEN ARJT.PaymentDisplayName IS NULL THEN ZPS.PaymentDisplayName ELSE  ARJT.PaymentDisplayName END PaymentDisplayName ,ZOOD.CultureCode 
	,ZOOD.PortalId
   FROM ZnodeOmsOrder ZOO  
   INNER JOIN ZnodeOmsOrderDetails ZOOD ON (ZOOD.OmsOrderId = ZOO.OmsOrderId AND ZOOD.IsActive = 1 )  
   LEFT JOIN ZnodePaymentSetting ZPS ON (ZPS.PaymentSettingId = ZOOD.PaymentSettingId)  
   INNER JOIN ZnodeUser ZU ON (ZU.UserId = ZOOD.UserId)  
   LEFT JOIN ZnodeOmsPaymentState ZOPS ON (ZOPS.OmsPaymentStateId = ZOOD.OmsPaymentStateId )  
   LEFT JOIN ZnodeOmsOrderState ZOOS ON (ZOOS.OmsOrderStateId = ZOOD.OmsOrderStateId )  
   LEFT JOIN ZnodePaymentType  ZPT ON (ZPT.PaymentTypeId = ZOOD.PaymentTypeId)  
   LEFT JOIN AspNetUsers ASNU ON (ASNU.Id =ZU.AspNetUserId  )  
   LEFT JOIN AspNetZnodeUser  APZU ON (APZU.AspNetZnodeUserId = ASNU.UserName AND (APZU.PortalId = ZOOD.PortalId OR APZU.PortalId IS NULL  ))  
   LEFT JOIN ZnodePortalPaymentSetting ARJT ON (ARJT.PortalId = ZOOD.PortalId AND ARJT.PaymentSettingId = ZOOD.PaymentSettingId )  
   WHERE EXISTS (SELECT TOP 1 1 FROM #GetAccounts FNRA WHERE FNRA.AccountId= ZU.AccountId AND ZOOD.AccountId IS NULL)   
     OR EXISTS (SELECT TOP 1 1 FROM #GetAccounts FNRA WHERE FNRA.AccountId= ZOOD.AccountId AND ZOOD.AccountId IS NOT NULL)
   )  
   ,Cte_GetRMAdetails AS  
   (  
     SELECT OmsOrderId   
  FROM ZnodeOmsOrderDetails ZOOD   
  INNER JOIN ZnodeOmsOrderLIneItems ZOODLI ON (ZOODLI.OmsOrderDetailsId = ZOOD.OmsOrderDetailsId )  
  INNER JOIN ZnodeRmaRequestItem ZRRI ON (ZRRI.OmsOrderLineItemsId = ZOODLI.OmsOrderLineItemsId )  
     
   )  
   , Cte_GetQuote AS   
   (  
    SELECT CTEQD.*,CASE WHEN CTERD.OmsOrderid IS NULL THEN 0 ELSE 1 END IsInRMA   ,'+dbo.Fn_GetPagingRowId(@Order_BY,'CTEQD.OmsOrderId DESC')+',Count(*)Over() CountNo   
    FROM Cte_GetQuoteDetail CTEQD   
    LEFT JOIN Cte_GetRMAdetails CTERD ON (CTERD.OmsOrderId = CTEQD.OmsOrderid )  
       WHERE 1=1   
    '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'  
   )  
  
   SELECT OmsOrderId,UserName , OrderTotal QuoteAmount, OrderState Status ,OrderDate,OmsPaymentStateId  ,PaymentStatus,PaymentTypeId,PaymentType,OrderNumber,IsInRMA,RowId,CountNo,CurrencyCode,PaymentDisplayName,CultureCode
		,PortalId, UserId
   FROM Cte_GetQuote   
   WHERE '+dbo.Fn_GetRowsForPagination(@PageNo, @Rows, ' RowId ');  
            
	PRINT @SQL  
    INSERT INTO @TBL_QuoteDetails   
		EXEC (@SQL);  
		SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_QuoteDetails ), 0);  
  
		SELECT OmsOrderId,UserName,OrderAmount,[Status] AS 'OrderState',CreatedDate AS 'OrderDate',OmsPaymentStateId,OmsPaymentState AS 'PaymentStatus',PaymentTypeId,PaymentType,OrderNumber,IsInRMA,CurrencyCode,PaymentDisplayName,CultureCode
		,PortalId, UserId
		FROM @TBL_QuoteDetails;
    END TRY  
    BEGIN CATCH  
		DECLARE @Status BIT ;  
		SET @Status = 0;  
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetOmsAccountOrderList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@AccountId = '+CAST(@AccountId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
		EXEC Znode_InsertProcedureErrorLog  
		@ProcedureName = 'Znode_GetOmsAccountOrderList',  
		@ErrorInProcedure = @Error_procedure,  
		@ErrorMessage = @ErrorMessage,  
		@ErrorLine = @ErrorLine,  
		@ErrorCall = @ErrorCall;  
	END CATCH;  
END;
