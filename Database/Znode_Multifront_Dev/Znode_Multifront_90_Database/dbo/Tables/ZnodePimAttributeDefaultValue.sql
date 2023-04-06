CREATE TABLE [dbo].[ZnodePimAttributeDefaultValue] (
    [PimAttributeDefaultValueId] INT           IDENTITY (1, 1) NOT NULL,
    [PimAttributeId]             INT           NULL,
    [AttributeDefaultValueCode]  VARCHAR (100) NULL,
    [IsEditable]                 BIT           CONSTRAINT [DF_ZNodePimDefaultAttributeValue_Iseditable] DEFAULT ((0)) NULL,
    [DisplayOrder]               INT           NULL,
    [IsDefault]                  BIT           NULL,
    [SwatchText]                 VARCHAR (100) NULL,
    [MediaId]                    INT           NULL,
    [CreatedBy]                  INT           NOT NULL,
    [CreatedDate]                DATETIME      NOT NULL,
    [ModifiedBy]                 INT           NOT NULL,
    [ModifiedDate]               DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeDefaultValue] PRIMARY KEY CLUSTERED ([PimAttributeDefaultValueId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodePimAttributeDefaultValue_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId])
);


GO
CREATE NONCLUSTERED INDEX [IX_ZnodePimAttributeDefaultValue_AttributeDefaultValueCode]
    ON [dbo].[ZnodePimAttributeDefaultValue]([AttributeDefaultValueCode] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [ind_ZnodePimAttributeDefaultValue_PA]
    ON [dbo].[ZnodePimAttributeDefaultValue]([PimAttributeId] ASC, [AttributeDefaultValueCode] ASC) WITH (FILLFACTOR = 90);

