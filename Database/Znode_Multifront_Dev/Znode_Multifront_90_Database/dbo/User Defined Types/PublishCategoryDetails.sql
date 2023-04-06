CREATE TYPE [dbo].[PublishCategoryDetails] AS TABLE (
    [PublishCategoryId] INT NULL,
    [VersionId]         INT NULL,
    [LocaleId]          INT NULL,
    [PublishCatalogId]  INT NULL,
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC));

