CREATE FUNCTION [dbo].[Fn_GetPriceRoundOffValue]
(
)
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @PriceRoundOffValue INT;
         SELECT @PriceRoundOffValue = FeatureValues
         FROM ZnodeGlobalSetting
         WHERE FeatureName = 'PriceRoundOff';
         RETURN @PriceRoundOffValue;
     END;