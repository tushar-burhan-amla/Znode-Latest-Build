CREATE TABLE [dbo].[ZnodePublishCategoryEntity] (
    [PublishCategoryEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]               INT            NOT NULL,
    [ZnodeCategoryId]         INT            NOT NULL,
    [Name]                    NVARCHAR (300)  NOT NULL,
    [CategoryCode]            VARCHAR (100)  NOT NULL,
    [ZnodeCatalogId]          INT            NOT NULL,
    [CatalogName]             VARCHAR (300)  NOT NULL,
    [ZnodeParentCategoryIds]  VARCHAR (2000) NULL,
    [ProductIds]              VARCHAR (MAX)  NULL,
    [LocaleId]                INT            NOT NULL,
    [IsActive]                BIT            NOT NULL,
    [DisplayOrder]            INT            NOT NULL,
    [Attributes]              NVARCHAR (MAX)  NOT NULL,
    [ActivationDate]          DATETIME       NULL,
    [ExpirationDate]          DATETIME       NULL,
    [CategoryIndex]           INT            NOT NULL,
	[ElasticSearchEvent]      INT,
    CONSTRAINT [PK_ZnodePublishCategoryEntity] PRIMARY KEY CLUSTERED ([PublishCategoryEntityId] ASC) WITH (FILLFACTOR = 90)
);










GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishCategoryEntity]
    ON [dbo].[ZnodePublishCategoryEntity]([VersionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishCategoryEntity_ZnodeCatalogId_4701D]
    ON [dbo].[ZnodePublishCategoryEntity]([ZnodeCatalogId] ASC)
    INCLUDE([VersionId]) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishCategoryEntity_CatalogId]
    ON [dbo].[ZnodePublishCategoryEntity]([VersionId] ASC, [ZnodeCatalogId] ASC);


GO
CREATE NONCLUSTERED INDEX [Inx_ZnodePublishCategoryEntity_ZnodeCategoryId_IsActive_VersionId]
    ON [dbo].[ZnodePublishCategoryEntity]([VersionId] ASC, [ZnodeCategoryId] ASC, [IsActive] ASC);

