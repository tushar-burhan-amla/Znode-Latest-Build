CREATE TABLE [dbo].[ZnodePimProductTypeAssociation] (
    [PimProductTypeAssociationId] INT      IDENTITY (1, 1) NOT NULL,
    [PimParentProductId]          INT      NULL,
    [PimProductId]                INT      NULL,
    [PimAttributeId]              INT      NULL,
    [DisplayOrder]                INT      NULL,
    [CreatedBy]                   INT      NULL,
    [CreatedDate]                 DATETIME NULL,
    [ModifiedBy]                  INT      NULL,
    [ModifiedDate]                DATETIME NULL,
	[IsDefault]                   BIT NULL,
	[BundleQuantity]			  INT NULL,
    CONSTRAINT [PK_ZnodePimProductTypeDetail] PRIMARY KEY CLUSTERED ([PimProductTypeAssociationId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimProductTypeDetail_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId]),
    CONSTRAINT [FK_ZnodePimProductTypeDetail_ZnodePimProduct] FOREIGN KEY ([PimProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId]),
    CONSTRAINT [FK_ZnodePimProductTypeDetail_ZnodePimProduct_PimParentProductId] FOREIGN KEY ([PimParentProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId]),
    CONSTRAINT [UC_ZnodePimProductTypeAssociation] UNIQUE NONCLUSTERED ([PimParentProductId] ASC, [PimProductId] ASC) WITH (FILLFACTOR = 90)
);








GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimProductTypeAssociation_PimProductId_C352E]
    ON [dbo].[ZnodePimProductTypeAssociation]([PimProductId] ASC)
    INCLUDE([PimParentProductId], [PimAttributeId], [DisplayOrder], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) WITH (FILLFACTOR = 90);

