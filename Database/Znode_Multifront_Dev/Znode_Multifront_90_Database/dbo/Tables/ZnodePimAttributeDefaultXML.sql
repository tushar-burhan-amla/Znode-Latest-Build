CREATE TABLE [dbo].[ZnodePimAttributeDefaultXML] (
    [PimAttributeDefaultXMLId]   INT             IDENTITY (1, 1) NOT NULL,
    [PimAttributeDefaultValueId] INT             NULL,
    [AttributeDefaultValueCode]  VARCHAR (300)   NULL,
    [DefaultValueXML]            NVARCHAR (4000) NULL,
    [LocaleId]                   INT             NULL,
    [CreatedBy]                  INT             NOT NULL,
    [CreatedDate]                DATETIME        NOT NULL,
    [ModifiedBy]                 INT             NOT NULL,
    [ModifiedDate]               DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeDefaultXML] PRIMARY KEY CLUSTERED ([PimAttributeDefaultXMLId] ASC) WITH (FILLFACTOR = 90)
);




GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimAttributeDefaultXML_PimAttributeDefaultValueId]
    ON [dbo].[ZnodePimAttributeDefaultXML]([PimAttributeDefaultValueId] ASC)
    INCLUDE([DefaultValueXML]);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimAttributeDefaultXML_LocaleId]
    ON [dbo].[ZnodePimAttributeDefaultXML]([LocaleId] ASC)
    INCLUDE([PimAttributeDefaultValueId]);

