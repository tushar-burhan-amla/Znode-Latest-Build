


CREATE TABLE [dbo].[ZnodeImportAttributeDefaultValue](
	[ImportdefaultAttributeValueId] [int] IDENTITY(1,1) NOT NULL,
	[ImportAttributeType] [nvarchar](300) NULL,
	[TargetAttributeCode] [nvarchar](300) NULL,
	[AllowAttributeValue] [nvarchar](500) NULL,
	[ReplacedAttributeValue] [nvarchar](300) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] int NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] int NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_ZnodeImportAttributeDefaultValue] PRIMARY KEY CLUSTERED 
(
	[ImportdefaultAttributeValueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
