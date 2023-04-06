CREATE TABLE [dbo].[ZnodeOmsTemplatePersonalizeCartItem](
	[OmsTemplatePersonalizeCartItemId] [int] IDENTITY(1,1) NOT NULL,
	[OmsTemplateLineItemId] [int] NULL,
	[PersonalizeCode] [nchar](600) NULL,
	[PersonalizeValue] [nvarchar](max) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[DesignId] [nvarchar](2000) NULL,
	[ThumbnailURL] [nvarchar](max) NULL,
	[PersonalizeName] [nvarchar](600) NULL,
	CONSTRAINT [PK_ZnodeOmsTemplatePersonalizeCartItem_OmsTemplatePersonalizeCartItemId] PRIMARY KEY CLUSTERED 
	(
		[OmsTemplatePersonalizeCartItemId] ASC
	),
	CONSTRAINT [FK_ZnodeOmsTemplatePersonalizeCartItem_ZnodeOmsTemplateLineItem] FOREIGN KEY([OmsTemplateLineItemId])
	REFERENCES [dbo].[ZnodeOmsTemplateLineItem] ([OmsTemplateLineItemId])
) 
GO
