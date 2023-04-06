CREATE  FUNCTION [dbo].[Fn_GetGridPimAttributes]
(
)
-- Summary :- This function is used to     
-- Unit Testing 
-- EXEC [dbo].[Fn_GetAttributeDefault] 

RETURNS @Items TABLE
(Id             INT IDENTITY(1, 1),
 PimAttributeId INT,
 AttributeCode  VARCHAR(600)
)
AS
     BEGIN
    BEGIN
        INSERT INTO @Items
        (PimAttributeId,
         AttributeCode
        )
               SELECT PimAttributeId,
                      AttributeCode
               FROM ZnodePimAttribute ZPA
               WHERE IsShowOnGrid = 1 AND 
				     IsCategory = 0;
    END;
         RETURN;
     END; -- End Function