CREATE TABLE [dbo].[ZnodeSearchGlobalProductCategoryBoost] (
    [SearchGlobalProductCategoryBoostId] INT             IDENTITY (1, 1) NOT NULL,
    [PublishCatalogId]                   INT             NOT NULL,
    [PublishProductId]                   INT             NOT NULL,
    [PublishCategoryId]                  INT             NOT NULL,
    [Boost]                              NUMERIC (28, 6) NOT NULL,
    [CreatedBy]                          INT             NOT NULL,
    [CreatedDate]                        DATETIME        NOT NULL,
    [ModifiedBy]                         INT             NOT NULL,
    [ModifiedDate]                       DATETIME        NOT NULL,
    CONSTRAINT [PK_ZNodeProductCategoryBoost] PRIMARY KEY CLUSTERED ([SearchGlobalProductCategoryBoostId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [IX_ZNodeSearchGlobalProductCategoryUnique] UNIQUE NONCLUSTERED ([PublishProductId] ASC, [PublishCategoryId] ASC) WITH (FILLFACTOR = 90)
);











