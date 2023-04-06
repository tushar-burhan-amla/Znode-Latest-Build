CREATE TABLE [dbo].[ZnodeAccount] (
    [AccountId]        INT             IDENTITY (1, 1) NOT NULL,
    [ParentAccountId]  INT             NULL,
    [Name]             NVARCHAR (200)  NULL,
    [Description]       NVARCHAR (100)  NULL,
    [BudgetAmount]     NUMERIC (28, 6) NULL,
    [IsActive]         BIT             NULL,
    [ExternalId]       NVARCHAR (1000) NULL,
    [PublishCatalogId] INT             NULL,
    [CreatedBy]        INT             NOT NULL,
    [CreatedDate]      DATETIME        NOT NULL,
    [ModifiedBy]       INT             NOT NULL,
    [ModifiedDate]     DATETIME        NOT NULL,
	[AccountCode]      NVARCHAR(100) NULL,
	[SalesRepId] Int NULL,
    CONSTRAINT [PK_ZnodeAccount] PRIMARY KEY CLUSTERED ([AccountId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeAccount_ZnodeAccount] FOREIGN KEY ([ParentAccountId]) REFERENCES [dbo].[ZnodeAccount] ([AccountId])
);


GO
CREATE NONCLUSTERED INDEX NC_Idx_ZnodeAccount_ParentAccountId ON ZnodeAccount (ParentAccountId);
















