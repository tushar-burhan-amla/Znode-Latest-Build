--CREATE TABLE ZnodePimAttributeDefaultXML 
--(
-- PimAttributeDefaultXMLId INT IDENTITY(1,1) CONSTRAINT PK_ZnodePimAttributeDefaultXML PRIMARY KEY 
-- ,PimAttributeDefaultValueId INT 
-- ,AttributeDefaultValueCode VARCHAr(300)
-- ,DefaultValueXML NVARCHAR(4000)
-- ,LocaleId		  INT
-- ,CreatedBy    INT NOT NULL 
-- ,CreatedDate  DATETIME NOT NULL 
-- ,ModifiedBy   INT NOT NULL 
-- ,ModifiedDate DATETIME NOT NULL 
--)


CREATE PROCEDURE [dbo].[Znode_InsertUpdateAttributeDefaultValue] 
(
@PimAttributeDefaultValueId INT
 )
AS
BEGIN 
 BEGIN TRY 
    DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

	;With Cte_DefaultValue AS
	(
	SELECT ZPADV.PimAttributeDefaultValueId,ZPADV.AttributeDefaultValueCode,ZPADVL.AttributeDefaultValue,ZPADV.DisplayOrder,ZPADV.IsEditable,ZPADV.SwatchText,ZM.[Path],LocaleId 
	FROM ZnodePimAttributeDefaultValue ZPADV 
	INNER JOIN ZnodePimAttributeDefaultValueLocale ZPADVL ON (ZPADVL.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId )
	LEFT JOIN ZnodeMedia ZM ON (ZM.MediaId = ZPADV.MediaId)
	)
    
	MERGE INTO ZnodePimAttributeDefaultXML TARGET
    USING (SELECT PimAttributeDefaultValueId,AttributeDefaultValueCode,'<SelectValuesEntity>'+'<Value>'+ISNULL((SELECT ''+AttributeDefaultValue FOR XML PATH('')),'')+'</Value>'+'<Code>'
	+ISNULL(AttributeDefaultValueCode,'')+'</Code>'+'<Path>'+ISNULL([Path],'')+'</Path>'+'<SwatchText>'+ISNULL(SwatchText,'')+'</SwatchText>'+'<DisplayOrder>'+CAST(ISNULL(DisplayOrder,0) AS VARCHAR(50))
	+'</DisplayOrder>'+'</SelectValuesEntity>' AttributeValue ,LocaleId 
	FROM Cte_DefaultValue ) SOURCE 
	ON (
	    TARGET.PimAttributeDefaultValueId = SOURCE.PimAttributeDefaultValueId
		AND TARGET.LocaleId = SOURCE.LocaleId 
	)
	WHEN MATCHED THEN 
	UPDATE 
	SET 
	   TARGET.DefaultValueXML = SOURCE.AttributeValue
	   ,TARGET.LocaleId = SOURCE.LocaleId
	   ,ModifiedDate = @GetDate
	WHEN NOT MATCHED THEN 
	INSERT (PimAttributeDefaultValueId
			,AttributeDefaultValueCode
			,DefaultValueXML
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