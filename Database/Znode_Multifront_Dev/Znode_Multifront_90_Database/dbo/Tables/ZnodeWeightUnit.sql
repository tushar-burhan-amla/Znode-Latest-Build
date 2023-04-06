CREATE TABLE [dbo].[ZnodeWeightUnit] (
    [WeightUnitId]   INT           IDENTITY (1, 1) NOT NULL,
    [WeightUnitName] VARCHAR (300) NULL,
    [WeightUnitCode] VARCHAR (200) NULL,
    [IsActive]       BIT           CONSTRAINT [DF__ZnodeWeig__IsAct__59B045BD] DEFAULT ((1)) NULL,
    [IsDefault]      BIT           CONSTRAINT [DF__ZnodeWeig__IsDef__5AA469F6] DEFAULT ((0)) NOT NULL,
    [CreatedBy]      INT           NULL,
    [CreatedDate]    DATETIME      NULL,
    [ModifiedBy]     INT           NULL,
    [ModifiedDate]   DATETIME      NULL,
    CONSTRAINT [PK_ZnodeWeightUnit] PRIMARY KEY CLUSTERED ([WeightUnitId] ASC)
);






GO

Create Trigger [dbo].[Trg_ZnodeWeightUnit_ZnodeGlobalSetting]  ON [dbo].[ZnodeWeightUnit]
AFTER UPDATE AS 
BEGIN
 IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeGlobalSetting WHERE FeatureName = 'WeightUnit')
 BEGIN
  INSERT INTO ZnodeGlobalSetting (FeatureName
,FeatureValues
,CreatedBy
,CreatedDate
,ModifiedBy
,ModifiedDate)
SELECT 'WeightUnit',WeightUnitCode,1,GetDate(),1,Getdate() FROM [dbo].ZnodeWeightUnit WHERE IsActive = 1 AND IsDefault = 1
 END 
 ELSE 
 BEGIN 
 UPDATE ZnodeGlobalSetting
 SET
FeatureValues = (SELECT WeightUnitCode FROM ZnodeWeightUnit WHERE IsActive = 1 AND IsDefault = 1)
,CreatedBy  = (SELECT createdBy FROM Inserted ) 
,CreatedDate = (SELECT CreatedDate FROM Inserted ) 
,ModifiedBy = (SELECT ModifiedBy FROM Inserted ) 
,ModifiedDate = (SELECT ModifiedDate FROM Inserted ) 
 WHERE FeatureName = 'WeightUnit'
 
 END 
END