CREATE TABLE [dbo].[ZnodePublishWidgetTitleEntity] (
    [PublishWidgetTitleEntityId] INT           IDENTITY (1, 1) NOT NULL,
    [VersionId]                  INT           NOT NULL,
    [PublishStartTime]           DATETIME      NULL,
    [WidgetTitleConfigurationId] INT           NOT NULL,
    [PortalId]                   INT           NOT NULL,
    [MappingId]                  INT           NOT NULL,
    [MediaPath]                  VARCHAR (300) NULL,
    [Title]                      VARCHAR (300) NULL,
    [Url]                        VARCHAR (300) NULL,
    [WidgetsKey]                 VARCHAR (300) NULL,
    [TypeOFMapping]              VARCHAR (50)  NULL,
    [ActivationDate]             DATETIME      NULL,
    [ExpirationDate]             DATETIME      NULL,
    [IsActive]                   BIT           NOT NULL,
    [LocaleId]                   INT           NOT NULL,
    [TitleCode]                  VARCHAR (50)  NULL,
    [DisplayOrder]               INT           NOT NULL,
	[IsNewTab]					 bit		 NULL,
    CONSTRAINT [PK_ZnodePublishWidgetTitleEntity] PRIMARY KEY CLUSTERED ([PublishWidgetTitleEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishWidgetTitleEntityVersionId]
    ON [dbo].[ZnodePublishWidgetTitleEntity]([VersionId] ASC);

