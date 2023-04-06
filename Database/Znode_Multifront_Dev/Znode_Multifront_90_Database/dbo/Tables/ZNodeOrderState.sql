CREATE TABLE [dbo].[ZnodeOrderState] (
    [OrderStateId]   INT            NOT NULL,
    [OrderStateName] NVARCHAR (MAX) NULL,
    [Description]    NVARCHAR (MAX) NULL,
    [CreatedBy]      INT            NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     INT            NOT NULL,
    [ModifiedDate]   DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeOrderState] PRIMARY KEY CLUSTERED ([OrderStateId] ASC)
);



