CREATE PROCEDURE Znode_DeleteOmsQuoteLineItem
(
	@OmsQuoteLineItemId INT,
	@ParentOmsQuoteLineItemId INT,
	@Status BIT OUT
)
AS
BEGIN
BEGIN TRY
BEGIN TRAN DeleteOmsQuoteLineItem
	--Deleting child line item
	DELETE FROM ZnodeOmsQuoteLineItem WHERE OmsQuoteLineItemId = @OmsQuoteLineItemId
	--Deleting parent line item if no child is present in it
	DELETE FROM ZnodeOmsQuoteLineItem WHERE OmsQuoteLineItemId = @ParentOmsQuoteLineItemId
	AND NOT EXISTS(SELECT * FROM ZnodeOmsQuoteLineItem WHERE ParentOmsQuoteLineItemId = @ParentOmsQuoteLineItemId)
COMMIT TRAN DeleteOmsQuoteLineItem
	SET @Status = 1
	SELECT 1 ID, CAST(@Status AS BIT) [Status];
END TRY
BEGIN CATCH
ROLLBACK TRAN DeleteOmsQuoteLineItem
	SET @Status = 0
	SELECT 1 ID, CAST(@Status AS BIT) [Status];
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteOmsQuoteLineItem @OmsQuoteLineItemId = '+CAST(@OmsQuoteLineItemId AS VARCHAR(200))+', @ParentOmsQuoteLineItemId='+CAST(@ParentOmsQuoteLineItemId AS VARCHAR(200));


    EXEC Znode_InsertProcedureErrorLog
        @ProcedureName = 'Znode_DeleteOmsQuoteLineItem',
        @ErrorInProcedure = @Error_procedure,
        @ErrorMessage = @ErrorMessage,
        @ErrorLine = @ErrorLine,
        @ErrorCall = @ErrorCall;
END CATCH
END