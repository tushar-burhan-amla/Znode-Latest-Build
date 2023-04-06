IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeManageConditionalDefaultData WHERE ConditionalCode = 'UpdatePublishCatalogIdOneTime' AND IsActive=1)
BEGIN
	UPDATE ZnodePortalCatalog
	SET OldPublishCatalogId = PublishCatalogId
	WHERE OldPublishCatalogId IS NULL;

	UPDATE ZnodePortalCatalog
	SET PublishCatalogId = (SELECT TOP 1 PimCatalogId FROM ZnodePublishCatalog r
		WHERE r.PublishCatalogId = ZnodePortalCatalog.OldPublishCatalogId)
	WHERE (SELECT TOP 1 b.PimCatalogId FROM ZnodePublishCatalog b
		WHERE b.PublishCatalogId =ZnodePortalCatalog.PublishCatalogId ) IS NOT NULL; 	

	UPDATE ZnodeCatalogIndex
	SET PublishCatalogId = (SELECT TOP 1 r.PimCatalogId FROM ZnodePublishCatalog r
		WHERE r.PublishCatalogId =ZnodeCatalogIndex.PublishCatalogId )
	WHERE (SELECT TOP 1 b.PimCatalogId FROM ZnodePublishCatalog b
		WHERE b.PublishCatalogId =ZnodeCatalogIndex.PublishCatalogId ) IS NOT NULL;

    UPDATE ZnodePromotionProduct 
	SET PublishProductId = (SELECT TOP 1 PimProductId FROM ZnodePublishProduct WHERE ZnodePublishProduct.PublishProductId = ZnodePromotionProduct.PublishProductId)
	
	UPDATE ZnodePromotionCategory 
	SET PublishCategoryId = (SELECT TOP 1 PimCategoryId FROM ZnodePublishCategory WHERE ZnodePublishCategory.PublishCategoryId = ZnodePromotionCategory.PublishCategoryId)
	
	UPDATE ZnodePromotionCategory 
	SET PublishCategoryId = (SELECT TOP 1 t.PimCategoryHierarchyId  FROM ZnodePimCategoryHierarchy t WHERE t.PimCategoryId = ZnodePromotionCategory.PublishCategoryId)

	UPDATE ZnodePromotionCatalogs 
	SET PublishCatalogId = (SELECT TOP 1 PimcatalogId FROM ZnodePublishCatalog WHERE ZnodePublishCatalog.PublishCatalogId = ZnodePromotionCatalogs.PublishCatalogId)

	UPDATE ZnodeSearchGlobalProductBoost 
	SET PublishProductId = (SELECT TOP 1 PimProductId FROM ZnodePublishProduct WHERE ZnodePublishProduct.PublishProductId = ZnodeSearchGlobalProductBoost.PublishProductId)
		,PublishCatalogId = (SELECT TOP 1 PimcatalogId FROM ZnodePublishCatalog WHERE ZnodePublishCatalog.PublishCatalogId = ZnodeSearchGlobalProductBoost.PublishCatalogId)
	
	UPDATE ZnodeSearchGlobalProductCategoryBoost 
	SET PublishProductId = (SELECT TOP 1 PimProductId FROM ZnodePublishProduct WHERE ZnodePublishProduct.PublishProductId = ZnodeSearchGlobalProductCategoryBoost.PublishProductId)
		, PublishCategoryId = (SELECT TOP 1 PimCategoryId FROM ZnodePublishCategory WHERE ZnodePublishCategory.PublishCategoryId = ZnodeSearchGlobalProductCategoryBoost.PublishCategoryId)
		,PublishCatalogId = (SELECT TOP 1 PimcatalogId FROM ZnodePublishCatalog WHERE ZnodePublishCatalog.PublishCatalogId = ZnodeSearchGlobalProductCategoryBoost.PublishCatalogId)
	
	UPDATE ZnodeSearchDocumentMapping 
	SET PublishCatalogId = (SELECT TOP 1 PimcatalogId FROM ZnodePublishCatalog WHERE ZnodePublishCatalog.PublishCatalogId = ZnodeSearchDocumentMapping.PublishCatalogId)
	
	UPDATE ZnodeSearchKeywordsRedirect 
	SET PublishCatalogId = (SELECT TOP 1 PimcatalogId FROM ZnodePublishCatalog WHERE ZnodePublishCatalog.PublishCatalogId = ZnodeSearchKeywordsRedirect.PublishCatalogId)
	
	UPDATE ZnodePortalSearchProfile 
	SET PublishCatalogId = (SELECT TOP 1 PimcatalogId FROM ZnodePublishCatalog WHERE ZnodePublishCatalog.PublishCatalogId = ZnodePortalSearchProfile.PublishCatalogId)
	
	UPDATE ZnodeSearchSynonyms 
	SET PublishCatalogId = (SELECT TOP 1 PimcatalogId FROM ZnodePublishCatalog WHERE ZnodePublishCatalog.PublishCatalogId = ZnodeSearchSynonyms.PublishCatalogId)

	DECLARE @pimAttributeId INT = 0;

	DECLARE CUR_Attribute CURSOR FOR 
	SELECT PimAttributeId  FROM ZnodePimAttribute WHERE IsShowOnGrid = 1 
	AND IScategory = 0 AND  AttributeCode IN ('ProductName', 'SKU', 'IsActive','ProductType')

	OPEN CUR_Attribute

	FETCH NEXT FROM CUR_Attribute INTO @pimAttributeId

	WHILE @@FETCH_STATUS =  0 
	BEGIN 
		EXEC Znode_UpdateShowOnGridColumn @PimAttributeId, 2,1,0 ,0

		FETCH NEXT FROM CUR_Attribute INTO @pimAttributeId
	END
	CLOSE CUR_Attribute
	DEALLOCATE CUR_Attribute
