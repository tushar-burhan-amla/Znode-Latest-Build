CREATE TABLE [dbo].[ZnodePimAttributeGroupMapper] (
    [PimAttributeGroupMapperId] INT      IDENTITY (1, 1) NOT NULL,
    [PimAttributeGroupId]       INT      NULL,
    [PimAttributeId]            INT      NULL,
    [AttributeDisplayOrder]     INT      NULL,
    [IsSystemDefined]           BIT      CONSTRAINT [DF_ZnodePimAttributeGroupMapper_IsSystemDefined] DEFAULT ((0)) NOT NULL,
    [CreatedBy]                 INT      NOT NULL,
    [CreatedDate]               DATETIME NOT NULL,
    [ModifiedBy]                INT      NOT NULL,
    [ModifiedDate]              DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeGroupMapper] PRIMARY KEY CLUSTERED ([PimAttributeGroupMapperId] ASC),
    CONSTRAINT [FK_ZnodePimAttributeGroupMapper_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId]),
    CONSTRAINT [FK_ZnodePimAttributeGroupMapper_ZnodePimAttributeGroup] FOREIGN KEY ([PimAttributeGroupId]) REFERENCES [dbo].[ZnodePimAttributeGroup] ([PimAttributeGroupId])
);









