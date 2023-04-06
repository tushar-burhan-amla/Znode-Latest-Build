CREATE FUNCTION [dbo].[Fn_GetProductDefaultFilterAttributes]
(
)
-- Summary :- This function is used to     
-- Unit Testing 
-- EXEC [dbo].[Fn_GetAttributeDefault] 

RETURNS @Items TABLE
(PimAttributeId INT , AttributeCode VARCHAR(600)
)
AS
     BEGIN
         
                 INSERT INTO @Items (PimAttributeId , AttributeCode )
                 SELECT PimAttributeId , AttributeCode 
				 FROM ZnodePimAttribute ZPA 
				 WHERE AttributeCode IN ('Brand','Vendor','ShippingCostRules','Highlights') 
				
				
         RETURN;
     END; -- End Function