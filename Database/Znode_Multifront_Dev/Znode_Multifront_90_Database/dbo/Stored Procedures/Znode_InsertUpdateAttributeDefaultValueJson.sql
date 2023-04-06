CREATE  PROCEDURE [dbo].[Znode_InsertUpdateAttributeDefaultValueJson] 
(
@PimAttributeDefaultValueId INT
 )
AS
BEGIN 
 BEGIN TRY 
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
    
	SELECT ZPADV.PimAttributeDefaultValueId,ZPADV.AttributeDefaultValueCode,ZPADVL.AttributeDefaultValue,ZPADV.DisplayOrder,ZPADV.IsEditable,ZPADV.SwatchText,ZM.[Path],LocaleId 
	into #TBL_DefaultValue FROM ZnodePimAttributeDefaultValue ZPADV 
	INNER JOIN ZnodePimAttributeDefaultValueLocale ZPADVL ON (ZPADVL.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId )
	LEFT JOIN ZnodeMedia ZM ON (ZM.MediaId = ZPADV.MediaId)
    
	MERGE INTO ZnodePimAttributeDefaultJson TARGET
    USING (
	
	--SELECT PimAttributeDefaultValueId,AttributeDefaultValueCode,
	
	--'<SelectValuesEntity>'
	--+'<Value>'+ISNULL((SELECT ''+AttributeDefaultValue FOR XML PATH('')),'')+'</Value>'
	--+'<Code>'+ISNULL(AttributeDefaultValueCode,'')+'</Code>'
	--+'<Path>'+ISNULL([Path],'')+'</Path>'
	--+'<SwatchText>'+ISNULL(SwatchText,'')+'</SwatchText>'
	--+'<DisplayOrder>'+CAST(ISNULL(DisplayOrder,0) AS VARCHAR(50))	+'</DisplayOrder>'+
	--'</SelectValuesEntity>' AttributeValue ,
	
	--LocaleId 
	--FROM Cte_DefaultValue
		select  B.PimAttributeDefaultValueId AS PimAttributeDefaultValueId ,  B.AttributeDefaultValueCode AS AttributeDefaultValueCode,B.LocaleId,  
		(select 
		A.AttributeDefaultValueCode AS 'Code', 
		Isnull(A.LocaleId,0 ) 'LocaleId',
		A.AttributeDefaultValue 'Value',
		A.AttributeDefaultValue 'AttributeDefaultValue',
		Isnull(A.DisplayOrder,0)  'DisplayOrder',
		Isnull(A.IsEditable,'false') 'IsEditable',
		A.SwatchText  AS 'SwatchText',
		A.Path  AS 'Path'
		from #TBL_DefaultValue A  where 
		A.PimAttributeDefaultValueId = B.PimAttributeDefaultValueId  and A.AttributeDefaultValueCode = B.AttributeDefaultValueCode and 
		A.LocaleId = B.LocaleId
		For JSON path ,WITHOUT_ARRAY_WRAPPER
		) 
		as AttributeValue
		from #TBL_DefaultValue B 


	 ) SOURCE 
	ON (
	    TARGET.PimAttributeDefaultValueId = SOURCE.PimAttributeDefaultValueId
		AND TARGET.LocaleId = SOURCE.LocaleId 
	
	)
	WHEN MATCHED THEN 
	UPDATE 
	SET 
	   TARGET.DefaultValueJSON = SOURCE.AttributeValue
	   ,TARGET.LocaleId = SOURCE.LocaleId
	   ,ModifiedDate = @GetDate
	WHEN NOT MATCHED THEN 
	INSERT (PimAttributeDefaultValueId
			,AttributeDefaultValueCode
			,DefaultValueJSON
			,LocaleId
			,CreatedBy
			,CreatedDate
			,ModifiedBy
			,ModifiedDate)
	VALUES ( SOURCE.PimAttributeDefaultValueId
			,SOURCE.AttributeDefaultValueCode
			,SOURCE.AttributeValue
			,SOURCE.LocaleId
			,2
			,@GetDate
			,2
			,@GetDate);
			
 END TRY 
 BEGIN CATCH 
 SELECT ERROR_MESSAGE()
 END CATCH 

END