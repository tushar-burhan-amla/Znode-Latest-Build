CREATE TABLE [dbo].[ZnodeOmsPersonalizeCartItem] (
    [OmsPersonalizeCartItemId] INT             IDENTITY (1, 1) NOT NULL,
    [OmsSavedCartLineItemId]   INT             NULL,
    [PersonalizeCode]          NVARCHAR (600)  NULL,
    [PersonalizeValue]         NVARCHAR (MAX)  NULL,
    [CreatedBy]                INT             NULL,
    [CreatedDate]              DATETIME        NULL,
    [ModifiedBy]               INT             NULL,
    [ModifiedDate]             DATETIME        NULL,
    [DesignId]                 NVARCHAR (2000) NULL,
    [ThumbnailURL]             NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_ZnodeOmsPersonalizeCartItem] PRIMARY KEY CLUSTERED ([OmsPersonalizeCartItemId] ASC),
    CONSTRAINT [FK_ZnodeOmsPersonalizeCartItem_ZnodeOmsPersonalizeCartItem] FOREIGN KEY ([OmsPersonalizeCartItemId]) REFERENCES [dbo].[ZnodeOmsPersonalizeCartItem] ([OmsPersonalizeCartItemId]),
    CONSTRAINT [FK_ZnodeOmsSavedCartLineItem_ZnodeOmsPersonalizeCartItem] FOREIGN KEY ([OmsSavedCartLineItemId]) REFERENCES [dbo].[ZnodeOmsSavedCartLineItem] ([OmsSavedCartLineItemId])
);



