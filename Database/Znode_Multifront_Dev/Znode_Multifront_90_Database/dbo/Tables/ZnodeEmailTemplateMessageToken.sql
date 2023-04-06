CREATE TABLE [dbo].[ZnodeEmailTemplateMessageToken] (
    [EmailTemplateMessageTokenId] INT           IDENTITY (1, 1) NOT NULL,
    [MessageToken]                VARCHAR (500) NOT NULL,
    [CreatedBy]                   INT           NOT NULL,
    [CreatedDate]                 DATETIME      NOT NULL,
    [ModifiedBy]                  INT           NOT NULL,
    [ModifiedDate]                DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeEmailTemplateMessageToken] PRIMARY KEY CLUSTERED ([EmailTemplateMessageTokenId] ASC)
);

