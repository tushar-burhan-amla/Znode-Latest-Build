CREATE TABLE [dbo].[ZnodeSalesRepCustomerUserPortal] (
    [SalesRepCustomerUserPortalId] INT      IDENTITY (1, 1) NOT NULL,
    [UserPortalId]                 INT      NULL,
    [SalesRepUserId]               INT      NULL,
    [CustomerUserid]               INT      NULL,
    [CustomerPortalId]             INT      NULL,
    [CreatedBy]                    INT      NOT NULL,
    [CreatedDate]                  DATETIME NOT NULL,
    [ModifiedBy]                   INT      NOT NULL,
    [ModifiedDate]                 DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodeSalesRepCustomerUserPortal] PRIMARY KEY CLUSTERED ([SalesRepCustomerUserPortalId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeSalesRepCustomerUserPortal_ZnodePortal_PortalId] FOREIGN KEY ([CustomerPortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZnodeSalesRepCustomerUserPortal_ZnodeUser_SalesRepUserId] FOREIGN KEY ([SalesRepUserId]) REFERENCES [dbo].[ZnodeUser] ([UserId]),
    CONSTRAINT [FK_ZnodeSalesRepCustomerUserPortal_ZnodeUser_UserId] FOREIGN KEY ([CustomerUserid]) REFERENCES [dbo].[ZnodeUser] ([UserId]),
    CONSTRAINT [FK_ZnodeSalesRepUserPortal_ZnodeUserPortal_UserPortalId] FOREIGN KEY ([UserPortalId]) REFERENCES [dbo].[ZnodeUserPortal] ([UserPortalId])
);

