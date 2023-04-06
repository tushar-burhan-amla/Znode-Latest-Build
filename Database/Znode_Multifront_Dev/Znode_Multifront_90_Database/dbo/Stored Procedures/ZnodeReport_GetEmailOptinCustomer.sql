CREATE PROCEDURE [dbo].[ZnodeReport_GetEmailOptinCustomer]
(   @PortalId   VARCHAR(MAX)  = '',
    @FirstName VARCHAR(100)  = '',
    @LastName  VARCHAR(100)  = '',
    @Email     VARCHAR(50)   = '',
    @Name      NVARCHAR(400) = '')
AS 
/*
     Summary :- This Procedure is used to find the email OptIn 
     Unit Testing 
     EXEC ZnodeReport_GetEmailOptinCustomer
	 SELECT * FROM ZnodeUserPortal WHERE userId IN (select USerId from znodeuser where emailoptin=1)
	
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             SELECT Zu.UserId,
                    ZU.FirstName,
                    ZU.LastName,
                    zu.Email,
                    F.PortalId,
                    F.StoreName,
                    ZA.Name
             FROM ZnodeUser ZU  
			   INNER JOIN ZnodeUserPortal ZUP ON(ZUP.UserId = ZU.UserId
                                                    AND ((EXISTS
                                                         (
                                                             SELECT TOP 1 1
                                                             FROM dbo.split(@PortalId, ',') SP
                                                             WHERE CAST(ZUP.PortalId AS VARCHAR(100)) = SP.Item
                                                                   OR @PortalId = ''
                                                         ))))
				  INNER JOIN ZnodePortal AS F ON ZUP.PortalId = F.PortalId
				  LEFT JOIN AspNetUsers ANU ON(Zu.AspNetUserId = ANU.Id)
                  LEFT JOIN AspNetUserRoles ANUR ON(ANUR.UserId = ANU.Id)
                  LEFT JOIN AspNetRoles ANR ON(ANR.Id = ANUR.RoleId)
                
                  LEFT JOIN AspNetZnodeUser ANZU ON(ANZU.AspNetZnodeUserId = ANU.UserName
                                                     AND (ANZU.PortalId = ZUP.PortalId
                                                          OR ZUP.PortalId IS NULL))
                  
                  LEFT OUTER JOIN ZnodeAccount ZA ON ZU.AccountId = ZA.AccountId
             WHERE
			      (ANR.Name = 'Customer' OR ANR.TypeOfRole = 'B2B' OR ZU.AspNetUserId IS NULL  )
             AND 
				  ZU.EmailOptIn = 1
                  AND
				   (ZU.FirstName LIKE '%'+@FirstName+'%'
                       OR @FirstName = '')
                  AND (ZU.LastName LIKE '%'+@LastName+'%'
                       OR @LastName = '')
                  AND (ZU.Email LIKE '%'+@Email+'%'
                       OR @Email = '')
                  AND (ZA.Name LIKE '%'+@Name+'%'
                       OR @Name = '');
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetEmailOptinCustomer @PortalId = '+@PortalId+',@FirstName='+@FirstName+',@LastName='+@LastName+',@Email='+@Email+',@Name='+@Name+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetEmailOptinCustomer',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;