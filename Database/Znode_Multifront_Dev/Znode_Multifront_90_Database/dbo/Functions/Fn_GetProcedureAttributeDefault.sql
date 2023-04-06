CREATE FUNCTION [dbo].[Fn_GetProcedureAttributeDefault]
(@Value VARCHAR(600)
)
-- Summary :- This function is used to     
-- Unit Testing 
-- EXEC [dbo].[Fn_GetAttributeDefault] 

RETURNS @Items TABLE
(Id    INT IDENTITY(1, 1),
 Value VARCHAR(600)
)
AS
     BEGIN
         IF @Value = 'MediaAttributeType'
             BEGIN
                 INSERT INTO @Items
                 VALUES('Image'), ('Audio'), ('Video'), ('File'), ('GallaryImage');
             END;
         ELSE
         IF @Value = 'AttributeCode'
             BEGIN
                 INSERT INTO @Items
                 VALUES('ProductName'), ('Sku'), ('ProductType'), ('IsActive'), ('Price'), ('Quantity'), ('DispalyOrder'), ('Assortment'), ('Brand'), ('ShippingCost'), ('Vendor');
             END;
         ELSE
         IF @value = 'OrderState'
             BEGIN
                 INSERT INTO @Items
                 VALUES ('ORDERED');
             END;;
         RETURN;
     END; -- End Function