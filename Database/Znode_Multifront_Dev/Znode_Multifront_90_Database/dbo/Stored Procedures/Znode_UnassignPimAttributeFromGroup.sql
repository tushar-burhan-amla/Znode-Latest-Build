-- EXEC [Znode_UnassignPimAttributeFromGroup] 329,0


CREATE PROCEDURE [dbo].[Znode_UnassignPimAttributeFromGroup](
       @PimAttributeId      VARCHAR(1000) ,
       @PimAttributeGroupId INT           = 0 ,
       @IsCategory          BIT           = 0)
AS
/*
Summary: This Procedure is used to Unassign Product Attribute from group
Unit Testing:
EXEC Znode_UnassignPimAttributeFromGroup

*/
     BEGIN
         BEGIN TRAN;
         BEGIN TRY
		 SET NOCOUNT ON
             DECLARE @TBL_PimAttributeId TABLE (
                                               PimAttributeId INT
                                               );
             DECLARE @TBL_DeletedIds TABLE (
                                           PimAttributeId INT
                                           );
             IF @IsCategory = 0
                 BEGIN
                     INSERT INTO @TBL_PimAttributeId
                            SELECT Item
                            FROM dbo.split ( @PimAttributeId , ','
                                           ) AS sp;

                     --WHERE --NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeValue zpav WHERE zpav.PimAttributeId = sp.Item and
                     --EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper zpfm WHERE zpfm.PimAttributeId = sp.Item AND (zpfm.PimAttributeGroupId = @PimAttributeGroupId OR @PimAttributeGroupId = 0))
                     DELETE FROM ZnodePimFamilyGroupMapper
                     WHERE EXISTS ( SELECT TOP 1 1
                                    FROM @TBL_PimAttributeId AS tp
                                    WHERE tp.PimAttributeId = ZnodePimFamilyGroupMapper.PimAttributeId
                                          AND
                                          ( ZnodePimFamilyGroupMapper.PimAttributeGroupId = @PimAttributeGroupId
                                            OR
                                            @PimAttributeGroupId = 0 )
                                  );
                     DELETE FROM ZnodePimAttributeGroupMapper
                     OUTPUT DELETED.PimAttributeId
                            INTO @TBL_DeletedIds
                     WHERE EXISTS ( SELECT TOP 1 1
                                    FROM @TBL_PimAttributeId AS tp
                                    WHERE tp.PimAttributeId = ZnodePimAttributeGroupMapper.PimAttributeId
                                          AND
                                          ( ZnodePimAttributeGroupMapper.PimAttributeGroupId = @PimAttributeGroupId
                                            OR
                                            @PimAttributeGroupId = 0 )
                                  );
                     SELECT 1 AS ID ,
                                 CASE
                                     WHEN ( SELECT COUNT(1)
                                            FROM @TBL_PimAttributeId
                                          ) = 0
                                     THEN CAST(0 AS BIT)
                                     WHEN ( SELECT COUNT(1)
                                            FROM @TBL_PimAttributeId
                                          ) = ( SELECT COUNT(1)
                                                FROM @TBL_DeletedIds
                                              )
                                          OR
                                          NOT EXISTS ( SELECT TOP 1 1
                                                       FROM ZnodePimAttributeGroupMapper AS zpag INNER JOIN @TBL_PimAttributeId AS c ON ( c.PimAttributeId = zpag.PimAttributeId )
                                                     )
                                     THEN CAST(1 AS BIT)
                                     ELSE CAST(0 AS BIT)
                                 END AS Status;
                 END;
             ELSE
                 BEGIN
                     INSERT INTO @TBL_PimAttributeId
                            SELECT Item
                            FROM dbo.split ( @PimAttributeId , ','
                                           ) AS sp;
                     --WHERE --NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryAttributeValue zpav WHERE zpav.PimAttributeId = sp.Item and 
                     --EXISTS (SELECT TOP 1 1 FROM ZnodePimFamilyGroupMapper zpfm WHERE zpfm.PimAttributeId = sp.Item AND (zpfm.PimAttributeGroupId = @PimAttributeGroupId OR @PimAttributeGroupId = 0))

                     DELETE FROM ZnodePimFamilyGroupMapper
                     WHERE EXISTS ( SELECT TOP 1 1
                                    FROM @TBL_PimAttributeId AS tp
                                    WHERE tp.PimAttributeId = ZnodePimFamilyGroupMapper.PimAttributeId
                                          AND
                                          ( ZnodePimFamilyGroupMapper.PimAttributeGroupId = @PimAttributeGroupId
                                            OR
                                            @PimAttributeGroupId = 0 )
                                  );
                     DELETE FROM ZnodePimAttributeGroupMapper
                     OUTPUT DELETED.PimAttributeId
                            INTO @TBL_DeletedIds
                     WHERE EXISTS ( SELECT TOP 1 1
                                    FROM @TBL_PimAttributeId AS tp
                                    WHERE tp.PimAttributeId = ZnodePimAttributeGroupMapper.PimAttributeId
                                          AND
                                          ( ZnodePimAttributeGroupMapper.PimAttributeGroupId = @PimAttributeGroupId
                                            OR
                                            @PimAttributeGroupId = 0 )
                                  );
                     SELECT 1 AS ID ,
                                 CASE
                                     WHEN ( SELECT COUNT(1)
                                            FROM @TBL_PimAttributeId
                                          ) = 0
                                     THEN CAST(0 AS BIT)
                                     WHEN ( SELECT COUNT(1)
                                            FROM @TBL_PimAttributeId
                                          ) = ( SELECT COUNT(1)
                                                FROM @TBL_DeletedIds
                                              )
                                          OR
                                          NOT EXISTS ( SELECT TOP 1 1
                                                       FROM ZnodePimAttributeGroupMapper AS zpag INNER JOIN @TBL_PimAttributeId AS c ON ( c.PimAttributeId = zpag.PimAttributeId )
                                                     )
                                     THEN CAST(1 AS BIT)
                                     ELSE CAST(0 AS BIT)
                                 END AS Status;
                 END;
             COMMIT TRAN;
         END TRY
         BEGIN CATCH
               DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_UnassignPimAttributeFromGroup @PimAttributeId = '+@PimAttributeId+',@PimAttributeGroupId='+CAST(@PimAttributeGroupId AS VARCHAR(50))+',@IsCategory='+CAST(@IsCategory AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_UnassignPimAttributeFromGroup',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;