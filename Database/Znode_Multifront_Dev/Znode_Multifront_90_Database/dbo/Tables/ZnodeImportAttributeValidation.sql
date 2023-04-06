CREATE TABLE [dbo].[ZnodeImportAttributeValidation] (
    [ImportAttributeValidationId] INT           IDENTITY (1, 1) NOT NULL,
    [AttributeTypeName]           VARCHAR (300) NULL,
    [AttributeCode]               VARCHAR (300) NULL,
    [ImportHeadId]                INT           NOT NULL,
    [IsRequired]                  BIT           NULL,
    [ControlName]                 VARCHAR (300) NULL,
    [ValidationName]              VARCHAR (100) NULL,
    [SubValidationName]           VARCHAR (300) NULL,
    [ValidationValue]             VARCHAR (300) NULL,
    [RegExp]                      VARCHAR (300) NULL,
    [DisplayOrder]                INT           NULL,
    [CreatedBy]                   INT           NOT NULL,
    [CreatedDate]                 DATETIME      NOT NULL,
    [ModifiedBy]                  INT           NOT NULL,
    [ModifiedDate]                DATETIME      NOT NULL,
    [SequenceNumber]              INT           NULL,
    CONSTRAINT [PK_ZnodeImportAttributeValidation] PRIMARY KEY CLUSTERED ([ImportAttributeValidationId] ASC),
    CONSTRAINT [FK_ZnodeImportAttributeValidation_ZnodeImportHead] FOREIGN KEY ([ImportHeadId]) REFERENCES [dbo].[ZnodeImportHead] ([ImportHeadId])
);





