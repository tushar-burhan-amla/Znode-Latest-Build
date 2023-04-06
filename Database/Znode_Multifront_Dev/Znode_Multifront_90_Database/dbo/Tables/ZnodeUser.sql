CREATE TABLE [dbo].[ZnodeUser] (
    [UserId]                   INT             IDENTITY (1, 1) NOT NULL,
    [AccountId]                INT             NULL,
    [AspNetUserId]             NVARCHAR (128)  NULL,
    [FirstName]                VARCHAR (100)   NULL,
    [LastName]                 VARCHAR (100)   NULL,
    [MiddleName]               VARCHAR (100)   NULL,
    [CustomerPaymentGUID]      NVARCHAR (1000) NULL,
    [BudgetAmount]             NUMERIC (28, 6) NULL,
    [Email]                    VARCHAR (50)    NULL,
    [PhoneNumber]              VARCHAR (50)    NULL,
    [EmailOptIn]               BIT             CONSTRAINT [DF_ZnodeUser_EmailOptIn] DEFAULT ((0)) NOT NULL,
    [ReferralStatus]           VARCHAR (20)    NULL,
    [ReferralCommission]       NUMERIC (28, 6) NULL,
    [ReferralCommissionTypeId] INT             NULL,
    [IsActive]                 BIT             CONSTRAINT [DF_ZnodeUser_IsActive] DEFAULT ((0)) NOT NULL,
    [ExternalId]               NVARCHAR (1000) NULL,
    [IsShowMessage]            BIT             NULL,
    [CreatedBy]                INT             NOT NULL,
    [CreatedDate]              DATETIME        NOT NULL,
    [ModifiedBy]               INT             NOT NULL,
    [ModifiedDate]             DATETIME        NOT NULL,
    [Custom1]                  NVARCHAR (MAX)  NULL,
    [Custom2]                  NVARCHAR (MAX)  NULL,
    [Custom3]                  NVARCHAR (MAX)  NULL,
    [Custom4]                  NVARCHAR (MAX)  NULL,
    [Custom5]                  NVARCHAR (MAX)  NULL,
    [IsVerified]               BIT             CONSTRAINT [DF_ZnodeUser_IsVerified] DEFAULT ((1)) NOT NULL,
    [MediaId]                  INT             NULL,
	[UserVerificationType]     VARCHAR (50)    NULL,
    [UserName]                 NVARCHAR (512)  NULL,
    [SMSOptIn]               BIT             DEFAULT ((0)) NOT NULL,                 
    CONSTRAINT [PK_ZnodeUser] PRIMARY KEY CLUSTERED ([UserId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeUser_AspNetUsers] FOREIGN KEY ([AspNetUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_ZnodeUser_ZnodeAccount] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[ZnodeAccount] ([AccountId]),
    CONSTRAINT [FK_ZnodeUser_ZnodeReferralCommissionType] FOREIGN KEY ([ReferralCommissionTypeId]) REFERENCES [dbo].[ZnodeReferralCommissionType] ([ReferralCommissionTypeId])
);




















GO
CREATE NONCLUSTERED INDEX [IX_ZnodeUser_AspNetUserId]
    ON [dbo].[ZnodeUser]([AspNetUserId] ASC);

