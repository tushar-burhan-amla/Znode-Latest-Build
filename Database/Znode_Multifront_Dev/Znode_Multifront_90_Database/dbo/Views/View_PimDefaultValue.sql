CREATE VIEW [dbo].[View_PimDefaultValue] AS 
Select a.PimAttributeDefaultValueId
,a.PimAttributeId
,a.AttributeDefaultValueCode
,a.IsEditable
,a.CreatedBy,a.DisplayOrder
,CONVERT(DATE,a.CreatedDate)CreatedDate
,a.ModifiedBy
,CONVERt(DATE,a.ModifiedDate)ModifiedDate,b.AttributeDefaultValue,b.LocaleId,b.Description,b.PimAttributeDefaultValueLocaleId 
,a.IsDefault
from [dbo].[ZnodePimAttributeDefaultValue] a
INNER JOIN  [dbo].[ZnodePimAttributeDefaultValueLocale] b ON (a.PimAttributeDefaultValueId =b.PimAttributeDefaultValueId )
GO