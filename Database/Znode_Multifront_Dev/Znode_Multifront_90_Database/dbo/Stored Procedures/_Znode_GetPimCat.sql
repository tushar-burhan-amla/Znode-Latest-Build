

CREATE PROCEDURE [dbo].[_Znode_GetPimCat](
       @PimCategoryId        INT = 0 ,
       @PimAttributeFamilyId INT = 0 ,
       @LocaleId             INT = 0)
AS 
	-- Summary :- This procedure is used to get the family attribute values 
	
	-- Unit testing 
	-- SELECT * FROM ZnodePimFamilyGroupMapper
	-- SELECT * FROM ZNodePimAttributeGroupMapper WHERE PimAttributeGroupId IN (153,154)
	-- SELECT * FROM ZnodePimCategory 
	-- EXEC [dbo].[_Znode_GetPimCat]  2,27 ,0 
	

     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
          
			 DECLARE @DefaultLocaleId INT = 34 --dbo.Fn_GetDefaultValue('Locale')
			 		,@PimDefaultAttributeFamilyId INT = 23-- dbo.Fn_GetDefaultValue('CategoryFamily') ;
             DECLARE @TBL_AttributeValue TABLE (PimCategoryAttributeValueId INT, PimCategoryId INT ,CategoryValue NVARCHAR(max),AttributeCode VARCHAR(300),PimAttributeId INT )
			 DECLARE @TBL_DefaultAttributeValue TABLE(PimAttributeId INT ,AttributeDefaultValueCode VARCHAR(100),IsEditable BIT ,AttributeDefaultValue NVARCHAR(max) )
			 DECLARE @PimAttributeIds VARCHAR(max)  
             DECLARE @PIMfamilyIds TABLE (
                                         PimAttributefamilyid INT
                                         );
                     
			 DECLARE @PimAttributeId TABLE (
                                           PimAttributeId INT
										   ,PimAttributeFamilyId INT 
										   ,PimAttributeGroupId INT 
                                           );
			  
             INSERT INTO @PimAttributeId (PimAttributeId,PimAttributeFamilyId)
                    SELECT PimAttributeId,PimAttributeFamilyId
                    FROM ZnodePimCategoryAttributevalue
                    WHERE PimAttributeFamilyId = @PimDefaultAttributeFamilyId
                          AND PimCategoryId = @PimCategoryId
                    UNION
                    SELECT PimAttributeId,PimAttributeFamilyId
                    FROM ZnodePimFamilyGroupMapper
                    WHERE PimAttributeFamilyId = @PimDefaultAttributeFamilyId
                    UNION
                    SELECT PimAttributeId,PimAttributeFamilyId
                    FROM ZnodePimFamilyGroupMapper
                    WHERE PimAttributeFamilyId = @PimAttributeFamilyId;

		      SET @PimAttributeIds = SUBSTRING((SELECT ','+CAST(PimAttributeId AS VARCHAR(100)) FROM @PimAttributeId FOR XML PATH ('')),2,4000)
			 
			  INSERT INTO @PIMfamilyIds
                    SELECT DISTINCT
                           PimAttributeFamilyId
                    FROM View_PimCategoryAttributeValueLocale
                    WHERE PimCategoryId = @PimCategoryId;
		   
			INSERT INTO @TBL_AttributeValue(PimCategoryAttributeValueId,PimCategoryId ,CategoryValue ,AttributeCode,PimAttributeId)
			EXEC Znode_GetCategoryAttributeValue @PimCategoryId ,@PimAttributeIds,@LocaleId 
			
			INSERT INTO @TBL_DefaultAttributeValue (PimAttributeId  ,AttributeDefaultValueCode ,IsEditable  ,AttributeDefaultValue)
			EXEC Znode_GetAttributeDefaultValueLocale @PimAttributeIds,@LocaleId
		     
			UPDATE  TBA
			SET PimAttributeGroupId = ZPFGM.PimAttributeGroupId
			FROM @PimAttributeId TBA 
			INNER JOIN ZnodePimFamilyGroupMapper ZPFGM ON (ZPFGM.PimAttributeId = TBA.PimAttributeId AND ZPFGM.PimAttributeFamilyId = TBA.PimAttributeFamilyId )


			;With Cte_CategoryAttributeValue AS 
			(
			  SELECT ZPA.PimAttributeId  , ZTY.AttributeTypeId , AttributeTypeName , ZPA.AttributeCode , ZPA.IsRequired 
					  , ZPA.IsLocalizable , ZPA.IsFilterable , ZPAL.AttributeName   
					  ,  ControlName ,ZAIV.Name ValidationName 
					  ,  ZAIVR.ValidationName SubValidationName , ZAIVR.RegExp , ZPAV.Name ValidationValue 
					  ,  CAST(CASE	WHEN ZAIVR.RegExp IS NULL THEN 0 ELSE 1 END AS BIT)IsRegExp ,ZPA.DisplayOrder, ZPA.HelpDescription ,ZPAL.LocaleId 
                    
			  FROM ZnodePimAttribute ZPA
			  INNER JOIN ZnodeAttributeType ZTY ON (ZTY.AttributeTypeId = ZPA.AttributeTypeId)
			  INNER JOIN ZnodePimAttributeLocale ZPAL ON (ZPAL.PimAttributeId = ZPA.PimAttributeId )
			  INNER JOIN ZnodePimAttributeValidation ZPAV ON (ZPAV.PimAttributeId = ZPA.PimAttributeId)
			  INNER JOIN ZnodeAttributeInputValidation ZAIV ON (ZAIV.InputValidationId = ZPAV.InputValidationId ) 
			  INNER JOIN ZnodeAttributeInputValidationRule ZAIVR ON (ZAIVR.InputValidationId = ZPAV.InputValidationId OR ZAIVR.InputValidationRuleId = ZPAV.InputValidationRuleId )
			  WHERE EXISTS (SELECT TOP 1 1 FROM @PimAttributeId TBA WHERE ZPA.PimAttributeId = TBA.PimAttributeId) 
			)
			, Cte_CategoryAttributeFirstLocale AS 
			(
			  SELECT  PimAttributeId  , AttributeTypeId , AttributeTypeName , AttributeCode , IsRequired 
					  , IsLocalizable , IsFilterable , AttributeName   
					  ,  ControlName , ValidationName 
					  ,  SubValidationName , RegExp ,  ValidationValue 
					  ,  IsRegExp , HelpDescription ,DisplayOrder
			  FROM Cte_CategoryAttributeValue 
			  WHERE LocaleId = @LocaleId
			)
			, Cte_CategoryAttributeSecondeLocale AS 
			(
			  SELECT * 
			  FROM Cte_CategoryAttributeFirstLocale
			  UNION ALL 
			  SELECT  PimAttributeId  , AttributeTypeId , AttributeTypeName , AttributeCode , IsRequired 
					  , IsLocalizable , IsFilterable , AttributeName   
					  ,  ControlName , ValidationName 
					  ,  SubValidationName , RegExp ,  ValidationValue 
					  ,  IsRegExp , HelpDescription ,DisplayOrder
			  FROM Cte_CategoryAttributeValue CTCAV 
			  WHERE LocaleId = @DefaultLocaleId 
			  AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_CategoryAttributeFirstLocale CTCAFL  WHERE CTCAFL.PimAttributeId = CTCAV.PimAttributeId )
			)
			

			SELECT TBA.[PimAttributeFamilyId],[FamilyCode],CTCASL.[PimAttributeId], TBA.[PimAttributeGroupId] ,[AttributeTypeId]            
				  ,[AttributeTypeName],CTCASL.[AttributeCode],[IsRequired] ,[IsLocalizable],[IsFilterable],[AttributeName],TBAV.CategoryValue [AttributeValue]             
				  ,[PimCategoryAttributeValueId],NULL [PimAttributeDefaultValueId],[AttributeDefaultValue],0 [RowId],[IsEditable],[ControlName]                
				  ,[ValidationName],[SubValidationName],[RegExp],[ValidationValue],[IsRegExp],[HelpDescription], @LocaleId [LocaleId]                   
				  ,AttributeDefaultValueCode    
			FROM Cte_CategoryAttributeSecondeLocale CTCASL 
			INNER JOIN @PimAttributeId TBA ON (TBA.PimAttributeId = CTCASL.PimAttributeId )
			LEFT JOIN  @TBL_AttributeValue TBAV ON (TBAV.PimAttributeId  = CTCASL.PimAttributeId )
			LEFT JOIN  @TBL_DefaultAttributeValue TBDAV ON (TBDAV.PimAttributeId = CTCASL.PimAttributeId)
			LEFT JOIN  ZnodePimAttributeFamily ZPAF ON (ZPAF.PimAttributeFamilyId = TBA.PimAttributeFamilyId) 
			ORDER BY [DisplayOrder]



         END TRY
         BEGIN CATCH
             SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
         END CATCH;
     END;