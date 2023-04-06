
CREATE PROCEDURE [dbo].[Znode_GetAttributeValidationDetails]
( @AttributeCode VARCHAR(MAX) = NULL)

AS

/*
   Summary : Get Attribute Details when AttibuteCode is passed  
   Unit Testing:
   EXEC [Znode_GetAttributeValidationDetails] 'File' 
   SELECT * FROM ZnodeMediaAttribute
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             SELECT MediaAttributeId,AttributeTypeId,AttributeTypeName,AttributeCode,IsRequired,IsLocalizable,IsFilterable,AttributeName,ControlName,ValidationName,SubValidationName,ValidationValue,RegExp,IsRegExp,ISNULL(NULL, 0) AS RowId
             FROM View_AttributeValidationList
             WHERE EXISTS
             (
                 SELECT 1
                 FROM dbo.Split(ISNULL(@AttributeCode, AttributeCode), ',') AS e
                 WHERE e.Item = AttributeCode
             );
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAttributeValidationDetails @AttributeCode = '+@AttributeCode+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		   
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetAttributeValidationDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END; 