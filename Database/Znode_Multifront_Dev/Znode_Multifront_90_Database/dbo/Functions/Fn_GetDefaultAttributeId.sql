CREATE FUNCTION [dbo].[Fn_GetDefaultAttributeId]
(
)
-- Summary :- This function is used to     
-- Unit Testing 
-- SELECT * FROM  [dbo].[Fn_GetDefaultAttributeId] ()

RETURNS @Items TABLE
(PimAttributeId INT , AttributeCode VARCHAR(600)
)
AS
     BEGIN
         
                 INSERT INTO @Items (PimAttributeId , AttributeCode)
                 SELECT ZPA.PimAttributeId , AttributeCode 
				 FROM ZnodePimAttribute ZPA 
				 INNER JOIN ZnodePimAttributeDefaultValue ZPADV ON (ZPADV.PimAttributeId = ZPA.PimAttributeId)
				 INNER JOIN ZnodePimAttributeDefaultValueLocale ZPADVL ON (ZPADVL.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId )
				 INNER JOIN ZnodeAttributeType ZPT ON (ZPT.AttributeTypeId = ZPA.AttributeTypeId )
				 WHERE ZPADVL.AttributeDefaultValue IS NOT NULL AND ZPADVL.AttributeDefaultValue <> ''
				 AND ZPT.AttributeTypeName IN ('Simple Select','Multi Select')
				 GROUP BY ZPA.PimAttributeId , AttributeCode 

             
         RETURN;
     END; -- End Function

	 -- SELECT * FROM ZnodeAttributeType 