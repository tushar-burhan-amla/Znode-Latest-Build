-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetProductTypeAttributeCode]()
RETURNS VARCHAR(600)
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @ProductTypeAttributeCode VARCHAR(600);
         
		 SET @ProductTypeAttributeCode = 'ProductType'

                   
         RETURN @ProductTypeAttributeCode;
     END;