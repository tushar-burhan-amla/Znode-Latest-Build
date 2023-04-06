CREATE TABLE [dbo].[ZnodeEmailProvider](
	[EmailProviderId] [int] IDENTITY(1,1) NOT NULL,
	[ProviderCode] [nvarchar](300) NULL,
	[ProviderName] [nvarchar](300) NULL,
	[ClassName] [nvarchar](300) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[Description] [nvarchar](max) NULL,
	CONSTRAINT [PK_ZnodeEmailProvider] PRIMARY KEY CLUSTERED (	[EmailProviderId] ASC)
) 