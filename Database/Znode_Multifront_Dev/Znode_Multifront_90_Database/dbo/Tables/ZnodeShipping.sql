CREATE TABLE [dbo].[ZnodeShipping] (
    [ShippingId]             INT             IDENTITY (1, 1) NOT NULL,
    [ShippingTypeId]         INT             NOT NULL,
    [CurrencyId]             INT             NULL,
    [ShippingCode]           NVARCHAR (MAX)  NOT NULL,
    [ShippingName]           NVARCHAR (200)  NULL,
    [HandlingCharge]         NUMERIC (28, 6) NOT NULL,
    [HandlingChargeBasedOn]  NVARCHAR (50)   NULL,
    [DestinationCountryCode] NVARCHAR (10)   NULL,
    [StateCode]              NVARCHAR (255)  NULL,
    [CountyFIPS]             NVARCHAR (50)   NULL,
    [TrackingUrl]            NVARCHAR (2000) NULL,
    [Description]            NVARCHAR (MAX)  NOT NULL,
    [IsActive]               BIT             NOT NULL,
    [DisplayOrder]           INT             NOT NULL,
    [ZipCode]                NVARCHAR (MAX)  NULL,
    [CreatedBy]              INT             NOT NULL,
    [CreatedDate]            DATETIME        NOT NULL,
    [ModifiedBy]             INT             NOT NULL,
    [ModifiedDate]           DATETIME        NOT NULL,
    [DeliveryTimeframe]      VARCHAR (MAX)   NULL,
    [CultureId]              INT             NULL,
    CONSTRAINT [PK_ZNodeShipping] PRIMARY KEY CLUSTERED ([ShippingId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeShipping_ZnodeCulture] FOREIGN KEY ([CultureId]) REFERENCES [dbo].[ZnodeCulture] ([CultureId]),
    CONSTRAINT [FK_ZnodeShipping_ZnodeCurrency] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[ZnodeCurrency] ([CurrencyId]),
    CONSTRAINT [FK_ZnodeShipping_ZnodeShippingTypes] FOREIGN KEY ([ShippingTypeId]) REFERENCES [dbo].[ZnodeShippingTypes] ([ShippingTypeId])
);





















