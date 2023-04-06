CREATE TABLE [dbo].[ZnodePublishedPortalXml] (
    [PublishedPortalXmlId] INT      IDENTITY (1, 1) NOT NULL,
    [PublishPortalLogId]   INT      NOT NULL,
    [PortalId]             INT      NULL,
    [PublishedXML]         XML      NULL,
    [LocaleId]             INT      NULL,
    [CreatedBy]            INT      NOT NULL,
    [CreatedDate]          DATETIME NOT NULL,
    [ModifiedBy]           INT      NOT NULL,
    [ModifiedDate]         DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePublishedPortalXml] PRIMARY KEY CLUSTERED ([PublishedPortalXmlId] ASC) WITH (FILLFACTOR = 90)
);

