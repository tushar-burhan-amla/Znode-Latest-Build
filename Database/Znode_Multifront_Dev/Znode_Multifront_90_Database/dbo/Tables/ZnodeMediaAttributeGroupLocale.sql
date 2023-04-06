CREATE TABLE [dbo].[ZnodeMediaAttributeGroupLocale] (
    [MediaAttributeGroupLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                    INT            NULL,
    [MediaAttributeGroupId]       INT            NOT NULL,
    [AttributeGroupName]          NVARCHAR (300) NULL,
    [Description]                 VARCHAR (300)  NULL,
    [CreatedBy]                   INT            NOT NULL,
    [CreatedDate]                 DATETIME       NOT NULL,
    [ModifiedBy]                  INT            NOT NULL,
    [ModifiedDate]                DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeMediaAttributeGroupLocale] PRIMARY KEY CLUSTERED ([MediaAttributeGroupLocaleId] ASC),
    CONSTRAINT [FK_ZnodeMediaAttributeGroupLocale_ZnodeMediaAttributeGroup] FOREIGN KEY ([MediaAttributeGroupId]) REFERENCES [dbo].[ZnodeMediaAttributeGroup] ([MediaAttributeGroupId])
);



