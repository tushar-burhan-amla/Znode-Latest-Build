CREATE TABLE [dbo].[ZnodeShippingPortal] (
    [ShippingPortalId]          INT             IDENTITY (1, 1) NOT NULL,
    [PortalId]                  INT             NOT NULL,
    [ShippingOriginAddress1]    NVARCHAR (MAX)  NULL,
    [ShippingOriginAddress2]    NVARCHAR (MAX)  NULL,
    [ShippingOriginCity]        NVARCHAR (200)  NULL,
    [ShippingOriginStateCode]   NVARCHAR (255)  NULL,
    [ShippingOriginZipCode]     NVARCHAR (200)  NULL,
    [ShippingOriginCountryCode] NVARCHAR (20)   NULL,
    [ShippingOriginPhone]       NVARCHAR (2000) NULL,
    [FedExAccountNumber]        NVARCHAR (2000) NULL,
    [FedExLTLAccountNumber]     NVARCHAR (2000) NULL,
    [FedExMeterNumber]          NVARCHAR (2000) NULL,
    [FedExProductionKey]        NVARCHAR (2000) NULL,
    [FedExSecurityCode]         NVARCHAR (2000) NULL,
    [FedExDropoffType]          NVARCHAR (2000) NULL,
    [FedExPackagingType]        NVARCHAR (2000) NULL,
    [FedExUseDiscountRate]      BIT             NULL,
    [FedExAddInsurance]         BIT             NULL,
    [UPSUserName]               NVARCHAR (2000) NULL,
    [UPSPassword]               NVARCHAR (2000) NULL,
    [UPSKey]                    NVARCHAR (2000) NULL,
    [LTLUPSAccessLicenseNumber] NVARCHAR (2000) NULL,
    [LTLUPSUsername]            NVARCHAR (2000) NULL,
    [LTLUPSPassword]            NVARCHAR (2000) NULL,
    [LTLUPSAccountNumber]       NVARCHAR (2000) NULL,
    [UPSDropoffType]            NVARCHAR (2000) NULL,
    [UPSPackagingType]          NVARCHAR (2000) NULL,
    [IsUseWarehouseAddress]     BIT             CONSTRAINT [DF_ZnodeShippingPortal_IsDefaultWarehouse] DEFAULT ((0)) NULL,
    [USPSShippingAPIURL]        NVARCHAR (2000) NULL,
    [USPSWebToolsUserID]        NVARCHAR (2000) NULL,
    [PackageWeightLimit]        NUMERIC (28, 6) CONSTRAINT [DF_ZnodeShippingPortal_PackageWeightLimit] DEFAULT ((65)) NULL,
    [CreatedBy]                 INT             NOT NULL,
    [CreatedDate]               DATETIME        NOT NULL,
    [ModifiedBy]                INT             NOT NULL,
    [ModifiedDate]              DATETIME        NOT NULL,
    [PublishStateId]            TINYINT         NULL,
    CONSTRAINT [PK_ZnodeShippingPortal] PRIMARY KEY CLUSTERED ([ShippingPortalId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeShippingPortal_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);



















