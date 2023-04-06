CREATE TABLE [dbo].[ZnodeShippingRegion] (
    [ShippingRegionId] INT             IDENTITY (1, 1) NOT NULL,
    [ShippingId]       INT             NOT NULL,
    [CountryCode]      VARCHAR (50)    NOT NULL,
    [StateCode]        VARCHAR (50)    NULL,
    [CityId]           INT             NULL,
    [HandlingCharge]   NUMERIC (12, 6) NOT NULL,
    [CreatedBy]        INT             NOT NULL,
    [CreatedDate]      DATETIME        NOT NULL,
    [ModifiedBy]       INT             NOT NULL,
    [ModifiedDate]     DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeShippingRegion] PRIMARY KEY CLUSTERED ([ShippingRegionId] ASC),
    CONSTRAINT [FK_ZnodeShippingRegion_ZnodeShipping] FOREIGN KEY ([ShippingId]) REFERENCES [dbo].[ZnodeShipping] ([ShippingId])
);



