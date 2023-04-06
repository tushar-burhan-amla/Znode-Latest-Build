CREATE TABLE [dbo].[ZnodeTimeFormat] (
    [TimeFormatId]     INT            IDENTITY (1, 1) NOT NULL,
    [TimeFormat]       NVARCHAR (200) NULL,
    [ConversionFactor] INT            NULL,
    [IsDefault]        BIT            CONSTRAINT [DF_ZnodeTimeFormat_IsDefault] DEFAULT ((0)) NOT NULL,
    [IsActive]         BIT            CONSTRAINT [DF_ZnodeTimeFormat_IsActive] DEFAULT ((0)) NOT NULL,
    [MSSqlConvertId]   INT            NULL,
    [CultureName]      VARCHAR (200)  NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedBy]       INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeTimeFormat] PRIMARY KEY CLUSTERED ([TimeFormatId] ASC)
);


GO

CREATE Trigger [dbo].[Trg_ZnodeTimeFormat_GlobalSetting]  ON [dbo].[ZnodeTimeFormat]
AFTER UPDATE AS 
BEGIN
 IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeGlobalSetting WHERE FeatureName = 'TimeFormat')
 BEGIN
  INSERT INTO ZnodeGlobalSetting (FeatureName
,FeatureValues
,FeatureSubValues
,CreatedBy
,CreatedDate
,ModifiedBy
,ModifiedDate)
SELECT 'TimeFormat',[TimeFormat],MSSqlConvertId,1,GetDate(),1,Getdate() FROM [dbo].[ZnodeTimeFormat] WHERE IsActive = 1 AND IsDefault = 1
 END 
 ELSE 
 BEGIN 
 UPDATE ZnodeGlobalSetting
 SET
FeatureValues = (SELECT [TimeFormat] FROM [ZnodeTimeFormat] WHERE IsActive = 1 AND IsDefault = 1)
,FeatureSubValues = (SELECT MSSqlConvertId FROM [ZnodeTimeFormat] WHERE IsActive = 1 AND IsDefault = 1)
,CreatedBy  = (SELECT createdBy FROM Inserted ) 
,CreatedDate = (SELECT CreatedDate FROM Inserted ) 
,ModifiedBy = (SELECT ModifiedBy FROM Inserted ) 
,ModifiedDate = (SELECT ModifiedDate FROM Inserted ) 
 WHERE FeatureName = 'TimeFormat'
 
 END 
END