CREATE TABLE [dbo].[ZnodeDateFormat] (
    [DateFormatId]     INT           IDENTITY (1, 1) NOT NULL,
    [DateFormat]       VARCHAR (200) NULL,
    [ConversionFactor] INT           NULL,
    [IsDefault]        BIT           CONSTRAINT [DF_ZnodeDateFormat_IsDefault] DEFAULT ((0)) NOT NULL,
    [IsActive]         BIT           CONSTRAINT [DF_ZnodeDateFormat_IsActive] DEFAULT ((0)) NOT NULL,
    [MSSqlConvertId]   INT           NULL,
    [CultureName]      VARCHAR (200) NULL,
    [CreatedBy]        INT           NOT NULL,
    [CreatedDate]      DATETIME      NOT NULL,
    [ModifiedBy]       INT           NOT NULL,
    [ModifiedDate]     DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeDateFormat] PRIMARY KEY CLUSTERED ([DateFormatId] ASC)
);












GO
CREATE Trigger [dbo].[Trg_ZnodeDateFormat_GlobalSetting]  ON [dbo].[ZnodeDateFormat]
AFTER UPDATE AS 
BEGIN
 IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeGlobalSetting WHERE FeatureName = 'DateFormat')
 BEGIN
  INSERT INTO ZnodeGlobalSetting (FeatureName
,FeatureValues
,FeatureSubValues
,CreatedBy
,CreatedDate
,ModifiedBy
,ModifiedDate)
SELECT 'DateFormat',[DateFormat],MSSqlConvertId,1,GetDate(),1,Getdate() FROM [dbo].[ZnodeDateFormat] WHERE IsActive = 1 AND IsDefault = 1
 END 
 ELSE 
 BEGIN 
 UPDATE ZnodeGlobalSetting
 SET
FeatureValues = (SELECT [DateFormat] FROM [ZnodeDateFormat] WHERE IsActive = 1 AND IsDefault = 1)
,FeatureSubValues = (SELECT MSSqlConvertId FROM [ZnodeDateFormat] WHERE IsActive = 1 AND IsDefault = 1)
,CreatedBy  = (SELECT createdBy FROM Inserted ) 
,CreatedDate = (SELECT CreatedDate FROM Inserted ) 
,ModifiedBy = (SELECT ModifiedBy FROM Inserted ) 
,ModifiedDate = (SELECT ModifiedDate FROM Inserted ) 
 WHERE FeatureName = 'DateFormat'
 
 END 
END