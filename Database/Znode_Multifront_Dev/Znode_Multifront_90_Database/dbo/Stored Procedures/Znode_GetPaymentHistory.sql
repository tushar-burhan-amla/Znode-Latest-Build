CREATE PROCEDURE [dbo].[Znode_GetPaymentHistory]
( 
	@OrderId  INT
)
AS 
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
	   SELECT OP.OrderPaymentId, OP.OmsOrderId, OP.TransactionReference,	OP.Amount, OP.TransactionStatus, OP.TransactionDate, 
			OP.PaymentSettingId, OP.RemainingOrderAmount, OP.CreatedDate, OP.ModifiedDate, PT.PaymentDisplayName as PaymentType
	   FROM ZnodeOrderPayment OP
	   INNER JOIN ZnodeOmsOrderDetails OOD on OP.OmsOrderId = OOD.OmsOrderId
	   INNER JOIN ZnodePaymentSetting PT on OP.PaymentSettingId = PT.PaymentSettingId
	   WHERE OP.OmsOrderId=@OrderId and OOD.IsActive=1
	END TRY
    BEGIN CATCH
		DECLARE @Status BIT ;
		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
		@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(), 
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPaymentHistory @OrderId = '+CAST(@OrderId AS VARCHAR(50));
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetPaymentHistory',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
    END CATCH;
END