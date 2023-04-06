
CREATE View [dbo].[View_PimProductAttributeValueLocale] As 
   SELECT ZPAV.PimAttributeValueId
,ZPAV.PimAttributeFamilyId
,ZPAV.PimProductId
,ZPAV.PimAttributeId
,ZPAV.PimAttributeDefaultValueId
,ZPAV.CreatedBy
,CONVERT(DATE,ZPAV.CreatedDate)CreatedDate
,ZPAV.ModifiedBy
,CONVERt(DATE,ZPAV.ModifiedDate)ModifiedDate, ZPAVL.LocaleId,ZPAVL.AttributeValue   FROM ZnodePimAttributeValue ZPAV
   LEFT JOIN ZnodePimAttributeValueLocale ZPAVL ON (ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId)