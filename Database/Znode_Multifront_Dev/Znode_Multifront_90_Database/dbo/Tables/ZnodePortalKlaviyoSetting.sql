CREATE TABLE [dbo].[ZnodePortalKlaviyoSetting](
	[PortalKlaviyoSettingId] [int] IDENTITY(1,1) NOT NULL,
	[KlaviyoCode] [nvarchar](100) NULL,
	[PortalId] [int] NOT NULL,
	[UserName] [varchar](500) NULL,
	[Password] [varchar](500) NULL,
	[PublicApiKey] [varchar](500) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ZnodePortalKlaviyoSetting] PRIMARY KEY CLUSTERED 
(
	[PortalKlaviyoSettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZnodePortalKlaviyoSetting]  WITH CHECK ADD  CONSTRAINT [FK_ZnodePortalKlaviyoSetting_ZnodePortal] FOREIGN KEY([PortalId])
REFERENCES [dbo].[ZnodePortal] ([PortalId])
GO


