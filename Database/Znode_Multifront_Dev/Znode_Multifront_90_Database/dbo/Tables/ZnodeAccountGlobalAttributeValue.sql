CREATE TABLE [dbo].[ZnodeAccountGlobalAttributeValue] (
    [AccountGlobalAttributeValueId] INT            IDENTITY (1, 1) NOT NULL,
    [AccountId]                     INT            NULL,
    [GlobalAttributeId]             INT            NULL,
    [GlobalAttributeDefaultValueId] INT            NULL,
    [AttributeValue]                NVARCHAR (300) NULL,
    [CreatedBy]                     INT            NOT NULL,
    [CreatedDate]                   DATETIME       NOT NULL,
    [ModifiedBy]                    INT            NOT NULL,
    [ModifiedDate]                  DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeAccountGlobalAttributeValue] PRIMARY KEY CLUSTERED ([AccountGlobalAttributeValueId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeAccountGlobalAttributeValue_ZnodeGlobalAttribute] FOREIGN KEY ([GlobalAttributeId]) REFERENCES [dbo].[ZnodeGlobalAttribute] ([GlobalAttributeId]),
    CONSTRAINT [FK_ZnodeAccountGlobalAttributeValue_ZnodeGlobalAttributeDefaultValue] FOREIGN KEY ([GlobalAttributeDefaultValueId]) REFERENCES [dbo].[ZnodeGlobalAttributeDefaultValue] ([GlobalAttributeDefaultValueId])
);

