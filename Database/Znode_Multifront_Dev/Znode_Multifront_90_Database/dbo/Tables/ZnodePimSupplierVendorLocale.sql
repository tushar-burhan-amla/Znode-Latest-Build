CREATE TABLE [dbo].[ZnodePimSupplierVendorLocale] (
    [PimSupplierVendorLocaleId] INT             IDENTITY (1, 1) NOT NULL,
    [PimSupplierVendorId]       INT             NOT NULL,
    [LocaleId]                  INT             NOT NULL,
    [SupplierVendorName]        NVARCHAR (2000) NULL,
    [Descriptions]              NVARCHAR (MAX)  NULL,
    [CreatedBy]                 INT             NOT NULL,
    [CreatedDate]               DATETIME        NOT NULL,
    [ModifiedBy]                INT             NOT NULL,
    [ModifiedDate]              DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePimSupplierVendorLocale] PRIMARY KEY CLUSTERED ([PimSupplierVendorLocaleId] ASC),
    CONSTRAINT [FK_ZnodePimSupplierVendorLocale_ZnodePimSupplierVendor] FOREIGN KEY ([PimSupplierVendorId]) REFERENCES [dbo].[ZnodePimSupplierVendor] ([PimSupplierVendorId])
);

