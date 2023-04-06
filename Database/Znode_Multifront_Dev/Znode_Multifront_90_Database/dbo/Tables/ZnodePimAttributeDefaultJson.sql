CREATE TABLE [dbo].[ZnodePimAttributeDefaultJson] (
    [PimAttributeDefaultJsonId]  INT             IDENTITY (1, 1) NOT NULL,
    [PimAttributeDefaultValueId] INT             NULL,
    [AttributeDefaultValueCode]  VARCHAR (300)   NULL,
    [DefaultValueJson]           VARCHAR (max) NULL,
    [LocaleId]                   INT             NULL,
    [CreatedBy]                  INT             NOT NULL,
    [CreatedDate]                DATETIME        NOT NULL,
    [ModifiedBy]                 INT             NOT NULL,
    [ModifiedDate]               DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeDefaultJson] PRIMARY KEY CLUSTERED ([PimAttributeDefaultJsonId] ASC) WITH (FILLFACTOR = 90)
);




GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimAttributeDefaultJson_PimAttributeDefaultValueId_5A075]
    ON [dbo].[ZnodePimAttributeDefaultJson]([PimAttributeDefaultValueId] ASC)
    INCLUDE([DefaultValueJson]) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimAttributeDefaultJson_LocaleId_DC7FC]
    ON [dbo].[ZnodePimAttributeDefaultJson]([LocaleId] ASC)
    INCLUDE([PimAttributeDefaultValueId]) WITH (FILLFACTOR = 90);

