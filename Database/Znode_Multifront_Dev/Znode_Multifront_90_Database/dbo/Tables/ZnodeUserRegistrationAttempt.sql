
CREATE TABLE [dbo].[ZnodeUserRegistrationAttempt](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
	[PortalId] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ZnodeUserRegistrationAttempt] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


