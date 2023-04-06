CREATE FUNCTION [dbo].[Fn_GetProductMultiSelectAttributes]
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
				 INNER JOIN ZnodeAttributeType ZAT ON (ZAT.AttributeTypeId = ZPA.AttributeTypeId)
				 WHERE ZAT.AttributeTypeName = 'Multi Select'

             
         RETURN;
     END; -- End Function