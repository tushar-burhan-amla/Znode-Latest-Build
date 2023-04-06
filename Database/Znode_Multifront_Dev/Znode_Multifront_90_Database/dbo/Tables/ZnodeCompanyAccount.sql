CREATE TABLE [dbo].[ZnodeCompanyAccount] (
    [CompanyAccountId]       INT             IDENTITY (1, 1) NOT NULL,
    [ParentCompanyAccountId] INT             NULL,
    [Name]                   NCHAR (10)      NULL,
    [Desription]             NCHAR (10)      NULL,
    [BudgetAmount]           NUMERIC (12, 6) NULL,
    [IsActive]               BIT             NULL,
    [ExternalId]             NVARCHAR (50)   NULL,
    [CreatedBy]              INT             NOT NULL,
    [CreatedDate]            DATETIME        NOT NULL,
    [ModifiedBy]             INT             NOT NULL,
    [ModifiedDate]           DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeCompanyAccount] PRIMARY KEY CLUSTERED ([CompanyAccountId] ASC)
);

