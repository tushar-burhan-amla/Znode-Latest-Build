CREATE PROCEDURE [dbo].[ZnodeReport_DashboardTopBrands]    
(           
 @PortalId       VARCHAR(MAX)  = '',    
 @BeginDate      DATE          = NULL,    
 @EndDate        DATE          = NULL    
     
)    
AS     
   /*  Summary:- This procedure is used to get the order details     
    Unit Testing:    
     EXEC ZnodeReport_DashboardTopBrands    
    
 */    
     BEGIN    
  BEGIN TRY    
        SET NOCOUNT ON;    
      
  DECLARE @TopBrand TABLE (Brands nvarchar(100), Orders int , Sales  Nvarchar(100),Symbol NVARCHAR(10))    
    
     DECLARE @TBL_CultureCurrency TaBLE (Symbol Varchar(100),CurrencyCode varchar(100))    
   INSERT INTO @TBL_CultureCurrency (Symbol,CurrencyCode)    
   SELECT Symbol,CultureCode from  ZnodeCulture ZC         -- Chnaged table name from ZnodeCurrency to ZnodeCulture here.  
   -- LEFT JOIN ZnodeCulture ZCC ON (ZC.CurrencyId = ZCC.CurrencyId)    
     
      
  INSERT INTO @TopBrand (Brands, Orders, Sales,Symbol )    
   SELECT ZOOA.AttributeValue Brands,count(Distinct zood.OmsOrderDetailsId) Orders ,    
   --[dbo].[Fn_GetDefaultPriceRoundOff]( Sum(zoolit.price)) Sales    
   [dbo].[Fn_GetDefaultPriceRoundOff]( Sum(zoolit.Quantity * zoolit.price)) Sales    
   ,COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalID AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]()) Symbol    
   --,ISNULL(ZC.Symbol,[dbo].[Fn_GetDefaultCurrencySymbol]()) Symbol    
   from ZnodeOmsOrderLineItems zoolit INNER JOIN ZnodeOmsOrderDetails zood     
   --ON zoolit.OmsOrderDetailsId = zood.OmsOrderDetailsId LEFT JOIN ZnodeCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode )    
    ON zoolit.OmsOrderDetailsId = zood.OmsOrderDetailsId LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode )    
   INNER JOIN ZnodeOmsOrderAttribute ZOOA ON zoolit.OmsOrderLineItemsId = ZOOA.OmsOrderLineItemsId AND  ZOOA.AttributeCode = 'Brand'     
   AND ZOOA.AttributeValue IS NOT NULL    
   WHERE ZOOD.IsActive =1 AND     
    ((EXISTS    
       (    
        SELECT TOP 1 1    
        FROM dbo.split(@PortalId, ',') SP    
        WHERE CAST(zood.PortalId AS VARCHAR(100)) = SP.Item    
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
   Group by ZOOA.AttributeValue,COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalID AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]())       
   -- Group by ZOOA.AttributeValue,ISNULL(ZC.Symbol,[dbo].[Fn_GetDefaultCurrencySymbol]())  
    
   select top 5 Brands, Orders, Sales  ,Symbol   from @TopBrand Order by  Convert(numeric,Sales )  desc      
   END TRY    
   BEGIN CATCH    
   DECLARE @Status BIT ;    
       SET @Status = 0;    
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),    
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardTopBrands @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
             EXEC Znode_InsertProcedureErrorLog    
    @ProcedureName = 'ZnodeReport_DashboardTopBrands',    
    @ErrorInProcedure = @Error_procedure,    
    @ErrorMessage = @ErrorMessage,    
    @ErrorLine = @ErrorLine,    
    @ErrorCall = @ErrorCall;    
   END CATCH    
    
  END;