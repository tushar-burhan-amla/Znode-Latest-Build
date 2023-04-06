CREATE TABLE [dbo].[ZnodeCMSFormWidgetConfiguration] (
    [CMSFormWidgetConfigurationId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                     INT            NOT NULL,
    [CMSWidgetsId]                 INT            NOT NULL,
    [WidgetsKey]                   NVARCHAR (128) NOT NULL,
    [CMSMappingId]                 INT            NOT NULL,
    [TypeOFMapping]                NVARCHAR (50)  NOT NULL,
    [FormBuilderId]                INT            NULL,
    [FormTitle]                    NVARCHAR (200) NULL,
    [ButtonText]                   VARCHAR (100)  NULL,
    [IsTextMessage]                BIT            NULL,
    [TextMessage]                  VARCHAR (500)  NULL,
    [RedirectURL]                  NVARCHAR(MAX)  NULL,
	[IsShowCaptcha]                BIT            CONSTRAINT [DF__ZnodeCMSF__IsSho__332B7579] DEFAULT ((0)) NULL,
    [CreatedBy]                    INT            NOT NULL,
    [CreatedDate]                  DATETIME       NOT NULL,
    [ModifiedBy]                   INT            NOT NULL,
    [ModifiedDate]                 DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSFormWidgetConfiguration] PRIMARY KEY CLUSTERED ([CMSFormWidgetConfigurationId] ASC),
    CONSTRAINT [FK_ZnodeCMSFormWidgetConfiguration_ZnodeCMSWidgets] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId]),
    CONSTRAINT [FK_ZnodeCMSFormWidgetConfiguration_ZnodeFormBuilder] FOREIGN KEY ([FormBuilderId]) REFERENCES [dbo].[ZnodeFormBuilder] ([FormBuilderId])
);



