CREATE TABLE [dbo].[ZnodePimProductAttributeDefaultValue] (
    [PimProductAttributeDefaultValueId] INT      IDENTITY (1, 1) NOT NULL,
    [PimAttributeValueId]               INT      NOT NULL,
    [PimAttributeDefaultValueId]        INT      NOT NULL,
    [LocaleId]                          INT      NOT NULL,
    [CreatedBy]                         INT      NOT NULL,
    [CreatedDate]                       DATETIME NOT NULL,
    [ModifiedBy]                        INT      NOT NULL,
    [ModifiedDate]                      DATETIME NOT NULL,
    CONSTRAINT [PK_ZnodePimProductAttributeDefaultValue] PRIMARY KEY CLUSTERED ([PimProductAttributeDefaultValueId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimProductAttributeDefaultValue_ZnodePimAttributeDefaultValue] FOREIGN KEY ([PimAttributeDefaultValueId]) REFERENCES [dbo].[ZnodePimAttributeDefaultValue] ([PimAttributeDefaultValueId]),
    CONSTRAINT [FK_ZnodePimProductAttributeDefaultValue_ZnodePimAttributeValueId] FOREIGN KEY ([PimAttributeValueId]) REFERENCES [dbo].[ZnodePimAttributeValue] ([PimAttributeValueId])
);










GO
CREATE NONCLUSTERED INDEX [ind_ZnodePimProductAttributeDefaultValue_PPL]
    ON [dbo].[ZnodePimProductAttributeDefaultValue]([PimAttributeValueId] ASC, [PimAttributeDefaultValueId] ASC, [LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [ind_ZnodePimProductAttributeDefaultValue_PL]
    ON [dbo].[ZnodePimProductAttributeDefaultValue]([PimAttributeValueId] ASC, [LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePimProductAttributeDefaultValue_PimAttributeValueId]
    ON [dbo].[ZnodePimProductAttributeDefaultValue]([PimAttributeValueId] ASC);


GO
CREATE NONCLUSTERED INDEX [ZnodePimProductAttributeDefaultValue_ForPaging_Include]
    ON [dbo].[ZnodePimProductAttributeDefaultValue]([PimAttributeValueId] ASC)
    INCLUDE([PimAttributeDefaultValueId]);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimProductAttributeDefaultValue_PimAttributeDefaultValueId_LocaleId]
    ON [dbo].[ZnodePimProductAttributeDefaultValue]([PimAttributeDefaultValueId] ASC, [LocaleId] ASC)
    INCLUDE([PimAttributeValueId]);

