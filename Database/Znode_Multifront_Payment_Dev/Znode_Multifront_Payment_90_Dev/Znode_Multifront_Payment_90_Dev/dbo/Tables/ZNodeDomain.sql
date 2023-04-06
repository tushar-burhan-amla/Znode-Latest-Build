CREATE TABLE [dbo].[ZNodeDomain] (
    [DomainId]     INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]     INT            NOT NULL,
    [DomainName]   NVARCHAR (100) NOT NULL,
    [IsActive]     BIT            CONSTRAINT [DF_ZNodeDomain_IsActive] DEFAULT ((1)) NOT NULL,
    [ApiKey]       NVARCHAR (200) CONSTRAINT [DF_ZNodeDomain_ApiKey] DEFAULT (CONVERT([nvarchar](200),newid(),(0))) NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    CONSTRAINT [PK_ZNodeDomain] PRIMARY KEY CLUSTERED ([DomainId] ASC)
);



