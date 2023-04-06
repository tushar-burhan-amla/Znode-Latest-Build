CREATE TABLE [dbo].[ZnodeCMSWidgetConfiguration] (
    [CMSWidgetConfigurationId] INT           IDENTITY (1, 1) NOT NULL,
    [CMSWidgetsId]             INT           NOT NULL,
    [CMSSliderId]              INT           NOT NULL,
    [CMSAreaId]                INT           NULL,
    [PortalId]                 INT           NULL,
    [Type]                     NVARCHAR (50) NOT NULL,
    [Navigation]               NVARCHAR (50) NULL,
    [AutoPlay]                 BIT           CONSTRAINT [DF_ZnodeCMSWidgetConfiguration_AutoPlay] DEFAULT ((0)) NOT NULL,
    [AutoplayTimeOut]          INT           NULL,
    [AutoplayHoverPause]       BIT           CONSTRAINT [DF_ZnodeCMSWidgetConfiguration_AutoplayHoverPause] DEFAULT ((0)) NOT NULL,
    [TransactionStyle]         NVARCHAR (50) NULL,
    [CreatedBy]                INT           NOT NULL,
    [CreatedDate]              DATETIME      NOT NULL,
    [ModifiedBy]               INT           NOT NULL,
    [ModifiedDate]             DATETIME      NOT NULL,
    [CMSContentPagesId]        INT           NULL,
    CONSTRAINT [PK_ZnodeCMSWidgetConfiguration] PRIMARY KEY CLUSTERED ([CMSWidgetConfigurationId] ASC),
    CONSTRAINT [FK_ZnodeCMSWidgetConfiguration_ZnodeCMSArea] FOREIGN KEY ([CMSAreaId]) REFERENCES [dbo].[ZnodeCMSArea] ([CMSAreaId]),
    CONSTRAINT [FK_ZnodeCMSWidgetConfiguration_ZnodeCMSSlider1] FOREIGN KEY ([CMSSliderId]) REFERENCES [dbo].[ZnodeCMSSlider] ([CMSSliderId]),
    CONSTRAINT [FK_ZnodeCMSWidgetConfiguration_ZnodeCMSWidget] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId])
);







