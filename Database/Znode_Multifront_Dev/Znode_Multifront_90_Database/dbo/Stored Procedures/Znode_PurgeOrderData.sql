CREATE PROCEDURE [dbo].[Znode_PurgeOrderData]
(
	@DeleteAllOrder BIT = 0
)
AS
BEGIN 
BEGIN TRY
	SET NOCOUNT ON;

	--Delete return data against order
	DELETE FROM ZnodeRmaRequestItem
	DELETE FROM ZnodeRmaRequest
	DELETE FROM ZnodeRmaReturnNotes
	DELETE FROM ZnodeRmaReturnEmailHistory
	DELETE FROM ZnodeRmaReturnHistory
	DELETE FROM ZnodeRmaReturnPaymentRefund
	DELETE FROM ZnodeRmaReturnPaymentDetails
	DELETE FROM ZnodeRmaReturnLineItems
	DELETE FROM ZnodeRmaReturnDetails

	--Delete order data
	DECLARE @Loop int
	SET @Loop = 1

	WHILE @Loop = 1
	BEGIN
		
		DECLARE @DeletedIds TransferId 
		delete from @DeletedIds
		INSERT INTO @DeletedIds 
		SELECT top 4000 OmsOrderID 
		FROM ZnodeOmsOrder  ZP 

		EXEC [dbo].[Znode_DeleteOrderById] @OmsOrderIds = @DeletedIds , @status = 0  

		IF EXISTS(SELECT * FROM @DeletedIds)
			SET @Loop = 1
		ELSE 
			SET @Loop = 0

	END
END TRY
BEGIN CATCH
	SELECT ERROR_MESSAGE()	
	DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max) = 'EXEC Znode_PurgeOrderData'
	SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
	EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_PurgeOrderData', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
END CATCH;
END