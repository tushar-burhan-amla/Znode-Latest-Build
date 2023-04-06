IF NOT EXISTS(SELECT 1 FROM sys.columns
WHERE Name = N'IsNewTab'
AND Object_ID = Object_ID(N'dbo.ZnodePublishWidgetTitleEntity'))
BEGIN
Alter table ZnodePublishWidgetTitleEntity add IsNewTab bit null
END