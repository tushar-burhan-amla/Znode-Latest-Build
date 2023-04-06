CREATE TABLE [dbo].[ZnodeFormBuilderAttributeMapper] (
    [FormBuilderAttributeMapperId] INT      IDENTITY (1, 1) NOT NULL,
    [FormBuilderId]                INT      NOT NULL,
    [GlobalAttributeGroupId]       INT      NULL,
    [GlobalAttributeId]            INT      NULL,
    [DisplayOrder]                 INT      NULL,
    [CreatedBy]                    INT      NOT NULL,
    [CreatedDate]                  DATETIME NOT NULL,
    [ModifiedBy]                   INT      NOT NULL,
    [ModifiedDate]                 DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeFormBuilderAttributeMapper] PRIMARY KEY CLUSTERED ([FormBuilderAttributeMapperId] ASC),
    CONSTRAINT [FK_ZnodeFormBuilderAttributeMapper_ZnodeFormBuilder] FOREIGN KEY ([FormBuilderId]) REFERENCES [dbo].[ZnodeFormBuilder] ([FormBuilderId]),
    CONSTRAINT [FK_ZnodeFormBuilderAttributeMapper_ZnodeGlobalAttribute] FOREIGN KEY ([GlobalAttributeId]) REFERENCES [dbo].[ZnodeGlobalAttribute] ([GlobalAttributeId]),
    CONSTRAINT [FK_ZnodeFormBuilderAttributeMapper_ZnodeGlobalAttributeGroup] FOREIGN KEY ([GlobalAttributeGroupId]) REFERENCES [dbo].[ZnodeGlobalAttributeGroup] ([GlobalAttributeGroupId])
);



