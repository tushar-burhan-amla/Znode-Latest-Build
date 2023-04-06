CREATE PROCEDURE dbo.Znode_ImportGetTemplateMapping
(
	@ImportTemplateId		INT = 0,
	@PimAttributeFamilyId	INT = 0,
	@PromotionTypeId		INT = 0
)
AS
/*
	Summary : - This procedure is used to get template mapping details used in import.  
	Unit Testing
	EXEC Znode_ImportGetTemplateMapping @ImportTemplateId = 33, @PimAttributeFamilyId = 0, @PromotionTypeId = 17
	EXEC Znode_ImportGetTemplateMapping @ImportTemplateId = 33, @PimAttributeFamilyId = 0, @PromotionTypeId = 7
	EXEC Znode_ImportGetTemplateMapping @ImportTemplateId = 33, @PimAttributeFamilyId = 0, @PromotionTypeId = 13
	EXEC Znode_ImportGetTemplateMapping @ImportTemplateId = 33, @PimAttributeFamilyId = 0, @PromotionTypeId = 8
*/
 BEGIN 
	BEGIN TRY
	SET NOCOUNT ON
		DECLARE	@DiscountTypeName NVARCHAR(500) = (SELECT TOP 1 Name FROM ZnodePromotionType WHERE PromotionTypeId = @PromotionTypeId);

		SELECT ImportTemplateMappingId, ImportTemplateId, SourceColumnName, TargetColumnName, DisplayOrder, IsActive, IsAllowNull, 
			CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
		FROM ZnodeImportTemplateMapping
		WHERE ImportTemplateId = @ImportTemplateId

		UNION

		SELECT 999999 As PromotionAttributeId, @ImportTemplateId AS AttributeTypeId, AttributeCode As SourceColumnName, AttributeCode As TargetColumnName,
			0 As DisplayOrder, CAST(0 AS BIT) AS IsActive, CAST(0 AS BIT)  As IsAllowNull, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
		FROM ZnodePromotionAttribute 
		WHERE PromotionAttributeId IN (SELECT PromotionAttributeId FROM ZnodePromotionDiscountAttributeMapper WHERE DiscountTypeName=@DiscountTypeName)
			AND NOT EXISTS (SELECT * FROM ZnodeImportTemplateMapping WHERE ImportTemplateId = @ImportTemplateId AND TargetColumnName=ZnodePromotionAttribute.AttributeCode)
		ORDER BY ImportTemplateMappingId ASC
	END TRY

	BEGIN CATCH
		DECLARE @Status BIT;
		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportGetTemplateMapping 
					@ImportTemplateId = '+CAST(@ImportTemplateId AS VARCHAR(max))+',
					@PimAttributeFamilyId='+CAST(@PimAttributeFamilyId AS VARCHAR(50))+',
					@PromotionTypeId='+CAST(@PromotionTypeId AS VARCHAR(50));
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ImportGetTemplateMapping',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END