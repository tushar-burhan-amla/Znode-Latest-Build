--ZPD-22446 Dt.27-Sept-2022
DECLARE @TimeZoneDetailsDesc NVARCHAR(500);
IF EXISTS (SELECT TOP 1 1 FROM ZnodeTimeZone WHERE LEFT(TimeZoneDetailsDesc,4)='(GMT')
BEGIN
	SET @TimeZoneDetailsDesc='
	DISABLE TRIGGER dbo.Trg_ZnodeTimeZone_GlobalSetting ON dbo.ZnodeTimeZone;

	UPDATE ZnodeTimeZone
	SET TimeZoneDetailsDesc=REPLACE(TimeZoneDetailsDesc,''(GMT'',''(UTC'')
	WHERE LEFT(TimeZoneDetailsDesc,4)=''(GMT'';

	ENABLE TRIGGER dbo.Trg_ZnodeTimeZone_GlobalSetting ON dbo.ZnodeTimeZone;
	'
EXEC (@TimeZoneDetailsDesc)
END