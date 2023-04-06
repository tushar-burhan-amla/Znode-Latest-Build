
CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetUserList_bak]
(   @BeginDate   DATE          = NULL,
    @EndDate     DATE          = NULL,
	@StoreName   NVARCHAR(Max) = '',
	@CustomerStatus NVARCHAR(100)
	)
AS 
/*
     Summary :- This Procedure is used in user report for fetting the user details
     Unit Testing
     Exec [ZnodeDevExpressReport_GetUserList] @CustomerStatus = 1,@BeginDate= '2018-11-16 11:58:56.667',@EndDate= '2018-11-19 10:47:06.120'
*/	 
     BEGIN
         BEGIN TRY 
			SET NOCOUNT ON
			DECLARE @SQL NVARCHAR(MAX)
			DECLARE @TBL_PortalId TABLE (PortalId INT )
			DECLARE @TBL_UserReport TABLE ([RegistrationDate] DATETIME,UserId INT,CustomerName NVARCHAR(500),Email NVARCHAR(50),
			StoreName NVARCHAR(MAX),StoreCount INT,ProfileName NVARCHAR(200),PortalId INT, LockoutEndDateUtc datetime  )

			INSERT INTO @TBL_PortalId
			SELECT PortalId 
			FROM ZnodePortal ZP 
			INNER JOIN dbo.split(@StoreName,'|') SP ON (SP.Item = ZP.StoreName)

			INSERT INTO @TBL_UserReport
			SELECT DISTINCT  CONVERT(Date, ZUP.CreatedDate) AS [RegistrationDate],   zu.UserId,ZU.FirstName + ' ' + ZU.LastName AS CustomerName,
			zu.Email,Zp.StoreName,COUNT(Zp.StoreName) as StoreCount,
			SUBSTRING( ( SELECT ','+ISNULL(zp.ProfileName, '') FROM dbo.ZnodeUserProfile zup
			INNER JOIN dbo.ZnodeProfile zp ON zup.ProfileId = zp.ProfileId AND zup.UserId = zu.userID
			ORDER BY zp.ProfileName FOR XML PATH('') ), 2, 4000) ProfileName,ZP.PortalId, AU.LockoutEndDateUtc
			FROM dbo.ZnodeUser zu
			INNER JOIN AspNetUsers AU ON(AU.Id = Zu.AspNetUserId)
			INNER JOIN ZnodeUserPortal AUP ON(AUP.UserId = ZU.UserId)
			INNER JOIN dbo.ZnodeUserProfile zup ON zu.UserId = zup.UserId
			INNER JOIN dbo.ZnodePortalProfile zpp ON zup.ProfileId = zpp.ProfileId
			INNER JOIN dbo.ZnodePortal ZP ON(ZP.PortalId = zpp.PortalId AND Zp.PortalId = AUP.PortalId)
			WHERE CAST(zu.CreatedDate AS DATE) BETWEEN @BeginDate AND @EndDate	
			AND (EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId rt WHERE rt.PortalId = AUP.PortalId)
				OR NOT EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId ))		
			group by zu.UserId,ZU.FirstName,ZU.LastName,zu.Email,ZP.PortalId,Zp.StoreName,ZUP.CreatedDate,AU.LockoutEndDateUtc
                      
				
				  IF @CustomerStatus IN (1) 
				  BEGIN
				  SELECT DISTINCT UserId,CustomerName, Email, StoreName,StoreCount, [RegistrationDate],'IsActive' AS CustomerStatus
				  FROM @TBL_UserReport
				  WHERE LockoutEndDateUtc IS NULL
				  ORDER BY StoreName,CustomerName
				  END 
				  ELSE IF @CustomerStatus IN (0)
				  BEGIN
				  SELECT DISTINCT UserId,CustomerName, Email, StoreName,StoreCount, [RegistrationDate],'InActive' AS CustomerStatus
				  FROM @TBL_UserReport
				  WHERE LockoutEndDateUtc IS NOT NULL
				  ORDER BY StoreName,CustomerName
				  END
				  ELSE 
				  BEGIN
				  SELECT DISTINCT UserId,CustomerName, Email, StoreName,StoreCount, [RegistrationDate],
				  CASE WHEN LockoutEndDateUtc IS NULL THEN 'IsActive' ELSE  'InActive' END AS CustomerStatus
				  FROM @TBL_UserReport
				  ORDER BY StoreName,CustomerName
				  END  
				  
		
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetUserList @BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetUserList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;