CREATE TABLE [dbo].[ZnodeUserGlobalAttributeValueLocale] (
    [UserGlobalAttributeValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [UserGlobalAttributeValueId]       INT            NOT NULL,
    [LocaleId]                         INT            NOT NULL,
    [AttributeValue]                   NVARCHAR (MAX) NULL,
    [CreatedBy]                        INT            NOT NULL,
    [CreatedDate]                      DATETIME       NOT NULL,
    [ModifiedBy]                       INT            NOT NULL,
    [ModifiedDate]                     DATETIME       NOT NULL,
    [GlobalAttributeDefaultValueId]    INT            NULL,
    [MediaId]                          INT            NULL,
    [MediaPath]                        NVARCHAR (300) NULL,
    CONSTRAINT [PK_ZnodeUserGlobalAttributeValueLocale] PRIMARY KEY CLUSTERED ([UserGlobalAttributeValueLocaleId] ASC),
    CONSTRAINT [FK_ZnodeUserGlobalAttributeValueLocale_ZnodeMedia] FOREIGN KEY ([MediaId]) REFERENCES [dbo].[ZnodeMedia] ([MediaId]),
    CONSTRAINT [FK_ZnodeUserGlobalAttributeValueLocale_ZnodeUserGlobalAttributeValue] FOREIGN KEY ([UserGlobalAttributeValueId]) REFERENCES [dbo].[ZnodeUserGlobalAttributeValue] ([UserGlobalAttributeValueId])
);
GO
CREATE NONCLUSTERED INDEX Ind_ZnodeUserGlobalAttributeValueLocale_UserGlobalAttributeValueId
ON [dbo].[ZnodeUserGlobalAttributeValueLocale] ([UserGlobalAttributeValueId])
INCLUDE ([AttributeValue],[GlobalAttributeDefaultValueId],[MediaId],[MediaPath])

