CREATE TABLE [dbo].[ZnodePublishPortalBrandEntity] (
    [PublishPortalBrandEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                  INT            NOT NULL,
    [PublishStartTime]           DATETIME       NULL,
    [PortalId]                   INT            NULL,
    [LocaleId]                   INT            NOT NULL,
    [BrandId]                    INT            NOT NULL,
    [BrandCode]                  VARCHAR (50)   NOT NULL,
    [BrandName]                  VARCHAR (300)  NOT NULL,
    [MediaId]                    INT            NULL,
    [WebsiteLink]                NVARCHAR (MAX) NULL,
    [Description]                NVARCHAR (MAX) NULL,
    [PublishState]               VARCHAR (30)   NOT NULL,
    [SEOTitle]                   NVARCHAR (MAX) NULL,
    [SEOKeywords]                NVARCHAR (MAX) NULL,
    [SEODescription]             NVARCHAR (MAX) NULL,
    [SEOFriendlyPageName]        NVARCHAR (MAX) NULL,
    [DisplayOrder]               INT            NOT NULL,
    [IsActive]                   BIT            NOT NULL,
    [MediaPath]                  VARCHAR (300)  NULL,
    [CMSSEODetailId]             INT            NULL,
    [CMSSEODetailLocaleId]       INT            NULL,
    [BrandDetailLocaleId]        INT            NOT NULL,
    [ImageName]                  VARCHAR (300)  NULL,
    [Custom1]                    nvarchar(max),
    [Custom2] NVARCHAR(MAX) NULL,
    [Custom3] NVARCHAR(MAX) NULL,
    [Custom4] NVARCHAR(MAX) NULL,
    [Custom5] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_ZnodePublishPortalBrandEntity] PRIMARY KEY CLUSTERED ([PublishPortalBrandEntityId] ASC)
);




GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishPortalBrandEntityVersionId]
    ON [dbo].[ZnodePublishPortalBrandEntity]([VersionId] ASC);


GO
CREATE NONCLUSTERED INDEX [Inx_ZnodePublishPortalBrandEntity_PortalId_BrandCode_LocaleId_VersionId_IsActive]
    ON [dbo].[ZnodePublishPortalBrandEntity]([VersionId] ASC, [PortalId] ASC, [LocaleId] ASC, [BrandCode] ASC, [IsActive] ASC);

