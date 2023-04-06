CREATE  PROCEDURE [dbo].[Znode_ImportGetPimAttributeDefaultValue] 
AS 
/*
Summary: This Procedure is used to Import Default value of attribute of product
Unit Testing:
EXEC Znode_ImportGetPimAttributeDefaultValue
*/
BEGIN
    BEGIN TRY 
	   SELECT zat.AttributeTypeName,zpav.PimAttributeDefaultValueId , zpa.PimAttributeId , zpav.AttributeDefaultValueCode
	   FROM ZnodePimAttributeDefaultValue AS zpav 
	   RIGHT  OUTER JOIN dbo.ZnodePimAttribute AS zpa ON zpav.PimAttributeId = zpa.PimAttributeId
	   INNER JOIN dbo.ZnodeAttributeType AS zat ON zpa.AttributeTypeId = zat.AttributeTypeId
	   WHERE 
	   zat.AttributeTypeName IN ('Simple Select','Multi Select','Yes/No');

    END TRY
    BEGIN CATCH
	  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportGetPimAttributeDefaultValue @Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportGetPimAttributeDefaultValue',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
    END CATCH;
END