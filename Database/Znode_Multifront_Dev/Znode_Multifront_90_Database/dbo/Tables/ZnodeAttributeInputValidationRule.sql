CREATE TABLE [dbo].[ZnodeAttributeInputValidationRule] (
    [InputValidationRuleId] INT           IDENTITY (1, 1) NOT NULL,
    [InputValidationId]     INT           NULL,
    [ValidationRule]        VARCHAR (300) NULL,
    [ValidationName]        VARCHAR (300) NULL,
    [DisplayOrder]          INT           NOT NULL,
    [RegExp]                VARCHAR (300) NULL,
    [CreatedBy]             INT           NOT NULL,
    [CreatedDate]           DATETIME      NOT NULL,
    [ModifiedBy]            INT           NOT NULL,
    [ModifiedDate]          DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeAttributeInputValidationRule] PRIMARY KEY CLUSTERED ([InputValidationRuleId] ASC),
    CONSTRAINT [FK_ZnodeAttributeInputValidationRule_ZnodeAttributeInputValidation] FOREIGN KEY ([InputValidationId]) REFERENCES [dbo].[ZnodeAttributeInputValidation] ([InputValidationId])
);



