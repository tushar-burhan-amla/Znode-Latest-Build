CREATE TABLE [dbo].[ZnodeCMSWidgetSliderBanner] (
    [CMSWidgetSliderBannerId] INT            IDENTITY (1, 1) NOT NULL,
    [CMSSliderId]             INT            NOT NULL,
    [Type]                    NVARCHAR (50)  NULL,
    [Navigation]              NVARCHAR (50)  NULL,
    [AutoPlay]                BIT            CONSTRAINT [DF_ZnodeCMSWidgetConfiguration_AutoPlay] DEFAULT ((0)) NOT NULL,
    [AutoplayTimeOut]         INT            NULL,
    [AutoplayHoverPause]      BIT            CONSTRAINT [DF_ZnodeCMSWidgetConfiguration_AutoplayHoverPause] DEFAULT ((0)) NOT NULL,
    [TransactionStyle]        NVARCHAR (50)  NULL,
    [CMSWidgetsId]            INT            NOT NULL,
    [WidgetsKey]              NVARCHAR (128) NOT NULL,
    [CMSMappingId]            INT            NOT NULL,
    [TypeOFMapping]           NVARCHAR (50)  NOT NULL,
    [CreatedBy]               INT            NOT NULL,
    [CreatedDate]             DATETIME       NOT NULL,
    [ModifiedBy]              INT            NOT NULL,
    [ModifiedDate]            DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSWidgetConfiguration] PRIMARY KEY CLUSTERED ([CMSWidgetSliderBannerId] ASC),
    CONSTRAINT [FK_ZnodeCMSWidgetConfiguration_ZnodeCMSSlider1] FOREIGN KEY ([CMSSliderId]) REFERENCES [dbo].[ZnodeCMSSlider] ([CMSSliderId]),
    CONSTRAINT [FK_ZnodeCMSWidgetConfiguration_ZnodeCMSWidget] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId]),
    CONSTRAINT [FK_ZnodeCMSWidgetSliderBanner_ZnodeCMSWidgets] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId])
);



