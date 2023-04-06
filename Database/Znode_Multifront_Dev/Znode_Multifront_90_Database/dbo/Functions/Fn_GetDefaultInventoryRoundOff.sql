CREATE FUNCTION [dbo].[Fn_GetDefaultInventoryRoundOff]
(@Inv NUMERIC(28, 6)
)
RETURNS VARCHAR(100)
AS
     -- Summary :- This function is used to round off the price value based on global setting 
     BEGIN
         -- Declare the return variable here
         DECLARE @RoundOffValue INT= Dbo.Fn_GetDefaultValue('InventoryRoundOff');
         IF @RoundOffValue = 0
             SET @RoundOffValue = -1;
         DECLARE @InvDetail VARCHAR(100);
         SET @InvDetail = SUBSTRING(CAST(@Inv AS VARCHAR(100)), 1, CHARINDEX('.', CAST(@Inv AS VARCHAR(100)))+@RoundOffValue);
         RETURN @InvDetail;
     END;