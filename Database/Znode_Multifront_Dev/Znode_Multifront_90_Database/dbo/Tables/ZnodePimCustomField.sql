CREATE TABLE [dbo].[ZnodePimCustomField] (
    [PimCustomFieldId] INT           IDENTITY (1, 1) NOT NULL,
    [PimProductId]     INT           NULL,
    [CustomCode]       VARCHAR (300) NULL,
    [CreatedBy]        INT           NULL,
    [CreatedDate]      DATETIME      NULL,
    [ModifiedBy]       INT           NULL,
    [ModifiedDate]     DATETIME      NULL,
    [DisplayOrder]     INT           NULL,
    CONSTRAINT [PK_ZnodePimCustomField] PRIMARY KEY CLUSTERED ([PimCustomFieldId] ASC),
    CONSTRAINT [FK_ZnodePimCustomField_ZnodePimProduct] FOREIGN KEY ([PimProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId])
);



