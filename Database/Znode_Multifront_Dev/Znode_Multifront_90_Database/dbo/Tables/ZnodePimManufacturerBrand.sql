CREATE TABLE [dbo].[ZnodePimManufacturerBrand] (
    [PimManufacturerBrandId]    INT            IDENTITY (1, 1) NOT NULL,
    [WebsiteLink]               NVARCHAR (500) NULL,
    [ManufacturerBrandCode]     NVARCHAR (600) NULL,
    [EmailId]                   VARCHAR (100)  NULL,
    [IsDropShipper]             BIT            NULL,
    [IsFacet]                   BIT            CONSTRAINT [DF_ZnodePimManufacturerBrand_IsFacet] DEFAULT ((1)) NULL,
    [EmailNotificationTemplate] NVARCHAR (500) NULL,
    [DisplayOrder]              INT            NULL,
    [IsActive]                  BIT            CONSTRAINT [DF_ZnodePimManufacturerBrand_ActiveInd] DEFAULT ((1)) NOT NULL,
    [CreatedBy]                 INT            NOT NULL,
    [CreatedDate]               DATETIME       NOT NULL,
    [ModifiedBy]                INT            NOT NULL,
    [ModifiedDate]              DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimManufacturerBrand] PRIMARY KEY CLUSTERED ([PimManufacturerBrandId] ASC)
);



