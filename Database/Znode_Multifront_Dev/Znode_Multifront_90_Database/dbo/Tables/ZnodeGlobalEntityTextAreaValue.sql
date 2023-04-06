CREATE TABLE [dbo].[ZnodeGlobalEntityTextAreaValue] (
    [GlobalEntityTextAreaValueId] INT            IDENTITY (1, 1) NOT NULL,
    [GlobalAttributeValueId]      INT            NOT NULL,
    [AttributeValue]              NVARCHAR (MAX) NULL,
    [LocaleId]                    INT            NOT NULL,
    [CreatedBy]                   INT            NOT NULL,
    [CreatedDate]                 DATETIME       NOT NULL,
    [ModifiedBy]                  INT            NOT NULL,
    [ModifiedDate]                DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalEntityTextAreaValue] PRIMARY KEY CLUSTERED ([GlobalEntityTextAreaValueId] ASC),
    CONSTRAINT [FK_ZnodeGlobalEntityTextAreaValue_ZnodeGlobalAttributeValueId] FOREIGN KEY ([GlobalAttributeValueId]) REFERENCES [dbo].[ZnodeGlobalAttributeValue] ([GlobalAttributeValueId])
);

