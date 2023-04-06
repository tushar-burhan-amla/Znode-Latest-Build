CREATE PROCEDURE [dbo].[Znode_UpdateSearchProfileStateOnIndexCreationFailure] 
(
	@PublishCatalogId INT,
	@UserId INT = 0
) 
AS
/*
	Unit Testing
	EXEC dbo.Znode_UpdateSearchProfileStateOnIndexCreationFailure @PublishCatalogId=5
*/
BEGIN
	BEGIN TRAN UpdateSearchProfileState;
	BEGIN TRY
		SET NOCOUNT ON
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		DECLARE @PublishStateId TINYINT;
		SET @PublishStateId=(SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE StateName='Publish Failed');

		UPDATE SP
		SET SP.PublishStateId=@PublishStateId, SP.ModifiedBy=@UserId, SP.ModifiedDate=@GetDate
		FROM ZnodePublishCatalogSearchProfile PCSP
		INNER JOIN ZnodeSearchProfile SP ON PCSP.SearchProfileId=SP.SearchProfileId
		WHERE PCSP.PublishCatalogId=@PublishCatalogId;

	COMMIT TRAN UpdateSearchProfileState
	END TRY
	BEGIN CATCH
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteSearchProfile @PublishCatalogId='+ISNULL(CAST(@PublishCatalogId AS VARCHAR(50)),'''');

		ROLLBACK TRAN UpdateSearchProfileState 

		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_UpdateSearchProfileStateOnIndexCreationFailure',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END