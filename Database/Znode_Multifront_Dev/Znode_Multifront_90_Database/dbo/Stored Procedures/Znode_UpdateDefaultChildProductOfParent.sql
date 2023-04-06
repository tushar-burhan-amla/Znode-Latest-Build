CREATE PROCEDURE [dbo].[Znode_UpdateDefaultChildProductOfParent]
(
	@PimParentProductId int,
	@PimProductId int,
	@IsDefault bit = 1,
	@DisplayOrder int,
	@Status bit = 0 out
)
as
--execute Znode_UpdateDefaultChildProductOfParent @PimParentProductId = 89, @PimProductId =1, @IsDefault = 'true'
Begin 
	SET NOCOUNT ON
	BEGIN TRY

		update ZnodePimProductTypeAssociation set IsDefault = 0 
		where IsDefault = 1 and PimParentProductId = @PimParentProductId

		update ZnodePimProductTypeAssociation set IsDefault = @IsDefault , DisplayOrder = @DisplayOrder
		where PimParentProductId = @PimParentProductId and PimProductId = @PimProductId
		
		set @Status = 1
		select @PimParentProductId Id, @Status Status
	END TRY
	BEGIN CATCH
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_UpdateDefaultChildProductOfParent @PimParentProductId = '+cast(@PimParentProductId as varchar(10))+',@PimProductId = '+cast(@PimProductId as varchar(10))+',@IsDefault='+CAST(@IsDefault AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_UpdateDefaultChildProductOfParent',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;
End