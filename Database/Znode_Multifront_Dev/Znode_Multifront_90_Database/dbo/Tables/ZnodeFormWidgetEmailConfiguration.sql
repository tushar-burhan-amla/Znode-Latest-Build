CREATE TABLE [dbo].[ZnodeFormWidgetEmailConfiguration] (
    [FormWidgetEmailConfigurationId] INT           IDENTITY (1, 1) NOT NULL,
    [CMSContentPagesId]              INT           NOT NULL,
    [NotificationEmailId]            VARCHAR (500) NULL,
    [NotificationEmailTemplateId]    INT           NULL,
    [AcknowledgementEmailTemplateId] INT           NULL,
    [CreatedBy]                      INT           NOT NULL,
    [CreatedDate]                    DATETIME      NOT NULL,
    [ModifiedBy]                     INT           NOT NULL,
    [ModifiedDate]                   DATETIME      NOT NULL,
    CONSTRAINT [PK_ZnodeFormWidgetEmailConfiguration] PRIMARY KEY CLUSTERED ([FormWidgetEmailConfigurationId] ASC),
    CONSTRAINT [FK_ZnodeFormWidgetEmailConfiguration_ZnodeCMSContentPages] FOREIGN KEY ([CMSContentPagesId]) REFERENCES [dbo].[ZnodeCMSContentPages] ([CMSContentPagesId])
);

