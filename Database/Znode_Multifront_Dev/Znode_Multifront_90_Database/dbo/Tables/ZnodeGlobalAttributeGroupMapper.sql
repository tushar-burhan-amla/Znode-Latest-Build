CREATE TABLE [dbo].[ZnodeGlobalAttributeGroupMapper] (
    [GlobalAttributeGroupMapperId] INT      IDENTITY (1, 1) NOT NULL,
    [GlobalAttributeGroupId]       INT      NULL,
    [GlobalAttributeId]            INT      NULL,
    [AttributeDisplayOrder]        INT      NULL,
    [CreatedBy]                    INT      NOT NULL,
    [CreatedDate]                  DATETIME NOT NULL,
    [ModifiedBy]                   INT      NOT NULL,
    [ModifiedDate]                 DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalAttributeGroupMapper] PRIMARY KEY CLUSTERED ([GlobalAttributeGroupMapperId] ASC),
    CONSTRAINT [FK_ZnodeGlobalAttributeGroupMapper_ZnodeGlobalAttribute] FOREIGN KEY ([GlobalAttributeId]) REFERENCES [dbo].[ZnodeGlobalAttribute] ([GlobalAttributeId]),
    CONSTRAINT [FK_ZnodeGlobalAttributeGroupMapper_ZnodeGlobalAttributeGroup] FOREIGN KEY ([GlobalAttributeGroupId]) REFERENCES [dbo].[ZnodeGlobalAttributeGroup] ([GlobalAttributeGroupId])
);

