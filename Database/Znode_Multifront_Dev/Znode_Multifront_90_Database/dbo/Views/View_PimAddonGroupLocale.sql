

CREATE view [dbo].[View_PimAddonGroupLocale]
AS 
SELECT a.PimAddonGroupId
,a.DisplayType
,a.CreatedBy
,CONVERT(DATE,a.CreatedDate)CreatedDate
,a.ModifiedBy
,CONVERt(DATE,a.ModifiedDate)ModifiedDate,b.LocaleId,b.AddonGroupName 
FROM ZnodePimAddonGroup a
INNER JOIN ZnodePimAddonGroupLocale b ON (a.PimAddonGroupId = b.PimAddonGroupId)