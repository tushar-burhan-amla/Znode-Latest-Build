
CREATE Procedure [dbo].[Znode_InsertUpdateCustomeFieldJson] 
(
 @PimProductId VARCHAR(2000)
)
AS
BEGIN 
  BEGIN TRY
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
    
	;With Cte_CustomeAttributeValue AS
	(
			

				SELECT ZPCF.PimProductId, ZPCF.CustomCode,
				( Select  ZPCF.CustomCode AS 'AttributeCode', 
					ZPCFL1.CustomKey AS AttributeName, 
					ZPCFL1.CustomKeyValue AS AttributeValues,
					'false' AS IsUseInSearch  ,
					'false' AS IsHtmlTags,
					'false' AS IsComparable, 
					'false' AS IsFacets,
					'Text Area' AS AttributeTypeName,
					'false' AS IsPersonalizable,
					'True' AS IsCustomField,
					'false' AS IsConfigurable,
					'false' AS IsSwatch,
				Isnull(DisplayOrder,0) AS DisplayOrder
			from ZnodePimCustomFieldLocale ZPCFL1 where (ZPCFL1.PimCustomFieldId = ZPCF.PimCustomFieldId)
			For Json Path,WITHOUT_ARRAY_WRAPPER
			)  AS AttributeValue ,ZPCFL.LocaleId  
			FROM ZnodePimCustomField ZPCF Inner join ZnodePimCustomFieldLocale ZPCFL  On ZPCFL.PimCustomFieldId = ZPCF.PimCustomFieldId
	)

  MERGE INTO ZnodePimCustomeFieldJSON TARGET
  USING Cte_CustomeAttributeValue SOURCE 
  ON (TARGET.PimProductId = SOURCE.PimProductId
    AND  TARGET.LocaleId = SOURCE.LocaleId
	AND TARGET.CustomCode = SOURCE.CustomCode
  )
  WHEN MATCHED THEN 
  UPDATE 
   SET TARGET.CustomeFiledJson = SOURCE.AttributeValue
       ,TARGET.ModifiedBy      = 2 
	   ,TARGET.ModifiedDAte   = @GetDate

  WHEN NOT MATCHED THEN 
  INSERT (PimProductId
				,CustomCode
				,CustomeFiledJson
				,LocaleId
				,CreatedBy
				,CreatedDate
				,ModifiedBy
				,ModifiedDate)
				  VALUES (SOURCE.PimProductId
				  ,SOURCE.CustomCode
				,Source.AttributeValue
				,SOURCE.LocaleId
				,2
				,@GetDate
				,2
				,@GetDate)
				WHEN NOT MATCHED BY SOURCE THEN 
	DELETE;

  END TRY 
  BEGIN CATCH 
  SELECT ERROR_MESSAGE()
  END CATCH 
END