-- 
 --  SELECT * FROM dbo.
--  <ArrayOfProductWhereClause>
-- <ProductWhereClause>
--   <WhereClause>attributecode = 'productname' and attributevalue like '%apple%'</WhereClause>
-- </ProductWhereClause>
-- <ProductWhereClause>
--   <WhereClause>attributecode = producttype and attributevalue = 'simple product'</WhereClause>
-- </ProductWhereClause>
--</ArrayOfProductWhereClause>
	     

CREATE FUNCTION [dbo].[Fn_GetWhereClauseXML]
(
       @WhereClauseXml     XML 
)
RETURNS  @ConvertTableData TABLE (Id INT  IDENTITY(1,1),WhereClause NVARCHAR(1000)  )
AS
  -- This function convert the XML where clause into table rows 
 
     BEGIN
    
	 INSERT INTO @ConvertTableData(WhereClause)
	SELECT   ' AttributeCode '+Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(max)') +' AND AttributeValue '+Tbl.Col.value ( 'attributevalue[1]' , 'NVARCHAR(max)') 
FROM @WhereClauseXml.nodes ( '//ArrayOfWhereClauseModel/WhereClauseModel'  ) AS Tbl(Col)


	
	RETURN 
END;