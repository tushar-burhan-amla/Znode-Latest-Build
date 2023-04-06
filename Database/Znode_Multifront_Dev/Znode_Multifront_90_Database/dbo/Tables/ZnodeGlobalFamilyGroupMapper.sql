CREATE TABLE ZnodeGlobalFamilyGroupMapper (
    GlobalFamilyGroupMapperId int NOT NULL  IDENTITY(1,1),
	GlobalAttributeFamilyId int not null,
	GlobalAttributeGroupId int not null,
    AttributeGroupDisplayOrder int ,
    CreatedBy int NOT NULL,
    CreatedDate datetime NOT NULL, 
    ModifiedBy int NOT NULL,
	ModifiedDate datetime  NOT NULL ,
	CONSTRAINT [PK_ZnodeGlobalFamilyGroupMapper] PRIMARY KEY CLUSTERED ([GlobalFamilyGroupMapperId] ASC) WITH (FILLFACTOR = 90),
	CONSTRAINT [FK_ZnodeGlobalFamilyGroupMapper_ZnodeGlobalAttributeFamily] FOREIGN KEY ([GlobalAttributeFamilyId]) REFERENCES [dbo].[ZnodeGlobalAttributeFamily] ([GlobalAttributeFamilyId]),
	CONSTRAINT [FK_ZnodeGlobalFamilyGroupMapper_ZnodeGlobalAttributeGroup] FOREIGN KEY ([GlobalAttributeGroupId]) REFERENCES [dbo].[ZnodeGlobalAttributeGroup] ([GlobalAttributeGroupId])

);