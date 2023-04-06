CREATE TABLE [dbo].[ZnodeMediaAttributeGroupMapper] (
    [MediaAttributeGroupMapperId] INT      IDENTITY (1, 1) NOT NULL,
    [MediaAttributeGroupId]       INT      NULL,
    [MediaAttributeId]            INT      NULL,
    [AttributeDisplayOrder]       INT      NULL,
    [IsSystemDefined]             BIT      CONSTRAINT [DF_ZnodeMediaAttributeGroupMapper_IsSystemDefined] DEFAULT ((0)) NOT NULL,
    [CreatedBy]                   INT      NOT NULL,
    [CreatedDate]                 DATETIME NOT NULL,
    [ModifiedBy]                  INT      NOT NULL,
    [ModifiedDate]                DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeMediaAttributeGroupMapper] PRIMARY KEY CLUSTERED ([MediaAttributeGroupMapperId] ASC),
    CONSTRAINT [FK_ZnodeMediaAttributeGroupMapper_ZnodeMediaAttribute] FOREIGN KEY ([MediaAttributeId]) REFERENCES [dbo].[ZnodeMediaAttribute] ([MediaAttributeId]),
    CONSTRAINT [FK_ZnodeMediaAttributeGroupMapper_ZnodeMediaAttributeGroup] FOREIGN KEY ([MediaAttributeGroupId]) REFERENCES [dbo].[ZnodeMediaAttributeGroup] ([MediaAttributeGroupId])
);







