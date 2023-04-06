
CREATE PROCEDURE [dbo].[Znode_DeletePimFamily](
       @PimAttributeFamilyId VARCHAR(100) = NULL ,
       @Status               INT OUT)
AS
/*
Summary: This Procedure is used to delete Product family
Unit Testing:
EXEC Znode_DeletePimFamily '108',0
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN A;
             DECLARE @V_tabledeleted TABLE (
                                           PimAttributeFamilyId INT
                                           );
             INSERT INTO @V_tabledeleted
                    SELECT b.PimAttributeFamilyId
                    FROM dbo.Split ( @PimAttributeFamilyId , ','
                                   ) AS a INNER JOIN ZnodePimAttributeFamily AS b ON ( a.Item = b.PimAttributeFamilyId )
                    WHERE b.IsSystemDefined <> 1
                          AND
                          b.IsDefaultFamily <> 1
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePimAttributeValue AS c
                                       WHERE c.PimAttributeFamilyId = b.PimAttributeFamilyId
                                     )
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodePimCategoryAttributeValue AS d
                                       WHERE d.PimAttributeFamilyId = b.PimAttributeFamilyId
                                     );
             DELETE FROM ZnodePimFamilyLocale
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @V_tabledeleted AS c
                            WHERE c.PimAttributeFamilyId = ZnodePimFamilyLocale.PimAttributeFamilyId
                          );
             DELETE FROM ZnodePimFamilyGroupMapper
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @V_tabledeleted AS c
                            WHERE c.PimAttributeFamilyId = ZnodePimFamilyGroupMapper.PimAttributeFamilyId
                          );
             DELETE FROM ZnodePimAttributeFamily
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @V_tabledeleted AS c
                            WHERE c.PimAttributeFamilyId = ZnodePimAttributeFamily.PimAttributeFamilyId
                          );
             IF ( SELECT COUNT(1)
                  FROM @V_tabledeleted
                ) = ( SELECT COUNT(1)
                      FROM dbo.split ( @PimAttributeFamilyId , ','
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
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePimFamily @PimAttributeFamilyId = '+@PimAttributeFamilyId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeletePimFamily',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;