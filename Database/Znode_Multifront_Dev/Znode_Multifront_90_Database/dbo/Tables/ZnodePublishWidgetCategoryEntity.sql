CREATE TABLE [dbo].[ZnodePublishWidgetCategoryEntity] (
    [PublishWidgetCategoryEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                     INT            NOT NULL,
    [PublishStartTime]              DATETIME       NULL,
    [WidgetCategoryId]              INT            NOT NULL,
    [ZnodeCategoryId]               INT            NOT NULL,
    [MappingId]                     INT            NOT NULL,
    [PortalId]                      INT            NOT NULL,
    [WidgetsKey]                    NVARCHAR (500) NULL,
    [TypeOFMapping]                 VARCHAR (100)  NULL,
    [DisplayOrder]                  INT            NULL,
    [CategoryCode]                  VARCHAR (600)  NULL,
    CONSTRAINT [PK_ZnodePublishWidgetCategoryEntity] PRIMARY KEY CLUSTERED ([PublishWidgetCategoryEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishWidgetCategoryEntityVersionId]
    ON [dbo].[ZnodePublishWidgetCategoryEntity]([VersionId] ASC);

