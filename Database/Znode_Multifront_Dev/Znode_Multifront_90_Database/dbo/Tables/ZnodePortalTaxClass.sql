CREATE TABLE [dbo].[ZnodePortalTaxClass] (
    [PortalTaxClassId] INT      IDENTITY (1, 1) NOT NULL,
    [PortalId]         INT      NULL,
    [TaxClassId]       INT      NULL,
    [IsDefault]        BIT      CONSTRAINT [Df_ZnodePortalTaxClass] DEFAULT ((0)) NULL,
    [CreatedBy]        INT      NOT NULL,
    [CreatedDate]      DATETIME NOT NULL,
    [ModifiedBy]       INT      NOT NULL,
    [ModifiedDate]     DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePortalTaxClass] PRIMARY KEY CLUSTERED ([PortalTaxClassId] ASC),
    CONSTRAINT [FK_ZnodePortalTaxClass_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodePortalTaxClass_ZnodeTaxClass] FOREIGN KEY ([TaxClassId]) REFERENCES [dbo].[ZnodeTaxClass] ([TaxClassId])
);

