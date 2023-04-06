


CREATE PROCEDURE [dbo].[Znode_ImportGetGlobalTemplateDetails](
	@TemplateId					int,
	@IsValidationRules			bit= 1,
	@IsIncludeRespectiveFamily	bit= 0,
	@IsProductPriceTemplate		bit= 0,
	@IsCategory					int= 0, 
	@DefaultFamilyId			int= 0,
	@ImportHeadId				int = 0 )
AS
	/*
	  Summary:  Get template details for import process
	  SourceColumnName : CSV file column headers
	  TargetColumnName : Attributecode from ZnodePimAttribute Table 
	  Unit Testing   
	  Exec Znode_ImportGetTemplateDetails @TemplateId =4
	  Exec Znode_ImportGetTemplateDetails @TemplateId =5 ,@IsValidationRules = 0 ,@IsIncludeRespectiveFamily = 0,@IsProductPriceTemplate =1 
*/
BEGIN
	BEGIN TRY
	    DECLARE @DefaultAttributePimFamilyId INT 
	    SET @DefaultAttributePimFamilyId = dbo.Fn_GetDefaultPimProductFamilyId();

		IF @IsValidationRules = 1 AND  @IsIncludeRespectiveFamily = 0 AND  @IsProductPriceTemplate = 0 
		BEGIN
			SELECT zpa.GlobalAttributeId, zat.AttributeTypeName, zpa.AttributeCode, zitm.SourceColumnName, zpa.IsRequired, b.ControlName, b.Name AS ValidationName, c.ValidationName AS SubValidationName, a.Name AS ValidationValue, c.RegExp
			FROM dbo.ZnodeGlobalAttribute AS zpa INNER JOIN dbo.ZnodeAttributeType AS zat ON zat.AttributeTypeId = zpa.AttributeTypeId
				 INNER JOIN dbo.ZnodeImportTemplateMapping AS zitm ON zpa.AttributeCode = zitm.TargetColumnName 
				 LEFT OUTER JOIN dbo.ZnodeGlobalAttributeValidation AS a ON zpa.GlobalAttributeId = a.GlobalAttributeId
				 LEFT OUTER JOIN dbo.ZnodeAttributeInputValidation AS b ON a.InputValidationId = b.InputValidationId
				 LEFT OUTER JOIN dbo.ZnodeAttributeInputValidationRule AS c ON a.InputValidationRuleId = c.InputValidationRuleId
			WHERE zitm.ImportTemplateId = @TemplateId;
		END;

	END TRY
	BEGIN CATCH
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportGetGlobalTemplateDetails @TemplateId = '+CAST(@TemplateId AS VARCHAR(20))+',@IsValidationRules='+CAST(@IsValidationRules AS VARCHAR(50))+',@IsIncludeRespectiveFamily='+CAST(@IsIncludeRespectiveFamily AS VARCHAR(50
))+',@IsProductPriceTemplate='+CAST(@IsProductPriceTemplate AS VARCHAR(50))+',@IsCategory = '+CAST(@IsCategory AS VARCHAR(50))+',@DefaultFamilyId='+CAST(@DefaultFamilyId AS VARCHAR(50))+',@ImportHeadId='+CAST(@ImportHeadId AS VARCHAR(50))+',@Status='+CAST
(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportGetGlobalTemplateDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH;

end