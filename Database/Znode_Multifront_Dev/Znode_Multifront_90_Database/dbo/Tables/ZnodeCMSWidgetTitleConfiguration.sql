CREATE TABLE [dbo].[ZnodeCMSWidgetTitleConfiguration] (
    [CMSWidgetTitleConfigurationId] INT            IDENTITY (1, 1) NOT NULL,
    [TitleCode]                     NVARCHAR (300) NOT NULL,
    [CMSWidgetsId]                  INT            NOT NULL,
    [WidgetsKey]                    NVARCHAR (128) NOT NULL,
    [CMSMappingId]                  INT            NOT NULL,
    [TypeOFMapping]                 NVARCHAR (50)  NOT NULL,
    [CreatedBy]                     INT            NOT NULL,
    [CreatedDate]                   DATETIME       NOT NULL,
    [ModifiedBy]                    INT            NOT NULL,
    [ModifiedDate]                  DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSWidgetTitleConfiguration] PRIMARY KEY CLUSTERED ([CMSWidgetTitleConfigurationId] ASC),
    CONSTRAINT [FK_ZnodeCMSWidgetTitleConfiguration_ZnodeCMSWidgets] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId])
);











