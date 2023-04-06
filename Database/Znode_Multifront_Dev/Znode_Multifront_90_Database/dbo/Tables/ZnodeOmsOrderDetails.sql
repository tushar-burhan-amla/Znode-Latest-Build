CREATE TABLE [dbo].[ZnodeOmsOrderDetails] (
    [OmsOrderDetailsId]       INT             IDENTITY (1, 1) NOT NULL,
    [OmsOrderId]              INT             NOT NULL,
    [PortalId]                INT             NOT NULL,
    [UserId]                  INT             NOT NULL,
    [OrderDate]               DATETIME        NOT NULL,
    [OmsOrderStateId]         INT             NOT NULL,
    [ShippingId]              INT             NULL,
    [PaymentTypeId]           INT             NULL,
    [BillingFirstName]        NVARCHAR (100)  NULL,
    [BillingLastName]         NVARCHAR (100)  NULL,
    [BillingCompanyName]      NVARCHAR (100)  NULL,
    [BillingStreet1]          NVARCHAR (200)  NULL,
    [BillingStreet2]          NVARCHAR (200)  NULL,
    [BillingCity]             NVARCHAR (100)  NULL,
    [BillingStateCode]        NVARCHAR (255)  NULL,
    [BillingPostalCode]       NVARCHAR (30)   NULL,
    [BillingCountry]          NVARCHAR (10)   NULL,
    [BillingPhoneNumber]      NVARCHAR (100)  NULL,
    [BillingEmailId]          NVARCHAR (100)  NULL,
    [TaxCost]                 NUMERIC (28, 6) NULL,
    [ShippingCost]            NUMERIC (28, 6) NULL,
    [SubTotal]                NUMERIC (28, 6) NULL,
    [DiscountAmount]          NUMERIC (28, 6) NULL,
    [CurrencyCode]            VARCHAR (100)   NULL,
    [OverDueAmount]           NUMERIC (28, 6) NULL,
    [Total]                   NUMERIC (28, 6) NULL,
    [ShippingNumber]          NVARCHAR (MAX)  NULL,
    [TrackingNumber]          NVARCHAR (500)  NULL,
    [CouponCode]              NVARCHAR (500)  NULL,
    [PromoDescription]        NVARCHAR (MAX)  NULL,
    [ReferralUserId]          INT             NULL,
    [PurchaseOrderNumber]     NVARCHAR (500)  NULL,
    [OmsPaymentStateId]       INT             NULL,
    [WebServiceDownloadDate]  DATETIME        NULL,
    [PaymentSettingId]        INT             NULL,
    [PaymentTransactionToken] NVARCHAR (300)  NULL,
    [ShipDate]                DATETIME        NULL,
    [ReturnDate]              DATETIME        NULL,
    [AddressId]               INT             NULL,
    [PoDocument]              NVARCHAR (300)  NULL,
    [IsActive]                BIT             CONSTRAINT [DF_ZnodeOmsOrderDetails_IsActive] DEFAULT ((1)) NOT NULL,
    [ExternalId]              NVARCHAR (500)  NULL,
    [CreatedBy]               INT             NOT NULL,
    [CreatedDate]             DATETIME        NOT NULL,
    [ModifiedBy]              INT             NOT NULL,
    [ModifiedDate]            DATETIME        NOT NULL,
    [CreditCardNumber]        VARCHAR (4)     NULL,
    [IsShippingCostEdited]    BIT             NULL,
    [IsTaxCostEdited]         BIT             NULL,
    [ShippingDifference]      NUMERIC (28, 6) NULL,
    [EstimateShippingCost]    NUMERIC (28, 6) NULL,
    [TransactionId]           NVARCHAR (400)  NULL,
    [Custom1]                 NVARCHAR (MAX)  NULL,
    [Custom2]                 NVARCHAR (MAX)  NULL,
    [Custom3]                 NVARCHAR (MAX)  NULL,
    [Custom4]                 NVARCHAR (MAX)  NULL,
    [Custom5]                 NVARCHAR (MAX)  NULL,
    [FirstName]               NVARCHAR (100)  NULL,
    [LastName]                NVARCHAR (100)  NULL,
    [CardType]                VARCHAR (50)    NULL,
    [CreditCardExpMonth]      INT             NULL,
    [CreditCardExpYear]       INT             NULL,
    [TotalAdditionalCost]     NUMERIC (28, 6) NULL,
    [PaymentDisplayName]      NVARCHAR (1200) NULL,
    [PaymentExternalId]       NVARCHAR (1000) NULL,
    [CultureCode]             VARCHAR (100)   NULL,
    [DisplayName]             NVARCHAR (1200) NULL,
    [InHandDate]              DATETIME        NULL,
    [IpAddress]               VARCHAR (100)   NULL,
    [JobName]                 NVARCHAR (100)  NULL,
    [ShippingConstraintCode]  NVARCHAR (50)   NULL,
    [ShippingDiscount]        NUMERIC (28, 6) NULL,
    [ShippingHandlingCharges] NUMERIC (28, 6) NULL,
    [ReturnCharges]           NUMERIC (28, 6) NULL,
	[IsCalculateTaxAfterDiscount] BIT  NULL,
    [Email]                   VARCHAR (50) NULL,
    [PhoneNumber]             VARCHAR (50) NULL,
	[OrderTotalWithoutVoucher]      NUMERIC (28, 6) NULL,
    [ImportDuty] NUMERIC(28,6) NULL,
	[IsPricesInclusiveOfTaxes] BIT NULL,
    [RemainingOrderAmount] NUMERIC(28, 6) NULL, 
    [AccountId] INT NULL, 
    CONSTRAINT [PK_ZnodeOmsOrderDetails] PRIMARY KEY CLUSTERED ([OmsOrderDetailsId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsOrderDetails_ZnodeOmsOrder] FOREIGN KEY ([OmsOrderId]) REFERENCES [dbo].[ZnodeOmsOrder] ([OmsOrderId]),
    CONSTRAINT [FK_ZnodeOmsOrderDetails_ZnodeOmsOrderState] FOREIGN KEY ([OmsOrderStateId]) REFERENCES [dbo].[ZnodeOmsOrderState] ([OmsOrderStateId]),
    CONSTRAINT [FK_ZnodeOmsOrderDetails_ZnodeOmsPaymentState] FOREIGN KEY ([OmsPaymentStateId]) REFERENCES [dbo].[ZnodeOmsPaymentState] ([OmsPaymentStateId]),
    CONSTRAINT [FK_ZnodeOmsOrderDetails_ZnodeOmsPaymentStatus] FOREIGN KEY ([OmsPaymentStateId]) REFERENCES [dbo].[ZnodeOmsPaymentState] ([OmsPaymentStateId]),
    CONSTRAINT [FK_ZnodeOmsOrderDetails_ZnodePaymentSetting] FOREIGN KEY ([PaymentSettingId]) REFERENCES [dbo].[ZnodePaymentSetting] ([PaymentSettingId]),
    CONSTRAINT [FK_ZnodeOmsOrderDetails_ZnodePaymentType] FOREIGN KEY ([PaymentTypeId]) REFERENCES [dbo].[ZnodePaymentType] ([PaymentTypeId]),
    CONSTRAINT [FK_ZnodeOmsOrderDetails_ZnodeShipping] FOREIGN KEY ([ShippingId]) REFERENCES [dbo].[ZnodeShipping] ([ShippingId]),
    CONSTRAINT [FK_ZnodeOmsOrderDetails_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);











































