CREATE TABLE [dbo].[ZnodeUserWishList] (
    [UserWishListId]    INT             IDENTITY (1, 1) NOT NULL,
    [UserId]            INT             NULL,
    [SKU]               NVARCHAR (600)  NULL,
    [WishListAddedDate] DATETIME        NULL,
    [CreatedBy]         INT             NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL,
    [ModifiedBy]        INT             NOT NULL,
    [ModifiedDate]      DATETIME        NOT NULL,
    [AddOnSKUs]         NVARCHAR (1000) NULL,
    [PortalId]          INT             NULL,
    CONSTRAINT [PK_ZnodeUserWishList] PRIMARY KEY CLUSTERED ([UserWishListId] ASC),
    CONSTRAINT [FK_ZnodeUserWishList_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);





