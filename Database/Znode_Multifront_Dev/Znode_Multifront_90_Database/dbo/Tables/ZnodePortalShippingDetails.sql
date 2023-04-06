CREATE TABLE [dbo].[ZnodePortalShippingDetails] (
    [PortalShippingDetailId]    INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]                  INT            NULL,
    [ShippingOriginZipCode]     NVARCHAR (50)  NULL,
    [ShopByPriceMin]            INT            NULL,
    [ShopByPriceMax]            INT            NULL,
    [ShopByPriceIncrement]      INT            NULL,
    [FedExClientProductId]      NVARCHAR (MAX) NULL,
    [FedExClientProductVersion] NVARCHAR (MAX) NULL,
    [FedExDropoffType]          NVARCHAR (MAX) NULL,
    [FedExPackagingType]        NVARCHAR (MAX) NULL,
    [ShippingOriginStateCode]   NVARCHAR (255) NULL,
    [ShippingOriginCountryCode] NVARCHAR (MAX) NULL,
    [CreatedBy]                 INT            NOT NULL,
    [CreatedDate]               DATETIME       NOT NULL,
    [ModifiedBy]                INT            NOT NULL,
    [ModifiedDate]              DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePortalShippingDetails] PRIMARY KEY CLUSTERED ([PortalShippingDetailId] ASC),
    CONSTRAINT [FK_ZnodePortalShippingDetails_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);



