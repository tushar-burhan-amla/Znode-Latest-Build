CREATE TABLE [dbo].[ZnodeCMSCustomerReview] (
    [CMSCustomerReviewId] INT             IDENTITY (1, 1) NOT NULL,
    [PublishProductId]    INT             NULL,
    [UserId]              INT             NULL,
    [Headline]            NVARCHAR (200)  NOT NULL,
    [Comments]            NVARCHAR (500)  NULL,
    [UserName]            NVARCHAR (300)  NULL,
    [UserLocation]        NVARCHAR (1000) NULL,
    [Rating]              INT             NULL,
    [Status]              NVARCHAR (10)   NULL,
    [CreatedBy]           INT             NOT NULL,
    [CreatedDate]         DATETIME        NOT NULL,
    [ModifiedBy]          INT             NOT NULL,
    [ModifiedDate]        DATETIME        NOT NULL,
    [PortalId]            INT             NULL,
    [SKU]                 VARCHAR (600)   NULL,
    CONSTRAINT [PK_ZnodeCMSCustomerReview] PRIMARY KEY CLUSTERED ([CMSCustomerReviewId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeCMSCustomerReview_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);












GO
CREATE NONCLUSTERED INDEX [IX_ZnodeCMSCustomerReview_Status]
    ON [dbo].[ZnodeCMSCustomerReview]([Status] ASC)
    INCLUDE([PublishProductId], [Rating], [PortalId]);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodeCMSCustomerReview_PublishProductId_Status]
    ON [dbo].[ZnodeCMSCustomerReview]([PublishProductId] ASC, [Status] ASC)
    INCLUDE([Rating], [PortalId]);


GO
CREATE NONCLUSTERED INDEX [Inx_ZnodeCMSCustomerReview_SKU_Status_PortalId]
    ON [dbo].[ZnodeCMSCustomerReview]([Status] ASC, [PortalId] ASC, [SKU] ASC);

