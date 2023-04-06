CREATE TABLE [dbo].[ZnodeOmsPaymentState] (
    [OmsPaymentStateId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (MAX) NOT NULL,
    [Description]       NVARCHAR (MAX) NOT NULL,
    [CreatedBy]         INT            NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL,
    [ModifiedBy]        INT            NOT NULL,
    [ModifiedDate]      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeOmsPaymentStatus] PRIMARY KEY CLUSTERED ([OmsPaymentStateId] ASC)
);

