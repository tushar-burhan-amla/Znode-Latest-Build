CREATE TABLE [dbo].[ZnodeCMSStaticPageTemplate] (
    [CMSStaticPageTemplateId] INT           IDENTITY (1, 1) NOT NULL,
    [StaticPageTemplateCode]  VARCHAR (50)  NOT NULL,
    [StaticPageTemplateName]  VARCHAR (100) NULL,
    [IsConfigurable]          BIT           CONSTRAINT [DF_ZnodeCMSStaticPageTemplate_IsConfigurable] DEFAULT ((0)) NOT NULL,
    [CreatedBy]               INT           NOT NULL,
    [CreatedDate]             DATETIME      NOT NULL,
    [ModifiedBy]              INT           NOT NULL,
    [ModifiedDate]            DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeCMSStaticPageTemplate] PRIMARY KEY CLUSTERED ([CMSStaticPageTemplateId] ASC)
);

