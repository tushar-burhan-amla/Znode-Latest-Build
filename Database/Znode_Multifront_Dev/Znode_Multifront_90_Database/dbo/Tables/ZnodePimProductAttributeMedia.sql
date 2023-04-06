CREATE TABLE [dbo].[ZnodePimProductAttributeMedia] (
    [PimProductAttributeMediaId] INT           IDENTITY (1, 1) NOT NULL,
    [PimAttributeValueId]        INT           NOT NULL,
    [MediaPath]                  VARCHAR (300) NULL,
    [MediaId]                    INT           NULL,
    [LocaleId]                   INT           NOT NULL,
    [CreatedBy]                  INT           NOT NULL,
    [CreatedDate]                DATETIME      NOT NULL,
    [ModifiedBy]                 INT           NOT NULL,
    [ModifiedDate]               DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePimProductAttributeMedia] PRIMARY KEY CLUSTERED ([PimProductAttributeMediaId] ASC),
    CONSTRAINT [FK_ZnodePimProductAttributeMedia_ZnodePimAttributeValueId] FOREIGN KEY ([PimAttributeValueId]) REFERENCES [dbo].[ZnodePimAttributeValue] ([PimAttributeValueId])
);










GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePimAttMEdia_PL]
    ON [dbo].[ZnodePimProductAttributeMedia]([PimAttributeValueId] ASC, [LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [IND_ZnodePimProductAttributeMedia_PML]
    ON [dbo].[ZnodePimProductAttributeMedia]([PimAttributeValueId] ASC, [MediaPath] ASC, [LocaleId] ASC);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePimProductAttributeMedia_PimAttributeValueId]
    ON [dbo].[ZnodePimProductAttributeMedia]([PimAttributeValueId] ASC);

