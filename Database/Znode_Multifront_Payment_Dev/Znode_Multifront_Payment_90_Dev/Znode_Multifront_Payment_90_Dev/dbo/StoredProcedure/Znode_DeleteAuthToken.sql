CREATE PROCEDURE [dbo].[Znode_DeleteAuthToken]
(
	@AuthToken NVARCHAR(200),
	@Status BIT = 0 OUT 
)
/* SP used to delete auth token*/
AS
BEGIN
BEGIN TRY
	SET NOCOUNT ON;
	DELETE FROM ZnodeAuthToken
	WHERE AuthToken = @AuthToken

	SET @Status = 1
END TRY
BEGIN CATCH
	SET @Status = 0
	SELECT ERROR_MESSAGE()
END CATCH
END