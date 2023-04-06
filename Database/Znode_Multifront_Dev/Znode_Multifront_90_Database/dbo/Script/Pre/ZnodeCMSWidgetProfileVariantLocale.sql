
IF (EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodeCMSWidgetProfileVariant' AND COLUMN_NAME = 'CMSWidgetTemplateId')
 AND EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ZnodeCMSWidgetProfileVariant' AND COLUMN_NAME = 'LocaleId'))
 BEGIN

 IF EXISTS(SELECT * FROM SYS.TABLES WHERE NAME = 'ZnodeCMSWidgetProfileVariant_Temp')
 BEGIN
	DROP TABLE ZnodeCMSWidgetProfileVariant_Temp
 END
CREATE TABLE ZnodeCMSWidgetProfileVariant_Temp (
    CMSWidgetProfileVariantId int NULL,
    CMSContentWidgetId int  NULL,
	ProfileId int,
	LocaleId int  NULL,
	CMSWidgetTemplateId int,
	CreatedBy int,
	CreatedDate datetime,
	ModifiedBy int,
	ModifiedDate datetime
);

declare @sql varchar(max)
set @sql = '
INSERT INTO ZnodeCMSWidgetProfileVariant_Temp(CMSWidgetProfileVariantId,CMSContentWidgetId,ProfileId,LocaleId,CMSWidgetTemplateId)
SELECT CMSWidgetProfileVariantId,CMSContentWidgetId,ProfileId,LocaleId,CMSWidgetTemplateId
FROM ZnodeCMSWidgetProfileVariant'
exec (@sql)
END
