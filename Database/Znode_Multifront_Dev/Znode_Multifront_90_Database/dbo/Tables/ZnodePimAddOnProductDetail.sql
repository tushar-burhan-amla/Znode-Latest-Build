CREATE TABLE [dbo].[ZnodePimAddOnProductDetail] (
    [PimAddOnProductDetailId] INT      IDENTITY (1, 1) NOT NULL,
    [PimAddOnProductId]       INT      NULL,
    [PimChildProductId]       INT      NOT NULL,
    [IsDefault]               BIT      CONSTRAINT [DF_ZnodePimAddOnProductDetail_IsDefault] DEFAULT ((0)) NULL,
    [DisplayOrder]            INT      NULL,
    [CreatedBy]               INT      NOT NULL,
    [CreatedDate]             DATETIME NOT NULL,
    [ModifiedBy]              INT      NOT NULL,
    [ModifiedDate]            DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePimAddOnProductDetail] PRIMARY KEY CLUSTERED ([PimAddOnProductDetailId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimAddonGroupChildProduct_ZnodePimProduct] FOREIGN KEY ([PimChildProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId]),
    CONSTRAINT [FK_ZnodePimAddOnProductDetail_ZnodePimAddOnProduct] FOREIGN KEY ([PimAddOnProductId]) REFERENCES [dbo].[ZnodePimAddOnProduct] ([PimAddOnProductId])
);

GO
CREATE NONCLUSTERED INDEX IDX_ZnodePimAddOnProductDetail_PimAddOnProductId_PimChildProductId
ON [dbo].[ZnodePimAddOnProductDetail] ([PimAddOnProductId],[PimChildProductId])
INCLUDE ([IsDefault],[DisplayOrder]);

GO
CREATE NONCLUSTERED INDEX IDX_ZnodePimAddOnProductDetailPim_ChildProductId
ON [dbo].[ZnodePimAddOnProductDetail] ([PimChildProductId])
INCLUDE ([PimAddOnProductId],[IsDefault],[DisplayOrder]);



