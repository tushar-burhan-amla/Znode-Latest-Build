CREATE TABLE [dbo].[ZnodeCMSTemplate] (
    [CMSTemplateId] INT             IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (100)  NOT NULL,
    [FileName]      NVARCHAR (2000) NOT NULL,
    [CreatedBy]     INT             NOT NULL,
    [CreatedDate]   DATETIME        NOT NULL,
    [ModifiedBy]    INT             NOT NULL,
    [ModifiedDate]  DATETIME        NOT NULL,
	MediaId         INT             NULL,
    CONSTRAINT [PK_ZnodeCMSTemplate] PRIMARY KEY CLUSTERED ([CMSTemplateId] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ZnodeCMSTemplate]
    ON [dbo].[ZnodeCMSTemplate]([Name] ASC);

