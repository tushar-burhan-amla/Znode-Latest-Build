CREATE TABLE [dbo].[ZnodeProfilePaymentSetting] (
    [ProfilePaymentSettingId] INT      IDENTITY (1, 1) NOT NULL,
    [PaymentSettingId]        INT      NULL,
    [ProfileId]               INT      NULL,
    [CreatedBy]               INT      NOT NULL,
    [CreatedDate]             DATETIME NOT NULL,
    [ModifiedBy]              INT      NOT NULL,
    [ModifiedDate]            DATETIME NOT NULL,
    [DisplayOrder]            INT      NULL,
    [PublishStateId]          TINYINT  NULL,
    CONSTRAINT [PK_ZnodeProfilePaymentSetting] PRIMARY KEY CLUSTERED ([ProfilePaymentSettingId] ASC),
    CONSTRAINT [FK_ZnodeProfilePaymentSetting_ZnodePaymentSetting] FOREIGN KEY ([PaymentSettingId]) REFERENCES [dbo].[ZnodePaymentSetting] ([PaymentSettingId]),
    CONSTRAINT [FK_ZnodeProfilePaymentSetting_Znodeprofile] FOREIGN KEY ([ProfileId]) REFERENCES [dbo].[ZnodeProfile] ([ProfileId]),
    CONSTRAINT [FK_ZnodeProfilePaymentSetting_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId])
);





