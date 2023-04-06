   
CREATE PROCEDURE [dbo].[Znode_GetPimPersonalisedAttributeValues]    
(  @PimProductId INT,    
   @LocaleId     INT = NULL)    
AS    
/*    
Summary: This Procedure is used to get Personalised attribute of product from all locale    
Unit Testing:    
begin tran      
 EXEC Znode_GetPimPersonalisedAttributeValues 15,1    
rollback tran    
    
*/    
     BEGIN     
  BEGIN TRY    
      
  SET NOCOUNT ON;    
         DECLARE @DefaultLocaleId INT;    
         SET @DefaultLocaleId = dbo.Fn_GetDefaultValue('Locale');    
         DECLARE @TBL_PimPersonalAttribute TABLE    
         (PimProductId               INT,    
          [PimAttributeFamilyId]     INT,    
          PimAttributeId             INT,    
          [PimAttributeGroupId]      INT,    
          AttributeTypeId            INT,    
          AttributeTypeName          VARCHAR(300),    
          IsRequired                 BIT,    
          IsLocalizable              BIT,    
          IsFilterable               BIT,    
          AttributeName              NVARCHAR(MAX),    
          AttributeValue             NVARCHAR(MAX),    
          PimAttributeValueId        INT,    
          PimAttributeDefaultValueId INT,    
          AttributeDefaultValueCode  NVARCHAR(600),    
          AttributeDefaultValue      NVARCHAR(MAX),    
          IsEditable                 BIT,    
          ControlName                VARCHAR(300),    
          ValidationName             VARCHAR(100),    
          SubValidationName          VARCHAR(300),    
          RegExp                     VARCHAR(300),    
          ValidationValue            VARCHAR(300),    
          IsRegExp                   BIT,    
          HelpDescription            NVARCHAR(MAX),    
          AttributeCode              VARCHAR(600),  
		  DisplayOrder    INT   
         );    
         WITH Cte_AttributeIds    
         AS (SELECT PimProductId,PimAttributeId,ZAV.PimAttributeValueId FROM ZnodePimAttributeValue ZAV WHERE EXISTS    
            (SELECT TOP 1 1 FROM ZnodePimAttribute ZPA WHERE ZPA.PimAttributeId = ZAV.PimAttributeId AND ZPA.IsPersonalizable = 1 AND ZPA.IsCategory = 0)    
            AND ZAV.PimAttributeFamilyId IS NULL AND ZAV.PimProductId = @PimProductId)    
       
   ,Cte_AttributeLocale    
          AS (SELECT @PimProductId PimProductId,PimAttributeValueId,ZPA.PimAttributeId,ZPA.AttributeTypeId, ZAT.AttributeTypeName,ZPA.IsRequired,    
              ZPA.IsLocalizable,ZPA.IsFilterable,ZPAL.AttributeName,ZPA.AttributeCode,ZPAL.LocaleId,ZPA.DisplayOrder FROM ZnodePimAttribute ZPA    
              INNER JOIN ZnodePimAttributeLocale ZPAL ON(ZPAL.PimAttributeId = ZPA.PimAttributeId)    
              INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)    
              INNER JOIN Cte_AttributeIds ZPAV ON(ZPAV.PimAttributeId = ZPA.PimAttributeId) WHERE ZPAL.LocaleId IN(@LocaleId, @DefaultLocaleId)),    
                  
    Cte_AttributeFirstLocale    
          AS (SELECT PimProductId,PimAttributeValueId,PimAttributeId,AttributeTypeId,AttributeTypeName,IsRequired,IsLocalizable,IsFilterable,    
              AttributeName,AttributeCode,DisplayOrder  FROM Cte_AttributeLocale WHERE LocaleId = @LocaleId),    
    
          Cte_AttributeSecondLocale    
          AS (SELECT PimProductId,PimAttributeValueId,PimAttributeId,AttributeTypeId,AttributeTypeName,IsRequired,IsLocalizable,IsFilterable,    
              AttributeName,AttributeCode,DisplayOrder FROM Cte_AttributeFirstLocale    
              UNION ALL    
              SELECT PimProductId,PimAttributeValueId,PimAttributeId,AttributeTypeId,AttributeTypeName,IsRequired,IsLocalizable,IsFilterable,    
              AttributeName,AttributeCode,CTAL.DisplayOrder FROM Cte_AttributeLocale CTAL WHERE LocaleId = @DefaultLocaleId     
     AND NOT EXISTS ( SELECT TOP 1 1 FROM Cte_AttributeFirstLocale CTAFL WHERE CTAFL.PimAttributeId = CTAL.PimAttributeId )),    
                 
    Cte_AttributeValidation    
          AS (SELECT CTASL.PimProductId,CTASL.PimAttributeValueId,CTASL.PimAttributeId,CTASL.AttributeTypeId,CTASL.AttributeTypeName,CTASL.IsRequired,    
              CTASL.IsLocalizable,CTASL.IsFilterable,CTASL.AttributeName,CTASL.AttributeCode,i.[ControlName],i.[Name] AS [ValidationName],j.[ValidationName] AS [SubValidationName],    
              j.[RegExp],k.[Name] AS [ValidationValue],CAST(CASE WHEN j.[RegExp] IS NULL THEN 0 ELSE 1 END AS BIT) AS [IsRegExp],CTASL.DisplayOrder FROM Cte_AttributeSecondLocale CTASL    
              LEFT JOIN [dbo].[ZnodePimAttributeValidation] AS k ON(CTASL.PimAttributeId = K.PimAttributeId)    
              LEFT JOIN [dbo].[ZnodeAttributeInputValidation] AS i ON(k.[InputValidationId] = i.[InputValidationId] AND CTASL.AttributeTypeId = I.AttributeTypeId)    
              LEFT JOIN [dbo].[ZnodeAttributeInputValidationRule] AS j ON(k.[InputValidationRuleId] = j.[InputValidationRuleId])),    
                  
    Cte_DefaultValuesLocale    
          AS (SELECT PimProductId,PimAttributeValueId,CTASL.PimAttributeId,AttributeTypeId,AttributeTypeName, IsRequired,IsLocalizable,IsFilterable,    
              AttributeName,AttributeCode,PimAttributeDefaultValueId,AttributeDefaultValue,AttributeDefaultValueCode,IsEditable,[ControlName],    
              [ValidationName],[SubValidationName],[RegExp],[ValidationValue],[IsRegExp],LocaleId,CTASL.DisplayOrder FROM Cte_AttributeValidation CTASL    
              LEFT JOIN View_PimDefaultValue VIPDV ON(CTASL.PimAttributeId = VIPDV.PimAttributeId AND LocaleId IN(@LocaleId, @DefaultLocaleId))),    
                  
       Cte_DefaultValuesFirstLocale    
          AS (SELECT PimProductId,PimAttributeValueId,PimAttributeId,AttributeTypeId,AttributeTypeName,IsRequired,IsLocalizable,IsFilterable,AttributeName,    
              AttributeCode,PimAttributeDefaultValueId,AttributeDefaultValue,AttributeDefaultValueCode,IsEditable,[ControlName],[ValidationName],[SubValidationName],    
              [RegExp],[ValidationValue],[IsRegExp],CTDVL.DisplayOrder FROM Cte_DefaultValuesLocale CTDVL WHERE(CTDVL.LocaleId = @LocaleId  OR CTDVL.LocaleId IS NULL)),    
                  
    Cte_DefaultValueSecondLocale    
          AS (SELECT PimProductId,PimAttributeValueId,PimAttributeId,AttributeTypeId,AttributeTypeName,IsRequired,IsLocalizable,IsFilterable,AttributeName,    
              AttributeCode,PimAttributeDefaultValueId,AttributeDefaultValue,AttributeDefaultValueCode,IsEditable,[ControlName],[ValidationName],[SubValidationName],    
              [RegExp],[ValidationValue],[IsRegExp],DisplayOrder FROM Cte_DefaultValuesFirstLocale    
     UNION ALL    
              SELECT PimProductId,PimAttributeValueId,PimAttributeId,AttributeTypeId,AttributeTypeName,IsRequired, IsLocalizable,IsFilterable,AttributeName,AttributeCode,    
              PimAttributeDefaultValueId, AttributeDefaultValue,AttributeDefaultValueCode,IsEditable,[ControlName],[ValidationName],[SubValidationName],    
              [RegExp],[ValidationValue],[IsRegExp],CTDVL.DisplayOrder FROM Cte_DefaultValuesLocale CTDVL WHERE CTDVL.LocaleId = @DefaultLocaleId AND NOT EXISTS    
              (SELECT TOP 1 1 FROM Cte_DefaultValuesFirstLocale CTDVF WHERE CTDVF.PimAttributeDefaultValueId = CTDVL.PimAttributeDefaultValueId))    
                  
     INSERT INTO @TBL_PimPersonalAttribute(PimProductId,PimAttributeValueId,PimAttributeId,AttributeTypeId,AttributeTypeName,IsRequired,IsLocalizable,    
     IsFilterable,AttributeName,AttributeCode,PimAttributeDefaultValueId,AttributeDefaultValue,AttributeDefaultValueCode,IsEditable,[ControlName],    
     [ValidationName],[SubValidationName],[RegExp],[ValidationValue],[IsRegExp],DisplayOrder)    
              SELECT PimProductId,PimAttributeValueId,PimAttributeId,AttributeTypeId,AttributeTypeName,IsRequired,IsLocalizable,IsFilterable,AttributeName,AttributeCode,    
     PimAttributeDefaultValueId,AttributeDefaultValue,AttributeDefaultValueCode,IsEditable,[ControlName],[ValidationName],[SubValidationName],[RegExp],    
              [ValidationValue],[IsRegExp],DisplayOrder FROM Cte_DefaultValueSecondLocale;    
    
          WITH Cte_PersonalAttributeValue    
          AS (SELECT TBPPA.PimProductId,TBPPA.PimAttributeId,ZAVL.AttributeValue,ZAVL.LocaleId FROM @TBL_PimPersonalAttribute TBPPA    
              LEFT JOIN ZnodePimAttributeValueLocale ZAVL ON(ZAVL.PimAttributeValueId = TBPPA.PimAttributeValueId) WHERE ZAVL.LocaleId IN(@LocaleId, @DefaultLocaleId)),    
                  
    Cte_AttributeValueFirst    
          AS (SELECT PimProductId,PimAttributeId, AttributeValue FROM Cte_PersonalAttributeValue CTAV WHERE CTAV.LocaleId = @LocaleId OR CTAV.LocaleId IS NULL),    
                  
    Cte_AttributeValueSecond    
          AS (SELECT PimProductId,PimAttributeId, AttributeValue FROM Cte_AttributeValueFirst CTAVF    
              UNION ALL    
              SELECT PimProductId,PimAttributeId,AttributeValue FROM Cte_PersonalAttributeValue CTAV WHERE CTAV.LocaleId = @DefaultLocaleId    
              AND NOT EXISTS(SELECT TOP 1 1 FROM Cte_AttributeValueFirst CTAVF WHERE CTAVF.PimAttributeId = CTAV.PimAttributeId))    
                  
    UPDATE TBPA SET AttributeValue = CTAM.AttributeValue FROM @TBL_PimPersonalAttribute TBPA    
          INNER JOIN Cte_AttributeValueSecond CTAM ON(CTAM.PimAttributeId = TBPA.PimAttributeId);    
             
    WITH Cte_MediaDetails    
          AS (SELECT TBPA.PimAttributeId,[Path],MEdiaId, AttributeValue FROM ZnodeMedia ZM INNER JOIN @TBL_PimPersonalAttribute TBPA     
    ON(EXISTS(SELECT TOP 1 1  FROM dbo.Split(AttributeValue, ',') SP WHERE SP.Item = ZM.MediaId ))    
          WHERE EXISTS(SELECT TOP 1 1 FROM [dbo].[Fn_GetProcedureAttributeDefault]('MediaAttributeType') FNGP WHERE FNGP.[Value] = TBPA.AttributeTypeName)),    
                  
    Cte_Attributemedia    
          AS (SELECT PimAttributeId,SUBSTRING((SELECT DISTINCT ','+[Path] FROM Cte_MediaDetails CTMD WHERE CTMD.PimAttributeId = TBPA.PimAttributeId FOR XML PATH('')), 2, 4000) MediaPath    
              FROM @TBL_PimPersonalAttribute TBPA WHERE EXISTS(SELECT TOP 1 1 FROM [dbo].[Fn_GetProcedureAttributeDefault]('MediaAttributeType') FNGP    
              WHERE FNGP.[Value] = TBPA.AttributeTypeName) GROUP BY TBPA.PimAttributeId)    
                  
    UPDATE TBPA SET AttributeValue = [dbo].[Fn_GetMediaThumbnailMediaPath](MediaPath)+'~'+AttributeValue FROM @TBL_PimPersonalAttribute TBPA    
    INNER JOIN Cte_Attributemedia CTAM ON(CTAM.PimAttributeId = TBPA.PimAttributeId)     
    WHERE EXISTS(SELECT TOP 1 1 FROM [dbo].[Fn_GetProcedureAttributeDefault]('MediaAttributeType') FNGP WHERE FNGP.[Value] = TBPA.AttributeTypeName);    
             
    SELECT [PimProductId],[PimAttributeFamilyId],[PimAttributeId],[PimAttributeGroupId],[AttributeTypeId],[AttributeTypeName],[AttributeCode],[IsRequired],    
    [IsLocalizable],[IsFilterable],[AttributeName],[AttributeValue],[PimAttributeValueId],[PimAttributeDefaultValueId],AttributeDefaultValueCode,[AttributeDefaultValue],    
    ISNULL(NULL, 0) AS [ROWID],ISNULL([IsEditable], 1) AS [IsEditable],[ControlName],[ValidationName],[SubValidationName],[RegExp],[ValidationValue],[IsRegExp],    
    [HelpDescription],DisplayOrder FROM @TBL_PimPersonalAttribute  
	ORDER BY DisplayOrder;    
        
   END TRY    
   BEGIN CATCH    
    DECLARE @Status BIT ;    
    SET @Status = 0;    
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimPersonalisedAttributeValues @PimProductId = '+cast (@PimProductId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
          SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
      
          EXEC Znode_InsertProcedureErrorLog    
            @ProcedureName = 'Znode_GetPimPersonalisedAttributeValues',    
            @ErrorInProcedure = @Error_procedure,    
            @ErrorMessage = @ErrorMessage,    
            @ErrorLine = @ErrorLine,    
            @ErrorCall = @ErrorCall;    
   END CATCH    
  END;