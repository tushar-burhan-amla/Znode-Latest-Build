


CREATE PROCEDURE [dbo].[Znode_ImportGetGlobalAttributeDefaultValue] 
AS 
/*
Summary: This Procedure is used to Import Default value of global attribute of product
Unit Testing:
EXEC Znode_ImportGetGlobalAttributeDefaultValue
*/
BEGIN
    BEGIN TRY 

	   SELECT zat.AttributeTypeName,zpav.GlobalAttributeDefaultValueId , zpa.GlobalAttributeId , zpav.AttributeDefaultValueCode
	   FROM ZnodeGlobalAttributeDefaultValue AS zpav 
	   RIGHT  OUTER JOIN dbo.ZnodeGlobalAttribute AS zpa ON zpav.GlobalAttributeId = zpa.GlobalAttributeId
	   INNER JOIN dbo.ZnodeAttributeType AS zat ON zpa.AttributeTypeId = zat.AttributeTypeId
	   WHERE zat.AttributeTypeName IN ('Simple Select','Multi Select','Yes/No');

    END TRY
    BEGIN CATCH
	  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportGetGlobalAttributeDefaultValue @Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportGetGlobalAttributeDefaultValue',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
    END CATCH;
END