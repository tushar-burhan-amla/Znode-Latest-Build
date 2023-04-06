-- =============================================
-- Create the Order by clause for dyanamic statement  
-- =============================================
CREATE FUNCTION [dbo].[Fn_GetDefaultPriceRoundOff]
(@Price NUMERIC(28, 6)
)
RETURNS VARCHAR(100)
AS
     -- Summary :- This function is used to round off the price value based on global setting 
     BEGIN
         -- Declare the return variable here
         DECLARE @RoundOffValue INT= dbo.Fn_GetDefaultValue('PriceRoundOff');
         DECLARE @PriceDetail VARCHAR(100);
         SET @PriceDetail = SUBSTRING(CAST(@Price AS VARCHAR(100)),1, CHARINDEX('.', CAST(@Price AS VARCHAR(100)))+@RoundOffValue);
         RETURN @PriceDetail;
     END;