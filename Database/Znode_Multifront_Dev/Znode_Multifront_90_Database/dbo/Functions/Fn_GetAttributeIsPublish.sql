CREATE  FUNCTION [dbo].[Fn_GetAttributeIsPublish]()
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @ProductTypeAttributeId INT;
         
		 SET @ProductTypeAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'PublishStatus' AND ZPA.IsCategory = 0 )

                   
         RETURN @ProductTypeAttributeId;
     END;