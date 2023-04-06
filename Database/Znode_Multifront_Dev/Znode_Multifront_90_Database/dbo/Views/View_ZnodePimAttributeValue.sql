

CREATE vIEW [dbo].[View_ZnodePimAttributeValue] AS 
		SELECT b.PimAttributeValueId
,b.PimAttributeFamilyId
,b.PimProductId
,b.PimAttributeId
,b.PimAttributeDefaultValueId
,b.AttributeValue
,b.CreatedBy
,CONVERT(DATE,b.CreatedDate)CreatedDate
,b.ModifiedBy
,CONVERt(DATE,b.ModifiedDate)ModifiedDate, e.AttributeValue ProductName,e.localeid
		FROM ZnodePimAttributeValue b 
		INNER JOIN ZnodePimAttributeValueLocale e ON (b.PimAttributeValueId = e.PimAttributeValueId)