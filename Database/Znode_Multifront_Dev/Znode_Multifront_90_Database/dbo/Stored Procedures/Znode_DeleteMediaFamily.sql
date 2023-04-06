
CREATE PROCEDURE [dbo].[Znode_DeleteMediaFamily](
       @MediaAttributeFamilyId VARCHAR(300) = NULL ,
       @Status                 INT OUT)
AS
/*
Summary: This Procedure is used to delete MediaAttribute from family with their reference data
Unit Testing:
begin tran
 DECLARE @DD INT = NULL  
EXEC Znode_DeleteMediaFamily '51',@DD OUT
rollback tran 
 SELECT * FROM ZnodePimAttributeFamily

*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN A;
             DECLARE @MediaAttributeFamily TABLE (
                                                 Item INT
                                                 );
             INSERT INTO @MediaAttributeFamily
                    SELECT Item
                    FROM dbo.split ( @MediaAttributeFamilyId , ','
                                   ) AS asa
                    WHERE NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodeMediaCategory AS dd
                                       WHERE dd.MediaAttributeFamilyId = asa.Item
                                     );
             DECLARE @V_tabledeleted TABLE (
                                           MediaAttributeFamilyId INT
                                           );
             DELETE FROM ZnodeMediaFamilyLocale
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @MediaAttributeFamily AS a
                            WHERE a.item = ZnodeMediaFamilyLocale.MediaAttributeFamilyId
                                  AND
                                  a.Item NOT IN ( SELECT MediaAttributeFamilyId
                                                  FROM ZnodeMediaAttributeFamily
                                                  WHERE IsSystemDefined = 1
                                                )
                          );
             DELETE FROM ZnodeMediaFamilyGroupMapper
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @MediaAttributeFamily AS a
                            WHERE a.item = ZnodeMediaFamilyGroupMapper.MediaAttributeFamilyId
                                  AND
                                  a.Item NOT IN ( SELECT MediaAttributeFamilyId
                                                  FROM ZnodeMediaAttributeFamily
                                                  WHERE IsSystemDefined = 1
                                                )
                          );
             DELETE FROM ZnodeMediaAttributeFamily
             OUTPUT deleted.MediaAttributeFamilyId
                    INTO @V_tabledeleted
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @MediaAttributeFamily AS a
                            WHERE a.item = ZnodeMediaAttributeFamily.MediaAttributeFamilyId
                                  AND
                                  a.Item NOT IN ( SELECT MediaAttributeFamilyId
                                                  FROM ZnodeMediaAttributeFamily
                                                  WHERE IsSystemDefined = 1
                                                )
                          );
             IF ( SELECT COUNT(1)
                  FROM @V_tabledeleted
                ) = ( SELECT COUNT(1)
                      FROM dbo.split ( @MediaAttributeFamilyId , ','
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
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
            
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteMediaFamily @MediaAttributeFamilyId = '+@MediaAttributeFamilyId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeleteMediaFamily',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;