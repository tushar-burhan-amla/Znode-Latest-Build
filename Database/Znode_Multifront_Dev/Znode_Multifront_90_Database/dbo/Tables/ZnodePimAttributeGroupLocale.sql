CREATE TABLE [dbo].[ZnodePimAttributeGroupLocale] (
    [PimAttributeGroupLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                  INT            NULL,
    [PimAttributeGroupId]       INT            NOT NULL,
    [AttributeGroupName]        NVARCHAR (300) NULL,
    [Description]               VARCHAR (300)  NULL,
    [CreatedBy]                 INT            NOT NULL,
    [CreatedDate]               DATETIME       NOT NULL,
    [ModifiedBy]                INT            NOT NULL,
    [ModifiedDate]              DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeGroupLocale] PRIMARY KEY CLUSTERED ([PimAttributeGroupLocaleId] ASC),
    CONSTRAINT [FK_ZnodePimAttributeGroupLocale_ZnodePimAttributeGroup] FOREIGN KEY ([PimAttributeGroupId]) REFERENCES [dbo].[ZnodePimAttributeGroup] ([PimAttributeGroupId])
);



