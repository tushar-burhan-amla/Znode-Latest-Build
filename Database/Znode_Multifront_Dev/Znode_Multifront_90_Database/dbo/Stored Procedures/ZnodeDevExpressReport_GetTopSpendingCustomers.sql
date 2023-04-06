CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetTopSpendingCustomers]
(   @BeginDate    DATETIME        ,
	@EndDate      DATETIME       ,
	@StoreName    NVARCHAR(max) = '',
	@ShowAllCustomers BIT = 1,  
    @TopCustomers INT = 100 ,
	@AmountGreaterThan NVARCHAR(50) = 0 
	)
AS 
/*
     Summary :- This Procedure is used to get frequently Appear users  
     Unit Testing 
     EXEC ZnodeDevExpressReport_GetTopSpendingCustomers_bak @BeginDate = '2018/01/01',@EndDate= '2018/12/12'  , @AmountGreaterThan = '40'
*/
     BEGIN
	 BEGIN TRY
         DECLARE @TBL_ReportOrderUser TABLE (OrderCount INT, UserId     INT, PortalId   INT ,Total NUMERIC(28,6),Symbol  VARCHAR(100),
		 TotalShippingCost NUMERIC(28,6),TotalTaxCost NUMERIC(28,6),SubTotalAmount  NUMERIC(28,6) );  
         DECLARE @TBL_PortalId TABLE (PortalId INT );
		 INSERT INTO @TBL_PortalId
		 SELECT PortalId 
		 FROM ZnodePortal ZP 
		 INNER JOIN dbo.split(@StoreName,'|') SP ON (SP.Item = ZP.StoreName) 
	
	INSERT INTO @TBL_ReportOrderUser (OrderCount, UserId,PortalId,Total,Symbol,TotalShippingCost,TotalTaxCost,SubTotalAmount)                     
    SELECT COUNT(ZOOD.OmsOrderId) OrderCount, UserId, PortalId, SUM(Total)  Total, ZC.Symbol, SUM(ShippingCost) AS TotalShippingCost,
	Sum(TaxCost) AS TotalTaxCost, SUM(SubTotal) AS SubTotalAmount
	FROM ZnodeOmsOrderDetails ZOOD  
	INNER JOIN ZnodeCulture ZC ON (ZC.CultureCode  = ZOOD.CultureCode )
	INNER JOIN ZnodeOmsOrderState ZOOS ON (ZOOD.OmsOrderStateId = ZOOS.OmsOrderStateId)  
	WHERE ZOOD.IsActive = 1   
	AND (EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId rt WHERE rt.PortalId = ZOOD.PortalId)
				OR NOT EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId ))   
    AND CAST(OrderDate AS DATETIME) BETWEEN @BeginDate AND @EndDate 
	AND ZOOS.OrderStateName NOT IN ('CANCELED','RETURNED','REJECTED','FAILED')				               
    GROUP BY UserId,PortalId,ZC.Symbol
	HAVING SUM(Total)  >= CAST(@AmountGreaterThan AS NUMERIC(28,6))  
	;  
	
	SET @TopCustomers = CASE WHEN @ShowAllCustomers = 1 THEN (SELECT COUNT(1) FROM @TBL_ReportOrderUser ) ELSE @TopCustomers END 

    SELECT TOP (@TopCustomers) TBL.UserID , CASE WHEN REPLACE(ISNULL(FirstName,'')+' '+ISNULL(LastName,''),' ','') = '' THEN APZ.UserName ELSE ISNULL(FirstName,'')+' '+ISNULL(LastName,'') END  CustomerName 
				, StoreName , OrderCount ,dbo.Fn_GetDefaultPriceRoundOffReturnNumeric(Total) TotalAmount, Symbol,ZU.Email, CASE WHEN  ZU.AspNetUserId IS NULL THEN 'Guest User' ELSE 'Registered User' END  CustomerType  
				,dbo.Fn_GetDefaultPriceRoundOffReturnNumeric(Total/OrderCount)  AverageTotalAmount,TotalShippingCost,TotalTaxCost,SubTotalAmount
	FROM @TBL_ReportOrderUser TBL
	INNER JOIN ZnodePortal ZP ON (ZP.portalId = TBL.PortalId )
	LEFT JOIN ZnodeUser  ZU ON (ZU.UserId = TBL.UserId )
	LEFT JOIN AspNetUsers AP ON (AP.Id = ZU.AspNetUserId)
	LEFT JOIN AspNetZnodeUser APZ ON (APZ.AspNetZnodeUserId = AP.UserName)
	ORDER BY StoreName ASC,TotalAmount DESC ,CustomerName ASC
                 
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetTopSpendingCustomers @BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetTopSpendingCustomers',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;