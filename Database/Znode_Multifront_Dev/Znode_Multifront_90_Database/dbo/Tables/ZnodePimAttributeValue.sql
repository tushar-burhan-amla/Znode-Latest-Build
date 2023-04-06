CREATE TABLE [dbo].[ZnodePimAttributeValue] (
    [PimAttributeValueId]        INT            IDENTITY (1, 1) NOT NULL,
    [PimAttributeFamilyId]       INT            NULL,
    [PimProductId]               INT            NULL,
    [PimAttributeId]             INT            NULL,
    [PimAttributeDefaultValueId] INT            NULL,
    [AttributeValue]             NVARCHAR (300) NULL,
    [CreatedBy]                  INT            NOT NULL,
    [CreatedDate]                DATETIME       NOT NULL,
    [ModifiedBy]                 INT            NOT NULL,
    [ModifiedDate]               DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeValue] PRIMARY KEY CLUSTERED ([PimAttributeValueId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimAttributeValue_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId]),
    CONSTRAINT [FK_ZnodePimAttributeValue_ZnodePimAttributeDefaultValue] FOREIGN KEY ([PimAttributeDefaultValueId]) REFERENCES [dbo].[ZnodePimAttributeDefaultValue] ([PimAttributeDefaultValueId]),
    CONSTRAINT [FK_ZnodePimAttributeValue_ZnodePimAttributefamily] FOREIGN KEY ([PimAttributeFamilyId]) REFERENCES [dbo].[ZnodePimAttributeFamily] ([PimAttributeFamilyId]),
    CONSTRAINT [FK_ZnodePimAttributeValue_ZnodePimProduct] FOREIGN KEY ([PimProductId]) REFERENCES [dbo].[ZnodePimProduct] ([PimProductId])
);
























GO



GO
CREATE NONCLUSTERED INDEX [IDX_ZnodePimAttributeValue_PimProductId]
    ON [dbo].[ZnodePimAttributeValue]([PimProductId] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_ZnodePimAttributeValue_PimAttributeId]
    ON [dbo].[ZnodePimAttributeValue]([PimAttributeId] ASC);




GO
CREATE NONCLUSTERED INDEX [IDX_ZnodePImAttributeValue_PimAttributeIdProductId]
    ON [dbo].[ZnodePimAttributeValue]([PimAttributeId] ASC)
    INCLUDE([PimProductId]);


GO
CREATE NONCLUSTERED INDEX [IDX_ZnodePimAttributeValue_PimProductId_PimAttributeId]
    ON [dbo].[ZnodePimAttributeValue]([PimProductId] ASC)
    INCLUDE([PimAttributeId]);

