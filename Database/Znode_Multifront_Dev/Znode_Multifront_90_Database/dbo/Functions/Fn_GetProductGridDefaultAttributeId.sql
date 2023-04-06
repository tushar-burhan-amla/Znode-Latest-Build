-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE  FUNCTION [dbo].[Fn_GetProductGridDefaultAttributeId]()
RETURNS VARCHAR(max)
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @ProductGridDefaultAttributeId VARCHAR(max);
         
		 SET @ProductGridDefaultAttributeId = substring ((SELECT ','+CAST( PimAttributeId AS VARCHAR(100))  FROM ZnodePimAttribute ZPA 
											        WHERE ZPA.AttributeCode IN ('ProductName' , 'SKU' , 'Price' , 'Quantity' , 'IsActive' , 'ProductType' , 'ProductImage' , 'Assortment' , 'DisplayOrder')
                                                    FOR XML PATH ('')),2,4000)

                   
         RETURN @ProductGridDefaultAttributeId;
     END;