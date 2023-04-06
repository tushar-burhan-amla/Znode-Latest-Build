CREATE TABLE [dbo].[ZnodeEmailTemplate] (
    [EmailTemplateId] INT           IDENTITY (1, 1) NOT NULL,
    [TemplateName]    VARCHAR (300) NULL,
    [CreatedBy]       INT           NOT NULL,
    [CreatedDate]     DATETIME      NOT NULL,
    [ModifiedBy]      INT           NOT NULL,
    [ModifiedDate]    DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeEmailTemplate] PRIMARY KEY CLUSTERED ([EmailTemplateId] ASC) WITH (FILLFACTOR = 90)
);





