CREATE PROCEDURE [dbo].[ZnodeReport_GetUserList]
(   @BeginDate   DATE          = NULL,
    @EndDate     DATE          = NULL,
    @PortalId    NVARCHAR(MAX) = '',
    @UserId      VARCHAR(100)  = '',
    @FirstName   NVARCHAR(MAX) = '',
    @LastName    NVARCHAR(MAX) = '',
    @Email       NVARCHAR(MAX) = '',
    @ProfileName NVARCHAR(MAX) = '')
AS 
/*
     Summary :- This Procedure is used in user report for fetting the user details
     Unit Testing
     Exec [ZnodeReport_GetUserList] @FirstName = 'abc'
*/	 
     BEGIN
         BEGIN TRY 
			SET NOCOUNT ON
             ;WITH Cte_UserReport
                  AS (SELECT DISTINCT
                             zu.UserId,
                             zu.FirstName,
                             zu.LastName,
                             zu.Email,
                             Zp.StoreName,
                             SUBSTRING(
                                      (
                                          SELECT ','+ISNULL(zp.ProfileName, '')
                                          FROM dbo.ZnodeUserProfile zup
                                               INNER JOIN dbo.ZnodeProfile zp ON zup.ProfileId = zp.ProfileId
                                                                                 AND zup.UserId = zu.userID
                                          ORDER BY zp.ProfileName
                                          FOR XML PATH('')
                                      ), 2, 4000) ProfileName,
                             ZP.PortalId
                      FROM dbo.ZnodeUser zu
                           INNER JOIN AspNetUsers AU ON(AU.Id = Zu.AspNetUserId)
                           INNER JOIN ZnodeUserPortal AUP ON(AUP.UserId = ZU.UserId)
                           INNER JOIN dbo.ZnodeUserProfile zup ON zu.UserId = zup.UserId
                           INNER JOIN dbo.ZnodePortalProfile zpp ON zup.ProfileId = zpp.ProfileId
                           INNER JOIN dbo.ZnodePortal ZP ON(ZP.PortalId = zpp.PortalId
                                                            AND Zp.PortalId = AUP.PortalId)
                      WHERE((CAST(zu.CreatedDate AS DATE) BETWEEN CASE
                                                                      WHEN @BeginDate IS NULL
                                                                      THEN CAST(zu.CreatedDate AS DATE)
                                                                      ELSE @BeginDate
                                                                  END AND CASE
                                                                              WHEN @EndDate IS NULL
                                                                              THEN CAST(zu.CreatedDate AS DATE)
                                                                              ELSE @EndDate
                                                                          END))


                      )
                  SELECT DISTINCT
                         UserId,
                         FirstName,
                         LastName,
						 Email,
                         StoreName,
                         ProfileName
                  FROM Cte_UserReport
                  WHERE(EXISTS
                       (
                           SELECT TOP 1 1
                           FROM dbo.split(@UserId, ',') SP
                           WHERE CAST(UserId AS VARCHAR(100)) = SP.Item
                                 OR @UserId = ''
                       ))
                  AND (FirstName LIKE '%'+@FirstName+'%'
                       OR @FirstName = '')
                  AND (LastName LIKE '%'+@LastName+'%'
                       OR @LastName = '')
                  AND (Email LIKE '%'+@Email+'%'
                       OR @Email = '')
                  AND (ProfileName LIKE '%'+@ProfileName+'%'
                       OR @ProfileName = '')
                  AND (EXISTS
                      (
                          SELECT TOP 1 1
                          FROM dbo.split(@PortalId, ',') SP
                          WHERE CAST(PortalId AS VARCHAR(100)) = SP.Item
                                OR @PortalId = ''
                      )); 
		
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetUserList @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@UserId='+@UserId+',@FirstName='+@FirstName+',@LastName='+@LastName+',@Email='+@Email+',@ProfileName='+@ProfileName+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetUserList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;