CREATE TABLE [dbo].[ZnodeCustomPortalDetail] (
    [CustomePortalDetailsId] INT           IDENTITY (1, 1) NOT NULL,
    [PortalId]               INT           NULL,
    [CustomeData1]           VARCHAR (MAX) NULL,
    [CustomeData2]           VARCHAR (MAX) NULL,
    [CustomeData3]           VARCHAR (MAX) NULL,
    [CreatedBy]              INT           NOT NULL,
    [CreatedDate]            DATETIME      NOT NULL,
    [ModifiedBy]             INT           NOT NULL,
    [ModifiedDate]           DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCustomPortalDetail] PRIMARY KEY CLUSTERED ([CustomePortalDetailsId] ASC),
    CONSTRAINT [FK_ZnodeCustomPortalDetail_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);

