CREATE TABLE [dbo].[ZnodePublishPortalGlobalAttributeEntity] (
    [PublishPortalGlobalAttributeEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                            INT            NOT NULL,
    [PublishStartTime]                     DATETIME       NULL,
    [PortalId]                             INT            NOT NULL,
    [PortalName]                           VARCHAR (300)  NOT NULL,
    [LocaleId]                             INT            NOT NULL,
    [GlobalAttributeGroups]                NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ZnodePublishPortalGlobalAttributeEntity] PRIMARY KEY CLUSTERED ([PublishPortalGlobalAttributeEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishPortalGlobalAttributeEntityVersionId]
    ON [dbo].[ZnodePublishPortalGlobalAttributeEntity]([VersionId] ASC);

