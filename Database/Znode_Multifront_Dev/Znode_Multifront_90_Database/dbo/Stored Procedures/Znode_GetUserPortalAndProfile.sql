CREATE PROCEDURE [dbo].[Znode_GetUserPortalAndProfile] 
(
 @UserId INT = 0 
 ,@PortalIds VARCHAR(2000) = ''  OUT 
 ,@ProfileIds VARCHAR(2000) = '' OUT     
)
AS 
/*
  Summary :- This procedure is used to get the user profile ids and user portal ids 
			 In case of @UserId -1 this mean the procedure call for guest user 
			 If @UserId <> -1 then first check the is account user after that find the portal and profile associated to the user      
  Unit Testing 
  DECLARE @weewew VARCHAR(500) = ''
  EXEC Znode_GetUserPortalAndProfile 1319 ,1,@weewew  OUT SELECT @weewew 

*/
BEGIN 
 BEGIN TRY
  SET NOCOUNT ON 
   DECLARE @TBL_ProfileId  TABLE (ProfileId INT )
   DECLARE @ProfileId VARCHAR(2000)
   DECLARE @AccountId INT = 0 

  IF @UserId = -1 
  BEGIN 
	INSERT INTO @TBL_ProfileId(ProfileId)
	SELECT ISNULL(ProfileId  ,-1)
	FROM ZnodePortalProfile 
	WHERE PortalId = @PortalIds  
	AND IsDefaultAnonymousProfile = 1 
  END 
  ELSE 
  BEGIN 
    SET @AccountId = ISNULL((SELECT TOP 1 AccountId FROM znodeUser WHERE Userid = @UserId ),-1)
  	DECLARE @AccountIds TABLE (AccountId INT )
	INSERT INTO @AccountIds (AccountId)
	SELECT AccountId
	FROM [dbo].[Fn_GetRecurciveAccounts] (@AccountId)

	INSERT INTO @TBL_ProfileId(ProfileId)
	SELECT ISNULL(ProfileId  ,-1)
	FROM ZnodePortalProfile 
	WHERE PortalId = @PortalIds  
	--AND IsDefaultRegistedProfile = 1  
	AND (EXISTS (SELECT TOP 1 1 FROM  ZnodeUSerProfile ZUP WHERE  ZUP.ProfileId = ZnodePortalProfile.ProfileId AND ZUP.UserId = @UserId  )
	  OR EXISTS (SELECT TOP 1 1 FROM ZnodeAccountProfile ZAP WHERE ZAP.ProfileId = ZnodePortalProfile.ProfileId  AND EXISTS (SELECT TOP 1 1 FROM @AccountIds TBL WHERE TBL.AccountId = ZAP.AccountId)
	    AND NOT EXISTS (SELECT TOP 1 1 FROM  ZnodeUSerProfile ZUP WHERE  ZUP.ProfileId = ZnodePortalProfile.ProfileId AND ZUP.UserId = @UserId  )
	  ))
  END
   SET  @ProfileIds = SUBSTRING((SELECT ','+CAST(ProfileId AS VARCHAR(50)) FROM  ZnodeProfile ZP  WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_ProfileId TBP WHERE ( TBP.ProfileId = ZP.ProfileId  OR  TBP.ProfileId = -1)  )
					     			 FOR XML PATH ('') ),2,4000) 

 END TRY 
 BEGIN CATCH 
  SELECT ERROR_MESSAGE()
 END CATCH
END