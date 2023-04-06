CREATE TABLE [dbo].[ZnodeOmsCustomerShipping] (
    [OmsCustomerShippingId] INT             IDENTITY (1, 1) NOT NULL,
    [OmsOrderDetailsId]     INT             NULL,
    [UserId]                INT             NULL,
    [ShippingTypeId]        INT             NULL,
    [AccountNumber]         NVARCHAR (2000) NULL,
    [ShippingMethod]        NVARCHAR (2000) NULL,
    [CreatedBy]             INT             NOT NULL,
    [CreatedDate]           DATETIME        NOT NULL,
    [ModifiedBy]            INT             NOT NULL,
    [ModifiedDate]          DATETIME        NOT NULL,
    PRIMARY KEY CLUSTERED ([OmsCustomerShippingId] ASC),
    CONSTRAINT [FK_ZnodeOmsCustomerShipping_ZnodeOmsOrderDetails] FOREIGN KEY ([OmsOrderDetailsId]) REFERENCES [dbo].[ZnodeOmsOrderDetails] ([OmsOrderDetailsId]),
    CONSTRAINT [FK_ZnodeOmsCustomerShipping_ZnodeShippingTypes] FOREIGN KEY ([ShippingTypeId]) REFERENCES [dbo].[ZnodeShippingTypes] ([ShippingTypeId]),
    CONSTRAINT [FK_ZnodeOmsCustomerShipping_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);

