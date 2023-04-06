CREATE TABLE [dbo].[ZnodePortalUnit] (
    [PortalUnitId]   INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]       INT            NULL,
    [CurrencyId]     INT            NULL,
    [WeightUnit]     NVARCHAR (MAX) NULL,
    [DimensionUnit]  NVARCHAR (MAX) NULL,
    [CurrencySuffix] VARCHAR (1000) NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     INT            NOT NULL,
    [ModifiedDate]   DATETIME       NOT NULL,
    [CultureId]      INT            NULL,
    CONSTRAINT [PK_ZnodePortalUnit] PRIMARY KEY CLUSTERED ([PortalUnitId] ASC),
    CONSTRAINT [FK_ZnodePortalUnit_ZnodeCulture] FOREIGN KEY ([CultureId]) REFERENCES [dbo].[ZnodeCulture] ([CultureId]),
    CONSTRAINT [FK_ZnodePortalUnit_ZnodeCurrency] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[ZnodeCurrency] ([CurrencyId]),
    CONSTRAINT [FK_ZnodePortalUnit_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);







