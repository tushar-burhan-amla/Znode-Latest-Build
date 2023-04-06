CREATE TABLE ZnodeGlobalAttributeFamily (
    GlobalAttributeFamilyId int NOT NULL  IDENTITY(1,1),
    FamilyCode varchar(200),
    CreatedBy int NOT NULL,
    CreatedDate datetime NOT NULL, 
    ModifiedBy int NOT NULL,
	ModifiedDate datetime  NOT NULL ,
	IsSystemDefined bit  NOT NULL,
	GlobalEntityId int NOT NULL,
	CONSTRAINT [PK_ZnodeGlobalAttributeFamily] PRIMARY KEY CLUSTERED ([GlobalAttributeFamilyId] ASC) WITH (FILLFACTOR = 90)
);