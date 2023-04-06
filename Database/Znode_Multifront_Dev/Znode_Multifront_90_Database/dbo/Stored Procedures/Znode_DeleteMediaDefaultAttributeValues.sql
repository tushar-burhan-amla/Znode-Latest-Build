
CREATE PROCEDURE [dbo].[Znode_DeleteMediaDefaultAttributeValues](
       @MediaAttributeDefaultValueId VARCHAR(300) ,
       @Status                       BIT OUT)
AS
/*
Summary: This Procedure is used to delete Media Default AttributeValue with their reference data 
Unit Testing:
 SELECT * FROM ZnodePimAttribute WHERE PimAttributeId = 181
 SELECT * FROM ZnodeMediaAttributeDefaultValue WHERE MediaAttributeDefaultValueId = 108
 SELECT * FROM ZnodeMediaAttributeDefaultValueLocale  
 EXEC Znode_DeletePimDefaultAttributeValues '108',0

*/
     BEGIN
         BEGIN TRAN;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DeletedValues TABLE (
                                          MediaAttributeDefaultValueId INT
                                          );
             INSERT INTO @DeletedValues
                    SELECT Item
                    FROM dbo.Split ( @MediaAttributeDefaultValueId , ','
                                   ) AS a INNER JOIN ZnodeMediaAttributeDefaultValue AS b ON ( b.MediaAttributeDefaultValueId = a.item )
                                          INNER JOIN ZnodeMediaAttribute AS c ON ( c.MediaAttributeId = b.MediaAttributeId )
                    WHERE c.IsSystemDefined <> 1
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodeMediaAttributeValue AS d
                                       WHERE b.MediaAttributeDefaultValueId = d.MediaAttributeDefaultValueId
                                     );
             DELETE FROM ZnodeMediaAttributeDefaultValueLocale
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletedValues AS a
                            WHERE a.MediaAttributeDefaultValueId = ZnodeMediaAttributeDefaultValueLocale.MediaAttributeDefaultValueId
                          );
             DELETE FROM ZnodeMediaAttributeDefaultValue
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletedValues AS a
                            WHERE a.MediaAttributeDefaultValueId = ZnodeMediaAttributeDefaultValue.MediaAttributeDefaultValueId
                          );
             IF ( SELECT COUNT(1)
                  FROM @DeletedValues
                ) = ( SELECT COUNT(1)
                      FROM dbo.split ( @MediaAttributeDefaultValueId , ','
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
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteMediaDefaultAttributeValues @MediaAttributeDefaultValueId = '+@MediaAttributeDefaultValueId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN;;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeleteMediaDefaultAttributeValues',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;