CREATE TABLE [dbo].[ZnodeAuthToken]
(
	[AuthTokenId] [int] IDENTITY(1,1) NOT NULL,
	[AuthToken] [nvarchar](200) CONSTRAINT [DF_ZnodeAuthToken_AuthToken] DEFAULT (newid()) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[TotalAttempt] [int] NULL,
	[UserOrSessionId] varchar(300), 
	[IsFromAdminApp] bit
    CONSTRAINT [PK_ZnodeAuthToken] PRIMARY KEY CLUSTERED (	[AuthTokenId] ASC ),
) 
GO
CREATE INDEX Idx_ZnodeAuthToken_AuthTokenId ON [ZnodeAuthToken] ([AuthToken])
GO
CREATE INDEX Idx_ZnodeAuthToken_UserOrSessionIdIsFromAdminApp ON [ZnodeAuthToken] ([UserOrSessionId] ,[IsFromAdminApp])
GO