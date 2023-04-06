CREATE TABLE [dbo].[ZnodePimLinkProductDetail] (
    [PimLinkProductDetailId] INT      IDENTITY (1, 1) NOT NULL,
    [PimParentProductId]     INT      NULL,
    [PimProductId]           INT      NULL,
    [PimAttributeId]         INT      NULL,
    [CreatedBy]              INT      NULL,
    [CreatedDate]            DATETIME NULL,
    [ModifiedBy]             INT      NULL,
    [ModifiedDate]           DATETIME NULL,
    [DisplayOrder]           INT      NULL,
    CONSTRAINT [PK_ZnodePimLinkProductDetail] PRIMARY KEY CLUSTERED ([PimLinkProductDetailid] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimLinkProductDetail_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId]),
    CONSTRAINT [FK_ZnodePimLinkProductDetail_ZnodePimProduct] FOREIGN KEY ([PimProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId]),
    CONSTRAINT [FK_ZnodePimLinkProductDetail_ZnodePimProduct_PimParentProductId] FOREIGN KEY ([PimParentProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId])
);










GO
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20170201-114506]
    ON [dbo].[ZnodePimLinkProductDetail]([PimProductId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePimLinkProductDetail_PimParentProductId]
    ON [dbo].[ZnodePimLinkProductDetail]([PimParentProductId] ASC);

