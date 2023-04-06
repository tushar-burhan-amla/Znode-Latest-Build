CREATE FUNCTION [dbo].[Fn_GetDefaultCurrencySymbol]
(
)
RETURNS NVARCHAR(100)
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @DefaultCurrency NVARCHAR(100);
         SET @DefaultCurrency =
         (
             SELECT TOP 1 ZCC.Symbol
             FROM ZnodeGlobalSetting ZGS 
			 LEFT JOIN ZnodeCulture ZCC ON (ZCC.CultureCode = ZGS.FeatureValues)
             WHERE ZGS.FeatureName = 'Currency'
         );
         RETURN @DefaultCurrency;
     END;