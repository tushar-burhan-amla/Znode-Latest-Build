
CREATE PROCEDURE [dbo].[Znode_DeletePimDefaultAttributeValues](
       @PimAttributeDefaultValueId VARCHAR(300) ,
       @Status                     BIT OUT)
AS
/*
Summary: This Procedure is used to delete Product Attribute default value
Unit Testing:
EXEC Znode_DeletePimDefaultAttributeValues '108',0

*/
     BEGIN
         BEGIN TRAN;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DeletedValues TABLE (
                                          PimAttributeDefaultValueId INT
                                          );
             INSERT INTO @DeletedValues
                    SELECT Item
                    FROM dbo.Split ( @PimAttributeDefaultValueId , ','
                                   ) AS a INNER JOIN ZnodePimAttributeDefaultValue AS b ON ( b.PimAttributeDefaultValueId = a.item )
                                          INNER JOIN ZnodePimAttribute AS c ON ( c.PimAttributeId = b.PimAttributeId )
                    WHERE 
					
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePimAttributeValue AS d
                                       WHERE b.PimAttributeDefaultValueId = d.PimAttributeDefaultValueId
                                     );
             DELETE FROM ZnodePimAttributeDefaultValueLocale
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletedValues AS a
                            WHERE a.PimAttributeDefaultValueId = ZnodePimAttributeDefaultValueLocale.PimAttributeDefaultValueId
                          );
             DELETE FROM ZnodePimAttributeDefaultValue
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletedValues AS a
                            WHERE a.PimAttributeDefaultValueId = ZnodePimAttributeDefaultValue.PimAttributeDefaultValueId
                          );
             IF ( SELECT COUNT(1)
                  FROM @DeletedValues
                ) = ( SELECT COUNT(1)
                      FROM dbo.split ( @PimAttributeDefaultValueId , ','
                                     )
                    )
                 BEGIN
                     SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
                 END;
             SET @Status = 1;
             COMMIT TRAN;
         END TRY
         BEGIN CATCH
            
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePimDefaultAttributeValues @PimAttributeDefaultValueId = '+@PimAttributeDefaultValueId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeletePimDefaultAttributeValues',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;