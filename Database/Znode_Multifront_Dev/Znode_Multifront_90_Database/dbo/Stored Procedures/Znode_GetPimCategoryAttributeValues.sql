CREATE PROCEDURE [dbo].[Znode_GetPimCategoryAttributeValues]
(   @ChangeFamilyId INT = 0,
	@PimCategoryId  INT = 0,
	@LocaleId       INT = 0,
	@IsDebug		 BIT = 0 )
AS
   /*
    Summary :- This procedure is used to get the Category attribute values as per family 
			   The result contains Attribute values with family it belongs,category it falls upon
			   Result is fetched order by DisplayOrder and PimAttributeId
     Unit Testing 
	begin tran
     EXEC [dbo].[Znode_GetPimCategoryAttributeValues] @ChangeFamilyId= 0,@PimCategoryId =24 ,@LocaleId =1
	rollback tran
     
   */
	 BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @PimAttributeIds VARCHAR(MAX)= '';
             DECLARE @PimAttributeFamilyId INT= 0;

             DECLARE @TBL_Attribute TABLE
             (PimAttributeId       INT,
              ParentPimAttributeId INT,
              AttributeTypeId      INT,
              AttributeCode        VARCHAR(600),
              IsRequired           BIT,
              IsLocalizable        BIT,
              IsFilterable         BIT,
              IsSystemDefined      BIT,
              IsConfigurable       BIT,
              IsPersonalizable     BIT,
              DisplayOrder         INT,
              HelpDescription      NVARCHAR(MAX),
              IsCategory           BIT,
              IsHidden             BIT,
              CreatedDate          DATETIME,
              ModifiedDate         DATETIME,
              AttributeName        NVARCHAR(MAX),
              AttributeTypeName    VARCHAR(300)
             );
             DECLARE @TBL_AttributeValue TABLE
             (PimCategoryAttributeValueId INT,
              PimCategoryId               INT,
              CategoryValue               NVARCHAR(MAX),
              AttributeCode               VARCHAR(300),
              PimAttributeId              INT
             );
             DECLARE @TBL_DeFaultAttributeValue TABLE
             (PimAttributeId            INT,
              AttributeDefaultValueCode VARCHAR(600),
              IsEditable                BIT,
              AttributeDefaultValue     NVARCHAR(MAX)
			  ,DisplayOrder INT
             );
             DECLARE @TBL_FamilyDetails TABLE
             (PimAttributeFamilyId INT,
              FamilyCode           VARCHAR(600),
              IsSystemDefined      BIT,
              IsDefaultFamily      BIT,
              IsCategory           BIT,
              AttributeFamilyName  NVARCHAR(MAX)
             );
             IF @ChangeFamilyId = 0
                 BEGIN
                     SET @PimAttributeFamilyId = ISNULL((SELECT TOP 1 PimAttributeFamilyId  FROM ZnodePimCategory WHERE PimCategoryId = @PimCategoryId ), dbo.Fn_GetCategoryDefaultFamilyId());                                 
                 END;
             ELSE
                 BEGIN
                     SET @PimAttributeFamilyId = @ChangeFamilyId;
                 END;
				  -- Find the attributes of family
             WITH Cte_GetCategoryAttribute
			 AS (SELECT PimAttributeId FROM ZnodePimFamilyGroupMapper  WHERE PimAttributeFamilyId = @PimAttributeFamilyId GROUP BY PimAttributeId)

			 SELECT @PimAttributeIds = SUBSTRING( (SELECT ','+CAST(PimAttributeId AS VARCHAR(50)) FROM Cte_GetCategoryAttribute FOR XML PATH('')), 2, 4000);
                               
             INSERT INTO @TBL_Attribute(PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,
			 IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName)
             EXEC [dbo].[Znode_GetPimAttributesDetails] @PimAttributeIds,@LocaleId;
                                    
             INSERT INTO @TBL_AttributeValue(PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId)
             EXEC [dbo].[Znode_GetCategoryAttributeValue] @PimCategoryId,@PimAttributeIds,@LocaleId;

             INSERT INTO @TBL_DeFaultAttributeValue(PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder )
             EXEC Znode_GetAttributeDefaultValueLocale @PimAttributeIds,@LocaleId;                                    

             INSERT INTO @TBL_FamilyDetails(PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName)
             EXEC [dbo].[Znode_GetFamilyValueLocale] @PimAttributeFamilyId, @LocaleId;                                   
			
			  -- update the media path
             ;WITH Cte_productMedia
              AS (SELECT TBA.PimCategoryId,TBA.PimAttributeId,[dbo].[FN_GetMediaThumbnailMediaPath] 
				 (SUBSTRING((SELECT ','+ZM.PATH FROM ZnodeMedia ZM  WHERE EXISTS(SELECT TOP 1 1  FROM dbo.split(TBA.CategoryValue, ',') SP 
				 WHERE SP.Item = CAST(ZM.MediaId AS VARCHAR(50)) ) FOR XML PATH('')), 2, 4000)) CategoryValue FROM @TBL_AttributeValue TBA       
                 WHERE EXISTS(SELECT TOP 1 1  FROM [dbo].[Fn_GetProductMediaAttributeId]() FNMA WHERE FNMA.PimAttributeId = TBA.PimAttributeId)) 
                
			  UPDATE TBAV SET CategoryValue = CTCM.CategoryValue+'~'+ISNULL(TBAV.CategoryValue, '') FROM @TBL_AttributeValue TBAV
              INNER JOIN Cte_productMedia CTCM ON(CTCM.PimCategoryId = TBAV.PimCategoryId  AND CTCM.PimAttributeId = TBAV.PimAttributeId);  
                                      
              SELECT @PimAttributeFamilyId PimAttributeFamilyId,FamilyCode,TBA.PimAttributeId,PimAttributeGroupId,TBA.AttributeTypeId,AttributeTypeName,TBA.AttributeCode,
              IsRequired,IsLocalizable,IsFilterable,AttributeName,CategoryValue AttributeValue,PimCategoryAttributeValueId,ZPADV.PimAttributeDefaultValueId,TBDAV.AttributeDefaultValueCode,
			  AttributeDefaultValue,ISNULL(NULL, 0) AS RowId,ISNULL(TBDAV.IsEditable, 1) AS IsEditable,ControlName,ZAIV.Name AS ValidationName,ZAIVR.ValidationName AS SubValidationName,ZAIVR.RegExp,
			  ZPAV.Name AS ValidationValue,CAST(CASE WHEN ZAIVR.RegExp IS NULL THEN 0 ELSE 1 END AS BIT) AS IsRegExp,TBA.DisplayOrder,HelpDescription FROM @TBL_Attribute TBA
			  LEFT JOIN @TBL_AttributeValue TBAV ON(TBAV.PimAttributeId = TBA.PimAttributeId)
			  LEFT JOIN @TBL_DeFaultAttributeValue TBDAV ON(TBDAV.PimAttributeId = TBA.PimAttributeId)
			  LEFT JOIN ZnodePimAttributeValidation AS ZPAV ON(ZPAV.PimAttributeId = TBA.PimAttributeId)
			  LEFT JOIN ZnodeAttributeInputValidation AS ZAIV ON(ZPAV.InputValidationId = ZAIV.InputValidationId)
			  LEFT JOIN ZnodeAttributeInputValidationRule AS ZAIVR ON(ZPAV.InputValidationRuleId = ZAIVR.InputValidationRuleId)
			  LEFT JOIN @TBL_FamilyDetails TBFD ON(TBFD.PimAttributeFamilyId = @PimAttributeFamilyId)
			  LEFT JOIN ZnodePimFamilyGroupMapper ZPFG ON(ZPFG.PimAttributeFamilyId = TBFD.PimAttributeFamilyId AND ZPFG.PimAttributeId = TBA.PimAttributeId)														
			  LEFT JOIN ZnodePimAttributeDefaultValue ZPADV ON(ZPADV.AttributeDefaultValueCode = TBDAV.AttributeDefaultValueCode AND ZPADV.PimAttributeId = TBA.PimAttributeId)
              ORDER BY CASE WHEN TBA.DisplayOrder IS NULL THEN 0  ELSE 1 END,DisplayOrder,PimAttributeId;                                                   
                      
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimCategoryAttributeValues @ChangeFamilyId = '+CAST(@ChangeFamilyId AS VARCHAR(max))+',@PimCategoryId='+CAST(@PimCategoryId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimCategoryAttributeValues',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;