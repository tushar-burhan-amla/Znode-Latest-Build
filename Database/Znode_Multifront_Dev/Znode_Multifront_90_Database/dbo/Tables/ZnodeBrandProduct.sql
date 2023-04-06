CREATE TABLE [dbo].[ZnodeBrandProduct] (
    [BrandProductId] INT      IDENTITY (1, 1) NOT NULL,
    [PimProductId]   INT      NULL,
    [BrandId]        INT      NULL,
    [CreatedBy]      INT      NOT NULL,
    [CreatedDate]    DATETIME NOT NULL,
    [ModifiedBy]     INT      NOT NULL,
    [ModifiedDate]   DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeBrandProduct] PRIMARY KEY CLUSTERED ([BrandProductId] ASC),
    CONSTRAINT [FK_ZnodeBrandProduct_ZnodeBrandDetails] FOREIGN KEY ([BrandId]) REFERENCES [dbo].[ZnodeBrandDetails] ([BrandId]),
    CONSTRAINT [FK_ZnodeBrandProduct_ZnodePimProduct] FOREIGN KEY ([PimProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId])
);

