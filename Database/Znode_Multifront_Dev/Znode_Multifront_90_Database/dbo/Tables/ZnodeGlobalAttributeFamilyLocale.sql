CREATE TABLE ZnodeGlobalAttributeFamilyLocale (
    GlobalAttributeFamilyLocaleId int NOT NULL  IDENTITY(1,1),
	LocaleId int ,
	GlobalAttributeFamilyId int not null,
    AttributeFamilyName nvarchar(300),
	Description nvarchar(300),
    CreatedBy int NOT NULL,
    CreatedDate datetime NOT NULL, 
    ModifiedBy int NOT NULL,
	ModifiedDate datetime  NOT NULL ,
	CONSTRAINT [PK_ZnodeGlobalAttributeFamilyLocale] PRIMARY KEY CLUSTERED ([GlobalAttributeFamilyLocaleId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeGlobalAttributeFamilyLocale_ZnodeGlobalAttributeFamily] FOREIGN KEY ([GlobalAttributeFamilyId]) REFERENCES [dbo].[ZnodeGlobalAttributeFamily] ([GlobalAttributeFamilyId])

);