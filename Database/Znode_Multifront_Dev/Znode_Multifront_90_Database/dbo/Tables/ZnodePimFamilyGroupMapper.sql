CREATE TABLE [dbo].[ZnodePimFamilyGroupMapper] (
    [PimFamilyGroupMapperId] INT             IDENTITY (1, 1) NOT NULL,
    [PimAttributeFamilyId]   INT             NULL,
    [PimAttributeGroupId]    INT             NULL,
    [PimAttributeId]         INT             NULL,
    [GroupDisplayOrder]      NUMERIC (28, 6) NULL,
    [IsSystemDefined]        BIT             CONSTRAINT [DF_ZnodePimFamilyGroupMapper_IsSystemDefined] DEFAULT ((0)) NOT NULL,
    [CreatedBy]              INT             NOT NULL,
    [CreatedDate]            DATETIME        NOT NULL,
    [ModifiedBy]             INT             NOT NULL,
    [ModifiedDate]           DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePimFamilyGroupMapper] PRIMARY KEY CLUSTERED ([PimFamilyGroupMapperId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimFamilyGroupMapper_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId]),
    CONSTRAINT [FK_ZnodePimFamilyGroupMapper_ZnodePimAttributeFamily] FOREIGN KEY ([PimAttributeFamilyId]) REFERENCES [dbo].[ZnodePimAttributeFamily] ([PimAttributeFamilyId]),
    CONSTRAINT [FK_ZnodePimFamilyGroupMapper_ZnodePimAttributeGroup] FOREIGN KEY ([PimAttributeGroupId]) REFERENCES [dbo].[ZnodePimAttributeGroup] ([PimAttributeGroupId])
);










GO
CREATE NONCLUSTERED INDEX [ZnodePimFamilyGroupMapper_PimAttributeFamilyId]
    ON [dbo].[ZnodePimFamilyGroupMapper]([PimAttributeFamilyId] ASC, [PimAttributeId] ASC);

