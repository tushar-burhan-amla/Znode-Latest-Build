CREATE TABLE [dbo].[ZnodePimDefaultAttributeValue] (
    [PimDefaultAttributeValueId] INT      IDENTITY (1, 1) NOT NULL,
    [PimAttributeId]             INT      NULL,
    [IsEditable]                 BIT      CONSTRAINT [DF_ZNodePimDefaultAttributeValue_Iseditable] DEFAULT ((0)) NULL,
    [CreatedBy]                  INT      NOT NULL,
    [CreatedDate]                DATETIME NOT NULL,
    [ModifiedBy]                 INT      NOT NULL,
    [ModifiedDate]               DATETIME NOT NULL,
    CONSTRAINT [ZNodePimDefaultAttributeValue_PK] PRIMARY KEY CLUSTERED ([PimDefaultAttributeValueId] ASC),
    CONSTRAINT [ZnodePimAttribute_ZNodePimDefaultAttributeValue_FK1] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId])
);

