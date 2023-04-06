CREATE PROCEDURE [dbo].[ZnodeReport_DashboardSales]            
(                  
	@PortalId  BIGINT  = NULL,        
	@AccountId BIGINT  = NULL,    
	@SalesRepUserId INT = 0                      
)            
AS            
/*            
    Summary:- This procedure is used to get the order details            
    Unit Testing:            
    EXEC [ZnodeReport_DashboardSales] @PortalId=8, @AccountId=0, @SalesRepUserId=19          
*/            
BEGIN            
BEGIN TRY            
SET NOCOUNT ON;            
    DECLARE @GetDate DATE = dbo.Fn_GetDate();
	DECLARE @TotalNewCust INT, @Frequency INT, @TotalQuotes INT, @TotalReturns INT            
         
	DECLARE @TBL_CultureCurrency TABLE (Symbol VARCHAR(100),CurrencyCode VARCHAR(100),IsDefault BIT)            
	INSERT INTO @TBL_CultureCurrency (Symbol,CurrencyCode,IsDefault)            
	SELECT Symbol,CultureCode, IsDefault FROM  ZnodeCulture ZC  -- Changed ZnodeCurrency to ZnodeCulture here.          
           
	DECLARE @PortalCurrencySymbol NVARCHAR(20)        
	DECLARE @DefaultCurrencySymbol NVARCHAR(20)          
   
	----Verifying that the @SalesRepUserId is having 'Sales Rep' role
	IF NOT EXISTS
	(
	SELECT * FROM ZnodeUser ZU
	INNER JOIN AspNetZnodeUser ANZU ON ZU.UserName = ANZU.UserName
	INNER JOIN AspNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName
	INNER JOIN AspNetUserRoles ANUR ON ANU.Id = ANUR.UserId
	WHERE Exists(SELECT * FROM AspNetRoles ANR WHERE Name = 'Sales Rep' AND ANUR.RoleId = ANR.Id)
	AND ZU.UserId = @SalesRepUserId
	)  
	BEGIN
		SET @SalesRepUserId = 0
	End  
 
	  SET @PortalCurrencySymbol = [dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalID AS INTEGER) )        
	  SET @DefaultCurrencySymbol = [dbo].[Fn_GetDefaultCurrencySymbol]()        
       
	  IF @PortalCurrencySymbol IS NULL        
		UPDATE @TBL_CultureCurrency SET Symbol  =@DefaultCurrencySymbol WHERE  Symbol IS NULL        
	  ELSE        
		UPDATE @TBL_CultureCurrency SET Symbol  =@PortalCurrencySymbol WHERE  Symbol IS NULL        
       
	  -- this will CHECK for customer            
	  CREATE TABLE #User(UserId INT, FirstName VARCHAR(100), MiddleName VARCHAR(100), LastName VARCHAR(100), Email VARCHAR(50), PhoneNumber VARCHAR(50),AccountId INT )
 
	  CREATE TABLE #CalculateTotalValues(TotalOrders INT, TotalSales INT, Symbol VARCHAR(100))

	  IF @AccountId = -1
	  BEGIN
		INSERT INTO #User(UserId, FirstName, MiddleName, LastName, Email , PhoneNumber,AccountId)
		SELECT ZU.UserId, ZU.FirstName, ZU.MiddleName, ZU.LastName, ZU.Email , ZU.PhoneNumber ,ZU.AccountId                  
		FROM ZnodeUser ZU          
		WHERE EXISTS(SELECT * FROM ZnodeOmsQuote ZOQ WHERE ZU.UserId = ZOQ.UserID )      
		AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = @SalesRepUserId and ZU.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)  
		AND ISNULL(ZU.AccountId,0)=0

		SELECT @TotalNewCust = COUNT(*)        
		FROM View_CustomerUserDetail CUD  WHERE                      
		(CUD.PortalId=@PortalId OR ISNULL(@PortalId,0)=0) AND  (ISNULL(CUD.AccountId,0)=0)    
     
		INSERT INTO #CalculateTotalValues(TotalOrders, TotalSales, Symbol)        
		SELECT   count(*)  TotalOrders , sum(ZOOD.Total) TotalSales,      
		COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalId AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]()) Symbol                
		FROM ZNodeOmsOrder ZOO            
		INNER JOIN ZnodeOmsOrderDetails ZOOD ON(ZOOD.OmsOrderId = ZOO.OmsOrderId AND IsActive = 1)            
		INNER JOIN ZNodePortal P ON (P.PortalID = ZOOD.PortalId )        
		LEFT JOIN ZnodeUser ZU ON (ZU.UserId = ZOOD.UserId)    
		LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode )            
		WHERE ZOOD.IsActive =1 AND (P.PortalId = @PortalId OR ISNULL(@PortalId,0)=0)    
		AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = @SalesRepUserId and ZU.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)          
		AND NOT EXISTS(SELECT * FROM ZnodeAccount ZA WHERE isnull(ZU.AccountId,0) = ZA.AccountId)
		Group by ZC.Symbol

		SELECT @TotalQuotes = COUNT(*)                    
		FROM ZnodeOmsQuote ZOQ          
		INNER JOIN ZnodeOmsQuoteType ZOQT ON ZOQ.OmsQuoteTypeId = ZOQT.OmsQuoteTypeId          
		INNER JOIN #User U ON ZOQ.UserId = U.UserId          
		INNER JOIN ZnodePortal ZP ON ZOQ.PortalID = ZP.PortalID          
		INNER JOIN ZnodeOMSOrderState ZOOS ON ZOOS.OmsOrderStateId = ZOQ.OmsOrderStateId          
		LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZOQ.CultureCode )          
		WHERE ZOQT.QuoteTypeCode = 'QUOTE' AND
		(ZOQ.PortalID = @PortalId OR @PortalId = 0 OR @PortalId IS NULL)    
		AND NOT EXISTS(SELECT * FROM ZnodeAccount ZA WHERE isnull(U.AccountId,0) = ZA.AccountId)
		AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = @SalesRepUserId and U.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)      
     
		-- This will get returns count      
		SELECT @TotalReturns = COUNT(*)              
		FROM ZnodeRmaReturnDetails ZRRD          
		INNER JOIN ZnodeUser ZU ON ZRRD.UserId = ZU.UserId          
		INNER JOIN ZnodePortal ZP ON ZRRD.PortalId = ZP.PortalId          
		INNER JOIN ZnodeRmaReturnState ZRRS on ZRRD.RmaReturnStateId = ZRRS.RmaReturnStateId          
		LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZRRD.CultureCode )        
		WHERE isnull(ZRRD.RmaReturnStateId,0) NOT IN (SELECT ISNULL(RmaReturnStateId,0) FROM ZnodeRmaReturnState WHERE ReturnStateName = 'Not Submitted')          
		AND (ZRRD.PortalId = @PortalId OR @PortalId  =0 or @PortalId IS NULL)    
		AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = @SalesRepUserId and ZU.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)  
		AND NOT EXISTS(SELECT * FROM ZnodeAccount ZA WHERE isnull(ZU.AccountId,0) = ZA.AccountId)
	 END
	 ELSE
	 BEGIN
			-- This will get quotes count
		INSERT INTO #User(UserId, FirstName, MiddleName, LastName, Email , PhoneNumber)
		SELECT ZU.UserId, ZU.FirstName, ZU.MiddleName, ZU.LastName, ZU.Email , ZU.PhoneNumber                
		FROM ZnodeUser ZU          
		WHERE EXISTS(SELECT * FROM ZnodeOmsQuote ZOQ WHERE ZU.UserId = ZOQ.UserID )    AND (ZU.AccountId = @AccountId OR ISNULL(@AccountId,0)=0)    
		AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = @SalesRepUserId and ZU.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)  
     
		SELECT @TotalNewCust = COUNT(*)        
			FROM View_CustomerUserDetail CUD  WHERE                      
			(CUD.PortalId=@PortalId OR ISNULL(@PortalId,0)=0) AND  (CUD.AccountId = @AccountId OR ISNULL(@AccountId,0)=0)    
     
		INSERT INTO #CalculateTotalValues (TotalOrders, TotalSales, Symbol)
		SELECT   count(*)  TotalOrders , sum(ZOOD.Total) TotalSales,      
		COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalId AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]()) Symbol                      
		FROM ZNodeOmsOrder ZOO            
		INNER JOIN ZnodeOmsOrderDetails ZOOD ON(ZOOD.OmsOrderId = ZOO.OmsOrderId AND IsActive = 1)            
		INNER JOIN ZNodePortal P ON (P.PortalID = ZOOD.PortalId )        
		LEFT JOIN #User ZU ON (ZU.UserId = ZOOD.UserId)    
		LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode )            
		WHERE ZOOD.IsActive =1 AND (P.PortalId = @PortalId OR ISNULL(@PortalId,0)=0) AND (ZU.AccountId = @AccountId OR ISNULL(@AccountId,0)=0)    
		AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = @SalesRepUserId and ZU.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)          
		Group by ZC.Symbol

		SELECT @TotalQuotes = COUNT(*)                    
		FROM ZnodeOmsQuote ZOQ          
		INNER JOIN ZnodeOmsQuoteType ZOQT ON ZOQ.OmsQuoteTypeId = ZOQT.OmsQuoteTypeId          
		INNER JOIN #User U ON ZOQ.UserId = U.UserId          
		INNER JOIN ZnodePortal ZP ON ZOQ.PortalID = ZP.PortalID          
		INNER JOIN ZnodeOMSOrderState ZOOS ON ZOOS.OmsOrderStateId = ZOQ.OmsOrderStateId          
		LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZOQ.CultureCode )          
		WHERE ZOQT.QuoteTypeCode = 'QUOTE' AND (ZOQ.PortalID = @PortalId OR @PortalId = 0 OR @PortalId IS NULL)          
		AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = @SalesRepUserId and U.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)      
     
		-- This will get returns count      
		SELECT @TotalReturns = COUNT(*)              
		FROM ZnodeRmaReturnDetails ZRRD          
		INNER JOIN ZnodeUser ZU ON ZRRD.UserId = ZU.UserId          
		INNER JOIN ZnodePortal ZP ON ZRRD.PortalId = ZP.PortalId          
		INNER JOIN ZnodeRmaReturnState ZRRS on ZRRD.RmaReturnStateId = ZRRS.RmaReturnStateId          
		LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZRRD.CultureCode )        
		WHERE isnull(ZRRD.RmaReturnStateId,0) NOT IN (SELECT ISNULL(RmaReturnStateId,0) FROM ZnodeRmaReturnState WHERE ReturnStateName = 'Not Submitted')          
		AND (ZRRD.PortalId = @PortalId OR @PortalId  =0 or @PortalId IS NULL)  AND (ZU.AccountId = @AccountId OR ISNULL(@AccountId,0)=0)    
		AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = @SalesRepUserId and ZU.UserId = SalRep.CustomerUserid) or @SalesRepUserId = 0)  
 
	 END

	  UPDATE ZOQ set OmsOrderStateId = (SELECT top 1 OmsOrderStateId FROM ZnodeOMSOrderState WHERE OrderStateName = 'EXPIRED')          
	  FROM ZnodeOmsQuote ZOQ          
	  INNER JOIN ZnodeOmsQuoteType ZOQT ON ZOQ.OmsQuoteTypeId = ZOQT.OmsQuoteTypeId          
	  INNER JOIN #User U ON ZOQ.UserId = U.UserId          
	  INNER JOIN ZnodePortal ZP ON ZOQ.PortalID = ZP.PortalID          
	  INNER JOIN ZnodeOMSOrderState ZOOS ON ZOOS.OmsOrderStateId = ZOQ.OmsOrderStateId          
	  WHERE  (ZOQ.PortalID = @PortalId OR ISNULL(@PortalId,0)=0)          
	  and CAST(ZOQ.QuoteExpirationDate as date) < @GetDate         
	  and ZOQ.OmsOrderStateId <> (SELECT top 1 OmsOrderStateId FROM ZnodeOMSOrderState WHERE OrderStateName = 'EXPIRED')          
         
           
	  SELECT  Sum(TotalOrders) AS TotalOrders,Sum(TotalSales) AS TotalSales,Symbol,@TotalNewCust AS TotalNewCust, @TotalQuotes as TotalQuotes, @TotalReturns as TotalReturns      
	  INTO #TotalValues        
	  FROM #CalculateTotalValues          
	  GROUP BY Symbol            
       
	  IF NOT EXISTS(SELECT * FROM #TotalValues)
	  BEGIN
		SELECT 0 AS TotalOrders, CAST('0' AS VARCHAR(10)) AS TotalSales            
		 , 0 AS TotalNewCust ,0 AS TotalAvgOrders,0 AS TotalQuotes,0 as TotalReturns ,
		 [dbo].[Fn_GetDefaultCurrencySymbol]() Symbol     
 
	  END
	  ELSE
	  BEGIN
		 SELECT TotalOrders, [dbo].[Fn_GetDefaultPriceRoundOff](TotalSales) TotalSales            
		 , TotalNewCust ,Isnull(TotalOrders / @Frequency,0) AS TotalAvgOrders,TotalQuotes,TotalReturns ,Symbol        
		 FROM #TotalValues
	  END

       
END TRY            
BEGIN CATCH            
	DECLARE @Status BIT ;            
	SET @Status = 0;            
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),            
	@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardSales @PortalId='+CAST(@PortalId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));            
                             
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                                
               
	EXEC Znode_InsertProcedureErrorLog            
	@ProcedureName = 'ZnodeReport_DashboardSales',            
	@ErrorInProcedure = @Error_procedure,            
	@ErrorMessage = @ErrorMessage,            
	@ErrorLine = @ErrorLine,            
	@ErrorCall = @ErrorCall;            
END CATCH            
END;