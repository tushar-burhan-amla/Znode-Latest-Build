CREATE TABLE [dbo].[ZnodeAttributeInputValidation] (
    [InputValidationId] INT           IDENTITY (1, 1) NOT NULL,
    [AttributeTypeId]   INT           NULL,
    [Name]              VARCHAR (100) NULL,
    [DisplayOrder]      INT           NOT NULL,
    [DefaultValue]      VARCHAR (300) NULL,
    [IsList]            BIT           NULL,
    [ControlName]       VARCHAR (300) NULL,
    [CreatedBy]         INT           NOT NULL,
    [CreatedDate]       DATETIME      NOT NULL,
    [ModifiedBy]        INT           NOT NULL,
    [ModifiedDate]      DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeAttributeInputValidation] PRIMARY KEY CLUSTERED ([InputValidationId] ASC),
    CONSTRAINT [FK_ZnodeAttributeInputValidation_ZnodeAttributeType] FOREIGN KEY ([AttributeTypeId]) REFERENCES [dbo].[ZnodeAttributeType] ([AttributeTypeId])
);



