CREATE TABLE [dbo].[ZnodePimConfigureProductAttribute] (
    [PimConfigureProductAttributeId] INT      IDENTITY (1, 1) NOT NULL,
    [PimProductId]                   INT      NULL,
    [PimFamilyId]                    INT      NULL,
    [PimAttributeId]                 INT      NULL,
    [CreatedBy]                      INT      NOT NULL,
    [CreatedDate]                    DATETIME NOT NULL,
    [ModifiedBy]                     INT      NOT NULL,
    [ModifiedDate]                   DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePimConfigureProductAttribute] PRIMARY KEY CLUSTERED ([PimConfigureProductAttributeId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimConfigureProductAttribute_ZnodePimProduct] FOREIGN KEY ([PimProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId])
);






GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimConfigureProductAttribute_PimProductId_PimAttributeId]
    ON [dbo].[ZnodePimConfigureProductAttribute]([PimProductId] ASC, [PimAttributeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimConfigureProductAttribute_PimProductId]
    ON [dbo].[ZnodePimConfigureProductAttribute]([PimProductId] ASC);

