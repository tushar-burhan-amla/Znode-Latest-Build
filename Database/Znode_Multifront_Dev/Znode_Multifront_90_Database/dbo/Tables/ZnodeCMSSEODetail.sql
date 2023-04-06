CREATE TABLE [dbo].[ZnodeCMSSEODetail] (
    [CMSSEODetailId]  INT             IDENTITY (1, 1) NOT NULL,
    [CMSSEOTypeId]    INT             NOT NULL,
    [SEOId]           INT             NULL,
    [IsRedirect]      BIT             NULL,
    [MetaInformation] NVARCHAR (MAX)  NULL,
    [PortalId]        INT             NULL,
    [SEOUrl]          NVARCHAR (MAX)  NULL,
    [CreatedBy]       INT             NOT NULL,
    [CreatedDate]     DATETIME        NOT NULL,
    [ModifiedBy]      INT             NOT NULL,
    [ModifiedDate]    DATETIME        NOT NULL,
    [IsPublish]       BIT             NULL,
    [SEOCode]         NVARCHAR (2000) NULL,
    [PublishStateId]  TINYINT         CONSTRAINT [DF_ZnodeCMSSEODetail_PublishStateId] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ZnodeCMSSEODetails] PRIMARY KEY CLUSTERED ([CMSSEODetailId] ASC),
    CONSTRAINT [FK_ZnodeCMSSEODetail_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId]),
    CONSTRAINT [FK_ZnodeCMSSEODetails_ZnodeCMSSEOType] FOREIGN KEY ([CMSSEOTypeId]) REFERENCES [dbo].[ZnodeCMSSEOType] ([CMSSEOTypeId])
);














GO
CREATE NONCLUSTERED INDEX [IX_ZnodeCMSSEODetail]
    ON [dbo].[ZnodeCMSSEODetail]([SEOCode] ASC);

