CREATE TABLE [dbo].[ZnodeCMSMediaConfiguration] (
    [CMSMediaConfigurationId] INT            IDENTITY (1, 1) NOT NULL,
    [CMSWidgetsId]            INT            NULL,
    [WidgetsKey]              NVARCHAR (500) NULL,
    [CMSMappingId]            INT            NULL,
    [TypeOFMapping]           VARCHAR (50)   NULL,
    [MediaId]                 INT            NULL,
    [CreatedBy]               INT            NULL,
    [CreatedDate]             DATETIME       NULL,
    [ModifiedBy]              INT            NULL,
    [ModifiedDate]            DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([CMSMediaConfigurationId] ASC),
    CONSTRAINT [FK_ZnodeCMSMediaConfiguration_ZnodeCMSWidgets] FOREIGN KEY ([MediaId]) REFERENCES [dbo].[ZnodeMedia] ([MediaId]),
    CONSTRAINT [FK_ZnodeCMSMediaConfiguration_ZnodeCMSWidgets_CMSWidgetsId] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId])
);

