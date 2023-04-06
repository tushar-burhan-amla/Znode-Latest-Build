
--DELETE FROM ZnodePublishProduct 
--DELETE FROM ZnodePublishCategory
--DELETE FROM ZnodePublishCatalog

-- EXEC [ZNode_GetPublishCatalog]  1,@UserId=1 
CREATE  PROCEDURE [dbo].[ZNode_GetPublishCatalog_1]
(
@PimCatalogId INT 
,@PimCategoryId INT = NULL 
,@PimProductId INT = NULL 
,@PublishCatalogId INT = NULL 
,@UserId    INT 
)
AS 
BEGIN 
BEGIN TRAN 
BEGIN TRY 
SET NOCOUNT ON 
	

	-------------------------------------- Catalog details -------------------------------------------------------------
	DECLARE @V_PublishCatalogId INT  
	
	DECLARE @T_PublishCatalogId TABLE (PublishCatalogId INT ) 
	
	DELETE FROM ZnodePublishCatalog WHERE PimCatalogId = @PimCatalogId
	
	INSERT INTO ZnodePublishCatalog (PimCatalogId,CatalogName,IsActive,ExternalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate )
	OUTPUT INSERTED.PublishCatalogId INTO @T_PublishCatalogId
	SELECT PimCatalogId,CatalogName,IsActive,ExternalId,@UserId,GetUtcDAte(),@UserId,GetUtcDAte() 
	FROM  ZnodePimCatalog q
	WHERE PimCatalogId =@PimCatalogId  AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishCatalog a WHERE a.CatalogName =  q.CatalogName)

	--SELECT  PublishCatalogId FROM @T_PublishCatalogId
	SET @V_PublishCatalogId = (SELECT  PublishCatalogId FROM @T_PublishCatalogId)

	SELECT PublishCatalogId,CatalogName,IsActive,ExternalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate  
	FROM ZnodePublishCatalog WHERE PublishCatalogId = @V_PublishCatalogId


   -------------------------------------------- Category Details ----------------------------------------------------------------------------------------------

	DECLARE @AttriuteValues TABLE (PimCategoryId INT , AttributeName   nVARCHAr(200),AttributeValue nVarchar (600),PimProductId INT)
	
			
	INSERT INTO @AttriuteValues
	SELECT a.PimCategoryId,d.AttributeName , 
		CASE WHEN  EXISTS (SELECT TOP 1 1 FROM ZNodeMedia zm WHERE EXISTS (SELECT TOP 1 1 FROM dbo.Split(RTRIM(LTRIM(b.CategoryValue)),',') aa WHERE zm.MediaId = aa.item)) 
																			AND j.AttributeTypeName IN ('Image','Audio','Video','File','GalleryImages')
			THEN dbo.FN_GetThumbnailMediaPath(RTRIM(LTRIM(b.CategoryValue))) ELSE RTRIM(LTRIM(b.CategoryValue)) END  AttributeValue,e.PimProductId 
	
	FROM ZnodePimCategoryAttributeValue a  
	Inner JOIN ZnodePimCategoryAttributeValueLocale b ON (a.PimCategoryAttributeValueId = b.PimCategoryAttributeValueId AND b.LocaleId = 1 )
	INNER JOIN ZNodePimAttribute c ON (a.PimAttributeId = c.PimAttributeId AND c.IsCategory = 1 )
	INNER JOIN ZnodePimAttributeLocale d  ON (d.PimAttributeId = c.PimAttributeId AND d.Localeid = b.LocaleId)
	INNER JOIN ZnodePimCatalogCategory e ON (e.PimCategoryId = a.PimCategoryId)
	INNER JOIN ZnodePimCategory f ON (f.PimCategoryId = e.PimCategoryId)
	LEFT JOIN ZnodeAttributeType j oN (c.AttributeTypeId = j.AttributeTypeId)
	WHERE e.PimCatalogid  = @PimCatalogId 

	DECLARE @InsertedCategory table (PublishCategoryId INT , PublishCatalogId  INT ,PimCategoryId INT )

	DECLARE @V_AttributeName nVARCHAR(600), @VSQL nVARCHAR(MAX)

	IF EXISTS (SELECT TOP 1 1 FROM ZnodePublishCategory WHERE PublishCatalogId = @V_PublishCatalogId) 
	BEGIN
	 DELETE FROM ZnodePublishProduct WHERE PublishCatalogId = @V_PublishCatalogId
	 DELETE FROM ZnodePublishCategory WHERE PublishCatalogId = @V_PublishCatalogId
	 
	END 

	INSERT INTO ZnodePublishCategory (PublishCatalogId,PimCategoryId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	OUTPUT INSERTED.PublishCategoryId,inserted.PublishCatalogId,INSERTED.PimCategoryId INTO @InsertedCategory
	SELECT @V_PublishCatalogId,PimCategoryId,@UserId,GETUTCDATE(),@UserId,GETUTCDATE() FROM @AttriuteValues  GROUP BY PimCategoryId

	SELECT DISTINCT PublishCategoryId ,AttributeName,AttributeValue
		,SUBSTRING ((SELECT DISTINCT ','+CAST(PimProductId AS VARCHAR(4000)) FROM @AttriuteValues a WHERE a.PimCategoryId = q.PimCategoryId FOR XML PATH ('') ),2,4000 ) Productids
	INTO #CategoryAttriuteValues
	FROM @AttriuteValues q
	INNER JOIN @InsertedCategory b ON (q.PimCategoryId = b.PimCategoryId)
	
	SET @V_AttributeName = SUBSTRING ((SELECT DISTINCT ','+ QUOTENAME(AttributeName) FROM #CategoryAttriuteValues FOR XML PATH('') ) ,2, 40000)

	SET @VSQL = 'SELECT * 
	FROM #CategoryAttriuteValues a PIVOT ( 
	MAX(AttributeValue) FOR AttributeName IN ('+@V_AttributeName+')) piv '

	EXEC SP_EXECUTESQL @VSQL

 ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------




	DECLARE @ProductDetails TAble (PimProductId INT , AttributeName NVARCHAR(MAX) ,AttributeValue NVARCHAR(MAx),PimCategoryId INT  )

	INSERT INTO @ProductDetails 
	SELECT a.PimProductId,d.AttributeName 
			,CASE when D.AttributeName = 'Product Type' 
					then DBO.FN_GetPimAttributeDefaultValues(b.AttributeValue,b.LocaleId) else CASE WHEN  EXISTS (SELECT TOP 1 1 FROM ZNodeMedia zm 
					WHERE EXISTS (SELECT TOP 1 1 FROM dbo.Split(RTRIM(LTRIM(b.AttributeValue)),',') aa WHERE zm.MediaId = aa.item)) AND j.AttributeTypeName IN ('Image','Audio','Video','File','GalleryImages')
			THEN dbo.FN_GetThumbnailMediaPath(RTRIM(LTRIM(b.AttributeValue))) ELSE RTRIM(LTRIM(b.AttributeValue)) END end AttributeValue, e.PimCategoryId
	-- INTO #AttriuteValues
	FROM ZnodePimAttributeValue a  
	Inner JOIN ZnodePimAttributeValueLocale b ON (a.PimAttributeValueId = b.PimAttributeValueId AND b.LocaleId = 1 )
	INNER JOIN ZNodePimAttribute c ON (a.PimAttributeId = c.PimAttributeId)
	INNER JOIN ZnodePimAttributeLocale d  ON (d.PimAttributeId = c.PimAttributeId AND d.Localeid = b.LocaleId)
	INNER JOIN ZnodePimCatalogCategory e ON (e.PimProductId = a.PimProductId)
	LEFT JOIN ZnodePimCategory f ON (f.PimCategoryId = e.PimCategoryId)
	LEFT JOIN ZnodeAttributeType j oN (c.AttributeTypeId = j.AttributeTypeId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @AttriuteValues  w WHERE w.PimProductId = a.PimProductId) 
	--SELECT * FROM #AttriuteValues
	--DECLARE @V_AttributeName nVARCHAR(600), @VSQL nVARCHAR(MAX)


	DECLARE @InsertProduct TABLE (PublishProductId INT , PimProductId INT )

	INSERT INTO ZnodePublishProduct(PublishCategoryId
					,PublishCatalogId
					,PimProductId
					,CreatedBy
					,CreatedDate
					,ModifiedBy
					,ModifiedDate) 
    OUTPUT INSERTED.PublishProductId , INSERTED.PimProductId INTO @InsertProduct
	SELECT DISTINCT PublishCategoryId,PublishCatalogId,a.PimProductId,@userId ,GETUTCDATE(),@userId , GETUTCDATE()
	FROM @ProductDetails a 
	INNER JOIN @InsertedCategory b ON (a.PimCategoryId = b.PimCategoryId)
	

	SELECT a.PublishProductId , b.AttributeName , b.AttributeValue
	INTO #AttriuteValues
	FROM @InsertProduct a 
	INNER JOIN @ProductDetails b ON (a.PimProductId = b.PimProductId)

	SET @V_AttributeName = SUBSTRING ((SELECT DISTINCT ','+ QUOTENAME(RTRIM(LTRIM(AttributeName))) FROM #AttriuteValues FOR XML PATH('') ) ,2, 40000)

	SET @VSQL = 'SELECT * FROM #AttriuteValues a PIVOT ( 
	MAX(AttributeValue) FOR AttributeName IN ('+@V_AttributeName+')) piv '

	EXEC SP_EXECUTESQL @VSQL


	DROP TABLE #CategoryAttriuteValues , #AttriuteValues
COMMIT TRAN 
END TRY 
BEGIN CATCH 
ROLLBACK TRAN 
SELECT ERROR_MESSAGE (),ERROR_LINE ()
END CATCH 
END