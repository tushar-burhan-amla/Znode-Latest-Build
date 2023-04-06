CREATE TABLE [dbo].[ZnodeMediaAttributeValidation] (
    [MediaAttributeValidationId] INT           IDENTITY (1, 1) NOT NULL,
    [MediaAttributeId]           INT           NULL,
    [InputValidationId]          INT           NULL,
    [InputValidationRuleId]      INT           NULL,
    [Name]                       VARCHAR (300) NULL,
    [CreatedBy]                  INT           NOT NULL,
    [CreatedDate]                DATETIME      NOT NULL,
    [ModifiedBy]                 INT           NOT NULL,
    [ModifiedDate]               DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeMediaAttributeValidation] PRIMARY KEY CLUSTERED ([MediaAttributeValidationId] ASC),
    CONSTRAINT [FK_ZnodeMediaAttributeValidation_ZnodeAttributeInputValidation] FOREIGN KEY ([InputValidationId]) REFERENCES [dbo].[ZnodeAttributeInputValidation] ([InputValidationId]),
    CONSTRAINT [FK_ZnodeMediaAttributeValidation_ZnodeAttributeInputValidationRule] FOREIGN KEY ([InputValidationRuleId]) REFERENCES [dbo].[ZnodeAttributeInputValidationRule] ([InputValidationRuleId]),
    CONSTRAINT [FK_ZnodeMediaAttributeValidation_ZnodeAttributeInputValidationRule1] FOREIGN KEY ([InputValidationRuleId]) REFERENCES [dbo].[ZnodeAttributeInputValidationRule] ([InputValidationRuleId]),
    CONSTRAINT [FK_ZnodeMediaAttributeValidation_ZnodeMediaAttribute] FOREIGN KEY ([MediaAttributeId]) REFERENCES [dbo].[ZnodeMediaAttribute] ([MediaAttributeId])
);





