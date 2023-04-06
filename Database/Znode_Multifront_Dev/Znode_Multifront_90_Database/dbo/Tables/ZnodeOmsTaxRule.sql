CREATE TABLE [dbo].[ZnodeOmsTaxRule](
	[OmsTaxRuleId] [int] IDENTITY(1,1) NOT NULL,
	[OmsOrderId] [int] NOT NULL,
	[TaxRuleId] [int] NOT NULL,
	[TaxRuleTypeId] [int] NULL,
	[TaxClassId] [int] NULL,
	[DestinationCountryCode] [nvarchar](10) NULL,
	[DestinationStateCode] [nvarchar](255) NULL,
	[CountyFIPS] [nvarchar](50) NULL,
	[Precedence] [int] NOT NULL,
	[SalesTax] [decimal](18, 5) NULL,
	[VAT] [decimal](18, 2) NULL,
	[GST] [decimal](18, 2) NULL,
	[PST] [decimal](18, 2) NULL,
	[HST] [decimal](18, 2) NULL,
	[TaxShipping] [bit] NOT NULL,
	[Custom1] [nvarchar](max) NULL,
	[Custom2] [nvarchar](max) NULL,
	[Custom3] [nvarchar](max) NULL,
	[ExternalID] [varchar](50) NULL,
	[ZipCode] [nvarchar](max) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
    [AvataxIsSellerImporterOfRecord] [Bit] NULL, 
    CONSTRAINT [PK_ZnodeOmsTaxRule] PRIMARY KEY CLUSTERED ([OmsTaxRuleId] ASC), 
 CONSTRAINT [FK_ZnodeOmsTaxRule_ZnodeOmsOrder] FOREIGN KEY([OmsOrderId]) REFERENCES [dbo].[ZnodeOmsOrder] ([OmsOrderId])
 )
GO



