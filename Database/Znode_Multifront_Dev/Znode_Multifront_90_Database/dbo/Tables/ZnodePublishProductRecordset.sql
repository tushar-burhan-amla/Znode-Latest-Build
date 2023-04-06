CREATE TABLE [dbo].[ZnodePublishProductRecordset] (
    [PublishProductRecordsetId] INT           IDENTITY (1, 1) NOT NULL,
    [PublishCatalogId]          INT           NULL,
    [PublishProductId]          INT           NULL,
    [PimProductId]              INT           NULL,
    [VersionId]                 INT           NULL,
    [LocaleId]                  INT           NULL,
    [RevisionType]              VARCHAR (50)  NULL,
    [ImportGUID]                VARCHAR (500) NULL
);

