
CREATE FUNCTION [dbo].[Fn_GetDateTimeFormat]
(
)
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @MSSqlConvertId INT;
         SELECT @MSSqlConvertId = FeaturesubValues
         FROM ZnodeGlobalSetting
         WHERE FeatureName = 'DateFormat';
         RETURN @MSSqlConvertId;
     END;