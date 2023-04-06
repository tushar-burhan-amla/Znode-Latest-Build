CREATE PROCEDURE [dbo].[Znode_DeleteSearchProfile] 
(
	@SearchProfileId INT
) 
AS
/*
	Unit Testing
	EXEC dbo.Znode_DeleteSearchProfile @SearchProfileId=5
*/
BEGIN
	BEGIN TRAN DeleteSearchProfile;
		--DECLARE @Status BIT;
	BEGIN TRY
		SET NOCOUNT ON

		IF EXISTS (SELECT TOP 1 1 FROM ZnodeSearchProfile WHERE SearchProfileId=@SearchProfileId)
		BEGIN
			DELETE FROM ZnodePortalSearchProfile WHERE SearchProfileId=@SearchProfileId
			DELETE FROM ZnodePublishCatalogSearchProfile WHERE SearchProfileId=@SearchProfileId
			DELETE FROM ZnodeSearchProfileAttributeMapping WHERE SearchProfileId=@SearchProfileId
			DELETE FROM ZnodeSearchProfileFeatureMapping WHERE SearchProfileId=@SearchProfileId
			DELETE FROM ZnodeSearchProfileFieldValueFactor WHERE SearchProfileId=@SearchProfileId
			DELETE FROM ZnodeSearchProfileProductMapping WHERE SearchProfileId=@SearchProfileId
			DELETE FROM ZnodeSearchProfileTrigger WHERE SearchProfileId=@SearchProfileId
			DELETE FROM ZnodeSearchActivity WHERE SearchProfileId=@SearchProfileId
			DELETE FROM ZnodePublishSearchProfileEntity WHERE SearchProfileId=@SearchProfileId
			DELETE FROM ZnodeSearchProfile WHERE SearchProfileId=@SearchProfileId

			SELECT 1 As [Status];
		END
		ELSE
		BEGIN
			SELECT 0 As [Status];
		END

	COMMIT TRAN DeleteSearchProfile
	END TRY
	BEGIN CATCH
		SELECT 0 As [Status];

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteSearchProfile @SearchProfileId='+ISNULL(CAST(@SearchProfileId AS VARCHAR(50)),'''');

		ROLLBACK TRAN DeleteSearchProfile 

		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_DeleteSearchProfile',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END