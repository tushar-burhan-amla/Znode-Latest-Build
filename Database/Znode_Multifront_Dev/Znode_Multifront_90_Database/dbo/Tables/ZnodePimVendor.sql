CREATE TABLE [dbo].[ZnodePimVendor] (
    [PimVendorId]               INT             IDENTITY (1, 1) NOT NULL,
    [VendorCode]                VARCHAR (600)   NULL,
    [PimAttributeId]            INT             NULL,
    [AddressId]                 INT             NULL,
    [ExternalVendorNo]          NVARCHAR (300)  NULL,
    [Email]                     VARCHAR (100)   NULL,
    [NotificationEmailID]       VARCHAR (100)   NULL,
    [EmailNotificationTemplate] NVARCHAR (2000) NULL,
    [CompanyName]               NVARCHAR (1000) NULL,
    [DisplayOrder]              INT             NULL,
    [IsActive]                  BIT             CONSTRAINT [DF_ZnodePimSupplierVendor] DEFAULT ((1)) NULL,
    [CreatedBy]                 INT             NOT NULL,
    [CreatedDate]               DATETIME        NOT NULL,
    [ModifiedBy]                INT             NOT NULL,
    [ModifiedDate]              DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePimSupplierVendor] PRIMARY KEY CLUSTERED ([PimVendorId] ASC),
    CONSTRAINT [FK_ZnodePimVendor_ZnodeAddress] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[ZnodeAddress] ([AddressId]),
    CONSTRAINT [FK_ZnodePimVendor_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId])
);

