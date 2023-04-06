CREATE PROCEDURE [dbo].[Znode_GetRoleActionPermission]
   (@RoleId NVARCHAR(MAX) = '')
AS 
    /*
	 Summary:- This Procedure is Used to find the Role action permission on the basis of role 
     Unit Testing 
     EXEC Znode_GetRoleActionPermission 'c120e647-4cdd-48a8-8113-6fe0eae87b3d'
   
	*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             IF EXISTS
             (SELECT TOP 1 1 FROM AspNetRoles WHERE Id = @RoleId AND name = 'Admin')
                 BEGIN
                     SELECT AccessPermissionsId = 1,ZAM.MenuId,ZA.ControllerName+'/'+ZA.ActionName AS RequestUrlTemplate
                     FROM ZnodeActions ZA LEFT JOIN ZnodeActionMenu ZAM ON(ZAM.ActionId = ZA.ActionId);
                 END;
             ELSE
                 BEGIN                
				     SELECT ZMAP.AccessPermissionId,ZAM.MenuId,ZA.ControllerName+'/'+ZA.ActionName RequestUrlTemplate                    
					 FROM ZnodeRoleMenu ZRM INNER JOIN ZnodeRoleMenuAccessMapper ZRMAM ON(ZRM.RoleMenuId = ZRMAM.RoleMenuId)
                     INNER JOIN ZnodeMenuActionsPermission ZMAP ON(ZMAP.MenuId = ZRM.MenuId) LEFT JOIN ZnodeActionMenu ZAM ON(ZAM.ActionId = ZMAP.ActionId)
                     INNER JOIN ZnodeActions ZA ON(ZA.Actionid = ZMAP.ActionId) WHERE ZRM.RoleId = @RoleId AND ZRMAM.AccessPermissionId = ZMAP.AccessPermissionId
                     UNION 
					 SELECT DISTINCT ZMAP.AccessPermissionId,ZAM.MenuId,ZA.ControllerName+'/'+ZA.ActionName RequestUrlTemplate
                     FROM ZnodeActions ZA LEFT JOIN ZnodeMenuActionsPermission ZMAP ON(ZA.Actionid = ZMAP.ActionId)					
                     LEFT JOIN ZnodeActionMenu ZAM ON(ZAM.ActionId = ZMAP.ActionId) WHERE ZA.IsGlobalAccess=1										
				 END; 
                 
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetRoleActionPermission @RoleId='+@RoleId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetRoleActionPermission',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;