CREATE TABLE [dbo].[ZnodePublishBlogNewsEntity] (
    [PublishBlogNewsEntityId] INT             IDENTITY (1, 1) NOT NULL,
    [VersionId]               INT             NOT NULL,
    [PublishStartTime]        VARCHAR (20)    NOT NULL,
    [BlogNewsId]              INT             NOT NULL,
    [PortalId]                INT             NOT NULL,
    [MediaId]                 INT             NULL,
    [CMSContentPagesId]       INT             NULL,
    [LocaleId]                INT             NOT NULL,
    [BlogNewsType]            VARCHAR (300)   NOT NULL,
    [BlogNewsTitle]           NVARCHAR (1200) NULL,
    [BodyOverview]            VARCHAR (MAX)   NULL,
    [Tags]                    NVARCHAR (2000) NULL,
    [BlogNewsContent]         NVARCHAR (MAX)  NULL,
    [MediaPath]               VARCHAR (300)   NULL,
    [IsBlogNewsActive]        BIT             NOT NULL,
    [IsAllowGuestComment]     BIT             NOT NULL,
    [ActivationDate]          DATETIME        NULL,
    [ExpirationDate]          DATETIME        NULL,
    [CreatedDate]             DATETIME        NULL,
    [BlogNewsCode]            VARCHAR (50)    NOT NULL,
    CONSTRAINT [PK_ZnodePublishBlogNewsEntity] PRIMARY KEY CLUSTERED ([PublishBlogNewsEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishBlogNewsEntityVersionId]
    ON [dbo].[ZnodePublishBlogNewsEntity]([VersionId] ASC);

