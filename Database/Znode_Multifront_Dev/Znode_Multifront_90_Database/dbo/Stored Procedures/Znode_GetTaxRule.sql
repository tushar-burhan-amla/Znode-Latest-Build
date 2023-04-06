CREATE PROCEDURE Znode_GetTaxRule
(
	@SKU [dbo].[SelectColumnList] READONLY
)
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRY

	SELECT ZTC.[TaxClassId] AS [TaxClassId], ZTCSku.[SKU] AS [SKU], ZTR.[DestinationCountryCode] AS [DestinationCountryCode]
	FROM   [dbo].[ZnodeTaxClass] AS ZTC
	INNER JOIN [dbo].[ZnodeTaxClassSKU] AS ZTCSku ON ZTC.[TaxClassId] = ZTCSku.[TaxClassId]
	INNER JOIN [dbo].[ZnodeTaxRule] AS ZTR ON ZTC.[TaxClassId] = ZTR.[TaxClassId]
	INNER JOIN [dbo].[ZnodeTaxRuleTypes] AS ZTRT ON ZTR.[TaxRuleTypeId] = ZTRT.[TaxRuleTypeId]
	WHERE ZTC.[IsActive] = 1 AND ZTCSku.[SKU] IS NOT NULL
	AND ZTRT.[IsActive] = 1
	AND EXISTS(SELECT * FROM @SKU Sku WHERE ZTCSku.[SKU] = Sku.[StringColumn])

END TRY
	BEGIN CATCH

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetTaxRule ';                 
		  
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetTaxRule',
		@ErrorInProcedure = 'Znode_GetTaxRule',
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH;
END