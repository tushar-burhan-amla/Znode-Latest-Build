CREATE TABLE [dbo].[ZnodePriceListPortal] (
    [PriceListPortalId] INT      IDENTITY (1, 1) NOT NULL,
    [PriceListId]       INT      NOT NULL,
    [PortalId]          INT      NOT NULL,
    [Precedence]        INT      NULL,
    [CreatedBy]         INT      NOT NULL,
    [CreatedDate]       DATETIME NOT NULL,
    [ModifiedBy]        INT      NOT NULL,
    [ModifiedDate]      DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePriceListPortal] PRIMARY KEY CLUSTERED ([PriceListPortalId] ASC),
    CONSTRAINT [FK_ZnodePriceListPortal_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodePriceListPortal_ZnodePriceList] FOREIGN KEY ([PriceListId]) REFERENCES [dbo].[ZnodePriceList] ([PriceListId])
);





