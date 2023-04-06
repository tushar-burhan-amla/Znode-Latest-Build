CREATE PROCEDURE [dbo].[Znode_DeleteGlobalDefaultAttributeValues](
       @GlobalAttributeDefaultValueId VARCHAR(300) ,
       @Status                     BIT OUT)
AS
/*
Summary: This Procedure is used to delete Product Attribute default value
Unit Testing:
EXEC Znode_DeleteGlobalDefaultAttributeValues '108',0

*/
     BEGIN
         BEGIN TRAN;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DeletedValues TABLE (
                                          GlobalAttributeDefaultValueId INT,
										  GlobalAttributeId    int
                                          );
             INSERT INTO @DeletedValues
                    SELECT Item,c.GlobalAttributeId
                    FROM dbo.Split ( @GlobalAttributeDefaultValueId , ','
                                   ) AS a INNER JOIN ZnodeGlobalAttributeDefaultValue AS b ON ( b.GlobalAttributeDefaultValueId = a.item )
                                          INNER JOIN ZnodeGlobalAttribute AS c ON ( c.GlobalAttributeId = b.GlobalAttributeId )
                    WHERE 
					
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodeGlobalAttributeValue AS d
                                       WHERE b.GlobalAttributeDefaultValueId = d.GlobalAttributeDefaultValueId
                                     )
					and dbo.[Fn_CheckGlobalAttributeTransactionUsed]('GlobalAttributeDefaultValue',a.Item)=0

             DELETE FROM ZnodeGlobalAttributeDefaultValueLocale
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletedValues AS a
                            WHERE a.GlobalAttributeDefaultValueId = ZnodeGlobalAttributeDefaultValueLocale.GlobalAttributeDefaultValueId
                          );
             DELETE FROM ZnodeGlobalAttributeDefaultValue
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletedValues AS a
                            WHERE a.GlobalAttributeDefaultValueId = ZnodeGlobalAttributeDefaultValue.GlobalAttributeDefaultValueId
                          );
             
			 update aa
			 Set aa.IsActive=0
			 from ZnodeGlobalAttribute aa
			 inner join View_ZnodeGlobalAttribute vv on vv.GlobalAttributeId=aa.GlobalAttributeId
			 and vv.GroupAttributeType='Select'
			 WHere EXISTS ( SELECT TOP 1 1
                            FROM @DeletedValues AS a
                            WHERE a.GlobalAttributeId = aa.GlobalAttributeId
                          )
			and not EXISTS ( SELECT TOP 1 1
                            FROM ZnodeGlobalAttributeDefaultValue AS a
                            WHERE a.GlobalAttributeId = aa.GlobalAttributeId
                          )


             IF ( SELECT COUNT(1)
                  FROM @DeletedValues
                ) = ( SELECT COUNT(1)
                      FROM dbo.split ( @GlobalAttributeDefaultValueId , ','
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
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteGlobalDefaultAttributeValues @GlobalAttributeDefaultValueId = '+@GlobalAttributeDefaultValueId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeleteGlobalDefaultAttributeValues',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
     END CATCH;
     END;