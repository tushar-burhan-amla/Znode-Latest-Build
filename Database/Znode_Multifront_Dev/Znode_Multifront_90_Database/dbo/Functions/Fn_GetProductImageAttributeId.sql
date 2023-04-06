-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
 

Create FUNCTION [dbo].[Fn_GetProductImageAttributeId]()
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @ProductImageAttributeId INT;
         
		 SET @ProductImageAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'ProductImage' AND ZPA.IsCategory = 0 )

                   
         RETURN @ProductImageAttributeId;
     END;