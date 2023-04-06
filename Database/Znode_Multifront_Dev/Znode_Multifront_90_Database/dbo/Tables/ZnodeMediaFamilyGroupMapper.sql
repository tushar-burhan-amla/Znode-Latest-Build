CREATE TABLE [dbo].[ZnodeMediaFamilyGroupMapper] (
    [MediaFamilyGroupMapperId] INT      IDENTITY (1, 1) NOT NULL,
    [MediaAttributeFamilyId]   INT      NULL,
    [MediaAttributeGroupId]    INT      NULL,
    [MediaAttributeId]         INT      NULL,
    [GroupDisplayOrder]        INT      NULL,
    [IsSystemDefined]          BIT      CONSTRAINT [DF_ZnodeMediaFamilyGroupMapper_IsSystemDefined] DEFAULT ((0)) NOT NULL,
    [CreatedBy]                INT      NOT NULL,
    [CreatedDate]              DATETIME NOT NULL,
    [ModifiedBy]               INT      NOT NULL,
    [ModifiedDate]             DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeMediaFamilyGroupMapper] PRIMARY KEY CLUSTERED ([MediaFamilyGroupMapperId] ASC),
    CONSTRAINT [FK_ZnodeMediaFamilyGroupMapper_ZnodeMediaAttribute] FOREIGN KEY ([MediaAttributeId]) REFERENCES [dbo].[ZnodeMediaAttribute] ([MediaAttributeId]),
    CONSTRAINT [FK_ZnodeMediaFamilyGroupMapper_ZnodeMediaAttributeFamily] FOREIGN KEY ([MediaAttributeFamilyId]) REFERENCES [dbo].[ZnodeMediaAttributeFamily] ([MediaAttributeFamilyId]),
    CONSTRAINT [FK_ZnodeMediaFamilyGroupMapper_ZnodeMediaAttributeGroup] FOREIGN KEY ([MediaAttributeGroupId]) REFERENCES [dbo].[ZnodeMediaAttributeGroup] ([MediaAttributeGroupId])
);







