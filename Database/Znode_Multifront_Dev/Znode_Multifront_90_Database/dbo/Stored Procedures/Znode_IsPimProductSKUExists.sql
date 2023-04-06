
CREATE PROCEDURE [dbo].[Znode_IsPimProductSKUExists](
       @SKU          VARCHAR(MAX) ,
       @PimProductId INT ,
       @IsExists     INT          = 0 OUT)
AS
/*
Summary: This Procedure is used to get the status of PimProduct Sku exists or not
Unit Testing:
EXEC Znode_IsPimProductSKUExists 
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             SET @IsExists = 0;
             SELECT TOP 1 @IsExists = 1
             FROM ZnodePimAttribute AS zpa INNER JOIN ZnodePimAttributeValue AS val ON zpa.PimAttributeId = VAl.PimAttributeId
                                           INNER JOIN ZnodePimAttributeValueLocale AS loc ON loc.PimAttributeValueId = val.PimAttributeValueId
             WHERE zpa.AttributeCode = 'SKU'
                   AND
                   loc.AttributeValue = @SKU
                   AND
                   Val.PimProductId <> @PimProductId;
             SELECT 1 AS ID , CAST(@IsExists AS BIT) AS Status;
         END TRY
         BEGIN CATCH
		     DECLARE @Status BIT ;
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			  @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_IsPimProductSKUExists @SKU = '+@SKU+',@PimProductId='+CAST(@PimProductId AS VARCHAR(200))+',@IsExists='+CAST(@IsExists AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_IsPimProductSKUExists',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
            
         END CATCH;
     END;