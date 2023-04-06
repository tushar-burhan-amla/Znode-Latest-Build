CREATE TABLE [dbo].[ZnodePaymentSetting] (
    [PaymentSettingId]            INT             IDENTITY (1, 1) NOT NULL,
    [PaymentApplicationSettingId] INT             NOT NULL,
    [PaymentTypeId]               INT             NOT NULL,
    [PaymentGatewayId]            INT             NULL,
    [PaymentName]                 VARCHAR (600)   NULL,
    [IsActive]                    BIT             NOT NULL,
    [DisplayOrder]                INT             NOT NULL,
    [IsTestMode]                  BIT             NOT NULL,
    [IsPoDocUploadEnable]         BIT             CONSTRAINT [DF_ZnodePaymentSetting_IsPoDocUploadEnable] DEFAULT ((0)) NOT NULL,
    [IsPoDocRequire]              BIT             CONSTRAINT [DF_ZnodePaymentSetting_IsPoDocRequire] DEFAULT ((0)) NOT NULL,
    [CreatedBy]                   INT             NOT NULL,
    [CreatedDate]                 DATETIME        NOT NULL,
    [ModifiedBy]                  INT             NOT NULL,
    [ModifiedDate]                DATETIME        NOT NULL,
    [PaymentDisplayName]          NVARCHAR (1200) NULL,
    [IsCaptureDisable]            BIT             DEFAULT ((0)) NOT NULL,
    [PaymentCode]                 VARCHAR (200)   NULL,
    [IsBillingAddressOptional]    BIT             CONSTRAINT [DF_ZnodePaymentSetting_IsBillingAddressOptional] DEFAULT ((0)) NOT NULL,
    [IsOABRequired]               BIT             CONSTRAINT [DF_ZnodePaymentSetting_IsOABRequired] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ZnodePaymentSetting] PRIMARY KEY CLUSTERED ([PaymentSettingId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePaymentSetting_ZnodePaymentGateway] FOREIGN KEY ([PaymentGatewayId]) REFERENCES [dbo].[ZnodePaymentGateway] ([PaymentGatewayId]),
    CONSTRAINT [FK_ZnodePaymentSetting_ZnodePaymentType] FOREIGN KEY ([PaymentTypeId]) REFERENCES [dbo].[ZnodePaymentType] ([PaymentTypeId])
);

















