CREATE TABLE [dbo].[ZnodeCulture] (
    [CultureId]    INT           IDENTITY (1, 1) NOT NULL,
    [CultureCode]  VARCHAR (100) NULL,
    [CultureName]  VARCHAR (200) NULL,
    [CurrencyId]   INT           NULL,
    [Symbol]       VARCHAR (100) NULL,
    [IsActive]     BIT           CONSTRAINT [DF__ZnodeCulture__IsActive] DEFAULT ((0)) NOT NULL,
    [IsDefault]    BIT           CONSTRAINT [DF_ZnodeCulture_IsDefault] DEFAULT ((0)) NOT NULL,
    [CreatedBy]    INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [ModifiedBy]   INT           NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCulture] PRIMARY KEY CLUSTERED ([CultureId] ASC),
    CONSTRAINT [FK_ZnodeCulture_ZnodeCurrency] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[ZnodeCurrency] ([CurrencyId])
);




GO



CREATE Trigger [dbo].[Trg_ZNodeCulture_GlobalSetting]  ON [dbo].[ZnodeCulture]   
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
  /*  SELECT 'Currency',CurrencyCode,FeatureSubValues = ISNULL(Symbol,'') + '|' +  CurrencyName+'|'+ CurrencySuffix,1,GetDate(),1,Getdate() FROM ZnodeCurrency WHERE IsActive = 1 AND IsDefault = 1   */
  	  SELECT 'Currency',ZC.CultureCode,FeatureSubValues = ISNULL(ZC.Symbol,'') + '|' +  ZC.CultureName+'|'+ ZCC.CurrencyCode,1,GetDate(),1,Getdate() 
	  FROM ZnodeCulture AS ZC INNER JOIN ZnodeCurrency ZCC on ZC.CurrencyId = ZCC.CurrencyId
	  WHERE ZC.IsActive = 1 AND ZC.IsDefault = 1  

  END   
  ELSE   
  BEGIN   
/*     UPDATE ZnodeGlobalSetting  
    SET  
   FeatureValues = a.CurrencyCode   
   ,FeatureSubValues = ISNULL(a.Symbol,'') + '|' +  a.CurrencyName+'|'+a.CurrencySuffix  
   ,CreatedBy  = (SELECT createdBy FROM Inserted )   
   ,CreatedDate = (SELECT CreatedDate FROM Inserted )   
   ,ModifiedBy = (SELECT ModifiedBy FROM Inserted )   
   ,ModifiedDate = (SELECT ModifiedDate FROM Inserted )   
   FROM ZnodeCurrency a WHERE a.IsActive = 1 AND a.IsDefault = 1  
    AND FeatureName = 'Currency'    */
	
	
	UPDATE ZnodeGlobalSetting  
    SET  
   FeatureValues = ZC.CultureCode   
   ,FeatureSubValues = ISNULL(ZC.Symbol,'') + '|' +  ZC.CultureName+'|'+ZCC.CurrencyCode  
   ,CreatedBy  = (SELECT CreatedBy FROM Inserted )   
   ,CreatedDate = (SELECT CreatedDate FROM Inserted )   
   ,ModifiedBy = (SELECT ModifiedBy FROM Inserted )   
   ,ModifiedDate = (SELECT ModifiedDate FROM Inserted )   
   FROM ZnodeCulture ZC Inner Join ZnodeCurrency ZCC on ZC.CurrencyId = ZCC.CurrencyId
   WHERE ZC.IsActive = 1 AND ZC.IsDefault = 1  
   AND FeatureName = 'Currency' 
	
   END 
  
	UPDATE ZCULT 
	SET ZCULT.IsActive = ZCUR.IsActive
	FROM ZnodeCulture ZCULT 
	INNER JOIN ZnodeCurrency ZCUR ON (ZCUR.CurrencyId = ZCULT.CurrencyId)
	  
 END