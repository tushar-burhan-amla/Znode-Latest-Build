CREATE TABLE [dbo].[ZNodePaymentSetting] (
    [PaymentSettingId]        INT           IDENTITY (1, 1) NOT NULL,
    [PaymentTypeId]           INT           NOT NULL,
    [PaymentGatewayId]        INT           NULL,
    [EnableVisa]              BIT           NULL,
    [EnableMasterCard]        BIT           NULL,
    [EnableAmex]              BIT           NULL,
    [EnableDiscover]          BIT           NULL,
    [EnableRecurringPayments] BIT           CONSTRAINT [DF_ZNodePaymentSetting_EnableRecurringPayments] DEFAULT ((0)) NULL,
    [EnableVault]             BIT           CONSTRAINT [DF_ZNodePaymentSetting_EnableVault] DEFAULT ((0)) NULL,
    [IsActive]                BIT           NOT NULL,
    [DisplayOrder]            INT           NOT NULL,
    [PreAuthorize]            BIT           CONSTRAINT [DF_ZNodePaymentSetting_PreAuthorize] DEFAULT ((0)) NOT NULL,
    [IsRMACompatible]         BIT           NULL,
    [TestMode]                BIT           CONSTRAINT [DF_ZNodePaymentSetting_TestMode] DEFAULT ((0)) NOT NULL,
    [CreatedDate]             DATETIME      NOT NULL,
    [ModifiedDate]            DATETIME      NOT NULL,
    [EnablePODocUpload]       BIT           CONSTRAINT [DF_ZNodePaymentSetting_EnablePODocUpload] DEFAULT ((0)) NOT NULL,
    [IsPODocRequired]         BIT           CONSTRAINT [DF_ZNodePaymentSetting_IsPODocRequired] DEFAULT ((0)) NOT NULL,
    [PaymentCode]             VARCHAR (200) NULL,
    CONSTRAINT [PK_SC_PaymentSetting] PRIMARY KEY CLUSTERED ([PaymentSettingId] ASC),
    CONSTRAINT [FK_SC_PaymentSetting_SC_Gateway] FOREIGN KEY ([PaymentGatewayId]) REFERENCES [dbo].[ZNodePaymentGateway] ([PaymentGatewayId]),
    CONSTRAINT [FK_ZNodePaymentSetting_ZNodePaymentType] FOREIGN KEY ([PaymentTypeId]) REFERENCES [dbo].[ZNodePaymentType] ([PaymentTypeId])
);









