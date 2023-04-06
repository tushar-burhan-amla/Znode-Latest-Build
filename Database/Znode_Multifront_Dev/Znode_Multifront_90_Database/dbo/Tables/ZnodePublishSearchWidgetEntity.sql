CREATE TABLE [dbo].[ZnodePublishSearchWidgetEntity] (
    [PublishSearchWidgetEntityId] INT           IDENTITY (1, 1) NOT NULL,
    [VersionId]                   INT           NOT NULL,
    [PublishStartTime]            DATETIME      NULL,
    [CMSSearchWidgetId]           INT           NOT NULL,
    [MappingId]                   INT           NOT NULL,
    [WidgetsId]                   INT           NOT NULL,
    [PortalId]                    INT           NOT NULL,
    [TypeOFMapping]               VARCHAR (50)  NOT NULL,
    [LocaleId]                    INT           NOT NULL,
    [WidgetsKey]                  VARCHAR (300) NULL,
    [AttributeCode]               VARCHAR (300) NULL,
    [SearchKeyword]               VARCHAR (300) NULL,
    CONSTRAINT [PK_ZnodePublishSearchWidgetEntity] PRIMARY KEY CLUSTERED ([PublishSearchWidgetEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishSearchWidgetEntityVersionId]
    ON [dbo].[ZnodePublishSearchWidgetEntity]([VersionId] ASC);

