CREATE FUNCTION [dbo].[Fn_GetProductGridAttributes]
(
)
-- Summary :- This function is used to  get default category attribute for grid   
-- Unit Testing 
-- EXEC [dbo].[Fn_GetCategoryGridDefaultAttribute] 

RETURNS @Items TABLE
(Id             INT IDENTITY(1, 1),
 PimAttributeId INT,
 AttributeCode  VARCHAR(600)
)
AS
     BEGIN
         INSERT INTO @Items (PimAttributeId, AttributeCode )
                SELECT PimAttributeId,AttributeCode
                FROM ZnodePimAttribute ZPA
                WHERE IsCategory = 0
				AND (ISNULL(IsShowONGrid,0) = 1 OR IsConfigurable = 1 ) 
                UNION 
				SELECT 0,'AttributeFamily'
				UNION 
				SELECT PimAttributeId,AttributeCode
				FROM ZnodePimAttribute ZPA
				WHERE AttributeCode = 'OutOfStockOptions'
         RETURN;
     END; -- End Function