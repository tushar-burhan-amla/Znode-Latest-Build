CREATE TABLE [dbo].[ZnodeOmsTemplate] (
    [OmsTemplateId] INT            IDENTITY (1, 1) NOT NULL,
    [PortalId]      INT            NOT NULL,
    [UserId]        INT            NOT NULL,
    [TemplateName]  NVARCHAR (500) NULL,
    [CreatedBy]     INT            NOT NULL,
    [CreatedDate]   DATETIME       NOT NULL,
    [ModifiedBy]    INT            NOT NULL,
    [ModifiedDate]  DATETIME       NOT NULL,
    [TemplateType] VARCHAR(20) NULL, 
    CONSTRAINT [PK_ZnodeOmsTemplate] PRIMARY KEY CLUSTERED ([OmsTemplateId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_ZnodeOmsTemplate_ZnodeUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[ZnodeUser] ([UserId])
);





