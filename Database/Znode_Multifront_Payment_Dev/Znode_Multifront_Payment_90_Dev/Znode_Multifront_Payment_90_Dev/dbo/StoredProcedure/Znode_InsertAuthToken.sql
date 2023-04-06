CREATE PROCEDURE [dbo].[Znode_InsertAuthToken]
@UserOrSessionId varchar(300), 
@IsFromAdminApp bit
AS
/* SP used to generate auth token*/
BEGIN
BEGIN TRY
	SET NOCOUNT ON;
	DECLARE @AuthTokenId INT
	IF (@IsFromAdminApp =0 )
	BEGIN
	--Max time is in seconds and minus it goes back to the date i.e -300 sec and check the count after that. Note: It should be less than the delete job which runs every hour
		 Declare @MaxTime int=-300, @TokenLimit int=10
		 IF (select count(*) from ZnodeAuthToken WITH (NOLOCK)  where CreatedDate >= DATEADD(SECOND, @MaxTime, GETDATE()) and UserOrSessionId =@UserOrSessionId) >= @TokenLimit
		 BEGIN
			Select 'Token Limit Exceeded.';
			Return ; 		 
		END
	END

	INSERT INTO ZnodeAuthToken(CreatedDate,UserOrSessionId,IsFromAdminApp)
	SELECT GETDATE(),@UserOrSessionId,@IsFromAdminApp

	SET @AuthTokenId = @@IDENTITY

	SELECT * FROM ZnodeAuthToken WITH (NOLOCK) WHERE AuthTokenId = @AuthTokenId
END TRY
BEGIN CATCH
	SELECT ERROR_MESSAGE()
END CATCH
END
