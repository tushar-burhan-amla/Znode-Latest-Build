
CREATE PROCEDURE [dbo].[ZnodeReport_Dashboard]    
(           
  @PortalId       VARCHAR(MAX)  = '',    
  @BeginDate      DATE          = NULL,    
  @EndDate        DATE          = NULL    
            
)    
AS     
/*    
    Summary:- This procedure is used to get the order details     
    Unit Testing:    
    EXEC ZnodeReport_Dashboard    
*/    
  BEGIN    
  BEGIN TRY    
  SET NOCOUNT ON;    
  
  DECLARE @TotalNewCust int, @Frequency int    
  
  DECLARE @TBL_CultureCurrency TaBLE (Symbol Varchar(100),CurrencyCode varchar(100))    
  INSERT INTO @TBL_CultureCurrency (Symbol,CurrencyCode)    
  SELECT Symbol,CultureCode from  ZnodeCulture ZC  -- Changed ZnodeCurrency to ZnodeCulture here.  
    
  DECLARE @PortalCurrencySymbol nvarchar(20)
  DECLARE @DefaultCurrencySymbol nvarchar(20)  

  SET @PortalCurrencySymbol = [dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalID AS INTEGER) )
  SET @DefaultCurrencySymbol = [dbo].[Fn_GetDefaultCurrencySymbol]() 

  IF @PortalCurrencySymbol IS NULL 
	UPDATE @TBL_CultureCurrency SET Symbol  =@DefaultCurrencySymbol WHERE  Symbol IS NULL
  ELSE 
    UPDATE @TBL_CultureCurrency SET Symbol  =@PortalCurrencySymbol WHERE  Symbol IS NULL

  
   IF @BeginDate IS NOT NULL  AND @EndDate IS NOT NULL  AND @BeginDate <> @EndDate     
   SET  @Frequency = DateDiff(day,@BeginDate,@EndDate)    
   ELSE     
   SET  @Frequency = 1    
         
	SELECT     
	Convert(Date, ZOOD.OrderDate) OrderDate, count(*)  TotalOrders , sum(ZOOD.Total) TotalSales,ZC.Symbol AS Symbol
	--COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalID AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]()) Symbol         
	INTO #CalculateTotalValues
	FROM ZNodeOmsOrder ZOO    
	INNER JOIN ZnodeOmsOrderDetails ZOOD ON(ZOOD.OmsOrderId = ZOO.OmsOrderId AND IsActive = 1)    
	INNER JOIN ZNodePortal P ON (P.PortalID = ZOOD.PortalId )
	LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode )    
	WHERE ZOOD.IsActive =1 AND     
      ((EXISTS    
               (    
                   SELECT TOP 1 1    
                   FROM dbo.split(@PortalId, ',') SP    
                   WHERE CAST(P.PortalId AS VARCHAR(100)) = SP.Item    
                         OR @PortalId = ''    
               ))    
    )    
   AND (CAST(ZOOD.OrderDate AS DATE) BETWEEN CASE    
            WHEN @BeginDate IS NULL    
            THEN CAST(ZOOD.OrderDate AS DATE)    
            ELSE @BeginDate    
            END AND CASE    
                WHEN @EndDate IS NULL    
                THEN CAST(ZOOD.OrderDate AS DATE)    
                ELSE @EndDate    
                END)    
 Group by Convert(Date, ZOOD.OrderDate) ,ZC.Symbol
 Order by Convert(Date, ZOOD.OrderDate)     
     
        
    -- this will CHECK for customer     
  SELECT @TotalNewCust = COUNT(*) 
  FROM View_CustomerUserDetail CUD  WHERE               
   ((EXISTS    
               (    
                   SELECT TOP 1 1    
                   FROM dbo.split(@PortalId, ',') SP    
                   WHERE CAST(CUD.PortalId AS VARCHAR(100)) = SP.Item    
                         OR @PortalId = ''    
               )))    
    AND (CAST(CUD.CreatedDate AS DATE) BETWEEN CASE    
            WHEN @BeginDate IS NULL    
            THEN CAST(CUD.CreatedDate AS DATE)    
            ELSE @BeginDate    
            END AND CASE    
                WHEN @EndDate IS NULL    
                THEN CAST(CUD.CreatedDate AS DATE)    
                ELSE @EndDate    
                END)    
      
  SELECT  Sum(TotalOrders) AS TotalOrders,Sum(TotalSales) AS TotalSales,Symbol,@TotalNewCust AS TotalNewCust
  INTO #TotalValues
  FROM #CalculateTotalValues  
  GROUP BY Symbol    
      
  SELECT TotalOrders, [dbo].[Fn_GetDefaultPriceRoundOff](TotalSales) TotalSales    
  , TotalNewCust ,TotalOrders / @Frequency AS TotalAvgOrders ,Symbol 
  FROM #TotalValues     
      
  END TRY    
  BEGIN CATCH    
  DECLARE @Status BIT ;    
	SET @Status = 0;    
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),    
	@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_Dashboard @BeginDate = '+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@PortalId='+CAST(@PortalId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
    EXEC Znode_InsertProcedureErrorLog    
    @ProcedureName = 'ZnodeReport_Dashboard',    
    @ErrorInProcedure = @Error_procedure,    
    @ErrorMessage = @ErrorMessage,    
    @ErrorLine = @ErrorLine,    
    @ErrorCall = @ErrorCall;    
  END CATCH    
  END;