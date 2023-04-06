CREATE TABLE [dbo].[ZnodeMediaAttributeValue] (
    [MediaAttributeValueId]        INT           IDENTITY (1, 1) NOT NULL,
    [MediaCategoryId]              INT           NULL,
    [MediaAttributeId]             INT           NULL,
    [MediaAttributeDefaultValueId] INT           NULL,
    [AttributeValue]               VARCHAR (300) NULL,
    [CreatedBy]                    INT           NOT NULL,
    [CreatedDate]                  DATETIME      NOT NULL,
    [ModifiedBy]                   INT           NOT NULL,
    [ModifiedDate]                 DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeMediaAttributeValue] PRIMARY KEY CLUSTERED ([MediaAttributeValueId] ASC),
    CONSTRAINT [FK_ZnodeMediaAttributeValue_ZnodeMediaAttribute] FOREIGN KEY ([MediaAttributeId]) REFERENCES [dbo].[ZnodeMediaAttribute] ([MediaAttributeId]),
    CONSTRAINT [FK_ZnodeMediaAttributeValue_ZnodeMediaAttributeDefaultValue] FOREIGN KEY ([MediaAttributeDefaultValueId]) REFERENCES [dbo].[ZnodeMediaAttributeDefaultValue] ([MediaAttributeDefaultValueId]),
    CONSTRAINT [FK_ZnodeMediaAttributeValue_ZnodeMediaCategory] FOREIGN KEY ([MediaCategoryId]) REFERENCES [dbo].[ZnodeMediaCategory] ([MediaCategoryId])
);









