CREATE TABLE [dbo].[ZnodeGlobalAttributeGroupLocale] (
    [GlobalAttributeGroupLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                     INT            NULL,
    [GlobalAttributeGroupId]       INT            NOT NULL,
    [AttributeGroupName]           NVARCHAR (300) NULL,
    [Description]                  VARCHAR (300)  NULL,
    [CreatedBy]                    INT            NOT NULL,
    [CreatedDate]                  DATETIME       NOT NULL,
    [ModifiedBy]                   INT            NOT NULL,
    [ModifiedDate]                 DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalAttributeGroupLocale] PRIMARY KEY CLUSTERED ([GlobalAttributeGroupLocaleId] ASC),
    CONSTRAINT [FK_ZnodeGlobalAttributeGroupLocale_ZnodeGlobalAttributeGroup] FOREIGN KEY ([GlobalAttributeGroupId]) REFERENCES [dbo].[ZnodeGlobalAttributeGroup] ([GlobalAttributeGroupId])
);

