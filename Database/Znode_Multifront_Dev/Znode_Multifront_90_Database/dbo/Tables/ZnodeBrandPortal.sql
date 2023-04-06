CREATE TABLE [dbo].[ZnodeBrandPortal] (
    [BrandPortalId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]      INT      NULL,
    [BrandId]       INT      NULL,
    [CreatedBy]     INT      NOT NULL,
    [CreatedDate]   DATETIME NOT NULL,
    [ModifiedBy]    INT      NOT NULL,
    [ModifiedDate]  DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeBrandPortalMapper] PRIMARY KEY CLUSTERED ([BrandPortalId] ASC),
    CONSTRAINT [FK_ZnodeBrandPortalMapper_ZnodeBrandDetails] FOREIGN KEY ([BrandId]) REFERENCES [dbo].[ZnodeBrandDetails] ([BrandId]),
    CONSTRAINT [FK_ZnodeBrandPortalMapper_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);

