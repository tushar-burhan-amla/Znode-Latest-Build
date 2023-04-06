
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetProductVendorAttributeId]
(
)
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @ProductSKUAttributeId INT;
         SET @ProductSKUAttributeId =
         (
             SELECT TOP 1 PimAttributeId
             FROM ZnodePimAttribute ZPA
             WHERE ZPA.AttributeCode = 'Vendor'
                   AND ZPA.IsCategory = 0
         );
         RETURN @ProductSKUAttributeId;
     END;