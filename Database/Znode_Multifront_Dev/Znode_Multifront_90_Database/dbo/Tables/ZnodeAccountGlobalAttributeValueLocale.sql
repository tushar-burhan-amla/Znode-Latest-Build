CREATE TABLE [dbo].[ZnodeAccountGlobalAttributeValueLocale] (
    [AccountGlobalAttributeValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [AccountGlobalAttributeValueId]       INT            NOT NULL,
    [LocaleId]                            INT            NOT NULL,
    [AttributeValue]                      NVARCHAR (MAX) NULL,
    [CreatedBy]                           INT            NOT NULL,
    [CreatedDate]                         DATETIME       NOT NULL,
    [ModifiedBy]                          INT            NOT NULL,
    [ModifiedDate]                        DATETIME       NOT NULL,
    [GlobalAttributeDefaultValueId]       INT            NULL,
    [MediaId]                             INT            NULL,
    [MediaPath]                           NVARCHAR (300) NULL,
    CONSTRAINT [PK_ZnodeAccountGlobalAttributeValueLocale] PRIMARY KEY CLUSTERED ([AccountGlobalAttributeValueLocaleId] ASC),
    CONSTRAINT [FK_ZnodeAccountGlobalAttributeValueLocale_ZnodeAccountGlobalAttributeValue] FOREIGN KEY ([AccountGlobalAttributeValueId]) REFERENCES [dbo].[ZnodeAccountGlobalAttributeValue] ([AccountGlobalAttributeValueId]),
    CONSTRAINT [FK_ZnodeAccountGlobalAttributeValueLocale_ZnodeMedia] FOREIGN KEY ([MediaId]) REFERENCES [dbo].[ZnodeMedia] ([MediaId])
);

