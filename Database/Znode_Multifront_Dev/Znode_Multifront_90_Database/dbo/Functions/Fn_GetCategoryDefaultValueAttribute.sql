CREATE FUNCTION [dbo].[Fn_GetCategoryDefaultValueAttribute]()
	-- Summary :- This function is used to Get the category default attribute Id 
	-- Unit Testing 
	-- EXEC [dbo].[Fn_GetAttributeDefault] 

RETURNS @Items TABLE ( Id int Identity(1,1),PimAttributeId INT ,AttributeCode  VARCHAR(600) )

AS
BEGIN
	  
	   INSERT INTO @Items(PimAttributeId ,AttributeCode)
	   SELECT PimAttributeId ,AttributeCode 
	   FROM ZnodePimAttribute ZPA 
	   INNER JOIN ZnodeAttributeType ZAY ON (ZAY.AttributeTypeId = ZPA.AttributeTypeId )
	   WHERE AttributeTypeName IN ('Multi Select','Simple Select','Yes/No') 
  
	
      RETURN

END -- End Function