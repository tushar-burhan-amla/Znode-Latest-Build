CREATE TABLE [dbo].[ZnodePimSupplierVendor] (
    [PimSupplierVendorId]       INT             IDENTITY (1, 1) NOT NULL,
    [ExternalSupplierNo]        NVARCHAR (300)  NULL,
    [SupplierVendorCode]        NVARCHAR (600)  NULL,
    [ContactFirstName]          NVARCHAR (200)  NULL,
    [ContactLastName]           NVARCHAR (200)  NULL,
    [ContactPhone]              VARCHAR (50)    NULL,
    [ContactEmail]              VARCHAR (100)   NULL,
    [NotificationEmailID]       VARCHAR (100)   NULL,
    [EmailNotificationTemplate] NVARCHAR (2000) NULL,
    [EnableEmailNotification]   BIT             NULL,
    [DisplayOrder]              INT             NULL,
    [IsActive]                  BIT             CONSTRAINT [DF_ZnodePimSupplierVendor] DEFAULT ((1)) NULL,
    [CreatedBy]                 INT             NOT NULL,
    [CreatedDate]               DATETIME        NOT NULL,
    [ModifiedBy]                INT             NOT NULL,
    [ModifiedDate]              DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePimSupplierVendor] PRIMARY KEY CLUSTERED ([PimSupplierVendorId] ASC)
);



