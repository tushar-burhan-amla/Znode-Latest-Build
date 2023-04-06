CREATE FUNCTION [dbo].[Fn_GetDate]
(

)
RETURNS DATETIME
AS
/*
	This function is used to Get the current date.
*/
BEGIN
	-- Declare the return variable here
	DECLARE @Date DATETIME;
	SET @Date = GETDATE();
	--		@FeatureValues NVARCHAR(MAX),
	--		@BaseUtcOffsetSec INT;

	--SET @FeatureValues = (SELECT TOP 1 FeatureValues FROM ZnodeGlobalSetting WITH (NOLOCK) WHERE FeatureName = 'SavedTimeZone');

	--SET @BaseUtcOffsetSec = (SELECT TOP 1 DifferenceInSeconds FROM ZnodeTimeZone WITH (NOLOCK) WHERE TimeZoneDetailsCode=@FeatureValues);

	--SET @Date = DATEADD(SECOND, @BaseUtcOffsetSec,GETUTCDATE());

	RETURN @Date;
END