CREATE TABLE [dbo].[ZnodePortalAddress] (
    [PortalAddressId]   INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]          INT            NOT NULL,
    [AddressId]         INT            NOT NULL,
    [MediaId]           INT            NULL,
    [StoreName]         VARCHAR (600)  NULL,
    [DisplayOrder]      INT            NULL,
    [Latitude]          DECIMAL (9, 6) NULL,
    [Longitude]         DECIMAL (9, 6) NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    [StoreLocationCode] NVARCHAR (200) NULL,
    CONSTRAINT [PK_ZnodePortalAddress] PRIMARY KEY CLUSTERED ([PortalAddressId] ASC),
    CONSTRAINT [FK_ZnodePortalAddress_ZnodeAddress] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[ZnodeAddress] ([AddressId]),
    CONSTRAINT [FK_ZnodePortalAddress_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [UK_ZnodePortalAddress] UNIQUE NONCLUSTERED ([StoreLocationCode] ASC)
);







