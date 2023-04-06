CREATE TABLE [dbo].[ZnodeRmaReturnState] (
    [RmaReturnStateId]      INT            NOT NULL,
    [ReturnStateName]       NVARCHAR (50)  NOT NULL,
    [DisplayOrder]          INT            NOT NULL,
    [Description]           NVARCHAR (500) NULL,
    [IsReturnState]         BIT            NOT NULL,
    [IsReturnLineItemState] BIT            NOT NULL,
    [IsSendEmail]           BIT            NOT NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedDate]           DATETIME       NOT NULL,
    [ModifiedBy]            INT            NOT NULL,
    [ModifiedDate]          DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeRmaReturnState] PRIMARY KEY CLUSTERED ([RmaReturnStateId] ASC)
);

