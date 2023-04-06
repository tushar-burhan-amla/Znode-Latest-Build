

CREATE PROCEDURE [dbo].[Znode_CheckRoleAssociatedWithUser] 
(
 @RoleId  NVARCHAR(Max),
 @Status  BIT OUT
)
AS 
 /*
 	 Summary :- This procedures is used to check whether role is associated with user or not 
	 Unit Testing 
	 EXEC Znode_CheckRoleAssociatedWithUser @RoleId = '6f69d0a4-d7ea-4d0f-8a50-bd10fb08f8a5', @Status = 0
	
*/
 BEGIN 
  BEGIN TRY 
  SET NOCOUNT ON
    
	IF EXISTS (SELECT TOP 1 1 FROM AspNetUserRoles  WHERE  RoleId = @RoleId )
	BEGIN 
	 
	 SET @Status = 1;
	 SELECT 1 AS ID,
            CAST(1 AS BIT) AS Status;
	END 
	ELSE 
	BEGIN
	 
	  SET @Status = 0;
	  SELECT 0 AS ID,
             CAST(0 AS BIT) AS Status;
	END 

  END TRY 
  BEGIN CATCH 
		
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_CheckRoleAssociatedWithUser @RoleId = '+@RoleId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_CheckRoleAssociatedWithUser',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
  END CATCH

 END