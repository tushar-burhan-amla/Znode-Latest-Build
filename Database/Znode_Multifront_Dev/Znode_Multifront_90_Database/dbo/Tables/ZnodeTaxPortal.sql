CREATE TABLE [dbo].[ZnodeTaxPortal] (
    [TaxPortalId]              INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]                 INT            NOT NULL,
    [AvataxUrl]                NVARCHAR (100) NULL,
    [AvalaraAccount]           NVARCHAR (100) NULL,
    [AvalaraLicense]           NVARCHAR (100) NULL,
    [AvalaraCompanyCode]       NVARCHAR (100) NULL,
    [AvalaraFreightIdentifier] NVARCHAR (100) NULL,
    [STOCCHUrl]                NVARCHAR (50)  NULL,
    [EntityId]                 NVARCHAR (50)  NULL,
    [DivisionId]               NVARCHAR (50)  NULL,
    [FreightTaxGroupCode]      NVARCHAR (50)  NULL,
    [FreightTaxItemCode]       NVARCHAR (50)  NULL,
    [CreatedDate]              DATETIME       NOT NULL,
    [ModifiedDate]             DATETIME       NOT NULL,
    [CreatedBy]                INT            NOT NULL,
    [ModifiedBy]               INT            NOT NULL,
    [TaxRuleTypeId]            INT            NULL,
    [Custom1]                  NVARCHAR (MAX) NULL,
    [Custom2]                  NVARCHAR (MAX) NULL,
    [Custom3]                  NVARCHAR (MAX) NULL,
    [Custom4]                  NVARCHAR (MAX) NULL,
    [Custom5]                  NVARCHAR (MAX) NULL,
    [AvataxIsTaxIncluded] BIT,
    CONSTRAINT [PK_ZnodeTaxPortal] PRIMARY KEY CLUSTERED ([TaxPortalId] ASC),
    CONSTRAINT [FK_ZnodeTaxPortal_znodeportal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodeTaxPortal_ZnodeTaxRuleTypes] FOREIGN KEY ([TaxRuleTypeId]) REFERENCES [dbo].[ZnodeTaxRuleTypes] ([TaxRuleTypeId])
);









