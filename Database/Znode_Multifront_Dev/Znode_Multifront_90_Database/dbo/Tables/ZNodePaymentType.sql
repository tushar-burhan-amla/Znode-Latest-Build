CREATE TABLE [dbo].[ZnodePaymentType] (
    [PaymentTypeId]      INT            IDENTITY (1, 1) NOT NULL,
    [Code]               VARCHAR (100)  NULL,
    [Name]               VARCHAR (500)  NOT NULL,
    [Description]        NVARCHAR (MAX) NULL,
    [IsActive]           BIT            CONSTRAINT [DF_PaymentType_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedBy]          INT            NOT NULL,
    [CreatedDate]        DATETIME       NOT NULL,
    [ModifiedBy]         INT            NOT NULL,
    [ModifiedDate]       DATETIME       NOT NULL,
    [IsCallToPaymentAPI] BIT            DEFAULT ((0)) NOT NULL,
    [BehaviorType]      VARCHAR (100)  NULL,
    [IsUsedForOfflinePayment] BIT NULL DEFAULT ((0)), 
    CONSTRAINT [PK_ZNodePaymentType] PRIMARY KEY CLUSTERED ([PaymentTypeId] ASC) WITH (FILLFACTOR = 90)
);











