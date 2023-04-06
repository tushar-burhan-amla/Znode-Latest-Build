CREATE TABLE [dbo].[ZnodeOmsFailedOrderPayments](
	[OmsFailedOrderPaymentId] [int] IDENTITY(1,1) NOT NULL,
	[PaymentCode] [nvarchar](200) NOT NULL,
	[PaymentDisplayName] [nvarchar](1200) NULL,
	[TransactionToken] [nvarchar](300) NOT NULL,
	[TotalAmount] [decimal](28, 6) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[UserId] [int] NOT NULL,
	[Email] [nvarchar](100) NULL,
	[PaymentSettingId] [int] NOT NULL,
	[OrderNumber] [nvarchar](200) NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[PaymentStatusId] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ZnodeOmsFailedOrderPayments] PRIMARY KEY CLUSTERED 
(
	[OmsFailedOrderPaymentId] ASC
)
)
GO