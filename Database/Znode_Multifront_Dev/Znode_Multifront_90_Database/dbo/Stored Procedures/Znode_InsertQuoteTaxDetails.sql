CREATE PROCEDURE [dbo].[Znode_InsertQuoteTaxDetails]
(
	@OmsQuoteId INT,
	@TaxRuleId INT,
	@TaxRate NUMERIC(28,6),
	@status BIT OUT
)
AS
/* Summary :- This Procedure is used to insert the  TaxRule Data
Unit Testing
EXEC [Znode_InsertQuoteTaxDetails] @OmsQuoteId = 1467, @TaxRuleId = 10
*/
BEGIN
SET NOCOUNT ON;

BEGIN TRY
BEGIN TRAN
DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

	--------Insert TaxRule Data
	IF(NOT EXISTS(SELECT 1 from ZnodeOmsQuoteTaxRule where OmsQuoteId= @OmsQuoteId))
	BEGIN
		INSERT INTO ZnodeOmsQuoteTaxRule
		(
			OmsQuoteId,TaxRuleId,TaxRuleTypeId,TaxClassId,DestinationCountryCode,DestinationStateCode,CountyFIPS,Precedence, SalesTax,
			VAT,GST,PST,HST,TaxShipping,Custom1,Custom2,Custom3,ExternalID,ZipCode,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
		)
		--OUTPUT INSERTED.OmsOrderId INTO @ZnodeOmsQuoteTaxRule
		SELECT @OmsQuoteId,TaxRuleId,TaxRuleTypeId,TaxClassId,DestinationCountryCode,DestinationStateCode,CountyFIPS,Precedence,
			(CASE When @TaxRate > 0 then @TaxRate ELSE SalesTax END) AS SalesTax,VAT,GST,PST,HST,TaxShipping,Custom1,Custom2,Custom3,
			ExternalID,ZipCode,CreatedBy,@GetDate ,ModifiedBy,@GetDate
		FROM ZnodeTaxRule
		WHERE TaxRuleId = @TaxRuleId
	END

SET @status = 1;
COMMIT TRAN
END TRY
BEGIN CATCH
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertQuoteTaxDetails @OmsQuoteId = '+CAST(@OmsQuoteId AS VARCHAR(10))+',@TaxRuleId='+CAST(@TaxRuleId AS VARCHAR(50))
             
	ROLLBACK TRAN
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_InsertQuoteTaxDetails',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH;
END
GO
