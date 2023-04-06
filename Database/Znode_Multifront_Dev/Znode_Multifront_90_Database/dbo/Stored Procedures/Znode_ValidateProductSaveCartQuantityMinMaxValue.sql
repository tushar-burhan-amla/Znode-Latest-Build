CREATE PROCEDURE [dbo].[Znode_ValidateProductSaveCartQuantityMinMaxValue]
(
	@ProductQuantityValidate DBO.ProductQuantityValidation READONLY,
	@OmsCookieMappingId INT,
	@Status BIT = 0 OUT
)
--EXECUTE Znode_ValidateProductSaveCartQuantityMinMaxValue @SKU = 'ap1234',@Quantity=1,@OmsCookieMappingId=1
AS
BEGIN
BEGIN TRY 
SET NOCOUNT ON;

		IF OBJECT_ID('tempdb..#GroupProductQuantity') IS NOT NULL
			DROP TABLE #ProductQuantity

		CREATE TABLE #ProductQuantity(SKU VARCHAR(600),Quantity NUMERIC(28,6),Status BIT)

		--To get the quantity for save car for group poducts
		INSERT INTO #ProductQuantity(SKU ,Quantity ,Status)
		SELECT S.SKU,ISNULL(S.Quantity,0)+CASE WHEN ISNULL(S.IsEditCart,0) = 1  THEN 0 ELSE ISNULL(SUM(ISNULL(SCLI.Quantity,0)),0) END AS Quantity, 0 AS Status
		FROM @ProductQuantityValidate S
		LEFT JOIN ZnodeOmsSavedCartLineItem SCLI WITH (NOLOCK) ON SCLI.SKU = S.SKU
		AND EXISTS(SELECT * FROM ZnodeOmsSavedCart SC WITH (NOLOCK) WHERE SC.OmsCookieMappingId = @OmsCookieMappingId AND SCLI.OmsSavedCartId = SC.OmsSavedCartId)
		GROUP BY S.SKU,S.Quantity,S.IsEditCart

		--Validating Quantity for group poducts
		UPDATE P set Status = 1 
		FROM #ProductQuantity P
		INNER JOIN @ProductQuantityValidate S on S.sku = P.SKU 
		WHERE P.Quantity BETWEEN S.MinimumQuantity AND S.MaximumQuantity

		IF EXISTS(SELECT * FROM #ProductQuantity WHERE Status = 0)
			SET @Status = 0
		ELSE
			SET @Status = 1

	SELECT 1 AS ID,CAST(@Status AS BIT) AS Status;  

END TRY    
BEGIN CATCH      
	SET @Status = 0;    
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= 
	ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 
	'EXEC Znode_ValidateProductSaveCartQuantityMinMaxValue @OmsCookieMappingId ='+CAST(@OmsCookieMappingId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                       
        
	EXEC Znode_InsertProcedureErrorLog    
	@ProcedureName = 'Znode_ValidateProductSaveCartQuantityMinMaxValue',    
	@ErrorInProcedure = @Error_procedure,    
	@ErrorMessage = @ErrorMessage,    
	@ErrorLine = @ErrorLine,    
	@ErrorCall = @ErrorCall;                                
END CATCH; 
END