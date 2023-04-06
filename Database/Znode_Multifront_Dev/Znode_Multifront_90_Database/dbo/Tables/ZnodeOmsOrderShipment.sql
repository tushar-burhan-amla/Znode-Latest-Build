CREATE TABLE [dbo].[ZnodeOmsOrderShipment] (
    [OmsOrderShipmentId] INT             IDENTITY (1, 1) NOT NULL,
    [ShipName]           VARCHAR (100)   NOT NULL,
    [ShipToFirstName]    VARCHAR (100)   NULL,
    [ShipToLastName]     VARCHAR (100)   NULL,
    [ShipToCompanyName]  VARCHAR (100)   NULL,
    [ShipToStreet1]      VARCHAR (300)   NULL,
    [ShipToStreet2]      VARCHAR (300)   NULL,
    [ShipToCity]         VARCHAR (50)    NULL,
    [ShipToStateCode]    NVARCHAR (255)  NULL,
    [ShipToPostalCode]   VARCHAR (10)    NULL,
    [ShipToCountry]      VARCHAR (10)    NULL,
    [ShipToPhoneNumber]  VARCHAR (30)    NULL,
    [ShipToEmailId]      VARCHAR (50)    NULL,
    [ShippingId]         INT             NULL,
    [AddressId]          INT             NULL,
    [CreatedBy]          INT             NOT NULL,
    [CreatedDate]        DATETIME        NOT NULL,
    [ModifiedBy]         INT             NOT NULL,
    [ModifiedDate]       DATETIME        NOT NULL,
    [DisplayName]        NVARCHAR (1200) NULL,
    CONSTRAINT [PK_ZnodeOmsOrderShipment] PRIMARY KEY CLUSTERED ([OmsOrderShipmentId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsOrderShipment_ZnodeShipping] FOREIGN KEY ([ShippingId]) REFERENCES [dbo].[ZnodeShipping] ([ShippingId])
);







