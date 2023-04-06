CREATE TABLE [dbo].[ZnodeStockNotice](
	[StockNoticeId] [int] IDENTITY(1,1) NOT NULL,
	[ParentSKU] [varchar](255) NOT NULL,
	[SKU] [varchar](255) NOT NULL,
	[EmailId] [varchar](255) NOT NULL,
	[Quantity] [numeric](18, 0) NULL,
	[PortalId] [int] NOT NULL,
	[CatalogId] [int] NOT NULL,
	[IsEmailSent] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StockNoticeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO