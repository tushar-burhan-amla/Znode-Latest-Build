CREATE TABLE [dbo].[ZnodePublishSeoEntity] (
    [PublishSeoEntityId]   INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]            INT            NOT NULL,
    [PublishStartTime]     DATETIME       NULL,
    [ItemName]             VARCHAR (300)  NULL,
    [CMSSEODetailId]       INT            NOT NULL,
    [CMSSEODetailLocaleId] INT            NOT NULL,
    [CMSSEOTypeId]         INT            NOT NULL,
    [SEOId]                INT            NULL,
    [SEOTypeName]          VARCHAR (50)   NULL,
    [SEOTitle]             NVARCHAR (MAX) NULL,
    [SEODescription]       NVARCHAR (MAX) NULL,
    [SEOKeywords]          NVARCHAR (MAX) NULL,
    [SEOUrl]               NVARCHAR (MAX) NULL,
    [IsRedirect]           BIT            NULL,
    [MetaInformation]      NVARCHAR (MAX) NULL,
    [LocaleId]             INT            NULL,
    [OldSEOURL]            NVARCHAR (MAX) NULL,
    [CMSContentPagesId]    INT            NOT NULL,
    [PortalId]             INT            NULL,
    [SEOCode]              VARCHAR (500)  NULL,
    [CanonicalURL]         VARCHAR (200)  NULL,
    [RobotTag]             NVARCHAR (MAX) NULL,
	[ElasticSearchEvent]     INT,
    CONSTRAINT [PK_ZnodePublishSeoEntity] PRIMARY KEY CLUSTERED ([PublishSeoEntityId] ASC) WITH (FILLFACTOR = 90)
);






GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishSeoEntityVersionId]
    ON [dbo].[ZnodePublishSeoEntity]([VersionId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishSEOEntity]
    ON [dbo].[ZnodePublishSeoEntity]([VersionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishSeoEntity_VersionId_SEOTypeName_LocaleId_PortalId_SEOCode_53398]
    ON [dbo].[ZnodePublishSeoEntity]([VersionId] ASC, [SEOTypeName] ASC, [LocaleId] ASC, [PortalId] ASC, [SEOCode] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishSeoEntity_SEOTypeName_LocaleId_PortalId_SEOCode_F8B79]
    ON [dbo].[ZnodePublishSeoEntity]([SEOTypeName] ASC, [LocaleId] ASC, [PortalId] ASC, [SEOCode] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishSeoEntity_PortalId_CMSSEOTypeId_F1539]
    ON [dbo].[ZnodePublishSeoEntity]([PortalId] ASC, [CMSSEOTypeId] ASC)
    INCLUDE([VersionId]);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishSeoEntity_CMSSEOTypeId_F822F]
    ON [dbo].[ZnodePublishSeoEntity]([CMSSEOTypeId] ASC)
    INCLUDE([VersionId], [PortalId]);

