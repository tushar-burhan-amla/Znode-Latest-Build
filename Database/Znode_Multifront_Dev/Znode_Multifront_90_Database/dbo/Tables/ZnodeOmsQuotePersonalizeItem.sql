CREATE TABLE [dbo].[ZnodeOmsQuotePersonalizeItem] (
    [OmsQuotePersonalizeItemId] INT             IDENTITY (1, 1) NOT NULL,
    [OmsQuoteLineItemId]        INT             NOT NULL,
    [PersonalizeCode]           NVARCHAR (200)  NOT NULL,
    [PersonalizeValue]          NVARCHAR (MAX)  NOT NULL,
    [CreatedBy]                 INT             NOT NULL,
    [CreatedDate]               DATETIME        NOT NULL,
    [ModifiedBy]                INT             NOT NULL,
    [ModifiedDate]              DATETIME        NOT NULL,
    [DesignId]                  NVARCHAR (2000) NULL,
    [ThumbnailURL]              NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_ZnodeOmsQuotePersonalizeItem] PRIMARY KEY CLUSTERED ([OmsQuotePersonalizeItemId] ASC)
);



