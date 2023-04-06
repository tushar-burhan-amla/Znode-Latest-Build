CREATE TABLE [dbo].[ZnodeImportTemplate] (
    [ImportTemplateId]     INT            IDENTITY (1, 1) NOT NULL,
    [ImportHeadId]         INT            NOT NULL,
    [TemplateName]         NVARCHAR (300) NULL,
    [TemplateVersion]      NVARCHAR (100) NULL,
    [PimAttributeFamilyId] INT            NULL,
    [IsActive]             BIT            NOT NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    [PromotionTypeId]      INT            NULL, 
    CONSTRAINT [PK_ZnodeImportTemplate] PRIMARY KEY CLUSTERED ([ImportTemplateId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeImportTemplate_ZnodeImportHead] FOREIGN KEY ([ImportHeadId]) REFERENCES [dbo].[ZnodeImportHead] ([ImportHeadId]),
    CONSTRAINT [FK_ZnodeImportTemplate_ZnodePimAttributeFamily] FOREIGN KEY ([PimAttributeFamilyId]) REFERENCES [dbo].[ZnodePimAttributeFamily] ([PimAttributeFamilyId]),
    CONSTRAINT [FK_ZnodeImportTemplate_ZnodePromotionType] FOREIGN KEY ([PromotionTypeId]) REFERENCES [dbo].[ZnodePromotionType] ([PromotionTypeId])
);





