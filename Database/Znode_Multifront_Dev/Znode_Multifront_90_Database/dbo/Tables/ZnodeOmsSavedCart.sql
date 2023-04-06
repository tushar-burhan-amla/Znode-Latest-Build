CREATE TABLE [dbo].[ZnodeOmsSavedCart] (
    [OmsSavedCartId]     INT      IDENTITY (1, 1) NOT NULL,
    [OmsCookieMappingId] INT      NOT NULL,
    [SalesTax]           MONEY    NULL,
    [RecurringSalesTax]  MONEY    NULL,
    [CreatedBy]          INT      NOT NULL,
    [CreatedDate]        DATETIME NOT NULL,
    [ModifiedBy]         INT      NOT NULL,
    [ModifiedDate]       DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeOmsSavedCart] PRIMARY KEY CLUSTERED ([OmsSavedCartId] ASC),
    CONSTRAINT [FK_ZnodeOmsSavedCart_ZnodeOmsCookieMapping] FOREIGN KEY ([OmsCookieMappingId]) REFERENCES [dbo].[ZnodeOmsCookieMapping] ([OmsCookieMappingId])
);

