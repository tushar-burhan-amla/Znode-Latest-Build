
CREATE PROCEDURE [dbo].[Znode_GetConfigureAttributeDetail]
( @PimFamilyId  INT = 0,
  @PimProductId INT = 0,
  @LocaleId     INT = 1)
AS
 /*
   Summary: sp Get configuration of attribute of Product
 begin tran  
 EXEC Znode_GetConfigureAttributeDetail 0,0,1
 rollback tran
*/

     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             IF @LocaleId IS NULL
                 BEGIN
                     SET @LocaleId = dbo.Fn_GetDefaultLocaleId();
                 END;
             IF @PimFamilyId = 0
                 BEGIN
                     SET @PimFamilyId = [dbo].[Fn_GetDefaultPimProductFamilyId]();
                 END;
             SELECT ZPFGM.PimAttributeFamilyId,ZPFGM.PimAttributeGroupId,ZPAGL.AttributeGroupName,VPAL.PimAttributeId,VPAL.AttributeName,VPAL.AttributeCode,ZAT.AttributeTypeId,VPAL.IsFilterable,VPAL.IsRequired,VPAL.IsLocalizable,g.IsEditable,ZAT.AttributeTypeName,ZAIV.Name,ZAIV.ControlName,ZAIVR.ValidationName,ZPAV.Name AS SubValidationName,ZAIVR.RegExp,g.AttributeDefaultValue,
                    CASE
                        WHEN ZPCPA.PimAttributeId IS NULL
                        THEN 0
                        ELSE 1
                    END AS IsConfigurableAttribute
             FROM ZnodePimFamilyGroupMapper AS ZPFGM
                  INNER JOIN ZnodePimAttributeGroupLocale AS ZPAGL ON(ZPAGL.PimAttributeGroupId = ZPFGM.PimAttributeGroupId
                                                                   AND ZPAGL.LocaleId = @LocaleId)
                  INNER JOIN View_PimAttributeLocale AS VPAL ON(VPAL.PimAttributeId = ZPFGM.PimAttributeId
                                                             AND VPAL.IsConfigurable = 1
                                                             AND VPAL.LocaleId = ZPAGL.LocaleId)
                  INNER JOIN ZnodeAttributeType AS ZAT ON(ZAT.AttributeTypeId = VPAL.AttributeTypeId
                                                       AND ZAT.IsPimAttributeType = 1)
                  LEFT JOIN ZnodePimAttributeValidation AS ZPAV ON(ZPAV.PimAttributeId = VPAL.PimAttributeId)
                  LEFT JOIN ZnodeAttributeInputValidation AS ZAIV ON(ZAIV.InputValidationId = ZPAV.InputValidationId)
                  LEFT JOIN ZnodeAttributeInputValidationRule AS ZAIVR ON(ZAIVR.InputValidationRuleId = ZPAV.InputValidationRuleId)
                  LEFT JOIN View_PimDefaultValue AS g ON(VPAL.PimAttributeId = g.PimAttributeId
                                                      AND g.LocaleId = VPAL.LocaleId)
                  LEFT JOIN ZnodePimConfigureProductAttribute AS ZPCPA ON(ZPCPA.PimFamilyId = @PimFamilyId
                                                                       AND ZPCPA.PimAttributeId = VPAL.PimAttributeId
                                                                       AND ZPCPA.PimProductId = @PimProductId)
             WHERE ZPFGM.PimAttributeFamilyId = @PimFamilyId
                   AND EXISTS
             (
                 SELECT TOP 1 1
                 FROM
                 (
                     SELECT 1 AS idm
                 ) AS ZPFGM
                 LEFT JOIN ZnodePimConfigureProductAttribute AS ZPCPA ON(ZPFGM.Idm = 1)
                 WHERE(ZPCPA.PimAttributeId = VPAL.PimAttributeId
                       AND ZPCPA.PimProductId = @PimProductId)
                       OR @PimProductId = 0
             ); 
            
         END TRY
         BEGIN CATCH
               DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetConfigureAttributeDetail @PimFamilyId = '+CAST(@PimFamilyId AS VARCHAR(50))+',@PimProductId='+CAST(@PimProductId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		 
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetConfigureAttributeDetail',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;