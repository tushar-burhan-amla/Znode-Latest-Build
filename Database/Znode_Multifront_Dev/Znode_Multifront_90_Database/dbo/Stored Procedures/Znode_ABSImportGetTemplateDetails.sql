﻿CREATE PROCEDURE [dbo].[Znode_ABSImportGetTemplateDetails](
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
			SELECT zpa.PimAttributeId, zat.AttributeTypeName, zpa.AttributeCode, zitm.SourceColumnName, zpa.IsRequired, b.ControlName, b.Name AS ValidationName, c.ValidationName AS SubValidationName, a.Name AS ValidationValue, c.RegExp
			FROM dbo.ZnodePimAttribute AS zpa INNER JOIN dbo.ZnodeAttributeType AS zat ON zat.AttributeTypeId = zpa.AttributeTypeId
				 INNER JOIN dbo.ZnodeImportTemplateMapping AS zitm ON zpa.AttributeCode = zitm.TargetColumnName 
				 LEFT OUTER JOIN dbo.ZnodePimAttributeValidation AS a ON zpa.PimAttributeId = a.PimAttributeId
				 LEFT OUTER JOIN dbo.ZnodeAttributeInputValidation AS b ON a.InputValidationId = b.InputValidationId
				 LEFT OUTER JOIN dbo.ZnodeAttributeInputValidationRule AS c ON a.InputValidationRuleId = c.InputValidationRuleId
			WHERE zitm.ImportTemplateId = @TemplateId;
		END;
		ELSE 
		BEGIN
			IF @IsValidationRules = 0 AND @IsIncludeRespectiveFamily = 0 AND @IsProductPriceTemplate = 0
			BEGIN
				SELECT zpa.PimAttributeId, zat.AttributeTypeName, zpa.AttributeCode, zitm.SourceColumnName, zpa.IsRequired
				FROM dbo.ZnodePimAttribute AS zpa INNER JOIN dbo.ZnodeAttributeType AS zat ON zat.AttributeTypeId = zpa.AttributeTypeId
					 INNER JOIN dbo.ZnodeImportTemplateMapping AS zitm ON zpa.AttributeCode = zitm.TargetColumnName
				WHERE zitm.ImportTemplateId = @TemplateId;
			END;
			ELSE
			BEGIN
				IF @IsValidationRules = 0 AND  @IsIncludeRespectiveFamily = 0 AND @IsProductPriceTemplate = 1
				BEGIN
					SELECT DISTINCT  zpa.AttributeCode, zitm.SourceColumnName, zpa.IsRequired
					FROM dbo.ZnodeImportAttributeValidation AS zpa LEFT OUTER JOIN dbo.ZnodeImportTemplateMapping AS zitm ON zpa.AttributeCode = zitm.TargetColumnName AND 
							zitm.ImportTemplateId = @TemplateId;
				END;
				ELSE
				BEGIN
					--IF @IsValidationRules = 0 AND @IsIncludeRespectiveFamily = 1 AND @IsProductPriceTemplate = 0 AND @DefaultFamilyId = 0
					--BEGIN
					--	SELECT zpa.PimAttributeId, zat.AttributeTypeName, zpa.AttributeCode, zitm.SourceColumnName, zpa.IsRequired, 0 AS PimAttributeFamilyId
					--	FROM dbo.ZnodePimAttribute AS zpa INNER JOIN dbo.ZnodeAttributeType AS zat ON zat.AttributeTypeId = zpa.AttributeTypeId
					--		 LEFT OUTER JOIN dbo.ZnodeImportTemplateMapping AS zitm ON zpa.AttributeCode = zitm.TargetColumnName AND zitm.ImportTemplateId = @TemplateId
					--	WHERE zpa.IsCategory = @IsCategory ORDER BY zpa.PimAttributeId;
					--END;
					--ELSE
					BEGIN
						IF @IsValidationRules = 0 AND @IsIncludeRespectiveFamily = 1 AND  @IsProductPriceTemplate = 0 --AND @DefaultFamilyId <> 0
						BEGIN
							Declare @PRDDAAttributeCode TABLE (AttributeCode nvarchar(255)) 
							insert into @PRDDAAttributeCode(AttributeCode)
							Select Distinct Attribute from  PRDDA
							SELECT distinct zpa.PimAttributeId, zat.AttributeTypeName, zpa.AttributeCode, zitm.SourceColumnName, zpa.IsRequired ,@DefaultFamilyId
							FROM dbo.ZnodePimAttributeFamily AS zpaf INNER JOIN dbo.ZnodePimFamilyGroupMapper AS zpfgm ON zpaf.PimAttributeFamilyId = zpfgm.PimAttributeFamilyId
								 INNER JOIN dbo.ZnodePimAttribute AS zpa ON zpa.PimAttributeId = zpfgm.PimAttributeId AND zpa.PimAttributeId = zpfgm.PimAttributeId
								 INNER JOIN dbo.ZnodeAttributeType AS zat ON zat.AttributeTypeId = zpa.AttributeTypeId 
								 LEFT OUTER JOIN dbo.ZnodeImportTemplateMapping AS zitm
								 ON zpa.AttributeCode = zitm.TargetColumnName AND zitm.ImportTemplateId = @TemplateId
							WHERE zpaf.IsCategory = @IsCategory AND zpfgm.PimAttributeFamilyId in (@DefaultAttributePimFamilyId, @DefaultFamilyId)  AND zpfgm.PimAttributeId IS NOT NULL
							Union all 
							SELECT distinct zpa.PimAttributeId, zat.AttributeTypeName, zpa.AttributeCode, zpa.AttributeCode, zpa.IsRequired ,@DefaultFamilyId
							FROM dbo.ZnodePimAttributeFamily AS zpaf INNER JOIN dbo.ZnodePimFamilyGroupMapper AS zpfgm ON zpaf.PimAttributeFamilyId = zpfgm.PimAttributeFamilyId
								 INNER JOIN dbo.ZnodePimAttribute AS zpa ON zpa.PimAttributeId = zpfgm.PimAttributeId AND zpa.PimAttributeId = zpfgm.PimAttributeId
								 INNER JOIN dbo.ZnodeAttributeType AS zat ON zat.AttributeTypeId = zpa.AttributeTypeId 
								 LEFT OUTER JOIN @PRDDAAttributeCode  AS PRDDA
								 ON zpa.AttributeCode = PRDDA.AttributeCode 
								 WHERE zpaf.IsCategory = @IsCategory AND zpfgm.PimAttributeFamilyId in (@DefaultAttributePimFamilyId, @DefaultFamilyId)  AND zpfgm.PimAttributeId IS NOT NULL
				
						END;
					END;

				END;;
			END;
		END;
		
	END TRY
	BEGIN CATCH
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportGetTemplateDetails @TemplateId = '+CAST(@TemplateId AS VARCHAR(20))+',@IsValidationRules='+CAST(@IsValidationRules AS VARCHAR(50))+',@IsIncludeRespectiveFamily='+CAST(@IsIncludeRespectiveFamily AS VARCHAR(50
))+',@IsProductPriceTemplate='+CAST(@IsProductPriceTemplate AS VARCHAR(50))+',@IsCategory = '+CAST(@IsCategory AS VARCHAR(50))+',@DefaultFamilyId='+CAST(@DefaultFamilyId AS VARCHAR(50))+',@ImportHeadId='+CAST(@ImportHeadId AS VARCHAR(50))+',@Status='+CAST
(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportGetTemplateDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH;
END;