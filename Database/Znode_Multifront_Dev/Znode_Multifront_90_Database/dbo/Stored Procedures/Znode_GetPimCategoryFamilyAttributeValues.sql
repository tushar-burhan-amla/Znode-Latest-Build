CREATE PROCEDURE [dbo].[Znode_GetPimCategoryFamilyAttributeValues]
(   @PimCategoryId        INT = 0,
	@PimAttributeFamilyId INT = 0,
	@LocaleId             INT = 0)
AS 
   /*
     Summary :- This procedure is used to get the family attribute values 
				Rows fetched has MediaAttributeType for the attributevalues
				
     Unit testing 
     SELECT * FROM ZnodePimFamilyGroupMapper
     SELECT * FROM ZNodePimAttributeGroupMapper WHERE PimAttributeGroupId IN (153,154)
     SELECT * FROM ZnodePimCategory 
	 begin tran
     EXEC [dbo].[Znode_GetPimCategoryFamilyAttributeValues]  2,27 ,1 
	 rollback tran
	*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultValue('Locale'), @CategoryDefaultAttributeFamilyId INT= dbo.Fn_GetDefaultValue('CategoryFamily');
             
			 DECLARE @TBL_AttributeValue TABLE
             (PimCategoryAttributeValueId INT,
              PimCategoryId               INT,
              CategoryValue               NVARCHAR(MAX),
              AttributeCode               VARCHAR(300),
              PimAttributeId              INT
             );
             DECLARE @TBL_DefaultAttributeValue TABLE
             (PimAttributeId            INT,
              AttributeDefaultValueCode VARCHAR(100),
              IsEditable                BIT,
              AttributeDefaultValue     NVARCHAR(MAX),
			  DisplayOrder INT
             );
             DECLARE @PimAttributeIds VARCHAR(MAX);
             DECLARE @PIMfamilyIds TABLE(PimAttributefamilyid INT);

             DECLARE @TBL_PimAttributeId TABLE
             (PimAttributeId       INT,
              PimAttributeFamilyId INT,
              PimAttributeGroupId  INT
             );
             INSERT INTO @TBL_PimAttributeId
             (PimAttributeId,
              PimAttributeFamilyId
             )
             SELECT PimAttributeId,PimAttributeFamilyId  FROM ZnodePimCategoryAttributevalue WHERE PimAttributeFamilyId = @CategoryDefaultAttributeFamilyId
             AND PimCategoryId = @PimCategoryId                                                                              
			 UNION
			 SELECT PimAttributeId,PimAttributeFamilyId FROM ZnodePimFamilyGroupMapper WHERE PimAttributeFamilyId = @CategoryDefaultAttributeFamilyId
			 UNION
			 SELECT PimAttributeId,PimAttributeFamilyId FROM ZnodePimFamilyGroupMapper WHERE PimAttributeFamilyId = @PimAttributeFamilyId;
             SET @PimAttributeIds = SUBSTRING(
                                             (
                                                 SELECT ','+CAST(PimAttributeId AS VARCHAR(100))
                                                 FROM @TBL_PimAttributeId
                                                 FOR XML PATH('')
                                             ), 2, 4000);
             INSERT INTO @PIMfamilyIds SELECT DISTINCT PimAttributeFamilyId FROM View_PimCategoryAttributeValueLocale WHERE PimCategoryId = @PimCategoryId;
             INSERT INTO @TBL_AttributeValue(PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId)
             EXEC Znode_GetCategoryAttributeValue @PimCategoryId,@PimAttributeIds,@LocaleId;

             WITH Cte_AttributeValueMediaData
             AS (SELECT TBAV.PimAttributeId,PimCategoryId,SUBSTRING((SELECT ','+[Path] FROM ZnodeMedia ZM WHERE EXISTS
                (SELECT TOP 1 1 FROM dbo.Split(TBAV.CategoryValue, ',') SP WHERE CAST(ZM.MEdiaId AS VARCHAR(50)) = Sp.Item)FOR XML PATH('')), 2, 4000) MediaPath                                                                                                                                                          
                FROM @TBL_AttributeValue TBAV INNER JOIN ZnodePimAttribute ZPA ON(ZPA.PimAttributeId = TBAV.PimAttributeId)
                INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
                WHERE EXISTS (SELECT TOP 1 1 FROM dbo.Fn_GetProcedureAttributeDefault('MediaAttributeType') FNPD WHERE ZAT.AttributeTypeName = FNPD.Value))

             UPDATE TBA SET CategoryValue = [dbo].[Fn_GetMediaThumbnailMediaPath](MediaPath) FROM @TBL_AttributeValue TBA
             INNER JOIN Cte_AttributeValueMediaData CTAVM ON(CTAVM.PimAttributeId = TBA.PimAttributeId AND CTAVM.PimCategoryId = TBA.PimCategoryId);
             INSERT INTO @TBL_DefaultAttributeValue(PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder)
             EXEC Znode_GetAttributeDefaultValueLocale @PimAttributeIds,@LocaleId;

             UPDATE TBA SET PimAttributeGroupId = ZPFGM.PimAttributeGroupId FROM @TBL_PimAttributeId TBA
             INNER JOIN ZnodePimFamilyGroupMapper ZPFGM ON(ZPFGM.PimAttributeId = TBA.PimAttributeId AND ZPFGM.PimAttributeFamilyId = TBA.PimAttributeFamilyId);

             WITH Cte_CategoryAttributeValue
			 AS (SELECT ZPA.PimAttributeId,ZPA.AttributeTypeId,ZPA.AttributeCode,ZPA.IsRequired,ZPA.IsLocalizable,
				 ZPA.IsFilterable,ZPAL.AttributeName,ZPA.DisplayOrder,ZPA.HelpDescription,ZPAL.LocaleId
				 FROM ZnodePimAttribute ZPA INNER JOIN ZnodePimAttributeLocale ZPAL ON(ZPAL.PimAttributeId = ZPA.PimAttributeId)
				 WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_PimAttributeId TBA WHERE ZPA.PimAttributeId = TBA.PimAttributeId)),
                  
	         Cte_CategoryAttributeFirstLocale 
			 AS (SELECT PimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,AttributeName,DisplayOrder,
			     HelpDescription FROM Cte_CategoryAttributeValue WHERE LocaleId = @LocaleId),

             Cte_CategoryAttributeSecondeLocale
             AS (SELECT PimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,AttributeName,DisplayOrder,HelpDescription FROM Cte_CategoryAttributeFirstLocale
                 UNION ALL
                 SELECT PimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,AttributeName,DisplayOrder,HelpDescription
                 FROM Cte_CategoryAttributeValue CTCAV WHERE LocaleId = @DefaultLocaleId AND NOT EXISTS (
                 SELECT TOP 1 1 FROM Cte_CategoryAttributeFirstLocale CTCAFL WHERE CTCAFL.PimAttributeId = CTCAV.PimAttributeId)),

             Cte_CategoryAttributeValueDetail
             AS (SELECT CTCASL.PimAttributeId,CTCASL.AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,AttributeName,CTCASL.DisplayOrder,
                 HelpDescription,AttributeTypeName,ControlName,ZAIV.Name ValidationName,ZAIVR.ValidationName SubValidationName,ZAIVR.RegExp,ZPAV.Name ValidationValue,
                 CAST(CASE WHEN ZAIVR.RegExp IS NULL THEN 0 ELSE 1 END AS BIT) IsRegExp FROM Cte_CategoryAttributeSecondeLocale CTCASL
                 INNER JOIN ZnodeAttributeType ZTY ON(ZTY.AttributeTypeId = CTCASL.AttributeTypeId)
                 LEFT JOIN ZnodePimAttributeValidation ZPAV ON(ZPAV.PimAttributeId = CTCASL.PimAttributeId)
                 LEFT JOIN ZnodeAttributeInputValidation ZAIV ON(ZAIV.InputValidationId = ZPAV.InputValidationId AND ZAIV.AttributeTypeId = ZTY.AttributeTypeId)
                 LEFT JOIN ZnodeAttributeInputValidationRule ZAIVR ON(ZAIVR.InputValidationId = ZPAV.InputValidationId AND ZAIVR.InputValidationRuleId = ZPAV.InputValidationRuleId))

			 SELECT TBA.[PimAttributeFamilyId],ZPAF.[FamilyCode],CTCASL.[PimAttributeId],TBA.[PimAttributeGroupId],[AttributeTypeId],[AttributeTypeName],
			 CTCASL.[AttributeCode],[IsRequired],[IsLocalizable],[IsFilterable],[AttributeName],TBAV.CategoryValue [AttributeValue],[PimCategoryAttributeValueId],
			 0 [PimAttributeDefaultValueId],[AttributeDefaultValue],0 [RowId],ISNULL([IsEditable], 0) [IsEditable],[ControlName],[ValidationName],
			 [SubValidationName],[RegExp],[ValidationValue],[IsRegExp],[HelpDescription],@LocaleId [LocaleId],CTCASL.[DisplayOrder],AttributeDefaultValueCode
			 FROM Cte_CategoryAttributeValueDetail CTCASL
			 INNER JOIN @TBL_PimAttributeId TBA ON(TBA.PimAttributeId = CTCASL.PimAttributeId)
			 LEFT JOIN @TBL_AttributeValue TBAV ON(TBAV.PimAttributeId = CTCASL.PimAttributeId)
			 LEFT JOIN @TBL_DefaultAttributeValue TBDAV ON(TBDAV.PimAttributeId = CTCASL.PimAttributeId)
			 LEFT JOIN ZnodePimAttributeFamily ZPAF ON(ZPAF.PimAttributeFamilyId = TBA.PimAttributeFamilyId AND ZPAF.PimAttributeFamilyId <> @CategoryDefaultAttributeFamilyId)
			 ORDER BY CASE WHEN CTCASL.[DisplayOrder] IS NULL THEN 1 ELSE 0 END,CTCASL.[DisplayOrder];
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimCategoryFamilyAttributeValues @PimCategoryId = '+CAST(@PimCategoryId AS VARCHAR(max))+',@PimAttributeFamilyId='+CAST(@PimAttributeFamilyId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimCategoryFamilyAttributeValues',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;