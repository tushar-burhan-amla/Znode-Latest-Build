CREATE FUNCTION [dbo].[Fn_GetReportDateTimeFormat]
(
)
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @MSSqlConvertId INT;
         SELECT @MSSqlConvertId = FeaturesubValues
         FROM ZnodeGlobalSetting
         WHERE FeatureName = 'TimeFormat';
         RETURN @MSSqlConvertId;
     END;