END

INSERT INTO ZnodeManageConditionalDefaultData (ConditionalCode,ConditionalName,DataSource,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 'UpdatePublishCatalogIdOneTime',
	'This is only for update old PublishCatalogId from respective tables during upgrade of existing database or create fresh database & this will execute only one time.'
		,'Upgrade Database',1,2, GETDATE(),2 , GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeManageConditionalDefaultData WHERE ConditionalCode = 'UpdatePublishCatalogIdOneTime' AND IsActive=1);


--ZPD-23505 Dt.02-Dec-2022 / 13-Dec-2022
DROP TABLE IF EXISTS #UpdatePimAttributeValue;

SELECT A.* 
INTO #UpdatePimAttributeValue
FROM 
(
SELECT a1.PimProductId, c1.AttributeCode, d1.AttributeDefaultValueCode As AttributeValue
FROM ZnodePimAttributeValue a1 WITH (NOLOCK)  --on a.pimproductid = a1.pimproductid
INNER JOIN ZnodePimProductAttributeDefaultValue b1  WITH (NOLOCK) ON a1.Pimattributevalueid = b1.Pimattributevalueid
INNER JOIN ZnodePimAttributeDefaultValue d1 WITH (NOLOCK)  ON b1.PimAttributeDefaultValueId = d1.PimAttributeDefaultValueId
INNER JOIN ZnodePimAttributeDefaultValueLocale d2 WITH (NOLOCK) ON (d1.pimattributedefaultvalueid = d2.pimattributedefaultvalueid AND d2.localeid = 1)
INNER JOIN ZnodePimAttribute c1 WITH (NOLOCK) ON a1.pimattributeid = c1.pimattributeid
--INNER JOIN ZnodeAttributeType f1 ON c1.AttributeTypeId = f1.AttributeTypeId
INNER JOIN ZnodePimProduct PP WITH (NOLOCK) ON a1.PimProductId=PP.PimProductId --AND PP.SKU IS NULL
WHERE c1.AttributeCode in ('ProductType','Assortment','Brand','Vendor','Highlights','IsActive','Weight','IsDownloadable')

UNION ALL 

SELECT a1.PimProductId, c1.attributeCode, b1.AttributeValue As AttributeValue
from ZnodePimAttributeValue a1 WITH (NOLOCK)  --on a.pimproductid = a1.pimproductid
INNER JOIN ZnodePimAttributeValueLocale b1 ON a1.PimAttributeValueId = b1.PimAttributeValueId and b1.LocaleId =1 
INNER JOIN ZnodePimAttribute c1 WITH (NOLOCK) ON a1.pimattributeid = c1.pimattributeid
--INNER JOIN ZnodeAttributeType f1 WITH (NOLOCK) ON c1.AttributeTypeId = f1.AttributeTypeId
INNER JOIN ZnodePimProduct PP WITH (NOLOCK) ON a1.PimProductId=PP.PimProductId --AND PP.SKU IS NULL
WHERE c1.AttributeCode in ('SKU', 'ProductCode','ProductName')

UNION ALL

SELECT PAV.PimProductId, PA.AttributeCode, CAST(PPAM.MediaId As VARCHAR(20)) As AttributeValue
FROM ZnodePimProductAttributeMedia PPAM WITH (NOLOCK)
INNER JOIN ZnodePimAttributeValue PAV WITH (NOLOCK) ON PAV.PimAttributeValueId=PPAM.PimAttributeValueId
INNER JOIN ZnodePimAttribute PA WITH (NOLOCK) ON PAV.PimAttributeId=PA.PimAttributeId
INNER JOIN ZnodePimProduct PP WITH (NOLOCK) ON PAV.PimProductId=PP.PimProductId --AND PP.SKU IS NULL
WHERE PA.AttributeCode IN ('ProductImage')
) A 
WHERE ISNULL(A.AttributeValue,'')<>''

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'Idx_101' AND object_id = OBJECT_ID('#UpdatePimAttributeValue'))
BEGIN
	CREATE INDEX Idx_101 ON #UpdatePimAttributeValue (AttributeCode);
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'Idx_102' AND object_id = OBJECT_ID('#UpdatePimAttributeValue'))
BEGIN
	CREATE INDEX Idx_102 ON #UpdatePimAttributeValue (PimProductId,AttributeCode);
