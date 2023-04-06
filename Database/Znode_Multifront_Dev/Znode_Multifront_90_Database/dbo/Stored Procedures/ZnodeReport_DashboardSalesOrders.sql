CREATE PROCEDURE [dbo].[ZnodeReport_DashboardSalesOrders]  
(   @BeginDate      DATE          = NULL,  
 @EndDate        DATE          = NULL,  
 @PortalId       VARCHAR(MAX)  = ''  
)  
AS   
   /* Summary:- This procedure is used to get the order details   
     Unit Testing:  
     EXEC ZnodeReport_DashboardSalesOrders  
  
 */  
     BEGIN  
  BEGIN TRY  
         SET NOCOUNT ON;  
    
   Declare @CalculateTotalValues TABLE (OrderDate Datetime , TotalOrders int,TotalSales numeric(28,6),Symbol NVARCHAR(10))  
         DECLARE @TotalNewCust int, @Frequency int 
		 
		   DECLARE @TBL_CultureCurrency TaBLE (Symbol Varchar(100),CurrencyCode varchar(100))  
   INSERT INTO @TBL_CultureCurrency (Symbol,CurrencyCode)  
   SELECT Symbol,CultureCode from  ZnodeCulture ZC    --- Changed table name from ZnodeCurrency to ZnodeCulture here.
   -- LEFT JOIN ZnodeCulture ZCC ON (ZC.CurrencyId = ZCC.CurrencyId) 
  
		  
   IF @BeginDate IS NOT NULL  AND @EndDate IS NOT NULL AND @BeginDate <> @EndDate   
   SET  @Frequency = DateDiff(day,@BeginDate,@EndDate)  
   ELSE   
   SET  @Frequency = 1    
      insert into @CalculateTotalValues (OrderDate, TotalOrders,TotalSales,Symbol)  
         SELECT   
                Convert(Date, ZOOD.OrderDate) OrderDate, count(*)  Orders , sum(ZOOD.Total) Sales  
	,COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalID AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]()) Symbol 	
    --,ISNULL(ZC.Symbol,[dbo].[Fn_GetDefaultCurrencySymbol]()) Symbol  
	
         FROM ZNodeOmsOrder ZOO  
              INNER JOIN ZnodeOmsOrderDetails ZOOD ON(ZOOD.OmsOrderId = ZOO.OmsOrderId AND IsActive = 1)  
              INNER JOIN ZNodePortal P ON P.PortalID = ZOOD.PortalId  
                
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
   Group by Convert(Date, ZOOD.OrderDate) ,COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalID AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]()) Order by Convert(Date, ZOOD.OrderDate)     
   --Group by Convert(Date, ZOOD.OrderDate) ,ISNULL(ZC.Symbol,[dbo].[Fn_GetDefaultCurrencySymbol]()) Order by Convert(Date, ZOOD.OrderDate)     
      
   SELECT OrderDate , TotalOrders, [dbo].[Fn_GetDefaultPriceRoundOff](TotalSales) TotalSales,Symbol  
   FROM @CalculateTotalValues     
  END TRY  
  BEGIN CATCH  
  DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardSalesOrders @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'ZnodeReport_DashboardSalesOrders',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
  END CATCH  
  END;