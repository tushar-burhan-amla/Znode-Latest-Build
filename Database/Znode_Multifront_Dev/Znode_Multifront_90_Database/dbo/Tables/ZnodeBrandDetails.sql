CREATE TABLE [dbo].[ZnodeBrandDetails] (
    [BrandId]      INT            IDENTITY (1, 1) NOT NULL,
    [BrandCode]    NVARCHAR (600) NULL,
    [MediaId]      INT            NULL,
    [WebsiteLink]  NVARCHAR (500) NULL,
    [DisplayOrder] INT            NULL,
    [IsActive]     BIT            CONSTRAINT [DF_ZnodePimManufacturerBrand_ActiveInd] DEFAULT ((1)) NOT NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [ModifiedBy]   INT            NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    [Custom1]      NVARCHAR (MAX) NULL,
    [Custom2]      NVARCHAR (MAX) NULL,
    [Custom3]      NVARCHAR (MAX) NULL,
    [Custom4]      NVARCHAR (MAX) NULL,
    [Custom5]      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ZnodePimManufacturerBrand] PRIMARY KEY CLUSTERED ([BrandId] ASC)
);









