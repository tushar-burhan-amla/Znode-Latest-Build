CREATE TABLE [dbo].[ZnodeDisplayUnit] (
    [DisplayUnitId]   INT          IDENTITY (1, 1) NOT NULL,
    [DisplayUnitName] VARCHAR (50) NULL,
    [DisplayUnitCode] VARCHAR (50) NULL,
    [IsActive]        BIT          CONSTRAINT [DF_ZnodeMediaUnit_IsActive] DEFAULT ((0)) NOT NULL,
    [IsDefault]       BIT          CONSTRAINT [DF_ZnodeDisplayUnit_IsDefault] DEFAULT ((0)) NOT NULL,
    [CreatedBy]       INT          NOT NULL,
    [CreatedDate]     DATETIME     NOT NULL,
    [ModifiedBy]      INT          NOT NULL,
    [ModifiedDate]    DATETIME     NOT NULL,
    CONSTRAINT [PK_ZnodeMediaUnit] PRIMARY KEY CLUSTERED ([DisplayUnitId] ASC)
);








GO

CREATE Trigger [dbo].[Trg_ZnodeDisplayUnitt_GlobalSetting]  ON [dbo].[ZnodeDisplayUnit]
AFTER UPDATE AS 
BEGIN
 IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeGlobalSetting WHERE FeatureName = 'DisplayUnit')
 BEGIN
  INSERT INTO ZnodeGlobalSetting (FeatureName
,FeatureValues
,CreatedBy
,CreatedDate
,ModifiedBy
,ModifiedDate)
SELECT 'DisplayUnit',DisplayUnitCode,1,GetDate(),1,Getdate() FROM [dbo].[ZnodeDisplayUnit] WHERE IsActive = 1 AND IsDefault = 1
 END 
 ELSE 
 BEGIN 
 UPDATE ZnodeGlobalSetting
 SET
FeatureValues = (SELECT DisplayUnitCode FROM [ZnodeDisplayUnit] WHERE IsActive = 1 AND IsDefault = 1)
,CreatedBy  = (SELECT createdBy FROM Inserted ) 
,CreatedDate = (SELECT CreatedDate FROM Inserted ) 
,ModifiedBy = (SELECT ModifiedBy FROM Inserted ) 
,ModifiedDate = (SELECT ModifiedDate FROM Inserted ) 
 WHERE FeatureName = 'DisplayUnit'
 
 END 
END