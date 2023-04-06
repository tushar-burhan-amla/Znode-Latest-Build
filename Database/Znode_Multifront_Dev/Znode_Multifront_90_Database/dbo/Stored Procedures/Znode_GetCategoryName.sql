

 CREATE PROCEDURE [dbo].[Znode_GetCategoryName]
  (
	  @CategoryCode nvarchar(300),
	  @LocaleId        INT        = 1
	  
)
AS

/*
exec Znode_GetCategoryName 'gg1',4

*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @DefaultLocaleId VARCHAR(20)= dbo.Fn_GetDefaultLocaleId()
	
; with cte_CategoryName as
(
SELECT distinct d.PimCategoryId , c.AttributeCode , b.CategoryValue , b.LocaleId  
FROM  
ZnodePimAttribute c 
INNER JOIN ZnodePimAttributeLocale zpal ON(c.PimAttributeId = zpal.PimAttributeId)
inner join ZnodePimCategoryAttributeValue d on (d.PimAttributeId = c.PimAttributeId)
inner JOIN  ZnodePimCategoryAttributeValueLocale b ON ( b.PimCategoryAttributeValueId = d.PimCategoryAttributeValueId )
where c.AttributeCode in  ('Categorycode','CategoryName' )

)

, CTE_BothLocale AS
(
select  cast(piv.CategoryName as nvarchar(max)) AS CategoryName, piv.localeid
from ZnodePimCategory aa
inner join  cte_CategoryName b PIVOT(MAX(CategoryValue) FOR AttributeCode IN (Categorycode,
																		CategoryName ))
																		piv on (piv.PimCategoryId = aa.PimCategoryId)
where piv.Categorycode = @CategoryCode and
 piv.LocaleId IN (@LocaleId,@DefaultLocaleId)

 )
 ,CTE_FirstLocale AS
 (
 SELECT CategoryName,localeid
 FROM CTE_BothLocale
 WHERE Localeid = @LocaleId
 )

 , CTE_FilterRecords AS
 (
  SELECT CategoryName,localeid
 FROM CTE_FirstLocale
 UNION ALL
  SELECT CategoryName,localeid
 FROM CTE_BothLocale BL
 WHERE Localeid = @DefaultLocaleId
 AND NOT EXISTS (SELECT TOP 1 1 FROM CTE_FirstLocale FL WHERE  FL.CategoryName = BL.CategoryName)

 )

 SELECT CategoryName
  from CTE_FilterRecords



--Declare @CategoryName nvarchar(300)

--SET @CategoryName = (select TOP 1 CategoryValue  from View_PimCategoryAttributeValue a
--inner join View_PimCategoryAttributeValue b on (a.PimCategoryId =b.PimCategoryId)

--where a.attributecode = 'categorycode' AND CategoryValue = @CategoryCode AND LocaleId = @Localeid)
--select @CategoryName

END