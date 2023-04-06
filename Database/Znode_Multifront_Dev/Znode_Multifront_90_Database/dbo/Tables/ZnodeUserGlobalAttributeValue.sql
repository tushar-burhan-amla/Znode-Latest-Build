CREATE TABLE [dbo].[ZnodeUserGlobalAttributeValue] (
    [UserGlobalAttributeValueId]    INT            IDENTITY (1, 1) NOT NULL,
    [UserId]                        INT            NULL,
    [GlobalAttributeId]             INT            NULL,
    [GlobalAttributeDefaultValueId] INT            NULL,
    [AttributeValue]                NVARCHAR (300) NULL,
    [CreatedBy]                     INT            NOT NULL,
    [CreatedDate]                   DATETIME       NOT NULL,
    [ModifiedBy]                    INT            NOT NULL,
    [ModifiedDate]                  DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeUserGlobalAttributeValue] PRIMARY KEY CLUSTERED ([UserGlobalAttributeValueId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeUserGlobalAttributeValue_ZnodeGlobalAttribute] FOREIGN KEY ([GlobalAttributeId]) REFERENCES [dbo].[ZnodeGlobalAttribute] ([GlobalAttributeId]),
    CONSTRAINT [FK_ZnodeUserGlobalAttributeValue_ZnodeGlobalAttributeDefaultValue] FOREIGN KEY ([GlobalAttributeDefaultValueId]) REFERENCES [dbo].[ZnodeGlobalAttributeDefaultValue] ([GlobalAttributeDefaultValueId])
);
GO
CREATE NONCLUSTERED INDEX Ind_ZnodeUserGlobalAttributeValue_UserId 
ON [dbo].[ZnodeUserGlobalAttributeValue] ([UserId])
