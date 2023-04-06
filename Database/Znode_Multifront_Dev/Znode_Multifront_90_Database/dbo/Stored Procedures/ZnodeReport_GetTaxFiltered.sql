CREATE PROCEDURE [dbo].[ZnodeReport_GetTaxFiltered]  
(   @BeginDate       DATE         = NULL,  
 @EndDate         DATE         = NULL,  
 @PortalId        VARCHAR(MAX) = '',  
 @ShipToStateCode VARCHAR(10)  = '')  
AS   
/*  
     Summary:- this procedure is used to get the TAX details   
  Unit Testing:  
     EXEC ZnodeReport_GetTaxFiltered @ShipToStateCode = '' ,@BeginDate = '2015-11-15 13:03:27.700', @EndDate= '2016-11-15 13:03:27.700',@PortalId = '1'  
 */  
   
  BEGIN  
  BEGIN TRY  
         SET NOCOUNT ON;  
  
  SELECT P.[StoreName] AS 'StoreName',  
                ZOOD.OrderDate,  
                OSP.ShipToStateCode,  
                [dbo].[Fn_GetDefaultPriceRoundOff](ZOOD.SubTotal) AS SubTotal,  
                [dbo].[Fn_GetDefaultPriceRoundOff](SUM(ISNULL(ZOTOD.[SalesTax], 0) + ISNULL(ZOTOD.[VAT], 0) + ISNULL(ZOTOD.[GST], 0) + ISNULL(ZOTOD.[PST], 0) + ISNULL(ZOTOD.[HST], 0))) AS TaxCost,  
                SPACE(10) AS Title,SPACE(30) AS Custom1,ISNULL(ZC.Symbol,dbo.Fn_GetDefaultCurrencySymbol()) Symbol  
         FROM ZNodeOmsOrder AS ZOO  
              INNER JOIN ZnodeOmsOrderDetails AS ZOOD ON(ZOOD.OmsOrderId = ZOO.OmsOrderId AND IsActive = 1)  
              INNER JOIN ZNodePortal AS P ON P.PortalID = ZOOD.PortalId  
              LEFT JOIN ZNodeOmsOrderShipment AS OSP ON(EXISTS  
                                                       (  
                                                           SELECT TOP 1 1  
                                                           FROM ZnodeOmsOrderLineItems AS ZOOLI  
                                                           WHERE ZOOLI.OmsOrderShipmentId = OSP.OmsOrderShipmentId  
                                                                 AND ZOOLI.OmsOrderDetailsId = ZOOD.OmsOrderDetailsId  
                                                       ))  
              LEFT JOIN ZnodeOmsTaxOrderDetails AS ZOTOD ON(ZOTOD.OmsOrderDetailsId = ZOOD.OmsOrderDetailsId)  
      --LEFT JOIN ZnodeCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode)  
	       LEFT JOIN ZnodeCulture ZC ON (ZC.CultureCode = ZOOD.CurrencyCode)  
         WHERE((EXISTS  
               (  
                   SELECT TOP 1 1  
                   FROM dbo.split(@PortalId, ',') AS SP  
                   WHERE CAST(P.PortalId AS VARCHAR(100)) = SP.Item  
                         OR @PortalId = ''  
               )))  
              AND (CAST(ZOOD.OrderDate AS DATE) BETWEEN CASE  
                                                            WHEN @BeginDate IS NULL  
                                                            THEN CAST(ZOOD.OrderDate AS DATE)  
                                                            ELSE @BeginDate  
                                                        END AND CASE  
                                                                    WHEN @EndDate IS NULL  
                                                                    THEN CAST(ZOOD.OrderDate AS DATE)  
                                                                    ELSE @EndDate  
                                                                END)  
              AND (OSP.ShipToStateCode LIKE '%'+@ShipToStateCode+'%' OR @ShipToStateCode = '')  
          
   GROUP BY P.[StoreName],ZOOD.OrderDate,OSP.ShipToStateCode,ZOOD.SubTotal,ZC.Symbol  
   HAving SUM(ISNULL(ZOTOD.[SalesTax], 0) + ISNULL(ZOTOD.[VAT], 0) + ISNULL(ZOTOD.[GST], 0) + ISNULL(ZOTOD.[PST], 0) + ISNULL(ZOTOD.[HST], 0)) > 0   
   ;  
  
   END TRY  
   BEGIN CATCH  
   DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetTaxFiltered @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@ShipToStateCode='+@ShipToStateCode+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'ZnodeReport_GetTaxFiltered',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
   END CATCH  
     END;