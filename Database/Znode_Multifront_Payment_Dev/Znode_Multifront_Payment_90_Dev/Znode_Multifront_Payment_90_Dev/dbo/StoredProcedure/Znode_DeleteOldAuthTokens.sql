CREATE PROCEDURE [dbo].[Znode_DeleteOldAuthTokens]
/*To delete older(since 1hr old) using scheduler(sql job)*/
(
	@Status int OUT
)
AS
BEGIN
BEGIN TRY
SET NOCOUNT ON
	DELETE
	FROM ZnodeAuthToken
	WHERE CreatedDate < DATEADD(HOUR, -1, GETDATE())
	SET @Status = 1
END TRY
BEGIN CATCH
	SET @Status = 0
	SELECT ERROR_MESSAGE()
END CATCH
END