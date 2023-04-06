
CREATE PROCEDURE [dbo].[Znode_DeleteMediaGroups](
       @MediaAttributeGroupIds VARCHAR(500) ,
       @Status                 BIT OUT)
AS
/*
Summary: This Procedure is used to delete MediaGroup with their respective data from all reference tables
Unit Testing:
begin tran
Exec Znode_DeleteMediaGroups 1
rollbcak tran

*/
     BEGIN
         BEGIN TRAN;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DeleteGroupId TABLE (
                                          MediaAttributeGroupId INT
                                          );
             INSERT INTO @DeleteGroupId
                    SELECT MediaAttributeGroupId
                    FROM [dbo].[ZnodeMediaAttributeGroup] AS a INNER JOIN dbo.Split ( @MediaAttributeGroupIds , ','
                                                                                    ) AS b ON ( a.MediaAttributeGroupId = b.Item )
                    WHERE a.IsSystemDefined != 1
                          --AND
                          --NOT EXISTS ( SELECT TOP 1 1
                          --             FROM ZnodeMediaFamilyGroupMapper AS d
                          --             WHERE d.MediaAttributeGroupId = a.MediaAttributeGroupId
                          --           );
             DELETE FROM ZnodeMediaFamilyGroupMapper
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeleteGroupId AS s
                            WHERE s.MediaAttributeGroupId = ZnodeMediaFamilyGroupMapper.MediaAttributeGroupId
                          );
             DELETE FROM ZnodeMediaAttributeGroupMapper
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeleteGroupId AS s
                            WHERE s.MediaAttributeGroupId = ZnodeMediaAttributeGroupMapper.MediaAttributeGroupId
                          );
             DELETE FROM ZnodeMediaAttributeGroupLocale
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeleteGroupId AS s
                            WHERE s.MediaAttributeGroupId = ZnodeMediaAttributeGroupLocale.MediaAttributeGroupId
                          );
             DELETE FROM ZnodeMediaAttributeGroup
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeleteGroupId AS s
                            WHERE s.MediaAttributeGroupId = ZnodeMediaAttributeGroup.MediaAttributeGroupId
                          );
             SET @Status = 1;
             IF ( SELECT COUNT(1)
                  FROM @DeleteGroupId
                ) = ( SELECT COUNT(1)
                      FROM dbo.Split ( @MediaAttributeGroupIds , ','
                                     )
                    )
                 BEGIN
                     SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
                 END;
             COMMIT TRAN;
         END TRY
         BEGIN CATCH
             
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteMediaGroups @MediaAttributeGroupIds = '+@MediaAttributeGroupIds+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeleteMediaGroups',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;