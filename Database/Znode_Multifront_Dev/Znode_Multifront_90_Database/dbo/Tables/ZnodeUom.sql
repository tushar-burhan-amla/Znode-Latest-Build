CREATE TABLE [dbo].[ZnodeUom] (
    [UomId]        INT           IDENTITY (1, 1) NOT NULL,
    [Uom]          VARCHAR (100) NOT NULL,
    [CreatedBy]    INT           NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [ModifiedBy]   INT           NOT NULL,
    [ModifiedDate] DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeUom] PRIMARY KEY CLUSTERED ([UomId] ASC)
);

