CREATE TABLE [dbo].[ZnodeDomain] (
    [DomainId]        INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]        INT            NOT NULL,
    [DomainName]      NVARCHAR (100) NOT NULL,
    [IsActive]        BIT            NOT NULL,
    [ApiKey]          NVARCHAR (200) CONSTRAINT [DF_ZNodeDomain_ApiKey] DEFAULT (CONVERT([nvarchar](200),newid(),(0))) NULL,
    [ApplicationType] VARCHAR (300)  NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    [IsDefault]       BIT            CONSTRAINT [DF_ZnodeDomain_IsDefault] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ZNodeDomain] PRIMARY KEY CLUSTERED ([DomainId] ASC),
    CONSTRAINT [FK_ZnodeActivityLog_ZnodeDomain] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [UC_ZnodeDomain_DomainName] UNIQUE NONCLUSTERED ([DomainName] ASC)
);












GO

CREATE TRIGGER [dbo].[ZNodeDomain_AspNet_SqlCacheNotification_Trigger] ON [dbo].[ZNodeDomain]
                       FOR INSERT, UPDATE, DELETE AS BEGIN
                       SET NOCOUNT ON
                       EXEC dbo.AspNet_SqlCacheUpdateChangeIdStoredProcedure N'ZNodeDomain'
                       END

