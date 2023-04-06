CREATE TABLE [dbo].[ZnodePimAttributeValidation] (
    [PimAttributeValidationId] INT           IDENTITY (1, 1) NOT NULL,
    [PimAttributeId]           INT           NULL,
    [InputValidationId]        INT           NULL,
    [InputValidationRuleId]    INT           NULL,
    [Name]                     VARCHAR (300) NULL,
    [CreatedBy]                INT           NOT NULL,
    [CreatedDate]              DATETIME      NOT NULL,
    [ModifiedBy]               INT           NOT NULL,
    [ModifiedDate]             DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePimAttributeValidation] PRIMARY KEY CLUSTERED ([PimAttributeValidationId] ASC),
    CONSTRAINT [FK_ZnodePimAttributeValidation_ZnodeAttributeInputValidation] FOREIGN KEY ([InputValidationId]) REFERENCES [dbo].[ZnodeAttributeInputValidation] ([InputValidationId]),
    CONSTRAINT [FK_ZnodePimAttributeValidation_ZnodeAttributeInputValidationRule] FOREIGN KEY ([InputValidationRuleId]) REFERENCES [dbo].[ZnodeAttributeInputValidationRule] ([InputValidationRuleId]),
    CONSTRAINT [FK_ZnodePimAttributeValidation_ZnodePimAttribute] FOREIGN KEY ([PimAttributeId]) REFERENCES [dbo].[ZnodePimAttribute] ([PimAttributeId])
);





