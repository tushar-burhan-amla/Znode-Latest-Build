

CREATE PROCEDURE [dbo].[Znode_GetPimAttributeValues]
( 
	@PimAttributeFamilyId INT = 0,
	@IsCategory           BIT = 0,
	@LocaleId             INT = 0
)
AS
   /*
   Summary: This procedure is used to get PimAttributeValues locale wise
			Result is fetched order by DisplayOrder, PimAttributeId
			If IsCategory = 1, FamilyCode is DefaultCategory and IsCategory = 0 then FamilyCode is Default
   Unit Testing:
   begin tran
   EXEC [Znode_GetPimAttributeValues] @PimAttributeFamilyId = 1,@IsCategory=0,@LocaleId=1
   rollback tran

   */
   
     BEGIN
         BEGIN TRY
             IF @LocaleId = 0
                 BEGIN 
				     -- find the default locale id 
                     SELECT TOP 1 @LocaleId = FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'Locale';                                        
                 END;

             -- this block required for the default family use to remove the configurable attribute from family  
             DECLARE @PimAttributeFamilyId2 INT;  
             SELECT @PimAttributeFamilyId2 = PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsDefaultFamily = 1 AND IsCategory = @IsCategory;
                                      
             IF @PimAttributeFamilyId = 0
                 BEGIN  
				     -- find the default family id
                     SET @PimAttributeFamilyId = @PimAttributeFamilyId2;
                 END;
                  
		      SELECT DISTINCT ZPA.displayorder,ZPAF.PimAttributeFamilyId,ZPAF.FamilyCode,ZPA.PimAttributeId,ZPFGM.PimAttributeGroupId,ZPA.AttributeTypeId,ZAT.AttributeTypeName,
              ZPA.AttributeCode,ZPA.IsRequired,ZPA.IsLocalizable,ZPA.IsFilterable,ZPAL.AttributeName,'' AS AttributeValue,NULL AS PimAttributeValueId,VPDV.PimAttributeDefaultValueId,
			  VPDV.AttributeDefaultValueCode,VPDV.AttributeDefaultValue,ISNULL(NULL, 0) AS RowId,ISNULL(VPDV.IsEditable, 1) AS IsEditable,ZAIV.ControlName,
			  ZAIV.Name AS ValidationName,ZAIVR.ValidationName AS SubValidationName,ZAIVR.RegExp,ZPAV.Name AS ValidationValue,
              CAST(CASE WHEN ZAIVR.RegExp IS NULL THEN 0 ELSE 1  END AS BIT) AS IsRegExp,ZPA.IsConfigurable,
			  ZPA.HelpDescription ,VPDV.DisplayOrder as DisplayOrderDefault, VPDV.IsDefault 
			  INTO #Temp_AttributeValue                                                                                                                                                                                             
              FROM dbo.ZnodePimAttributeFamily AS ZPAF
			  INNER JOIN dbo.ZnodePimFamilyGroupMapper AS ZPFGM ON(ZPAF.PimAttributeFamilyId = ZPFGM.PimAttributeFamilyId)
			  INNER JOIN [dbo].[ZnodePimAttribute] AS ZPA ON(ZPFGM.PimAttributeId = ZPA.PimAttributeId  AND ZPA.IsPersonalizable = 0 AND ZPA.IsCategory = ZPAF.IsCategory)
              LEFT JOIN [dbo].ZnodeAttributeType AS ZAT ON(ZPA.AttributeTypeId = ZAT.AttributeTypeId)
              LEFT JOIN [dbo].[ZnodePimAttributeLocale] AS ZPAL ON(ZPAL.LocaleId = @LocaleId  AND ZPAL.PimAttributeId = ZPA.PimAttributeId) 
              LEFT JOIN View_PimDefaultValue AS VPDV ON(VPDV.PimAttributeId = ZPA.PimAttributeId  AND VPDV.LocaleId = @LocaleId)                                                              
              LEFT JOIN [dbo].ZnodePimAttributeValidation AS ZPAV ON(ZPAV.PimAttributeId = ZPA.PimAttributeId)
              LEFT JOIN [dbo].ZnodeAttributeInputValidation AS ZAIV ON(ZPAV.InputValidationId = ZAIV.InputValidationId)
              LEFT JOIN [dbo].ZnodeAttributeInputValidationRule AS ZAIVR ON(ZPAV.InputValidationRuleId = ZAIVR.InputValidationRuleId)
              WHERE(ZPAF.PimAttributeFamilyId = @PimAttributeFamilyId OR ZPAF.IsDefaultFamily = 1) AND ZPAF.IsCategory = @IsCategory;
                                                   				
             -- changes for isconfigurable attribute 
             SELECT PimAttributeFamilyId,FamilyCode,PimAttributeId,PimAttributeGroupId,AttributeTypeId,AttributeTypeName,AttributeCode,IsRequired,IsLocalizable,
			 IsFilterable,AttributeName,AttributeValue,PimAttributeValueId,PimAttributeDefaultValueId,AttributeDefaultValueCode,AttributeDefaultValue,RowId,
			 IsEditable,ControlName,ValidationName,SubValidationName,RegExp,ValidationValue,IsRegExp,HelpDescription,
			 '' FilesName, IsDefault 			 
			 FROM #Temp_AttributeValue                        
             ORDER BY CASE WHEN DisplayOrder IS NULL THEN 0 ELSE 1  END, DisplayOrder,PimAttributeId,DisplayOrderDefault;                     
                      
         END TRY
         BEGIN CATCH
		
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimAttributeValues @PimAttributeFamilyId = '+CAST(@PimAttributeFamilyId AS VARCHAR(50))+',@IsCategory='+CAST(@IsCategory AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimAttributeValues',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;