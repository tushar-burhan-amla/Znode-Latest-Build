CREATE PROC [dbo].[Znode_PublishUpdateProductJson]
(
	@PimProductIds transferId readonly 
	, @VersionId varchar(200) = ''
	, @LocaleId INT = 1  
	, @IsSingleProductPublish bit = 0 
	, @NewGUID varchar(600)= ''
	, @CatalogName nvarchar(1000) = ''
	, @messagestring varchar(300) = ''
)

AS 
BEGIN 
BEGIN TRY 
SET NOCOUNT ON 
	DECLARE @col VARCHAR(max)= '', @wherecls varchar(2000)=''
	SET @wherecls = CASE WHEN @VersionId = '' THEN '' ELSE ' VersionId IN ('+@VersionId+')' END  
	DECLARE @DefaultLocaleId INT = dbo.fn_getDefaultLocaleId()
	DECLARE @LocaleIds_distinct TABLE (LocaleId INT, VersionId INT  )

	INSERT INTO @LocaleIds_distinct
	SELECT DISTINCT LocaleId ,VersionId
	FROM ZnodePublishVersionEntity a 
	WHERE  EXISTS (SELECT TOP 1 1 FROM string_split(@VersionId , ',') w WHERE w.value = a.VersionId)

	SELECT id PimProductId 
	INTO #pimProductIds
	FROM @PimProductIds

	CREATE INDEX IDX_#pimProductIds ON #pimProductIds(PimProductId)

SELECT b.PimAttributeId, b.AttributeCode, c.AttributeName,e.IsUseInSearch ,e.IsHtmlTags 
				,e.IsComparable ,e.IsFacets,b.DisplayOrder
				,d.AttributeTypeName , b.IsPersonalizable
				,IsConfigurable ,CASE WHEN b.IsSwatch = 1 THEN 'true' 
				WHEN b.IsSwatch = 0 THEN 'false' ELSE '' END IsSwatch , c.LocaleId
