CREATE TABLE [dbo].[ZnodePimSupplierVendorProduct] (
    [PimSupplierVendorProductId] INT      IDENTITY (1, 1) NOT NULL,
    [PimSupplierVendorId]        INT      NULL,
    [PublishProductId]           INT      NOT NULL,
    [CreatedBy]                  INT      NOT NULL,
    [CreatedDate]                DATETIME NOT NULL,
    [ModifiedBy]                 INT      NOT NULL,
    [ModifiedDate]               DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePimSupplierVendorProduct] PRIMARY KEY CLUSTERED ([PimSupplierVendorProductId] ASC),
    CONSTRAINT [FK_ZnodePimSupplierVendorProduct_ZnodePimSupplierVendor] FOREIGN KEY ([PimSupplierVendorId]) REFERENCES [dbo].[ZnodePimSupplierVendor] ([PimSupplierVendorId])
);



