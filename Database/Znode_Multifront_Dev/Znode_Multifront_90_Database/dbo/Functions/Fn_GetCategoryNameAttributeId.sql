-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetCategoryNameAttributeId]()
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @CategoryNameAttributeId INT;
         
		 SET @CategoryNameAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'CategoryName' AND ZPA.IsCategory = 1 )

                   
         RETURN @CategoryNameAttributeId;
     END;