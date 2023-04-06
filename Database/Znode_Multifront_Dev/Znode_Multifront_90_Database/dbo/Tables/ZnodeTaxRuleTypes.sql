CREATE TABLE [dbo].[ZnodeTaxRuleTypes] (
    [TaxRuleTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]      INT            NULL,
    [ClassName]     NVARCHAR (50)  NULL,
    [Name]          NVARCHAR (MAX) NULL,
    [Description]   NVARCHAR (MAX) NULL,
    [IsActive]      BIT            NOT NULL,
    [CreatedBy]     INT            NOT NULL,
    [CreatedDate]   DATETIME       NOT NULL,
    [ModifiedBy]    INT            NOT NULL,
    [ModifiedDate]  DATETIME       NOT NULL,
    CONSTRAINT [PK_ZNodeTaxRuleType] PRIMARY KEY CLUSTERED ([TaxRuleTypeId] ASC),
    CONSTRAINT [FK_ZnodeTaxRuleType_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);

