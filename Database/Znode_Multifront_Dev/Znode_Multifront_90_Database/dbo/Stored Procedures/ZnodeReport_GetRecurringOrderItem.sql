CREATE PROCEDURE [dbo].[ZnodeReport_GetRecurringOrderItem]  
(@OmsOrderId INT)  
AS  
/*  
     Summary :- This Procedure is used to find the ordeer line item for Recurring billing   
     Unit Testing   
     EXEC ZnodeReport_GetRecurringOrderItem 12  
*/  
     BEGIN  
  BEGIN TRY  
  SET NOCOUNT ON  
         SELECT ZOOLI.[ProductName] AS 'Product Name',  
                dbo.Fn_GetDefaultPriceRoundOff(ZOOLI.Price) AS 'Unit Price',  
                dbo.Fn_GetDefaultInventoryRoundOff(ZOOLI.[Quantity]) [Quantity],  
                dbo.Fn_GetDefaultPriceRoundOff(ZOOLI.[Quantity] * ZOOLI.[Price]) AS 'Price',  
                dbo.Fn_GetDefaultPriceRoundOff(ZOOLI.[RecurringBillingAmount]) AS 'Billing Amount',  
                CASE  
                    WHEN ZOOLI.[RecurringBillingFrequency] = 1  
                    THEN 'Day'  
                    WHEN ZOOLI.[RecurringBillingFrequency] = 7  
                    THEN 'Week'  
                    WHEN ZOOLI.[RecurringBillingFrequency] = 30  
                    THEN 'Month'  
                    WHEN ZOOLI.[RecurringBillingFrequency] = 360  
                    THEN 'Year'  
                END AS 'Billing Period',  
                ZOOLI.[TransactionNumber] AS 'Transaction Code'  
    ,ISNULL(ZC.Symbol,dbo.Fn_GetDefaultCurrencySymbol()) Symbol  
         FROM ZNodeOmsOrder ZOO  
              INNER JOIN ZNodeOmsOrderDetails ZOOD ON ZOO.OmsOrderId = ZOOD.OmsOrderId  
              INNER JOIN ZNodeOmsOrderLineItems ZOOLI ON ZOOD.OmsOrderDetailsId = ZOOLI.OmsOrderDetailsId  
     --LEFT JOIN ZnodeCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode)  
	   LEFT JOIN ZnodeCulture ZC ON (ZC.CultureCode = ZOOD.CurrencyCode)  
         WHERE ZOO.OmsOrderId = @OmsOrderId  
               AND ZOOLI.[IsRecurringBilling] = 1;  
  
  END TRY  
  BEGIN CATCH  
   DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetRecurringOrderItem @OmsOrderId='+CAST(@OmsOrderId AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'ZnodeReport_GetRecurringOrderItem',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
  END CATCH  
     END;