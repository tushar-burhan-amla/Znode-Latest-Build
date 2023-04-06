CREATE TABLE [dbo].[ZnodePimProductAttributeTextAreaValue] (
    [PimProductAttributeTextAreaValueId] INT            IDENTITY (1, 1) NOT NULL,
    [PimAttributeValueId]                INT            NOT NULL,
    [AttributeValue]                     NVARCHAR (MAX) NULL,
    [LocaleId]                           INT            NOT NULL,
    [CreatedBy]                          INT            NOT NULL,
    [CreatedDate]                        DATETIME       NOT NULL,
    [ModifiedBy]                         INT            NOT NULL,
    [ModifiedDate]                       DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodePimProductAttributeTextAreaValue] PRIMARY KEY CLUSTERED ([PimProductAttributeTextAreaValueId] ASC),
    CONSTRAINT [FK_ZnodePimProductAttributeTextAreaValue_ZnodePimAttributeValueId] FOREIGN KEY ([PimAttributeValueId]) REFERENCES [dbo].[ZnodePimAttributeValue] ([PimAttributeValueId])
);






GO
CREATE NONCLUSTERED INDEX [ind_ZnodePimProductAttributeTextAreaValue_PL]
    ON [dbo].[ZnodePimProductAttributeTextAreaValue]([PimAttributeValueId] ASC, [LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePimProductAttributeTextAreaValue_PimAttributeValueId]
    ON [dbo].[ZnodePimProductAttributeTextAreaValue]([PimAttributeValueId] ASC);

