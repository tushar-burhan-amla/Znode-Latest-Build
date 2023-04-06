CREATE TABLE [dbo].[ZnodeInventoryList] (
    [InventoryListId] INT           IDENTITY (1, 1) NOT NULL,
    [ListCode]        VARCHAR (200) NOT NULL,
    [ListName]        VARCHAR (600) NULL,
    [CreatedBy]       INT           NOT NULL,
    [CreatedDate]     DATETIME      NOT NULL,
    [ModifiedBy]      INT           NOT NULL,
    [ModifiedDate]    DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeInventoryList] PRIMARY KEY CLUSTERED ([InventoryListId] ASC)
);

