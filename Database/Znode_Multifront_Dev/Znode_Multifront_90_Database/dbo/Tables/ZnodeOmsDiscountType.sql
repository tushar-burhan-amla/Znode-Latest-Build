CREATE TABLE [dbo].[ZnodeOmsDiscountType] (
    [OmsDiscountTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]              VARCHAR (600) NOT NULL,
    [Description]       VARCHAR (MAX) NULL,
    [CreatedBy]         INT           NOT NULL,
    [CreatedDate]       DATETIME      NOT NULL,
    [ModifiedBy]        INT           NOT NULL,
    [ModifiedDate]      DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeOmsDiscountType] PRIMARY KEY CLUSTERED ([OmsDiscountTypeId] ASC)
);