INTO #Attributes_Dt
FROM ZnodePimAttribute b 
LEFT JOIN ZnodePimAttributeLocale c with(nolock) ON (c.PimAttributeId = b.PimAttributeId AND c.LocaleId =@DefaultLocaleId )
LEFT JOIN ZnodeAttributeType d with(nolock) ON (d.AttributeTypeId = b.AttributeTypeId )
LEFT JOIN ZnodePimFrontendProperties e with(nolock) ON (e.PimAttributeId = b.PimAttributeId)
WHERE b.IsCategory = 0
	UPDATE ZnodePublishProgressNotifierEntity
	SET JobName =  @messagestring+'Catalog-'+@CatalogName+'-Collecting Attribute Data'
		,ProgressMark = 40
	WHERE Jobid = @NewGUID

	DROP TABLE if exists  #tbl_ProductValueid
	DROP TABLE if exists  #tbl_AttributeValue
	DROP TABLE if exists  #tbl_Productjson
	DROP TABLE IF EXISTS  #default_value
	DROP TABLE IF EXISTS  #PublishProductEntityIdJson

	SELECT PimAttributeValueid , PimProductId ,  a.PimAttributeId
	INTO #tbl_ProductValueid
	FROM ZnodePimAttributeValue a with(nolock)
	WHERE EXISTS (SELECT TOP 1 1 FROM  #pimProductIds  t WHERE t.PimProductId = a.PimProductId)

	SELECT b.PimProductId, b.PimAttributeId , a.AttributeValue,'' CustomCode , 0 IsDefaultValue ,@DefaultLocaleId LocaleId, 0 Iscustomfield
	INTO #tbl_AttributeValue 
	FROM ZnodePimAttributeValueLocale a with(nolock)
	INNER JOIN #tbl_ProductValueid b ON (b.PimAttributeValueId = a.PimAttributeValueId)
	WHERE a.LocaleId = @DefaultLocaleId
	UNION ALL 
	SELECT b.PimProductId, b.PimAttributeId , a.AttributeValue ,'' CustomCode , 0 IsDefaultValue ,@DefaultLocaleId LocaleId, 0 Iscustomfield
	FROM ZnodePimProductAttributeTextAreaValue a with(nolock)
	INNER JOIN #tbl_ProductValueid b ON (b.PimAttributeValueId = a.PimAttributeValueId)
	WHERE a.LocaleId = @DefaultLocaleId
	UNION ALL 
	SELECT b.PimProductId, b.PimAttributeId , '' AttributeValue ,'' CustomCode , 1 IsDefaultValue ,@DefaultLocaleId LocaleId, 0 Iscustomfield
	FROM ZnodePimProductAttributeDefaultValue a with(nolock)
	INNER JOIN #tbl_ProductValueid b ON (b.PimAttributeValueId = a.PimAttributeValueId)
	WHERE a.LocaleId = @DefaultLocaleId
	UNION ALL 
	SELECT b.PimProductId, b.PimAttributeId , STRING_AGG( CONVERT(NVARCHAR(MAX),c.Path),',') AttributeValue,'' CustomCode, 0 IsDefaultValue 
				,@DefaultLocaleId LocaleId, 0 Iscustomfield
	FROM ZnodePimProductAttributeMedia a with(nolock)
	INNER JOIN ZnodeMedia c with(nolock) ON (c.MediaId = a.MediaId)
	INNER JOIN #tbl_ProductValueid b ON (b.PimAttributeValueId = a.PimAttributeValueId)
	WHERE a.LocaleId = @DefaultLocaleId
	GROUP BY b.PimProductId, b.PimAttributeId
	UNION ALL 
	SELECT b.PimProductId, c.PimCustomFieldId , a.CustomKeyValue AttributeValue,c.CustomCode, 0 IsDefaultValue ,@DefaultLocaleId LocaleId, 1 Iscustomfield
	FROM ZnodePimCustomFieldLocale a with(nolock)
	INNER JOIN ZnodePimCustomField c with(nolock) ON (c.PimCustomFieldId = a.PimCustomFieldId)
	INNER JOIN #tbl_ProductValueid b ON (b.PimProductId = C.PimProductId)
	WHERE a.LocaleId = @DefaultLocaleId
	UNION ALL 
	SELECT b.PimProductId, a.PimAttributeId , '' AttributeValue ,'' CustomCode , 1 IsDefaultValue ,@DefaultLocaleId LocaleId, 0 Iscustomfield
	FROM ZnodePimConfigureProductAttribute a with(nolock)
	INNER JOIN #pimProductIds  b ON (b.PimProductId = a.PimProductId)
	UNION ALL 
	SELECT DISTINCT  a.PimParentProductId, a.PimAttributeId , (SELECT STRING_AGG(po.SKU,',') 
				FROM ZnodePimLinkProductDetail aa with(nolock)
				INNER JOIN ZnodePimProduct po with(nolock) ON (po.PimProductId = aa.PimProductId)
				WHERE aa.PimParentProductId = a.PimParentProductId and aa.PimAttributeId = a.PimAttributeId 
				) AttributeValue ,'' CustomCode , 0 IsDefaultValue ,@DefaultLocaleId LocaleId, 0 Iscustomfield
	FROM ZnodePimLinkProductDetail a with(nolock)
	INNER JOIN #pimProductIds  b ON (b.PimProductId = a.PimParentProductId)

	-- If want child product configurable attributes 
	SELECT DISTINCT ty.PimProductId ,ty.PimAttributeId, ac.AttributeDefaultValueCode Code , bc.LocaleId	, bc.AttributeDefaultValue Value,ac.DisplayOrder,IsEditable,SwatchText,Path,rt.PimAttributeDefaultValueId, CAST('' AS VARCHAr(600)) VariantSKU ,0 VariantDisplayOrder
	INTO #default_value
	FROM ZnodePimAttributeDefaultValue ac with(nolock)
	INNER JOIN ZnodePimProductAttributeDefaultValue rt with(nolock) ON (rt.PimAttributeDefaultValueId = ac.PimAttributeDefaultValueId)
	INNER JOIN ZnodePimAttributeDefaultValueLocale bc with(nolock) ON (ac.PimAttributeDefaultValueId = bc.PimAttributeDefaultValueId)
	INNER JOIN #tbl_ProductValueid ty ON (ty.PimAttributeValueId = rt.PimAttributeValueId)
	LEFT JOIN ZnodeMedia ZM with(nolock) ON (zm.MediaId = ac.MediaId)
	WHERE  bc.LocaleId = @DefaultLocaleId AND rt.LocaleId = bc.LocaleId

	INSERT INTO #default_value 
	SELECT a.PimParentProductId PimProductId ,c.PimAttributeId, b.Code , b.LocaleId	, b.Value,b.DisplayOrder,IsEditable,SwatchText,Path,b.PimAttributeDefaultValueId , po.SKU VariantSKU ,a.DisplayOrder VariantDisplayOrder
	FROM ZnodePimProductTypeAssociation a with(nolock)
	INNER JOIN ZnodePimConfigureProductAttribute c with(nolock) ON (c.PimProductId = a.PimParentProductId)
	INNER JOIN #default_value b ON (b.PimProductId = a.PimProductId AND b.PimAttributeId = c.PimAttributeId )
	INNER JOIN #pimProductIds  bb ON (bb.PimProductId = c.PimProductId)
	INNER JOIN ZnodePimProduct po with(nolock) ON (po.PimProductId = a.PimProductId)

	create nonclustered index IDX_TempTable_AttributeValue on #tbl_AttributeValue (PimProductId) INCLUDE(PimAttributeId)
	create nonclustered index IDX_TempTable_defaultValue on #default_value (PimProductId) INCLUDE(PimAttributeId)

	SELECT DISTINCT PublishProductEntityId,ZnodeProductId,LocaleId
	INTO #PublishProductEntityIdJson
	FROM ZnodePublishProductEntity a WITH (nolock)
	WHERE EXISTS (SELECT TOP 1 1 FROM #pimProductIds  n WHERE n.PimProductId =  a.ZnodeProductId ) 
	AND EXISTS (SELECT TOP 1 1 FROM @LocaleIds_distinct w WHERE w.VersionId = a.VersionId)

	CREATE TABLE #tbl_AttributeValueLocale (PimProductId INT ,   AttributeValue NVARCHAR(max), PimAttributeId INT  , LocaleId INT)
	CREATE TABLE #default_valuelocale (PimAttributeDefaultValueId INT , AttributeDefaultValue NVARCHAR(max), LocaleId int  )

	IF EXISTS (SELECT TOP 1 1 FROM @LocaleIds_distinct WHERE LocaleId <> @DefaultLocaleId )
	BEGIN 

	INSERT INTO #Attributes_Dt 
	SELECT PimAttributeId, AttributeCode, AttributeName,IsUseInSearch ,IsHtmlTags 
				,IsComparable ,IsFacets,DisplayOrder
				,AttributeTypeName , IsPersonalizable
				,IsConfigurable ,IsSwatch , m.LocaleId
		FROM #Attributes_Dt 
		CROSS APPLY @LocaleIds_distinct m
		WHERE m.LocaleId <> @DefaultLocaleId

		UPDATE a 
		SET a.AttributeName = b.AttributeName
		FROM #Attributes_Dt a 
		INNER JOIN ZnodePimAttributeLocale b with(nolock)  ON (a.PimAttributeId = b.PimAttributeId AND a.LocaleId = b.LocaleId)
		WHERE a.LocaleId <> @DefaultLocaleId

		INSERT INTO #tbl_AttributeValueLocale
		SELECT b.PimProductId,   aa.AttributeValue , a.PimAttributeId , aa.LocaleId
		FROM #tbl_AttributeValue a 
		LEFT JOIN #tbl_ProductValueid b  ON (b.PimProductId = a.PimProductId AND b.PimAttributeId = a.PimAttributeId AND a.Iscustomfield = 0 )
		LEFT JOIN  ZnodePimAttributeValueLocale aa with(nolock) ON (b.PimAttributeValueId = aa.PimAttributeValueId  )
		WHERE aa.LocaleId <> @DefaultLocaleId

		INSERT INTO #tbl_AttributeValueLocale
		SELECT b.PimProductId,   aa.AttributeValue , a.PimAttributeId , aa.LocaleId
		FROM #tbl_AttributeValue a 
		INNER JOIN #tbl_ProductValueid b  ON (b.PimProductId = a.PimProductId AND b.PimAttributeId = a.PimAttributeId AND a.Iscustomfield = 0 )
		INNER JOIN  ZnodePimProductAttributeTextAreaValue aa with(nolock) ON (b.PimAttributeValueId = aa.PimAttributeValueId  )
		WHERE aa.LocaleId <> @DefaultLocaleId
 
		INSERT INTO #default_valuelocale
		SELECT DISTINCT   bc.PimAttributeDefaultValueId, bc.AttributeDefaultValue, bc.LocaleId 
		FROM #default_value a 
		INNER JOIN ZnodePimAttributeDefaultValueLocale bc with(nolock) ON (a.PimAttributeDefaultValueId = bc.PimAttributeDefaultValueId AND  a.LocaleId =@DefaultLocaleId)
		WHERE bc.LocaleId <> @DefaultLocaleId
	END 

	CREATE INDEX IDX_#default_valuelocale ON #default_valuelocale(PimAttributeDefaultValueId,LocaleId)
	CREATE INDEX IDX_#tbl_AttributeValueLocale ON #tbl_AttributeValueLocale (PimProductId,PimAttributeId,LocaleId)

	DROP TABLE IF EXISTS #tbl_ProductValueid
	
	UPDATE ZnodePublishProgressNotifierEntity
	SET JobName =  @messagestring+'Catalog-'+@CatalogName+'-'+'-Processing Attribute JSON'
		,ProgressMark = 50
	WHERE Jobid = @NewGUID
	
	DECLARE @intCount INT = (SELECT COUNT(1) FROM #PublishProductEntityIdJson)

	DECLARE @PublishTable  TABLE(PublishProductEntityId int PRIMARY KEY,ZnodeProductId int ,LocaleId int )

	WHILE EXISTS (SELECT TOP 1 1 FROM #PublishProductEntityIdJson)
	BEGIN 
	
	INSERT INTO @PublishTable
	SELECT TOP 10000 PublishProductEntityId,ZnodeProductId,LocaleId
	FROM #PublishProductEntityIdJson

	DROP TABLE IF EXISTS #ProductJsonData

	SELECT PublishProductEntityId,  (SELECT DISTINCT  IIF(ISNULL(b.AttributeCode,'') = '', a.CustomCode,b.AttributeCode)AttributeCode ,IIF(ISNULL(b.AttributeName,'') = '', f.CustomKey,b.AttributeName)AttributeName
		,ISNULL(b.IsUseInSearch,'false')IsUseInSearch ,ISNULL(b.IsHtmlTags,'false') IsHtmlTags
				,ISNULL(b.IsComparable,'false')IsComparable ,ISNULL(b.IsFacets,'false') IsFacets,ISNULL(b.DisplayOrder,0) DisplayOrder 
				,ISNULL(b.AttributeTypeName,'') AttributeTypeName, ISNULL(b.IsPersonalizable,'false')IsPersonalizable 
				,IIF(CustomCode= '','false' , 'true' )IsCustomField,ISNULL(IsConfigurable,'false') IsConfigurable,ISNULL(b.IsSwatch,'false') IsSwatch, ISNULL(IIF(ut.AttributeValue IS NULL , a.AttributeValue,ut.AttributeValue),'') AttributeValues
				,ISNULL((
				   SELECT   Code,m.LocaleId,IIF(  op.AttributeDefaultValue IS NULL ,nt.Value, op.AttributeDefaultValue) Value,DisplayOrder,ISNULL(IsEditable,'false') IsEditable,ISNULL(SwatchText,'')SwatchText,ISNULL(Path,'')Path,VariantSKU,VariantDisplayOrder
				   FROM #default_value nt 
				   LEFT JOIN #default_valuelocale op ON (op.PimAttributeDefaultValueId = nt.PimAttributeDefaultValueId AND op.LocaleId = m.LocaleId )
				   WHERE nt.PimProductId = m.ZnodeProductId AND nt.PimAttributeId = a.PimAttributeId AND a.IsDefaultValue = 1 
				   AND nt.LocaleId =@DefaultLocaleId
				   FOR JSON PATH 
				),'[]')  SelectValues
	FROM #tbl_AttributeValue a 
	LEFT JOIN #tbl_AttributeValueLocale ut ON (ut.PimProductId = m.ZnodeProductId AND ut.PimAttributeId = a.PimAttributeId 
											AND ut.LocaleId = m.LocaleId AND a.Iscustomfield = 0)
	LEFT JOIN #Attributes_Dt b ON (b.PimAttributeId = a.PimAttributeId AND b.LocaleId = m.LocaleId AND a.Iscustomfield = 0)
	LEFT JOIN ZnodePimCustomFieldLocale f with(nolock) ON (f.PimCustomFieldId = a.PimAttributeId AND f.LocaleId =m.LocaleId AND a.Iscustomfield = 1 )
	WHERE a.PimProductId = m.ZnodeProductId AND a.LocaleId = @DefaultLocaleId

	FOR JSON PATH 
		) Attributes
	INTO #ProductJsonData
	FROM @PublishTable m

	UPDATE m
	SET m.Attributes = h.Attributes
	FROM ZnodePublishProductEntity m
	INNER JOIN #ProductJsonData h ON (h.PublishProductEntityId = m.PublishProductEntityId)

	UPDATE  a
	SET name  = ty.AttributeValue 
	FROM ZnodePublishProductEntity a 
	INNER JOIN #tbl_AttributeValueLocale ty ON ( ty.PimProductId = a.ZnodeProductId AND ty.LocaleId = a.LocaleId 
		AND ty.PimAttributeId = (SELECT TOP 1 w.PimAttributeId FROM #Attributes_Dt W WHERE w.AttributeCode = 'ProductName')  )
	WHERE PublishProductEntityId  IN (SELECT PublishProductEntityId FROM @PublishTable)
	AND a.LocaleId <> @DefaultLocaleId
	
	DELETE p FROM #PublishProductEntityIdJson p WHERE EXISTS (SELECT TOP 1 1 FROM @PublishTable o WHERE o.PublishProductEntityId = p.PublishProductEntityId)
	DELETE FROM @PublishTable
	
	SET @intCount = @intCount-10000

	IF IS_SRVROLEMEMBER('sysadmin', SUSER_NAME())=1
	DBCC DROPCLEANBUFFERS
	

	UPDATE ZnodePublishProgressNotifierEntity
	SET JobName =  @messagestring+'Catalog-'+@CatalogName+'-'+'-Processing Attribute JSON-'+CAST(@intCount AS VARCHAR(2000))+' Products Remain'
		,ProgressMark = 50
	WHERE Jobid = @NewGUID

	END 

	DROP TABLE IF EXISTS #pimProductIds

	END TRY 
	BEGIN CATCH 
		SELECT ERROR_MESSAGE(), ERROR_LINE()
	END CATCH 
END