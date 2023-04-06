CREATE TABLE [dbo].[ZnodeGlobalAttributeDefaultValue] (
    [GlobalAttributeDefaultValueId] INT           IDENTITY (1, 1) NOT NULL,
    [GlobalAttributeId]             INT           NULL,
    [AttributeDefaultValueCode]     VARCHAR (100) NULL,
    [IsEditable]                    BIT           CONSTRAINT [DF_ZNodeGlobalDefaultAttributeValue_Iseditable] DEFAULT ((0)) NULL,
    [DisplayOrder]                  INT           NULL,
    [MediaId]                       INT           NULL,
    [SwatchText]                    VARCHAR (100) NULL,
    [IsDefault]                     BIT           NULL,
    [CreatedBy]                     INT           NOT NULL,
    [CreatedDate]                   DATETIME      NOT NULL,
    [ModifiedBy]                    INT           NOT NULL,
    [ModifiedDate]                  DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalAttributeDefaultValue] PRIMARY KEY CLUSTERED ([GlobalAttributeDefaultValueId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeGlobalAttributeDefaultValue_ZnodeGlobalAttribute] FOREIGN KEY ([GlobalAttributeId]) REFERENCES [dbo].[ZnodeGlobalAttribute] ([GlobalAttributeId])
);
GO
CREATE NONCLUSTERED INDEX Ind_ZnodeGlobalAttributeDefaultValue_GlobalAttributeId 
ON [dbo].[ZnodeGlobalAttributeDefaultValue] ([GlobalAttributeId])
