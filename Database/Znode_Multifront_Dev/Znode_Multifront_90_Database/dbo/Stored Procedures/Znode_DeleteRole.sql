
CREATE PROCEDURE [dbo].[Znode_DeleteRole]
     (  @RoleIds VARCHAR(500) ,
       @Status  BIT OUT)
AS 
   /* 
    summary : Delete the menu if it is not a parent menu.
    unit testing 
    Begin Tran   
    EXEC Znode_DeleteRole  @RoleIds ='',@Status =0
     
    Rollback tran 
  */
     BEGIN
         BEGIN TRAN;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DeleteRoleId TABLE (
                                         RoleId NVARCHAR(256)
                                         );
             INSERT INTO @DeleteRoleId
                    SELECT a.Id
                    FROM [dbo].[AspNetRoles] AS a INNER JOIN dbo.Split ( @RoleIds , ','
                                                                       ) AS b ON ( a.Id = b.Item )
                    WHERE a.IsSystemDefined != 1
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM AspNetUserRoles AS s
                                       WHERE s.RoleId = a.Id
                                     );
             DELETE FROM ZnodeRoleMenuAccessMapper
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeleteRoleId AS s
                            WHERE EXISTS ( SELECT TOP 1 1
                                           FROM ZnodeRoleMenu AS q
                                           WHERE q.RoleId = s.RoleId
                                                 AND
                                                 q.RoleMenuId = ZnodeRoleMenuAccessMapper.RoleMenuId
                                         )
                          );
             DELETE FROM ZnodeRoleMenu
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeleteRoleId AS s
                            WHERE s.RoleId = ZnodeRoleMenu.RoleId
                          );
             DELETE FROM AspNetRoles
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeleteRoleId AS s
                            WHERE s.RoleId = AspNetRoles.Id
                          );
             SET @Status = 1;
             IF ( SELECT COUNT(1)
                  FROM @DeleteRoleId
                ) = ( SELECT COUNT(1)
                      FROM dbo.Split ( @RoleIds , ','
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
              DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteRole @RoleIds = '+@RoleIds+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN ;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteRole',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;