CREATE TABLE [dbo].[ZnodeMediaAttributeDefaultValueLocale] (
    [MediaAttributeDefaultValueLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                           INT            NULL,
    [MediaAttributeDefaultValueId]       INT            NULL,
    [DefaultAttributeValue]              NVARCHAR (MAX) NULL,
    [Description]                        VARCHAR (300)  NULL,
    [CreatedBy]                          INT            NOT NULL,
    [CreatedDate]                        DATETIME       NOT NULL,
    [ModifiedBy]                         INT            NOT NULL,
    [ModifiedDate]                       DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeMediaAttributeDefaultValueLocale] PRIMARY KEY CLUSTERED ([MediaAttributeDefaultValueLocaleId] ASC),
    CONSTRAINT [FK_ZnodeMediaAttributeDefaultValueLocale_ZNodeMediaDefaultAttributeValue] FOREIGN KEY ([MediaAttributeDefaultValueId]) REFERENCES [dbo].[ZnodeMediaAttributeDefaultValue] ([MediaAttributeDefaultValueId])
);







