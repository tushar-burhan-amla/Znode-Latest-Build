

CREATE FUNCTION [dbo].[Fn_GetIsDownloadableAttributeId]()
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @ProductTypeAttributeId INT;
         
		 SET @ProductTypeAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'IsDownloadable' AND ZPA.IsCategory = 0 )

                   
         RETURN @ProductTypeAttributeId;
     END;