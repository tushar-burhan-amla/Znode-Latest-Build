CREATE TABLE [dbo].[ZnodeSearchGlobalProductBoost] (
    [SearchGlobalProductBoostId] INT             IDENTITY (1, 1) NOT NULL,
    [PublishCatalogId]           INT             NOT NULL,
    [PublishProductId]           INT             NOT NULL,
    [Boost]                      NUMERIC (28, 6) NOT NULL,
    [CreatedBy]                  INT             NOT NULL,
    [CreatedDate]                DATETIME        NOT NULL,
    [ModifiedBy]                 INT             NOT NULL,
    [ModifiedDate]               DATETIME        NOT NULL,
    CONSTRAINT [PK_ZNodeSearchGlobalProductBoost] PRIMARY KEY CLUSTERED ([SearchGlobalProductBoostId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [IX_ZNodeSearchGlobalProductBoost] UNIQUE NONCLUSTERED ([PublishProductId] ASC) WITH (FILLFACTOR = 90)
);









