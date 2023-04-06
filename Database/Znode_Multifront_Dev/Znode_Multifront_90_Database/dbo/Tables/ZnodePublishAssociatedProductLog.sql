CREATE TABLE [dbo].[ZnodePublishAssociatedProductLog] (
    [PublishAssociatedProductLogId] INT      IDENTITY (1, 1) NOT NULL,
    [PimCatalogId]                  INT      NULL,
    [ParentPimProductId]            INT      NULL,
    [PimProductId]                  INT      NULL,
    [PublishStateId]                INT      NULL,
    [IsConfigurable]                BIT      NULL,
    [IsBundle]                      BIT      NULL,
    [IsGroup]                       BIT      NULL,
    [IsAddOn]                       BIT      NULL,
    [IsLink]                        BIT      NULL,
    [DisplayOrder]                  INT      NULL,
    [CreatedBy]                     INT      NOT NULL,
    [CreatedDate]                   DATETIME NOT NULL,
    [ModifiedBy]                    INT      NOT NULL,
    [ModifiedDate]                  DATETIME NOT NULL,
	[IsDefault]                     BIT NULL,
);


GO
CREATE NONCLUSTERED INDEX [Idx_ZnodePublishAssociatedProductLog]
    ON [dbo].[ZnodePublishAssociatedProductLog]([PimCatalogId] ASC, [ParentPimProductId] ASC, [PimProductId] ASC, [PublishStateId] ASC, [IsConfigurable] ASC, [IsBundle] ASC, [IsGroup] ASC, [IsAddOn] ASC, [IsLink] ASC)
    INCLUDE([ModifiedBy]);

