CREATE TABLE [dbo].[ZnodePortalBrand] (
    [PortalBrandId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]      INT      NULL,
    [BrandId]       INT      NULL,
    [DisplayOrder]  INT      NULL,
    [CreatedBy]     INT      NOT NULL,
    [CreatedDate]   DATETIME NOT NULL,
    [ModifiedBy]    INT      NOT NULL,
    [ModifiedDate]  DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePortalBrandMapper] PRIMARY KEY CLUSTERED ([PortalBrandId] ASC),
    CONSTRAINT [FK_ZnodePortalBrand_ZnodeBrandDetails] FOREIGN KEY ([BrandId]) REFERENCES [dbo].[ZnodeBrandDetails] ([BrandId]),
    CONSTRAINT [FK_ZnodePortalBrand_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);

