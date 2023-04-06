CREATE TABLE [dbo].[ZnodeCountry] (
    [CountryId]    INT           IDENTITY (1, 1) NOT NULL,
    [CountryCode]  VARCHAR (100) NULL,
    [CountryName]  VARCHAR (200) NULL,
    [DisplayOrder] INT           NULL,
    [IsActive]     BIT           CONSTRAINT [DF__ZnodeCoun__IsAct__42E1EEFE] DEFAULT ((0)) NOT NULL,
    [IsDefault]    BIT           CONSTRAINT [DF_ZnodeCountry_IsDefault] DEFAULT ((0)) NOT NULL,
    [CreatedBy]    INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [ModifiedBy]   INT           NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCountry] PRIMARY KEY CLUSTERED ([CountryId] ASC) WITH (FILLFACTOR = 90)
);










GO
CREATE Trigger [dbo].[Trg_ZNodeCountry_GlobalSetting]  ON [dbo].[ZnodeCountry] 
AFTER UPDATE AS 
BEGIN
  IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeGlobalSetting WHERE FeatureName = 'Country')
  BEGIN
		   INSERT INTO ZnodeGlobalSetting (FeatureName
					,FeatureValues
					,FeatureSubValues
					,CreatedBy
					,CreatedDate
					,ModifiedBy
					,ModifiedDate)
					SELECT 'Country', zc.CountryCode,zc.CountryName  ,1,GetDate(),1,Getdate()  FROM dbo.ZnodeCountry zc WHERE  zc.IsActive = 1 AND zc.IsDefault = 1
  END 
  ELSE 
  BEGIN 
		  UPDATE ZnodeGlobalSetting
		  SET
			FeatureValues = zc.CountryCode
			,FeatureSubValues = zc.CountryName
			,CreatedBy  = (SELECT CreatedBy FROM Inserted ) 
			,CreatedDate = (SELECT CreatedDate FROM Inserted ) 
			,ModifiedBy = (SELECT ModifiedBy FROM Inserted ) 
			,ModifiedDate = (SELECT ModifiedDate FROM Inserted ) 
		 FROM dbo.ZnodeCountry zc WHERE zc.IsActive = 1 AND zc.IsDefault = 1
		  AND FeatureName = 'Country' 
  
  END 
 END