CREATE TABLE [dbo].[ZnodeCMSContainerConfiguration] (
    [CMSContainerConfigurationId] INT        IDENTITY (1, 1) NOT NULL,
    [CMSWidgetsId]                  INT            NOT NULL,
    [WidgetKey]                    nvarchar(50)  NOT NULL,
    [CMSMappingId]                 INT            NOT NULL,
    [TypeOFMapping]                NVARCHAR (50)  NOT NULL,
    [ContentContainerId]           int            NULL,
    [ContainerKey]                 nvarchar(100)  NULL,
    [CreatedBy]                   INT            NOT NULL,
    [CreatedDate]                  DATETIME       NOT NULL,
    [ModifiedBy]                   INT            NOT NULL,
    [ModifiedDate]                 DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeCMSContainerConfiguration] PRIMARY KEY CLUSTERED ([CMSContainerConfigurationId] ASC),
    CONSTRAINT [FK_ZnodeCMSContainerConfiguration_ZnodeCMSWidgets] FOREIGN KEY ([CMSWidgetsId]) REFERENCES [dbo].[ZnodeCMSWidgets] ([CMSWidgetsId])
);
