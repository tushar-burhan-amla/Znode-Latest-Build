CREATE TABLE [dbo].[ZnodeGlobalAttributeValidation] (
    [GlobalAttributeValidationId] INT           IDENTITY (1, 1) NOT NULL,
    [GlobalAttributeId]           INT           NULL,
    [InputValidationId]           INT           NULL,
    [InputValidationRuleId]       INT           NULL,
    [Name]                        VARCHAR (300) NULL,
    [CreatedBy]                   INT           NOT NULL,
    [CreatedDate]                 DATETIME      NOT NULL,
    [ModifiedBy]                  INT           NOT NULL,
    [ModifiedDate]                DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeGlobalAttributeValidation] PRIMARY KEY CLUSTERED ([GlobalAttributeValidationId] ASC),
    CONSTRAINT [FK_ZnodeGlobalAttributeValidation_ZnodeAttributeInputValidation] FOREIGN KEY ([InputValidationId]) REFERENCES [dbo].[ZnodeAttributeInputValidation] ([InputValidationId]),
    CONSTRAINT [FK_ZnodeGlobalAttributeValidation_ZnodeAttributeInputValidationRule] FOREIGN KEY ([InputValidationRuleId]) REFERENCES [dbo].[ZnodeAttributeInputValidationRule] ([InputValidationRuleId]),
    CONSTRAINT [FK_ZnodeGlobalAttributeValidation_ZnodeGlobalAttribute] FOREIGN KEY ([GlobalAttributeId]) REFERENCES [dbo].[ZnodeGlobalAttribute] ([GlobalAttributeId])
);

