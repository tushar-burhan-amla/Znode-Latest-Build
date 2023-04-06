CREATE TABLE [dbo].[ZnodePortalShipping] (
    [PortalShippingId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]         INT      NULL,
    [ShippingId]       INT      NULL,
    [CreatedBy]        INT      NOT NULL,
    [CreatedDate]      DATETIME NOT NULL,
    [ModifiedBy]       INT      NOT NULL,
    [ModifiedDate]     DATETIME NOT NULL,
    [PublishStateId]   TINYINT  NULL,
    CONSTRAINT [PK_ZnodePortalShipping] PRIMARY KEY CLUSTERED ([PortalShippingId] ASC),
    CONSTRAINT [FK_ZnodePortalShipping_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodePortalShipping_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId]),
    CONSTRAINT [FK_ZnodePortalShipping_Znodeshipping] FOREIGN KEY ([ShippingId]) REFERENCES [dbo].[ZnodeShipping] ([ShippingId])
);



