CREATE TABLE [dbo].[ZnodePortalGlobalAttributeValueLocale] (
    [PortalGlobalAttributeValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalGlobalAttributeValueId]       INT            NOT NULL,
    [LocaleId]                           INT            NOT NULL,
    [AttributeValue]                     NVARCHAR (MAX) NULL,
    [CreatedBy]                          INT            NOT NULL,
    [CreatedDate]                        DATETIME       NOT NULL,
    [ModifiedBy]                         INT            NOT NULL,
    [ModifiedDate]                       DATETIME       NOT NULL,
    [GlobalAttributeDefaultValueId]      INT            NULL,
    [MediaId]                            INT            NULL,
    [MediaPath]                          NVARCHAR (300) NULL,
    CONSTRAINT [PK_ZnodePortalGlobalAttributeValueLocale] PRIMARY KEY CLUSTERED ([PortalGlobalAttributeValueLocaleId] ASC),
    CONSTRAINT [FK_ZnodePortalGlobalAttributeValueLocale_ZnodeMedia] FOREIGN KEY ([MediaId]) REFERENCES [dbo].[ZnodeMedia] ([MediaId]),
    CONSTRAINT [FK_ZnodePortalGlobalAttributeValueLocale_ZnodePortalGlobalAttributeValue] FOREIGN KEY ([PortalGlobalAttributeValueId]) REFERENCES [dbo].[ZnodePortalGlobalAttributeValue] ([PortalGlobalAttributeValueId])
);

