
Update ZnodeGlobalEntity Set EntityName = 'Content Containers'
WHERE EntityName = 'Content Widgets'
go
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PortalId' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
BEGIN
	ALTER TABLE ZnodeCMSWidgetProfileVariant ADD PortalId INT
END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PortalId' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
AND EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PortalId' AND TABLE_NAME = 'ZnodeCMSContentWidget')
BEGIN
	DECLARE @SQL VARCHAR(MAX) = ''
	SET @SQL = '
	UPDATE A SET a.PortalId = b.PortalId
	FROM ZnodeCMSWidgetProfileVariant A
	INNER JOIN ZnodeCMSContentWidget B ON A.CMSContentWidgetId = B.CMSContentWidgetId
	WHERE a.PortalId IS NULL AND b.PortalId IS NOT NULL'

	EXEC (@SQL)
END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PortalId' AND TABLE_NAME = 'ZnodeCMSContentWidget')
BEGIN
	ALTER TABLE ZnodeCMSContentWidget DROP COLUMN PortalId
END
GO
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'CreatedBy' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
BEGIN
	ALTER TABLE ZnodeCMSWidgetProfileVariant ADD [CreatedBy] INT    NULL
END
GO
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'CreatedDate' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
BEGIN
	ALTER TABLE ZnodeCMSWidgetProfileVariant ADD [CreatedDate] DATETIME    NULL
END
GO
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ModifiedBy' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
BEGIN
	ALTER TABLE ZnodeCMSWidgetProfileVariant ADD ModifiedBy INT    NULL
END
GO
IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ModifiedDate' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
BEGIN
	ALTER TABLE ZnodeCMSWidgetProfileVariant ADD ModifiedDate DATETIME    NULL
END
GO

UPDATE A SET a.CreatedBy = b.CreatedBy
FROM ZnodeCMSWidgetProfileVariant A
INNER JOIN ZnodeCMSContentWidget B ON A.CMSContentWidgetId = B.CMSContentWidgetId
WHERE a.CreatedBy IS NULL AND b.CreatedBy IS NOT NULL

UPDATE A SET a.CreatedDate = b.CreatedDate
FROM ZnodeCMSWidgetProfileVariant A
INNER JOIN ZnodeCMSContentWidget B ON A.CMSContentWidgetId = B.CMSContentWidgetId
WHERE a.CreatedDate IS NULL AND b.CreatedDate IS NOT NULL

UPDATE A SET a.ModifiedBy = b.ModifiedBy
FROM ZnodeCMSWidgetProfileVariant A 
INNER JOIN ZnodeCMSContentWidget B ON A.CMSContentWidgetId = B.CMSContentWidgetId
WHERE a.ModifiedBy IS NULL AND b.ModifiedBy IS NOT NULL

UPDATE A SET a.ModifiedDate = b.ModifiedDate
FROM ZnodeCMSWidgetProfileVariant A
INNER JOIN ZnodeCMSContentWidget B ON A.CMSContentWidgetId = B.CMSContentWidgetId
WHERE a.ModifiedDate IS NULL AND b.ModifiedDate IS NOT NULL

GO

UPDATE A SET a.CreatedBy = 2
FROM ZnodeCMSWidgetProfileVariant A
WHERE a.CreatedBy IS NULL

UPDATE A SET a.CreatedDate = getdate()
FROM ZnodeCMSWidgetProfileVariant A
WHERE a.CreatedDate IS NULL

UPDATE A SET a.ModifiedBy = 2
FROM ZnodeCMSWidgetProfileVariant A
WHERE a.ModifiedBy IS NULL

UPDATE A SET a.ModifiedDate = getdate()
FROM ZnodeCMSWidgetProfileVariant A
WHERE a.ModifiedDate IS NULL

GO
if exists(select * from sys.tables where name = 'ZnodeCMSWidgetProfileVariant_Temp')
begin
	Declare @sql2 varchar(max)
	set @sql2 ='
	UPDATE A SET a.CreatedBy = b.CreatedBy, a.CreatedDate = b.CreatedDate,a.ModifiedBy = b.ModifiedBy,a.ModifiedDate = b.ModifiedDate
	from ZnodeCMSWidgetProfileVariant b
	inner join ZnodeCMSWidgetProfileVariant_Temp a on a.CMSWidgetProfileVariantId = b.CMSWidgetProfileVariantId'
	exec (@sql2)
end
go
IF EXISTS(SELECT * FROM SYS.TABLES WHERE NAME = 'ZnodeCMSWidgetProfileVariant_Temp')
AND EXISTS(SELECT * FROM SYS.TABLES WHERE NAME = 'ZnodeCMSWidgetProfileVariantLocale')
BEGIN
	DECLARE @SQL1 VARCHAR(MAX) = ''
	SET @SQL1 = '
	INSERT INTO ZnodeCMSWidgetProfileVariantLocale(CMSWidgetProfileVariantId,CMSWidgetTemplateId,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	SELECT CMSWidgetProfileVariantId,CMSWidgetTemplateId,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
	FROM ZnodeCMSWidgetProfileVariant_Temp a
	WHERE NOT EXISTS(SELECT * FROM ZnodeCMSWidgetProfileVariantLocale WHERE ZnodeCMSWidgetProfileVariantLocale.CMSWidgetProfileVariantId = a.CMSWidgetProfileVariantId)'

	EXEC (@SQL1)
END
GO
IF (EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodeCMSWidgetProfileVariant' AND COLUMN_NAME='CMSWidgetTemplateId')
AND EXISTS(select * from sys.objects where name =  'FK_ZnodeCMSWidgetProfileLocale_ZnodeLocale'))
BEGIN
	alter table ZnodeCMSWidgetProfileVariant drop constraint FK_ZnodeCMSWidgetProfileLocale_ZnodeCMSWidgetTemplate 
END
GO
IF (EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodeCMSWidgetProfileVariant' AND COLUMN_NAME='LocaleId')
AND EXISTS(select * from sys.objects where name =  'FK_ZnodeCMSWidgetProfileLocale_ZnodeLocale'))
BEGIN
	alter table ZnodeCMSWidgetProfileVariant drop constraint FK_ZnodeCMSWidgetProfileLocale_ZnodeLocale 
END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'CMSWidgetTemplateId' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
BEGIN
	ALTER TABLE ZnodeCMSWidgetProfileVariant DROP COLUMN CMSWidgetTemplateId
END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'LocaleId' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
BEGIN
	ALTER TABLE ZnodeCMSWidgetProfileVariant DROP COLUMN LocaleId
END

GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'CreatedBy' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
BEGIN
	ALTER TABLE ZnodeCMSWidgetProfileVariant ALTER COLUMN [CreatedBy] INT  NOT  NULL
END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'CreatedDate' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
BEGIN
	ALTER TABLE ZnodeCMSWidgetProfileVariant ALTER COLUMN [CreatedDate] DATETIME  NOT  NULL
END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ModifiedBy' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
BEGIN
	ALTER TABLE ZnodeCMSWidgetProfileVariant ALTER COLUMN ModifiedBy INT  NOT  NULL
END
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'ModifiedDate' AND TABLE_NAME = 'ZnodeCMSWidgetProfileVariant')
BEGIN
	ALTER TABLE ZnodeCMSWidgetProfileVariant ALTER COLUMN ModifiedDate DATETIME  NOT  NULL
END
GO
IF EXISTS(SELECT * FROM SYS.TABLES WHERE NAME = 'ZnodeCMSWidgetProfileVariant_Temp')
 BEGIN
	DROP TABLE ZnodeCMSWidgetProfileVariant_Temp
 END