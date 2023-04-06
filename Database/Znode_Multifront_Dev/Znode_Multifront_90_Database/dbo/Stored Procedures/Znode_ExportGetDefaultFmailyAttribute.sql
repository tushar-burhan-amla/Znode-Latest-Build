CREATE PROCEDURE [dbo].[Znode_ExportGetDefaultFmailyAttribute](@ImportHead NVARCHAR(100))
AS 
    -- Summary : - this procedure is ued to get the default family attribute 
    -- Unit Testing 
    -- EXEC Znode_ExportGetDefaultFmailyAttribute 'Product'
     BEGIN
         IF @ImportHead IN('Product')
             BEGIN
                 SELECT DISTINCT
                        AttributeCode ColumnList
                 FROM ZnodePimAttribute ZPA
                      INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
                      INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON(ZPFM.PimAttributeId = ZPA.PimAttributeId)
                 WHERE ZPA.IsCategory = 0;
                 SELECT DISTINCT
                        ZPA.PimAttributeId AS Id,
                        ZPA.AttributeCode Name,
                        CASE
                            WHEN ZAT.AttributeTypeName IN('Multi Select', 'Simple Select', 'Text', 'Text Area')
                            THEN 'String'
                            WHEN ZAT.AttributeTypeName IN('Number')
                            THEN 'Int32'
                            WHEN ZAT.AttributeTypeName IN('Yes/No')
                            THEN 'Boolean'
                            ELSE ZAT.AttributeTypeName
                        END DataType
                 FROM ZnodePimAttribute ZPA
                      INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
                      INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON(ZPFM.PimAttributeId = ZPA.PimAttributeId)
                 WHERE ZPA.IsFilterable = 1
                       AND ZAT.AttributeTypeName NOT IN('Image');
             END
			 ELSE IF @ImportHead IN('Category')
			 BEGIN 
				SELECT DISTINCT
                        AttributeCode ColumnList
                 FROM ZnodePimAttribute ZPA
                      INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
                      INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON(ZPFM.PimAttributeId = ZPA.PimAttributeId)
                 WHERE ZPA.IsCategory = 1;
                 SELECT DISTINCT
                        ZPA.PimAttributeId AS Id,
                        ZPA.AttributeCode Name,
                        CASE
                            WHEN ZAT.AttributeTypeName IN('Multi Select', 'Simple Select', 'Text', 'Text Area')
                            THEN 'String'
                            WHEN ZAT.AttributeTypeName IN('Number')
                            THEN 'Int32'
                            WHEN ZAT.AttributeTypeName IN('Yes/No')
                            THEN 'Boolean'
                            ELSE ZAT.AttributeTypeName
                        END DataType
                 FROM ZnodePimAttribute ZPA
                      INNER JOIN ZnodeAttributeType ZAT ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
                      INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON(ZPFM.PimAttributeId = ZPA.PimAttributeId)
                 WHERE ZPA.IsFilterable = 1
                       AND ZAT.AttributeTypeName NOT IN('Image');



			 END 









         --Else IF @ImportHead in ( 'Category')
         --BEGIN 
         --     SELECT Distinct AttributeCode TargetColumnName  
         --		   FROM ZnodePimAttribute ZPA 
         --	   INNER JOIN ZnodeAttributeType ZAT ON (ZAT.AttributeTypeId = ZPA.AttributeTypeId)
         --	   INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON (ZPFM.PimAttributeId = ZPA.PimAttributeId )
         --	   Where ZPA.IsCategory = 1  
         --END 
         --Else 
         --   Select Distinct AttributeCode TargetColumnName from ZnodeImportAttributeValidation where importHeadId =@ImportHeadId 
     END;