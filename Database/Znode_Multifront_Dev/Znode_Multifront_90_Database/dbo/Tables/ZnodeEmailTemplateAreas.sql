CREATE TABLE [dbo].[ZnodeEmailTemplateAreas] (
    [EmailTemplateAreasId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]                 VARCHAR (500) NOT NULL,
    [Code]                 VARCHAR (100) NOT NULL,
    [CreatedBy]            INT           NOT NULL,
    [CreatedDate]          DATETIME      NOT NULL,
    [ModifiedBy]           INT           NOT NULL,
    [ModifiedDate]         DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeEmailTemplateAreas] PRIMARY KEY CLUSTERED ([EmailTemplateAreasId] ASC)
);

