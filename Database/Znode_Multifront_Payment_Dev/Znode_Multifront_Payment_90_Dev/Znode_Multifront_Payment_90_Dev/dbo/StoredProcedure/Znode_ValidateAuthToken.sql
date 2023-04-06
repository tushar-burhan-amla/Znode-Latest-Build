CREATE PROCEDURE [dbo].[Znode_ValidateAuthToken]
(
	@AuthToken NVARCHAR(200),
    @DoNotCount BIT=0,
	@Status BIT = 0 OUT,
	@MaxAllowedAttempt INT = 3
)
/*SP used to validate auth token and expire token after 3 fail attempt*/
AS
BEGIN
BEGIN TRY
	SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM ZnodeAuthToken WITH (NOLOCK) WHERE AuthToken = @AuthToken AND ISNULL(TotalAttempt,0) <= @MaxAllowedAttempt)
	BEGIN
		SET @Status = 1
        IF @DoNotCount = 0
		begin
		UPDATE ZnodeAuthToken SET TotalAttempt = ISNULL(TotalAttempt,0) + 1 WHERE AuthToken = @AuthToken
                END
	END
	----If anyone wants to use time based token expiration then uncomment below code. Code will expire the token in 20 min
	--DECLARE @CreatedDate DATETIME = (SELECT TOP 1 CreatedDate FROM ZnodeAuthToken WITH (NOLOCK) WHERE AuthToken = @AuthToken)
	--ELSE IF DATEADD(minute,20,@CreatedDate) > @CreatedDate
	--BEGIN
	--	SET @Status = 0
	--END
	ELSE
	BEGIN
		SET @Status = 0
	END
END TRY
BEGIN CATCH
	SET @Status = 0
	SELECT ERROR_MESSAGE()
END CATCH

END
