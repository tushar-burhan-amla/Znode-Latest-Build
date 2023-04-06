CREATE PROCEDURE [dbo].[Znode_RemoveUserRegistrationAttemptDetails]	
(
	@Status BIT = 0 OUT
)
/* SP used to delete auth token*/
AS
BEGIN
BEGIN TRY
SET NOCOUNT ON;
	TRUNCATE Table [dbo].[ZnodeUserRegistrationAttempt]
	SET @Status = 1
	SELECT 1 AS Id, @Status AS Status
END TRY
BEGIN CATCH
	SET @Status = 0
	SELECT ERROR_MESSAGE()
	SELECT 0 AS Id, @Status AS Status
END CATCH
END