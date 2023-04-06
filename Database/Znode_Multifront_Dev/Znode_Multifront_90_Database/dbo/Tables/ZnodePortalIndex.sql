CREATE TABLE [dbo].[ZnodePortalIndex] (
    [PortalIndexId] INT           IDENTITY (1, 1) NOT NULL,
    [PortalId]      INT           NOT NULL,
    [IndexName]     NVARCHAR (50) NOT NULL,
    [CreatedBy]     INT           NOT NULL,
    [CreatedDate]   DATETIME      NOT NULL,
    [ModifiedBy]    INT           NOT NULL,
    [ModifiedDate]  DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePortalIndex] PRIMARY KEY CLUSTERED ([PortalIndexId] ASC),
    CONSTRAINT [FK_ZnodePortalIndex_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);

