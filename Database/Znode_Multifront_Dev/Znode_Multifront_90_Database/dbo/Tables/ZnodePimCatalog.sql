CREATE TABLE [dbo].[ZnodePimCatalog] (
    [PimCatalogId]    INT            IDENTITY (1, 1) NOT NULL,
    [CatalogName]     NVARCHAR (MAX) NOT NULL,
    [IsActive]        BIT            CONSTRAINT [DF_ZnodePimCatalog_IsActive] DEFAULT ((1)) NOT NULL,
    [ExternalId]      VARCHAR (50)   NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    [PortalId]        INT            NULL,
    [IsAllowIndexing] BIT            NULL,
	[CatalogCode] nvarchar(100)      NULL,
    CONSTRAINT [PK_ZnodePimCatalog] PRIMARY KEY CLUSTERED ([PimCatalogId] ASC)
);



