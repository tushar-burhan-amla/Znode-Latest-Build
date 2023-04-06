CREATE TABLE [dbo].[ZnodeOmsOrderStateShowToCustomer] (
    [OmsOrderStateShowToCustomerId] INT            IDENTITY (1, 1) NOT NULL,
    [OmsOrderStateId]               INT            NULL,
    [OrderStateName]                NVARCHAR (MAX) NULL,
    [Description]                   NVARCHAR (MAX) NULL,
    [CreatedBy]                     INT            NOT NULL,
    [CreatedDate]                   DATETIME       NOT NULL,
    [ModifiedBy]                    INT            NOT NULL,
    [ModifiedDate]                  DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeOmsOrderStateShowToCustomer] PRIMARY KEY CLUSTERED ([OmsOrderStateShowToCustomerId] ASC)
);

