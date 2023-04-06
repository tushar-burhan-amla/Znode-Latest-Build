

CREATE PROCEDURE [dbo].[Znode_GetRole] 
( @RoleId  NVARCHAR(Max) )
AS 
/*
	 Summary :- This procedures is used to get RoleDetails along with whether Role is associated with user or not.
	 Unit Testing 
	 EXEC Znode_GetRole '8622E90D-7652-41E7-8563-5DED4CC671DE'
	*/
 BEGIN 
  BEGIN TRY 
    SET NOCOUNT ON;
	SELECT ID as RoleID ,Name,IsActive,TypeOfRole,IsSystemDefined,IsDefault,CASE WHEN EXISTS (SELECT TOP 1 1 FROM AspNetUserRoles  WHERE  RoleId = @RoleId ) THEN CAST(1 AS BIT) 
    ELSE CAST(0 AS BIT) END as IsAssociated	FROM AspNetRoles WHERE Id = @RoleId
	
  END TRY 
  BEGIN CATCH 			
		DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetRole @RoleId = '+@RoleId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetRole',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;	
  END CATCH

 END