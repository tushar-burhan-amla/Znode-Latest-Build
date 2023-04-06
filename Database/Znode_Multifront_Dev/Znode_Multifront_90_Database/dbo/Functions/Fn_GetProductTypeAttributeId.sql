-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetProductTypeAttributeId]()
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @ProductTypeAttributeId INT;
         
		 SET @ProductTypeAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'ProductType' AND ZPA.IsCategory = 0 )

                   
         RETURN @ProductTypeAttributeId;
     END;