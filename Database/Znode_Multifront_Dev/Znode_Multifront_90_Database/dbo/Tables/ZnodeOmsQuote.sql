CREATE TABLE [dbo].[ZnodeOmsQuote] (
    [OmsQuoteId]              INT             IDENTITY (1, 1) NOT NULL,
    [PortalId]                INT             NOT NULL,
    [UserId]                  INT             NOT NULL,
    [OmsOrderStateId]         INT             NOT NULL,
    [ShippingId]              INT             NULL,
    [ShippingAddressId]       INT             NULL,
    [BillingAddressId]        INT             NULL,
    [ApproverUserId]          INT             NULL,
    [AdditionalInstruction]   NVARCHAR (500)  NULL,
    [QuoteOrderTotal]         NUMERIC (28, 6) NULL,
    [CreatedBy]               INT             NOT NULL,
    [CreatedDate]             DATETIME        NOT NULL,
    [ModifiedBy]              INT             NOT NULL,
    [ModifiedDate]            DATETIME        NOT NULL,
    [PaymentSettingId]        INT             NULL,
    [IsPendingPayment]        BIT             DEFAULT ((0)) NOT NULL,
    [CardType]                VARCHAR (50)    NULL,
    [CreditCardExpMonth]      INT             NULL,
    [CreditCardExpYear]       INT             NULL,
    [PaymentTransactionToken] NVARCHAR (300)  NULL,
    [CreditCardNumber]        VARCHAR (4)     NULL,
    [PoDocument]              NVARCHAR (300)  NULL,
    [PurchaseOrderNumber]     NVARCHAR (500)  NULL,
    [IsConvertedToOrder]      BIT             NULL,
    [TaxCost]                 NUMERIC (28, 8) NULL,
    [ShippingCost]            NUMERIC (28, 8) NULL,
    [OmsQuoteTypeId]          INT             NULL,
    [QuoteTypeCode]           VARCHAR (300)   NULL,
    [PublishStateId]          TINYINT         NULL,
    [QuoteExpirationDate]     DATETIME        NULL,
    [InHandDate]              DATETIME        NULL,
    [QuoteNumber]             VARCHAR (200)   NULL,
    [ShippingTypeId]          INT             NULL,
    [AccountNumber]           VARCHAR (200)   NULL,
    [ShippingMethod]          VARCHAR (200)   NULL,
	[CultureCode]             VARCHAR (100)   NULL,
	[JobName]                 nvarchar(200) NULL,
	[ShippingConstraintCode]  nvarchar(200) NULL,
	[IsTaxExempt]             Bit NULL,
	[ShippingHandlingCharges] NUMERIC (28, 6) NULL,
    [Custom1] nvarchar(max),
    [Custom2] nvarchar(max),
    [Custom3] nvarchar(max),
    [Custom4] nvarchar(max),
    [Custom5] nvarchar(max),
    [FirstName] VARCHAR(100),
    [MiddleName] VARCHAR(100), 
    [LastName] VARCHAR(100), 
    [Email] VARCHAR(50),
    [PhoneNumber] VARCHAR(50),
	[IsOldQuote] BIT  NULL,
    [ImportDuty] NUMERIC(28,6) NULL,
	[SubTotal] NUMERIC(28,6) NULL,
	[DiscountAmount] NUMERIC(28,6) NULL,
	[ShippingDiscount] NUMERIC(28,6) NULL,
    [AccountId] INT NULL, 
    CONSTRAINT [PK_ZnodeOmsQuote_1] PRIMARY KEY CLUSTERED ([OmsQuoteId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsOrder_ZnodePaymentsetting] FOREIGN KEY ([PaymentSettingId]) REFERENCES [dbo].[ZnodePaymentSetting] ([PaymentSettingId]),
    CONSTRAINT [FK_ZnodeOmsQuote_ZnodeOmsOrderState] FOREIGN KEY ([OmsOrderStateId]) REFERENCES [dbo].[ZnodeOmsOrderState] ([OmsOrderStateId]),
    CONSTRAINT [FK_ZnodeOmsQuote_ZnodeOmsQuoteType] FOREIGN KEY ([OmsQuoteTypeId]) REFERENCES [dbo].[ZnodeOmsQuoteType] ([OmsQuoteTypeId]),
    CONSTRAINT [FK_ZnodeOmsQuote_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId]),
    CONSTRAINT [FK_ZnodeOmsQuote_ZnodeShipping] FOREIGN KEY ([ShippingId]) REFERENCES [dbo].[ZnodeShipping] ([ShippingId]),
    CONSTRAINT [FK_ZnodeOmsQuote_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);

















