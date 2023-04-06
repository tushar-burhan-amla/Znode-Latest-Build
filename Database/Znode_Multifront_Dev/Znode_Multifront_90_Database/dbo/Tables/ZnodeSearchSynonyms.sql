CREATE TABLE [dbo].[ZnodeSearchSynonyms] (
    [SearchSynonymsId] INT            IDENTITY (1, 1) NOT NULL,
    [PublishCatalogId] INT            NULL,
    [OriginalTerm]     NVARCHAR (MAX) NULL,
    [ReplacedBy]       NVARCHAR (MAX) NULL,
    [IsBidirectional]  BIT            NULL,
    [LocaleId]         INT            NULL,
    [CreatedBy]        INT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedBy]       INT            NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    [SynonymCode]      NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_ZnodeSearchSynonyms] PRIMARY KEY CLUSTERED ([SearchSynonymsId] ASC) WITH (FILLFACTOR = 90)
);



