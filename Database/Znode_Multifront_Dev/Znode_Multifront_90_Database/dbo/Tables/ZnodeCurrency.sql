CREATE TABLE [dbo].[ZnodeCurrency] (
    [CurrencyId]     INT            IDENTITY (1, 1) NOT NULL,
    [CurrencyCode]   VARCHAR (100)  NULL,
    [CurrencyName]   VARCHAR (200)  NULL,
    [IsActive]       BIT            CONSTRAINT [DF__ZnodeCurr__IsAct__45BE5BA9] DEFAULT ((0)) NOT NULL,
    [IsDefault]      BIT            CONSTRAINT [DF_ZnodeCurrency_IsDefault] DEFAULT ((0)) NOT NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     INT            NOT NULL,
    [ModifiedDate]   DATETIME       NOT NULL,
    [CurrencySuffix] VARCHAR (1000) NULL,
    [Symbol]         VARCHAR (100)  NULL,
    CONSTRAINT [PK_ZnodeCurrency] PRIMARY KEY CLUSTERED ([CurrencyId] ASC)
);














GO
CREATE Trigger [dbo].[Trg_ZNodeCurrency_GlobalSetting]  ON [dbo].[ZnodeCurrency] 
AFTER UPDATE AS 
BEGIN
  IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeGlobalSetting WHERE FeatureName = 'Currency')
  BEGIN
		   INSERT INTO ZnodeGlobalSetting (FeatureName
					,FeatureValues
					,FeatureSubValues
					,CreatedBy
					,CreatedDate
					,ModifiedBy
					,ModifiedDate)
			SELECT 'Currency',CurrencyCode,FeatureSubValues = ISNULL(Symbol,'') + '|' +  CurrencyName+'|'+ CurrencySuffix,1,GetDate(),1,Getdate() FROM ZnodeCurrency WHERE IsActive = 1 AND IsDefault = 1
  END 
  ELSE 
  BEGIN 
		  UPDATE ZnodeGlobalSetting
		  SET
			FeatureValues = a.CurrencyCode 
			,FeatureSubValues = ISNULL(a.Symbol,'') + '|' +  a.CurrencyName+'|'+a.CurrencySuffix
			,CreatedBy  = (SELECT createdBy FROM Inserted ) 
			,CreatedDate = (SELECT CreatedDate FROM Inserted ) 
			,ModifiedBy = (SELECT ModifiedBy FROM Inserted ) 
			,ModifiedDate = (SELECT ModifiedDate FROM Inserted ) 
		 FROM ZnodeCurrency a WHERE a.IsActive = 1 AND a.IsDefault = 1
		  AND FeatureName = 'Currency' 
  
  END 
 END