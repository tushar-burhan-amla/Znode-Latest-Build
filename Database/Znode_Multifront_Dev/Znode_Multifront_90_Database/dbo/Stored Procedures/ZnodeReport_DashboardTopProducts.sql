
CREATE PROCEDURE [dbo].[ZnodeReport_DashboardTopProducts]    
(           
 @PortalId       VARCHAR(MAX)  = '',    
 @BeginDate      DATE          = NULL,    
 @EndDate        DATE          = NULL    
)    
AS     
/*    
     Summary:- This procedure is used to get the order details     
     Unit Testing:    
     EXEC ZnodeReport_DashboardTopProducts    
 */    
  BEGIN    
  BEGIN TRY    
  SET NOCOUNT ON;       
  DECLARE @RoundOffValue INT= dbo.Fn_GetDefaultValue('PriceRoundOff')
    
  DECLARE @TBL_CultureCurrency TaBLE (Symbol Varchar(100),CurrencyCode varchar(100))    
  INSERT INTO @TBL_CultureCurrency (Symbol,CurrencyCode)    
  SELECT Symbol,CultureCode FROM  ZnodeCulture ZC     
  DECLARE @PortalCurrencySymbol nvarchar(20)
  DECLARE @DefaultCurrencySymbol nvarchar(20)  

  SET @PortalCurrencySymbol = [dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalID AS INTEGER) )
  SET @DefaultCurrencySymbol = [dbo].[Fn_GetDefaultCurrencySymbol]() 

  IF @PortalCurrencySymbol IS NULL 
	UPDATE @TBL_CultureCurrency SET Symbol  =@DefaultCurrencySymbol WHERE  Symbol IS NULL
  ELSE 
    UPDATE @TBL_CultureCurrency SET Symbol  =@PortalCurrencySymbol WHERE  Symbol IS NULL

          
   SELECT ProductName AS Products,SKU,COUNT(zood.OmsOrderDetailsId) Orders ,       
   SUBSTRING(CAST(Sum(zoolit.Quantity * zoolit.price) AS VARCHAR(100)),1, CHARINDEX('.', CAST(SUM(zoolit.Quantity * zoolit.price) AS VARCHAR(100)))+@RoundOffValue) as Sales   
   ,ZC.Symbol Symbol 
	INTO #TopOrders      
    FROM ZnodeOmsOrderLineItems zoolit 
	INNER JOIN ZnodeOmsOrderDetails zood  ON (zoolit.OmsOrderDetailsId = zood.OmsOrderDetailsId   )
	LEFT JOIN @TBL_CultureCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode )                         
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
   AND zoolit.ParentOmsOrderLineItemsId IS  NULL  
   Group by zoolit.ProductName,zoolit.sku , ZC.Symbol
   --COALESCE (ZC.Symbol,[dbo].[Fn_GetPortalCurrencySymbol](CAST(@PortalID AS INTEGER)),[dbo].[Fn_GetDefaultCurrencySymbol]())       
    
   SELECT TOP 5 Products,SKU, Orders, Sales  ,Symbol   
   FROM #TopOrders ORDER BY  Convert(numeric,Sales )  DESC      
       
   END TRY    
   BEGIN CATCH    
   DECLARE @Status BIT ;    
   SET @Status = 0;    
   DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),    
   @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_DashboardTopProducts @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
   SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
   EXEC Znode_InsertProcedureErrorLog    
   @ProcedureName = 'ZnodeReport_DashboardTopProducts',    
   @ErrorInProcedure = @Error_procedure,    
   @ErrorMessage = @ErrorMessage,    
   @ErrorLine = @ErrorLine,    
   @ErrorCall = @ErrorCall;    
   END CATCH     
   END;