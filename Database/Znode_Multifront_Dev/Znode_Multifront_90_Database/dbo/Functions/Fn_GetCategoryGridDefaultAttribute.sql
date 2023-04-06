CREATE FUNCTION [dbo].[Fn_GetCategoryGridDefaultAttribute] 
(
   
)
	-- Summary :- This function is used to  get default category attribute for grid   
	-- Unit Testing 
	-- EXEC [dbo].[Fn_GetCategoryGridDefaultAttribute] 

RETURNS @Items TABLE ( Id int Identity(1,1),PimAttributeId INT,AttributeCode VARCHAR(600) )

AS
BEGIN
	 
	   INSERT INTO @Items (PimAttributeId,AttributeCode)
	   SELECT PimAttributeId , AttributeCode  
       FROM ZnodePimAttribute ZPA 
	   WHERE IsCategory = 1 
	   AND AttributeCode  IN ('CategoryName')
	
      RETURN

END -- End Function