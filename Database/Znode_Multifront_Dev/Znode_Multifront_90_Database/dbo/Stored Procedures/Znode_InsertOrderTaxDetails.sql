CREATE PROCEDURE [dbo].[Znode_InsertOrderTaxDetails]
(
	@OmsOrderId INT,
	@TaxRuleId INT,
	@TaxRate    DECIMAL(18, 5),
	@AvataxIsSellerImporterOfRecord bit,
	@status     BIT OUT
)
AS
/* Summary :- This Procedure is used to insert the  TaxRule Data
     Unit Testing
   EXEC [Znode_InsertOrderTaxDetails] @OmsOrderId = 1467, @TaxRuleId = 10
*/
BEGIN
	SET NOCOUNT ON;

	BEGIN TRAN
	BEGIN TRY
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

--------Insert TaxRule Data
		if(NOT EXISTS(SELECT 1 from ZnodeOmsTaxRule where OmsOrderId= @OmsOrderId))
		BEGIN
			INSERT INTO ZnodeOmsTaxRule
			(
			OmsOrderId,TaxRuleId,TaxRuleTypeId,TaxClassId,DestinationCountryCode,DestinationStateCode,CountyFIPS,Precedence,
			SalesTax,VAT,GST,PST,HST,TaxShipping,Custom1,Custom2,Custom3,ExternalID,ZipCode,CreatedBy,CreatedDate,ModifiedBy,
			ModifiedDate,AvataxIsSellerImporterOfRecord
			)
		--OUTPUT INSERTED.OmsOrderId INTO @ZnodeOmsTaxRule
			SELECT
			@OmsOrderId,TaxRuleId,TaxRuleTypeId,TaxClassId,DestinationCountryCode,DestinationStateCode,CountyFIPS,Precedence,
			(CASE When @TaxRate > 0 then @TaxRate ELSE SalesTax END) AS SalesTax,VAT,GST,PST,HST,TaxShipping,Custom1,Custom2,Custom3,
			ExternalID,ZipCode,CreatedBy,@GetDate,ModifiedBy,@GetDate,@AvataxIsSellerImporterOfRecord
			FROM ZnodeTaxRule
			WHERE TaxRuleId = @TaxRuleId
		END
		SET @status = 1;
	COMMIT TRAN
	END TRY
    BEGIN CATCH
       --DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertOrderTaxDetails @OmsOrderId = '+CAST(@OmsOrderId AS VARCHAR(10))+',@TaxRuleId='+CAST(@TaxRuleId AS VARCHAR(50))
             
        --SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		ROLLBACK TRAN
        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_InsertOrderTaxDetails',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
    END CATCH;
END
