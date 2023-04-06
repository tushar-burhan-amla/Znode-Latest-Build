CREATE TABLE [dbo].[ZnodePimAttributeXML] (
    [PimAttributeXMLId] INT            IDENTITY (1, 1) NOT NULL,
    [PimAttributeId]    INT            NULL,
    [AttributeCode]     VARCHAR (300)  NULL,
    [AttributeXml]      NVARCHAR (MAX) NULL,
    [LocaleId]          INT            NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeXML] PRIMARY KEY CLUSTERED ([PimAttributeXMLId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePimAttributeXML_PimAttributeXMLId]
    ON [dbo].[ZnodePimAttributeXML]([PimAttributeXMLId] ASC);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170518-144408]
    ON [dbo].[ZnodePimAttributeXML]([LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170518-144342]
    ON [dbo].[ZnodePimAttributeXML]([PimAttributeId] ASC)
    INCLUDE([LocaleId]);

