CREATE PROCEDURE [dbo].[Znode_InsertUpdateOmsOrderPayment]
(
	@OrderPaymentXML XML,
	@OrderId INT,
	@TransactionDate DATETIME,
	@UserId INT,
	@Status BIT = 0 OUT
)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRAN OrderInsert
	BEGIN TRY

		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

		IF OBJECT_ID('tempdb..#TempOrderPaymentData') IS NOT NULL
			DROP TABLE #TempOrderPaymentData
	
		CREATE TABLE #TempOrderPaymentData
			(OmsOrderId INT, TransactionReference NVARCHAR(50), Amount NUMERIC(28,6), TransactionStatus NVARCHAR(50), PaymentSettingId INT, RemainingOrderAmount NUMERIC(28,6))
	
		--------Getting Order Payment details
		INSERT INTO #TempOrderPaymentData
			(OmsOrderId, TransactionReference, Amount, TransactionStatus , PaymentSettingId, RemainingOrderAmount)

		SELECT
			CASE WHEN Tbl.Col.value( 'OmsOrderId[1]', 'NVARCHAR(2000)' ) = '' THEN NULL ELSE Tbl.Col.value( 'OmsOrderId[1]', 'NVARCHAR(2000)' ) END AS OmsOrderId,
			Tbl.Col.value( 'TransactionReference[1]', 'NVARCHAR(Max)' ) AS TransactionReference,
			CASE WHEN Tbl.Col.value( 'Total[1]', 'NVARCHAR(Max)' ) = '' THEN NULL ELSE Tbl.Col.value( 'Total[1]', 'NVARCHAR(Max)' ) END AS Amount,
			CASE WHEN Tbl.Col.value( 'TransactionStatus[1]', 'NVARCHAR(Max)' )='' THEN NULL ELSE Tbl.Col.value( 'TransactionStatus[1]', 'NVARCHAR(Max)' ) END AS TransactionStatus,
			CASE WHEN Tbl.Col.value( 'PaymentSettingId[1]', 'NVARCHAR(Max)' ) = '' THEN NULL ELSE Tbl.Col.value( 'PaymentSettingId[1]', 'NVARCHAR(Max)' ) END  AS PaymentGatewayId,
			CASE WHEN Tbl.Col.value( 'RemainingOrderAmount[1]', 'NVARCHAR(Max)' ) = '' THEN NULL ELSE Tbl.Col.value( 'RemainingOrderAmount[1]', 'NVARCHAR(Max)' ) END AS RemainingOrderAmount

		FROM @OrderPaymentXML.nodes( '//OrderPaymentDataModel' ) AS Tbl(Col);
	
		INSERT INTO ZnodeOrderPayment
			(OmsOrderId, TransactionReference, Amount, TransactionStatus, TransactionDate, PaymentSettingId , CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, RemainingOrderAmount)
		SELECT OmsOrderId, TransactionReference, Amount, TransactionStatus, @TransactionDate, PaymentSettingId, @UserId, @GetDate, @UserId, @GetDate, RemainingOrderAmount
		FROM #TempOrderPaymentData 

		SET @Status = 1;
		SELECT 1 AS Id,@Status AS Status;
	COMMIT TRAN OrderInsert
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN OrderInsert
		SET @Status = 0;

		SELECT 0 AS Id, @Status AS Status; 

		DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdateOmsOrderPayment @OrderPaymentXML ='+CAST(@OrderPaymentXML AS VARCHAR(MAX))+' ,@OrderId = '+CAST(@OrderId AS VARCHAR(50))
					+',@TransactionDate = '+CAST(@TransactionDate AS VARCHAR(50))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status = '+CAST(@Status AS VARCHAR(50));

		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName    = 'Znode_InsertUpdateOmsOrderPayment',
		@ErrorInProcedure = @ERROR_PROCEDURE,
		@ErrorMessage     = @ErrorMessage,
		@ErrorLine        = @ErrorLine,
		@ErrorCall        = @ErrorCall;
	END CATCH
END