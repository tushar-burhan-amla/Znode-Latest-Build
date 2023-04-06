CREATE TABLE [dbo].[ZnodePimAttributeLocale] (
    [PimAttributeLocaleId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]             INT            NULL,
    [PimAttributeId]       INT            NULL,
    [AttributeName]        NVARCHAR (300) NULL,
    [Description]          VARCHAR (300)  NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeLocale] PRIMARY KEY CLUSTERED ([PimAttributeLocaleId] ASC),
    CONSTRAINT [FK_ZnodePimAttributeLocale_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId])
);






GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20160816-154040]
    ON [dbo].[ZnodePimAttributeLocale]([LocaleId] ASC, [PimAttributeId] ASC);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20160816-123634]
    ON [dbo].[ZnodePimAttributeLocale]([LocaleId] ASC);

