CREATE TABLE [dbo].[ZnodeOmsRefundType] (
    [OmsRefundTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [RefundType]      NVARCHAR (100) NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeOmsRefundType] PRIMARY KEY CLUSTERED ([OmsRefundTypeId] ASC)
);

