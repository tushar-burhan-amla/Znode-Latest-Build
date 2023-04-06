CREATE TABLE [dbo].[ZnodeRmaRequest] (
    [RmaRequestId]       INT             IDENTITY (1, 1) NOT NULL,
    [RequestDate]        DATETIME        NOT NULL,
    [Comments]           NVARCHAR (MAX)  NULL,
    [RmaRequestStatusId] INT             NULL,
    [RequestNumber]      NVARCHAR (50)   NULL,
    [TaxCost]            NUMERIC (28, 6) NULL,
    [Discount]           NUMERIC (28, 6) NULL,
    [SubTotal]           NUMERIC (28, 6) NULL,
    [Total]              NUMERIC (28, 6) NULL,
    [CreatedBy]          INT             NOT NULL,
    [CreatedDate]        DATETIME        NOT NULL,
    [ModifiedBy]         INT             NOT NULL,
    [ModifiedDate]       DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeRmaRequest] PRIMARY KEY CLUSTERED ([RmaRequestId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeRmaRequest_ZnodeRmaRequestStatus] FOREIGN KEY ([RmaRequestStatusId]) REFERENCES [dbo].[ZnodeRmaRequestStatus] ([RmaRequestStatusId])
);





