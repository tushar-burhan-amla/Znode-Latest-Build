CREATE TABLE ZnodeGlobalEntityFamilyMapper (
GlobalEntityFamilyMapperId int NOT NULL  IDENTITY(1,1),
GlobalAttributeFamilyId int not null,
GlobalEntityId int not null,
GlobalEntityValueId int ,
CONSTRAINT [PK_ZnodeGlobalEntityFamilyMapper] PRIMARY KEY CLUSTERED ([GlobalEntityFamilyMapperId] ASC) WITH (FILLFACTOR = 90),
CONSTRAINT [FK_ZnodeGlobalEntityFamilyMapper_ZnodeGlobalAttributeFamily] FOREIGN KEY ([GlobalAttributeFamilyId]) REFERENCES [dbo].[ZnodeGlobalAttributeFamily] ([GlobalAttributeFamilyId]),
CONSTRAINT [FK_ZnodeGlobalEntityFamilyMapper_ZnodeGlobalEntity] FOREIGN KEY ([GlobalEntityId]) REFERENCES [dbo].[ZnodeGlobalEntity] ([GlobalEntityId])

);
