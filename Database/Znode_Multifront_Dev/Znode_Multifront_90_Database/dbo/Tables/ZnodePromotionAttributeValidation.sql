CREATE TABLE [dbo].[ZnodePromotionAttributeValidation] (
    [PromotionAttributeValidationId] INT           IDENTITY (1, 1) NOT NULL,
    [PromotionAttributeId]           INT           NULL,
    [InputValidationId]              INT           NULL,
    [InputValidationRuleId]          INT           NULL,
    [Name]                           VARCHAR (300) NULL,
    [CreatedBy]                      INT           NOT NULL,
    [CreatedDate]                    DATETIME      NOT NULL,
    [ModifiedBy]                     INT           NOT NULL,
    [ModifiedDate]                   DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodePromotionAttributeValidation] PRIMARY KEY CLUSTERED ([PromotionAttributeValidationId] ASC),
    CONSTRAINT [FK_ZnodePromotionAttributeValidation_ZnodePromotionAttributeValidation] FOREIGN KEY ([PromotionAttributeValidationId]) REFERENCES [dbo].[ZnodePromotionAttributeValidation] ([PromotionAttributeValidationId]),
    CONSTRAINT [FK_ZnodePromotionAttributeValidation_ZnodeAttributeInputValidation] FOREIGN KEY ([InputValidationId]) REFERENCES [dbo].[ZnodeAttributeInputValidation] ([InputValidationId]),
    CONSTRAINT [FK_ZnodePromotionAttributeValidation_ZnodeAttributeInputValidationRule] FOREIGN KEY ([InputValidationRuleId]) REFERENCES [dbo].[ZnodeAttributeInputValidationRule] ([InputValidationRuleId]),
    CONSTRAINT [FK_ZnodePromotionAttributeValidation_ZnodePromotionAttribute] FOREIGN KEY ([PromotionAttributeId]) REFERENCES [dbo].[ZnodePromotionAttribute] ([PromotionAttributeId])
);



