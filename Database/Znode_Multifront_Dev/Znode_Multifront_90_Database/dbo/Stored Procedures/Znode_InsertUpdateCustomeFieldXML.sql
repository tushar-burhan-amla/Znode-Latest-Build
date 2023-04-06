CREATE Procedure [dbo].[Znode_InsertUpdateCustomeFieldXML] 
(
 @PimProductId VARCHAR(2000)
)
AS
BEGIN 
  BEGIN TRY 
    DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

	;With Cte_CustomeAttributeValue AS
	(

			SELECT PimProductId ,ZPCF.CustomCode,'<AttributeCode>'+ISNULL((SELECT ''+ZPCF.CustomCode FOR XML PATH('')),'') +'</AttributeCode>'+'<AttributeName>'+ISNULL((SELECT ''+ZPCFL.CustomKey FOR XML PATH('')),'')+'</AttributeName>'
			+'<AttributeValues>'+ISNULL((SELECT ''+ZPCFL.CustomKeyValue FOR XML PATH('')),'')+'</AttributeValues>'+'<IsUseInSearch>0</IsUseInSearch>
			<IsHtmlTags>0</IsHtmlTags>
			<IsComparable>0</IsComparable>
			<IsFacets>0</IsFacets>
			<AttributeTypeName>Text Area</AttributeTypeName>
			<IsPersonalizable>0</IsPersonalizable>
			<IsCustomField>1</IsCustomField>
			<IsConfigurable>0</IsConfigurable>
			<IsSwatch>0</IsSwatch>
			<DisplayOrder>'+Convert(nvarchar(100),Isnull(DisplayOrder,0))+'</DisplayOrder>
			' AttributeValue,ZPCFL.LocaleId 
			FROM ZnodePimCustomField ZPCF
			INNER JOIN ZnodePimCustomFieldLocale ZPCFL ON (ZPCFL.PimCustomFieldId = ZPCF.PimCustomFieldId) 
	)

  MERGE INTO ZnodePimCustomeFieldXML TARGET
  USING Cte_CustomeAttributeValue SOURCE 
  ON (TARGET.PimProductId = SOURCE.PimProductId
    AND  TARGET.LocaleId = SOURCE.LocaleId
	AND TARGET.CustomCode = SOURCE.CustomCode
  )
  WHEN MATCHED THEN 
  UPDATE 
   SET TARGET.CustomeFiledXML = SOURCE.AttributeValue
       ,TARGET.ModifiedBy      = 2 
	   ,TARGET.ModifiedDAte   = @GetDate

  WHEN NOT MATCHED THEN 
  INSERT (PimProductId
				,CustomCode
				,CustomeFiledXML
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