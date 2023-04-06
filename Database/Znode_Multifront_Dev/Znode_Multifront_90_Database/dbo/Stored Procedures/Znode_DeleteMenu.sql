
CREATE PROCEDURE [dbo].[Znode_DeleteMenu](
       @MenuIds VARCHAR(500) ,
       @Status  BIT OUT)
AS 
  /*
    Summary : Delete the menu if it is not a parent menu.
    unit testing 
    Begin Tran
    DEclare @Status bit  =0 
    EXEC Znode_DeleteMenu @MenuIds    ='', @Status=0
       
    Rollback tran 
*/
     BEGIN
         BEGIN TRAN;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DeleteMenuId TABLE (
                                         MenuId INT
                                         );
             INSERT INTO @DeleteMenuId
                    SELECT MenuId
                    FROM [dbo].[ZnodeMenu] AS a INNER JOIN dbo.Split ( @MenuIds , ','
                                                                     ) AS b ON ( a.MenuId = b.Item )
                    WHERE NOT EXISTS ( SELECT TOP 1 1
                                       FROM ZnodeMenu AS d
                                       WHERE a.MenuId = d.ParentMenuId
                                             AND
                                             d.ParentMenuId IS NOT NULL
                                     );
             DELETE FROM ZnodeRoleMenuAccessMapper
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeleteMenuId AS s
                            WHERE EXISTS ( SELECT TOP 1 1
                                           FROM ZnodeRoleMenu AS q
                                           WHERE q.MenuId = s.MenuId
                                                 AND
                                                 q.RoleMenuId = ZnodeRoleMenuAccessMapper.RoleMenuId
                                         )
                          );
             DELETE FROM ZnodeRoleMenu
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeleteMenuId AS s
                            WHERE s.MenuId = ZnodeRoleMenu.MenuId
                          );
             DELETE FROM ZnodeMenuActionsPermission
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeleteMenuId AS s
                            WHERE s.MenuId = ZnodeMenuActionsPermission.MenuId
                          );
             DELETE FROM ZnodeMenu
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeleteMenuId AS s
                            WHERE s.MenuId = ZnodeMenu.MenuId
                          );
             SET @Status = 1;
             IF ( SELECT COUNT(1)
                  FROM @DeleteMenuId
                ) = ( SELECT COUNT(1)
                      FROM dbo.Split ( @MenuIds , ','
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
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteMenu @MenuIds = '+@MenuIds+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeleteMenu',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;