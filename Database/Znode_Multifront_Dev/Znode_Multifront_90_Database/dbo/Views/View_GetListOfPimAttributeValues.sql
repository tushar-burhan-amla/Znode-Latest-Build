

 CREATE View [dbo].[View_GetListOfPimAttributeValues]
AS
	----comment code for below requirement
	--SELECT ISNULL(NULL,0) RowId, ZPAVL.AttributeValue  
	--FROM ZnodePimAttribute  ZPA INNER JOIN ZnodePimAttributeValue ZPAV  ON ZPA.PimAttributeId = ZPAV.PimAttributeId
	--NNER JOIN ZnodePimAttributeValueLocale ZPAVL on ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
	
	---- Added Isdownloadable column
	WITH CTE AS
	(
		select DISTINCT ZPAV.PimProductId ,Case when ZPDP.SKU is not null then 1 else 0 END IsDownloadable 
		from ZnodePimAttribute  ZPA INNER JOIN ZnodePimAttributeValue ZPAV  ON ZPA.PimAttributeId = ZPAV.PimAttributeId
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL on ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
		LEFT join ZnodePimDownloadableProduct ZPDP ON zpavl.AttributeValue = ZPDP.SKU
		where ZPA.AttributeCode = 'SKU'
	)
	SELECT ISNULL(NULL,0) RowId, ZPAVL.AttributeValue, C.IsDownloadable  
	FROM ZnodePimAttribute  ZPA 
	INNER JOIN ZnodePimAttributeValue ZPAV  ON ZPA.PimAttributeId = ZPAV.PimAttributeId
	INNER JOIN ZnodePimAttributeValueLocale ZPAVL on ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
	INNER JOIN CTE C ON ZPAV.PimProductId = C.PimProductId