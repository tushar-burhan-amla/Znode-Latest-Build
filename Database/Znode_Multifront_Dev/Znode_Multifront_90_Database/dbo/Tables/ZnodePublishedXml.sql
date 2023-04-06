CREATE TABLE [dbo].[ZnodePublishedXml] (
    [PublishedXmlId]      INT            IDENTITY (1, 1) NOT NULL,
    [PublishCatalogLogId] INT            NOT NULL,
    [PublishedId]         INT            NULL,
    [PublishedXML]        XML            NULL,
    [IsCategoryXML]       BIT            CONSTRAINT [DF__ZnodePubl__IsCat__383A4359] DEFAULT ((0)) NULL,
    [IsProductXML]        BIT            CONSTRAINT [DF__ZnodePubl__IsPro__392E6792] DEFAULT ((0)) NULL,
    [IsAddOnXML]          BIT            CONSTRAINT [DF_ZnodePublishedXml_IsAddOnXML] DEFAULT ((0)) NULL,
    [IsGroupProductXML]   BIT            CONSTRAINT [DF_ZnodePublishedXml_IsGroupProductXML] DEFAULT ((0)) NULL,
    [IsBundleProductXML]  BIT            CONSTRAINT [DF_ZnodePublishedXml_IsBundleProductXML] DEFAULT ((0)) NULL,
    [IsConfigProductXML]  BIT            CONSTRAINT [DF_ZnodePublishedXml_IsConfigProductXML] DEFAULT ((0)) NULL,
    [LocaleId]            INT            NULL,
    [CreatedBy]           INT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedBy]          INT            NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    [ImportedGuId]        NVARCHAR (400) NULL,
    [PublishCategoryId]   INT            NULL,
    CONSTRAINT [PK_ZnodePublishedXml] PRIMARY KEY CLUSTERED ([PublishedXmlId] ASC) WITH (FILLFACTOR = 90)
);











GO
ALTER TABLE [dbo].[ZnodePublishedXml] SET (LOCK_ESCALATION = DISABLE);




GO
CREATE NONCLUSTERED INDEX [IDX_ZnodePublishedXml]
    ON [dbo].[ZnodePublishedXml]([PublishCatalogLogId] ASC);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170525-162251]
    ON [dbo].[ZnodePublishedXml]([LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170525-162030]
    ON [dbo].[ZnodePublishedXml]([PublishedId] ASC);


GO
CREATE NONCLUSTERED INDEX [ind_ZnodePublishedXML_Bulk]
    ON [dbo].[ZnodePublishedXml]([PublishCatalogLogId] ASC, [IsConfigProductXML] ASC, [IsGroupProductXML] ASC, [IsBundleProductXML] ASC);

