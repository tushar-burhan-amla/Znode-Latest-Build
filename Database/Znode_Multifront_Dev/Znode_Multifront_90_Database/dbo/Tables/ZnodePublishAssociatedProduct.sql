CREATE TABLE [dbo].[ZnodePublishAssociatedProduct] (
    [PublishAssociatedProductId] INT      IDENTITY (1, 1) NOT NULL,
    [PimCatalogId]               INT      NULL,
    [ParentPimProductId]         INT      NULL,
    [PimProductId]               INT      NULL,
    [PublishStateId]             INT      NULL,
    [IsConfigurable]             BIT      DEFAULT ((0)) NULL,
    [IsBundle]                   BIT      DEFAULT ((0)) NULL,
    [IsGroup]                    BIT      DEFAULT ((0)) NULL,
    [IsAddOn]                    BIT      DEFAULT ((0)) NULL,
    [IsLink]                     BIT      DEFAULT ((0)) NULL,
    [DisplayOrder]               INT      NULL,
    [CreatedBy]                  INT      NOT NULL,
    [CreatedDate]                DATETIME NOT NULL,
    [ModifiedBy]                 INT      NOT NULL,
    [ModifiedDate]               DATETIME NOT NULL,
	[IsDefault]                   BIT NULL,
    CONSTRAINT [PK_ZnodePublishAssociatedProduct] PRIMARY KEY CLUSTERED ([PublishAssociatedProductId] ASC)
);




GO
CREATE NONCLUSTERED INDEX [ZnodePublishAssociatedProduct_PimCatalogId]
    ON [dbo].[ZnodePublishAssociatedProduct]([PimCatalogId] ASC)
    INCLUDE([ParentPimProductId], [PimProductId], [PublishStateId], [IsConfigurable], [IsBundle], [IsGroup], [IsAddOn], [IsLink]);


GO
CREATE NONCLUSTERED INDEX [ZnodePublishAssociatedProduct_ParentPimProductId]
    ON [dbo].[ZnodePublishAssociatedProduct]([ParentPimProductId] ASC);

