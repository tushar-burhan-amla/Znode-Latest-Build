CREATE TABLE [dbo].[ZnodeMediaAttribute] (
    [MediaAttributeId]       INT           IDENTITY (1, 1) NOT NULL,
    [ParentMediaAttributeId] INT           NULL,
    [AttributeTypeId]        INT           NULL,
    [AttributeCode]          VARCHAR (300) NULL,
    [IsRequired]             BIT           NULL,
    [IsLocalizable]          BIT           NULL,
    [IsFilterable]           BIT           NULL,
    [IsSystemDefined]        BIT           CONSTRAINT [DF_ZnodeMediaAttribute_IsSystemDefined] DEFAULT ((0)) NOT NULL,
    [DisplayOrder]           INT           NULL,
    [HelpDescription]        VARCHAR (MAX) NULL,
    [CreatedBy]              INT           NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [ModifiedBy]             INT           NOT NULL,
    [ModifiedDate]           DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeMediaAttribute] PRIMARY KEY CLUSTERED ([MediaAttributeId] ASC),
    CONSTRAINT [FK_ZnodeMediaAttribute_ZnodeAttributeType] FOREIGN KEY ([AttributeTypeId]) REFERENCES [dbo].[ZnodeAttributeType] ([AttributeTypeId]),
    CONSTRAINT [FK_ZnodeMediaAttribute_ZnodeMediaAttribute] FOREIGN KEY ([ParentMediaAttributeId]) REFERENCES [dbo].[ZnodeMediaAttribute] ([MediaAttributeId])
);











