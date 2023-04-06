CREATE TABLE [dbo].[ZnodeGlobalAttributeValue] (
    [GlobalAttributeValueId]        INT            IDENTITY (1, 1) NOT NULL,
    [GlobalEntityId]                INT            NULL,
    [GlobalEntityValueId]           INT            NULL,
    [GlobalAttributeId]             INT            NULL,
    [GlobalAttributeDefaultValueId] INT            NULL,
    [AttributeValue]                NVARCHAR (300) NULL,
    [CreatedBy]                     INT            NOT NULL,
    [CreatedDate]                   DATETIME       NOT NULL,
    [ModifiedBy]                    INT            NOT NULL,
    [ModifiedDate]                  DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalAttributeValue] PRIMARY KEY CLUSTERED ([GlobalAttributeValueId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeGlobalAttributeValue_ZnodeGlobalAttribute] FOREIGN KEY ([GlobalAttributeId]) REFERENCES [dbo].[ZnodeGlobalAttribute] ([GlobalAttributeId]),
    CONSTRAINT [FK_ZnodeGlobalAttributeValue_ZnodeGlobalAttributeDefaultValue] FOREIGN KEY ([GlobalAttributeDefaultValueId]) REFERENCES [dbo].[ZnodeGlobalAttributeDefaultValue] ([GlobalAttributeDefaultValueId]),
    CONSTRAINT [FK_ZnodeGlobalAttributeValue_ZnodeGlobalEntity] FOREIGN KEY ([GlobalEntityId]) REFERENCES [dbo].[ZnodeGlobalEntity] ([GlobalEntityId])
);

