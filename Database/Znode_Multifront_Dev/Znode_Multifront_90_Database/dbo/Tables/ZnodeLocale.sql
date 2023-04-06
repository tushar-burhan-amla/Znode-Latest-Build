CREATE TABLE [dbo].[ZnodeLocale] (
    [LocaleId]     INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (100) NULL,
    [Code]         NVARCHAR (50)  NULL,
    [IsActive]     BIT            CONSTRAINT [DF__ZnodeLoca__IsAct__3D2915A8] DEFAULT ((1)) NOT NULL,
    [IsDefault]    BIT            CONSTRAINT [DF_ZnodeLocale_IsDefault] DEFAULT ((0)) NOT NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [ModifiedBy]   INT            NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeLocale] PRIMARY KEY CLUSTERED ([LocaleId] ASC)
);


















GO
CREATE Trigger [dbo].[Trg_ZnodeLocale_GlobalSetting]  ON [dbo].[ZnodeLocale] 
AFTER UPDATE AS 
BEGIN
  IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeGlobalSetting WHERE FeatureName = 'Locale')
  BEGIN
		   INSERT INTO ZnodeGlobalSetting (FeatureName
					,FeatureValues
					,CreatedBy
					,CreatedDate
					,ModifiedBy
					,ModifiedDate)
			SELECT 'Locale',Code,1,GetDate(),1,Getdate() FROM [dbo].[ZnodeLocale] WHERE IsActive = 1 AND IsDefault = 1
  END 
  ELSE 
  BEGIN 
		  UPDATE ZnodeGlobalSetting
		  SET
			FeatureValues = (SELECT LocaleId FROM [ZnodeLocale] WHERE IsActive = 1 AND IsDefault = 1)
			,ModifiedBy = (SELECT ModifiedBy FROM Inserted ) 
			,ModifiedDate = (SELECT ModifiedDate FROM Inserted ) 
		  WHERE FeatureName = 'Locale'
  
  END 
    EXEC dbo.AspNet_SqlCacheUpdateChangeIdStoredProcedure N'View_GetLocaleDetails'

 END