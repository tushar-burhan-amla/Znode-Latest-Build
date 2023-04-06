CREATE TABLE [dbo].[ZnodeCMSTextWidgetConfiguration] (
    [CMSTextWidgetConfigurationId] INT            IDENTITY (1, 1) NOT NULL,
    [LocaleId]                     INT            NOT NULL,
    [CMSWidgetsId]                 INT            NOT NULL,
    [WidgetsKey]                   NVARCHAR (128) NOT NULL,
    [CMSMappingId]                 INT            NOT NULL,
    [TypeOFMapping]                NVARCHAR (50)  NOT NULL,
    [Text]                         NVARCHAR (MAX) NULL,
    [CreatedBy]                    INT            NOT NULL,
    [CreatedDate]                  DATETIME       NOT NULL,
    [ModifiedBy]                   INT            NOT NULL,
    [ModifiedDate]                 DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSTextWidgetConfiguration] PRIMARY KEY CLUSTERED ([CMSTextWidgetConfigurationId] ASC),
    CONSTRAINT [FK_ZnodeCMSTextWidgetConfiguration_ZnodeCMSWidgets] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId])
);



