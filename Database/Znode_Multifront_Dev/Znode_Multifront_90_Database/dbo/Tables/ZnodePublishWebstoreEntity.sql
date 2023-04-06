CREATE TABLE [dbo].[ZnodePublishWebstoreEntity] (
    [PublishWebstoreEntityId] INT             IDENTITY (1, 1) NOT NULL,
    [VersionId]               INT             NOT NULL,
    [PublishStartTime]        DATETIME        NULL,
    [PortalThemeId]           INT             NOT NULL,
    [PortalId]                INT             NOT NULL,
    [ThemeId]                 INT             NOT NULL,
    [ThemeName]               VARCHAR (200)   NULL,
    [CSSId]                   INT             NOT NULL,
    [CSSName]                 NVARCHAR (2000) NULL,
    [WebsiteLogo]             VARCHAR (300)   NULL,
    [WebsiteTitle]            NVARCHAR (400)  NULL,
    [FaviconImage]            VARCHAR (300)   NULL,
    [WebsiteDescription]      NVARCHAR (MAX)  NULL,
    [PublishState]            VARCHAR (100)   NULL,
    [LocaleId]                INT             NOT NULL,
    CONSTRAINT [PK_ZnodePublishWebstoreEntity] PRIMARY KEY CLUSTERED ([PublishWebstoreEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishWebstoreEntityVersionId]
    ON [dbo].[ZnodePublishWebstoreEntity]([VersionId] ASC);

