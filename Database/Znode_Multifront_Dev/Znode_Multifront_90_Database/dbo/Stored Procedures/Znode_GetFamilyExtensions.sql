
CREATE PROCEDURE [dbo].[Znode_GetFamilyExtensions]
( @LocaleId INT = NULL)
AS
/*
 Summary: To Get all information of Extensions of the Family a Locale
 Unit Testing: 
 begin tran
 EXEC Znode_GetFamilyExtensions 1
 rollback tran
 
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             IF @LocaleId IS NULL
                 BEGIN
                     SET @LocaleId =
                     (
                         SELECT ZGS.FeatureValues
                         FROM ZnodeGlobalSetting AS ZGS
                         WHERE ZGS.FeatureName = 'locale'
                     );
                 END;
             
             SELECT DISTINCT ZAIVR.InputValidationRuleId,ZAIVR.ValidationName,ZAIV.name AS 'Extension',
             (
                 SELECT Name
                 FROM ZnodeMediaAttributeValidation
                 WHERE MediaAttributeId = ZMAG.MediaAttributeId
                       AND InputValidationId IN
                 (
                     SELECT InputValidationId
                     FROM ZnodeAttributeInputValidation
                     WHERE Name = 'MaxFileSize'
                 )
             ) AS 'MaxFileSize',
                   ZMAF.MediaAttributeFamilyId,
                   ZMAF.FamilyCode
             FROM ZnodeMediaAttributeFamily AS ZMAF
                  INNER JOIN ZnodeMediaFamilyLocale AS ZMFL ON(ZMAF.MediaAttributeFamilyId = ZMFL.MediaAttributeFamilyId
                                                            AND ZMFL.LocaleId = @LocaleId)
                  INNER JOIN ZnodeMediaFamilyGroupMapper AS ZMFG ON(ZMAF.MediaAttributeFamilyId = ZMFG.MediaAttributeFamilyId
                                                                 AND ZMFG.IsSystemDefined = 1)
                  INNER JOIN ZnodeMediaAttributeGroupMapper AS ZMAG ON(ZMFG.MediaAttributeGroupId = ZMAG.MediaAttributeGroupId
                                                                    AND ZMAG.IsSystemDefined = 1)
                  INNER JOIN ZnodeMediaAttributeValidation AS ZMAV ON(ZMAG.MediaAttributeId = ZMAV.MediaAttributeId)
                  INNER JOIN ZnodeAttributeInputValidationRule AS ZAIVR ON(ZMAV.InputValidationRuleId = ZAIVR.InputValidationRuleId)
                  INNER JOIN ZnodeAttributeInputValidation AS ZAIV ON(ZAIVR.InputValidationId = ZAIV.InputValidationId)
             WHERE ZMAF.IsSystemDefined = 1
                  AND ZAIV.Name = 'Extensions';
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetFamilyExtensions @LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetFamilyExtensions',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;          
         END CATCH;
     END;