CREATE TABLE [dbo].[ZnodePublishProductDetail] (
    [PublishProductInfoId] INT             IDENTITY (1, 1) NOT NULL,
    [PublishProductId]     INT             NULL,
    [ProductName]          NVARCHAR (2000) NULL,
    [SKU]                  NVARCHAR (2000) NULL,
    [IsActive]             BIT             CONSTRAINT [DF_ZnodePublishProductDetail_IsActive] DEFAULT ((0)) NULL,
    [LocaleId]             INT             NULL,
    [CreatedBy]            INT             NOT NULL,
    [CreatedDate]          DATETIME        NOT NULL,
    [ModifiedBy]           INT             NOT NULL,
    [ModifiedDate]         DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePublishProductDetail] PRIMARY KEY CLUSTERED ([PublishProductInfoId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePublishProductDetail_ZnodePublishProduct] FOREIGN KEY ([PublishProductId]) REFERENCES [dbo].[ZnodePublishProduct] ([PublishProductId])
);












GO
CREATE NONCLUSTERED INDEX [IDX_ZnodePublishProductDetail_SKU]
    ON [dbo].[ZnodePublishProductDetail]([SKU] ASC);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170521-001923]
    ON [dbo].[ZnodePublishProductDetail]([LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170521-001728]
    ON [dbo].[ZnodePublishProductDetail]([PublishProductId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePublishProductDetail_LocaleId_3B076]
    ON [dbo].[ZnodePublishProductDetail]([LocaleId] ASC)
    INCLUDE([PublishProductId], [SKU]) WITH (FILLFACTOR = 90);

