
CREATE PROCEDURE [dbo].[Znode_GetPimAttributesDetailsforPimProduct]
( @PimAttributeId TransferId READONLY ,
  @LocaleId       INT          = 0)
AS
   /* Summary :- This Procedure is used to get the attribute details with the attribute name locale wise 
     Unit Testing 
     EXEC [Znode_GetPimAttributesDetails] '54,53,56',1
    */ 
	 
	 BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetdefaultLocaleId();
             WITH Cte_PimAttribute
                  AS (SELECT ZPA.PimAttributeId,ZPA.ParentPimAttributeId,ZPA.AttributeTypeId,ZPA.AttributeCode,ZPA.IsRequired,ZPA.IsLocalizable,ZPA.IsFilterable,
					  ZPA.IsSystemDefined,ZPA.IsConfigurable,ZPA.IsPersonalizable,ZPA.DisplayOrder,ZPA.HelpDescription,ZPA.IsCategory,ZPA.IsHidden,ZPA.CreatedBy,
					  ZPA.CreatedDate,ZPA.ModifiedBy,ZPA.ModifiedDate,ZPAL.AttributeName,ZAT.AttributeTypeName,ZPAL.LocaleId
                      FROM ZnodePimAttribute ZPA
                      INNER JOIN ZnodePimAttributeLocale ZPAL ON(ZPAL.PimAttributeId = ZPA.PimAttributeId)
                      INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)),

                  Cte_PimAttributeFirstLocale
                  AS (SELECT PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,IsConfigurable,
					  IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,AttributeName,AttributeTypeName,CreatedDate,ModifiedDate,LocaleId
                      FROM Cte_PimAttribute CTA WHERE LOcaleId = @localeId),
                      
                  Cte_PimAttributeDefaultLocale
                  AS (SELECT PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,IsConfigurable,
				      IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,AttributeName,AttributeTypeName,CreatedDate,ModifiedDate,LocaleId
					  FROM Cte_PimAttributeFirstLocale
					  UNION ALL
                      SELECT PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,IsConfigurable,
					  IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,AttributeName,AttributeTypeName,CreatedDate,ModifiedDate,LocaleId
					  FROM Cte_PimAttribute CTA
					  WHERE LocaleId = @DefaultLocaleId
                      AND NOT EXISTS(SELECT TOP 1 1  FROM Cte_PimAttributeFirstLocale CTAFL WHERE CTAFL.PimAttributeId = CTA.PimAttributeId )),
                                                                                                  
                  Cte_PimAttributeFilter
                  AS (SELECT PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,IsConfigurable,
					  IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName,LocaleId
                      FROM Cte_PimAttributeDefaultLocale CTPADV
                      WHERE EXISTS(SELECT TOP 1 1 FROM @PimAttributeId SP WHERE SP.id = CTPADV.PimAttributeId))
                                                                                                                         
                  SELECT PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,IsConfigurable,
				  IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName
                  FROM Cte_PimAttributeFilter CTAF;
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimAttributesDetails ,@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimAttributesDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;