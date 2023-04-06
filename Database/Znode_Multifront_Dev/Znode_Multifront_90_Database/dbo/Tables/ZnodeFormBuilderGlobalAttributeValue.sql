CREATE TABLE [dbo].[ZnodeFormBuilderGlobalAttributeValue] (
    [FormBuilderGlobalAttributeValueId] INT            IDENTITY (1, 1) NOT NULL,
    [FormBuilderSubmitId]               INT            NULL,
    [GlobalAttributeId]                 INT            NULL,
    [GlobalAttributeDefaultValueId]     INT            NULL,
    [AttributeValue]                    NVARCHAR (300) NULL,
    [CreatedBy]                         INT            NOT NULL,
    [CreatedDate]                       DATETIME       NOT NULL,
    [ModifiedBy]                        INT            NOT NULL,
    [ModifiedDate]                      DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeFormBuilderGlobalAttributeValue] PRIMARY KEY CLUSTERED ([FormBuilderGlobalAttributeValueId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeFormBuilderGlobalAttributeValue_ZnodeFormBuilderSubmit] FOREIGN KEY ([FormBuilderSubmitId]) REFERENCES [dbo].[ZnodeFormBuilderSubmit] ([FormBuilderSubmitId]),
    CONSTRAINT [FK_ZnodeFormBuilderGlobalAttributeValue_ZnodeGlobalAttribute] FOREIGN KEY ([GlobalAttributeId]) REFERENCES [dbo].[ZnodeGlobalAttribute] ([GlobalAttributeId]),
    CONSTRAINT [FK_ZnodeFormBuilderGlobalAttributeValue_ZnodeGlobalAttributeDefaultValue] FOREIGN KEY ([GlobalAttributeDefaultValueId]) REFERENCES [dbo].[ZnodeGlobalAttributeDefaultValue] ([GlobalAttributeDefaultValueId])
);

