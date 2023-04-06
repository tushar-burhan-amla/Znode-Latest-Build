-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetProductNameAttributeId]()
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @ProductNameAttributeId INT;
         
		 SET @ProductNameAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'ProductName' AND ZPA.IsCategory = 0 )

                   
         RETURN @ProductNameAttributeId;
     END;