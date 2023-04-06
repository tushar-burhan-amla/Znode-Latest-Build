CREATE TABLE [dbo].[ZnodePimManufacturerBrandProduct] (
    [PimManufacturerBrandProductId] INT      IDENTITY (1, 1) NOT NULL,
    [PimManufacturerBrandId]        INT      NULL,
    [PublishProductId]              INT      NOT NULL,
    [CreatedBy]                     INT      NOT NULL,
    [CreatedDate]                   DATETIME NOT NULL,
    [ModifiedBy]                    INT      NOT NULL,
    [ModifiedDate]                  DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePimManufacturerBrandProduct] PRIMARY KEY CLUSTERED ([PimManufacturerBrandProductId] ASC),
    CONSTRAINT [FK_ZnodePimManufacturerBrandProduct_ZnodePimManufacturerBrand] FOREIGN KEY ([PimManufacturerBrandId]) REFERENCES [dbo].[ZnodePimManufacturerBrand] ([PimManufacturerBrandId])
);



