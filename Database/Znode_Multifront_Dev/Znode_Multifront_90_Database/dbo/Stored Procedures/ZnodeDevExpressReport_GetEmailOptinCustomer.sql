
CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetEmailOptinCustomer]
(   
    @StoreName  VARCHAR(MAX)  = '',
    @FirstName  VARCHAR(100)  = '',
    @LastName   VARCHAR(100)  = '',
    @Email      VARCHAR(50)   = '',
	@ShowOnlyRegisteredUsers BIT = 1
)
AS 
/*
     Summary :- This Procedure is used to find the email OptIn 
     Unit Testing 
     EXEC ZnodeDevExpressReport_GetEmailOptinCustomer
	 SELECT * FROM ZnodeUserPortal WHERE userId IN (select USerId from znodeuser where emailoptin=1)
	
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;

			  DECLARE @SQL NVARCHAR(MAX);

			  DECLARE @TBL_PortalId TABLE (PortalId INT );
			  INSERT INTO @TBL_PortalId
			  SELECT PortalId 
			  FROM ZnodePortal ZP 
			  INNER JOIN dbo.split(@StoreName,'|') SP ON (SP.Item = ZP.StoreName) 

			  ;WITH CTE_GetEmailOptinCustomer AS
			  (
				SELECT Zu.UserId, ZU.FirstName,ZU.LastName,zu.Email,F.PortalId, F.StoreName,ZA.Name
				,CASE WHEN  ZU.AspNetUserId IS NULL THEN 'Guest User' ELSE 'Registered User' END  CustomerType  
				FROM ZnodeUser ZU  
				INNER JOIN ZnodeUserPortal ZUP ON(ZUP.UserId = ZU.UserId
				AND (EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId rt WHERE rt.PortalId = ZUP.PortalId)
				OR NOT EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId )))
				INNER JOIN ZnodePortal AS F ON ZUP.PortalId = F.PortalId
				LEFT JOIN AspNetUsers ANU ON(Zu.AspNetUserId = ANU.Id)
				LEFT JOIN AspNetUserRoles ANUR ON(ANUR.UserId = ANU.Id)
				LEFT JOIN AspNetRoles ANR ON(ANR.Id = ANUR.RoleId)                
				LEFT JOIN AspNetZnodeUser ANZU ON(ANZU.AspNetZnodeUserId = ANU.UserName
				AND (ANZU.PortalId = ZUP.PortalId
				OR ZUP.PortalId IS NULL))                  
				LEFT OUTER JOIN ZnodeAccount ZA ON (ZU.AccountId = ZA.AccountId)
				WHERE
				(ANR.Name = 'Customer' OR ANR.TypeOfRole = 'B2B' OR ZU.AspNetUserId IS NULL  )
				AND ZU.EmailOptIn = 1
				AND
				   (ZU.FirstName LIKE '%'+@FirstName+'%'
                   OR @FirstName = '')
                AND(ZU.LastName LIKE '%'+@LastName+'%'
                   OR @LastName = '')
                AND(ZU.Email LIKE '%'+@Email+'%'
                   OR @Email = '')
				AND ((@ShowOnlyRegisteredUsers = 1 and ZU.AspNetUserId  IS NOT NULL) or (@ShowOnlyRegisteredUsers <> 1 ))
				)

				SELECT UserId,FirstName,LastName,Email,PortalId,StoreName,Name,CustomerType
				FROM CTE_GetEmailOptinCustomer		
				ORDER BY StoreName desc,FirstName,LastName,Email

         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetEmailOptinCustomer @Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetEmailOptinCustomer',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;