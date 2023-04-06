CREATE TABLE [dbo].[ZnodeGlobalGroupEntityMapper] (
    [GlobalGroupEntityId]        INT      IDENTITY (1, 1) NOT NULL,
    [GlobalAttributeGroupId]     INT      NULL,
    [GlobalEntityId]             INT      NULL,
    [AttributeGroupDisplayOrder] INT      NULL,
    [CreatedBy]                  INT      NOT NULL,
    [CreatedDate]                DATETIME NOT NULL,
    [ModifiedBy]                 INT      NOT NULL,
    [ModifiedDate]               DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalGroupEntityMapper] PRIMARY KEY CLUSTERED ([GlobalGroupEntityId] ASC),
    CONSTRAINT [FK_ZnodeGlobalGroupEntityMapper_ZnodeGlobalAttributeGroup] FOREIGN KEY ([GlobalAttributeGroupId]) REFERENCES [dbo].[ZnodeGlobalAttributeGroup] ([GlobalAttributeGroupId]),
    CONSTRAINT [FK_ZnodeGlobalGroupEntityMapper_ZnodeGlobalEntity] FOREIGN KEY ([GlobalEntityId]) REFERENCES [dbo].[ZnodeGlobalEntity] ([GlobalEntityId])
);

