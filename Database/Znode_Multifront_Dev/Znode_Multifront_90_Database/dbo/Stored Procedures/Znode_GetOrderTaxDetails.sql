Create PROCEDURE [dbo].[Znode_GetOrderTaxDetails]
(
@OrderId   INT
)
AS
 /*
   Summary :- This procedure is used to get tax  detail with rule associated to particular tax for an Order
   Unit Testing
   EXEC Znode_GetOrderTaxDetails
 */
 BEGIN
  BEGIN TRY
  SET NOCOUNT ON
   
   SELECT TaxRuleId, (isNull(SalesTax, 0 ) + isNull(VAT,0) + isNull(GST,0) + isNull(PST,0) + isNull(HST,0)) as TaxRate, TaxShipping
   FROM ZnodeOmsTaxRule ZOTR
   WHERE (ZOTR.OmsOrderId = @OrderId)

 END TRY
 BEGIN CATCH

DECLARE @Status BIT ;
    SET @Status = 0;
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetOrderTaxDetails @OrderId = '+ cast(@OrderId as varchar (10))+'';
             
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
 
             EXEC Znode_InsertProcedureErrorLog
@ProcedureName = 'Znode_GetOrderTaxDetails',
@ErrorInProcedure = @Error_procedure,
@ErrorMessage = @ErrorMessage,
@ErrorLine = @ErrorLine,
@ErrorCall = @ErrorCall;
 END CATCH
END
GO