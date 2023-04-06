CREATE TABLE [dbo].[ZnodePimManufacturerBrandLocale] (
    [PimManufacturerBrandLocaleId] INT             IDENTITY (1, 1) NOT NULL,
    [PimManufacturerBrandId]       INT             NOT NULL,
    [LocaleId]                     INT             NOT NULL,
    [BrandName]                    NVARCHAR (2000) NULL,
    [Descriptions]                 NVARCHAR (MAX)  NULL,
    [CreatedBy]                    INT             NOT NULL,
    [CreatedDate]                  DATETIME        NOT NULL,
    [ModifiedBy]                   INT             NOT NULL,
    [ModifiedDate]                 DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePimManufacturerBrandLocale] PRIMARY KEY CLUSTERED ([PimManufacturerBrandLocaleId] ASC),
    CONSTRAINT [FK_ZnodePimManufacturerBrandLocale_ZnodePimManufacturerBrand] FOREIGN KEY ([PimManufacturerBrandId]) REFERENCES [dbo].[ZnodePimManufacturerBrand] ([PimManufacturerBrandId])
);

