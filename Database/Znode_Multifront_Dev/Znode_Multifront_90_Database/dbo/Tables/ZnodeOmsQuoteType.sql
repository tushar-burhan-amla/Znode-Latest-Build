CREATE TABLE [dbo].[ZnodeOmsQuoteType] (
    [OmsQuoteTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [QuoteTypeName]  NVARCHAR (300) NULL,
    [QuoteTypeCode]  VARCHAR (300)  NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     INT            NOT NULL,
    [ModifiedDate]   DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeOmsQuoteType] PRIMARY KEY CLUSTERED ([OmsQuoteTypeId] ASC)
);

