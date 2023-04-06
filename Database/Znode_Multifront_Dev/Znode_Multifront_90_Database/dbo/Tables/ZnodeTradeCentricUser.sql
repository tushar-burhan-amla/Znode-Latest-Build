CREATE TABLE [dbo].[ZnodeTradeCentricUser] (
	[TradeCentricUserId]   INT            IDENTITY(1,1) NOT NULL, 
    [OrganizationId]       NVARCHAR(50)   NULL, 
    [OrganizationName]     NVARCHAR(500)  NOT NULL, 
    [ReturnUrl]            NVARCHAR(500)  NOT NULL, 
    [UserId]               INT            NOT NULL, 
    [CreatedBy]            INT            NOT NULL, 
    [CreatedDate]          DATETIME       NOT NULL, 
    [ModifiedBy]           INT            NOT NULL, 
    [ModifiedDate]         DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeTradeCentricUser] PRIMARY KEY CLUSTERED ([TradeCentricUserId] ASC),
    CONSTRAINT [FK_ZnodeTradeCentricUser_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);