END

IF EXISTS (SELECT TOP 1 1 FROM #UpdatePimAttributeValue)
BEGIN
 	DECLARE @sqlt NVARCHAR(MAX) = ''
	DECLARE @AttributeCodeAtt VARCHAR(600) , @PimAttributeIdAttr INT

	DECLARE Cur_AttributeDataUpdate CURSOR FOR 

	SELECT b.AttributeCode , PimAttributeId 
	FROM INFORMATION_SCHEMA.COLUMNS a 
	INNER JOIN ZnodePimAttribute b ON (a.COLUMN_NAME = b.AttributeCode )
	WHERE TABLE_NAME = 'ZnodePimProduct'
		AND IsCategory = 0 
		AND EXISTS (SELECT TOP 1 1 FROM #UpdatePimAttributeValue n  WHERE n.AttributeCode = b.AttributeCode  )

	OPEN Cur_AttributeDataUpdate 
	FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
	WHILE @@FETCH_STATUS = 0 
	BEGIN 
		SET @sqlt = 
			'
				UPDATE a  
				SET '+@AttributeCodeAtt+'= AttributeValue 
				FROM ZnodePimProduct a 
				INNER JOIN #UpdatePimAttributeValue m ON(m.PimProductId = a.pimProductId ) 
				WHERE m.AttributeCode = '''+@AttributeCodeAtt+'''
			' 
		PRINT @sqlt
		EXEC (@sqlt)

		FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
	END 
	CLOSE Cur_AttributeDataUpdate
	DEALLOCATE Cur_AttributeDataUpdate 
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodePimProduct' AND COLUMN_NAME = 'CategoryBanner')
BEGIN
    ALTER TABLE ZnodePimProduct DROP COLUMN CategoryBanner;
END             
        
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodePimProduct' AND COLUMN_NAME = 'CategoryName')
BEGIN
    ALTER TABLE ZnodePimProduct DROP COLUMN CategoryName;
END
              
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodePimProduct' AND COLUMN_NAME = 'CategoryTitle')
BEGIN
    ALTER TABLE ZnodePimProduct DROP COLUMN CategoryTitle;
END       
               
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodePimProduct' AND COLUMN_NAME = 'DisplayOrderCategory')
BEGIN
    ALTER TABLE ZnodePimProduct DROP COLUMN DisplayOrderCategory;
END      
        
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodePimProduct' AND COLUMN_NAME = 'CategoryImage')
BEGIN
    ALTER TABLE ZnodePimProduct DROP COLUMN CategoryImage;
END       
     
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodePimProduct' AND COLUMN_NAME = 'CategoryCode')
BEGIN
    ALTER TABLE ZnodePimProduct DROP COLUMN CategoryCode;
END